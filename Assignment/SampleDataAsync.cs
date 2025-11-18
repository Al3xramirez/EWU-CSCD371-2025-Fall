using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Assignment;

public class SampleDataAsync : IAsyncSampleData
{
    // 1.
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "People.csv");
    public IAsyncEnumerable<string> CsvRows 
        => GetCsvRowsAsync();

    // Private async iterator method
    private async IAsyncEnumerable<string> GetCsvRowsAsync()
    {
        using var reader = new StreamReader(_filePath);

        // Skip header
        await reader.ReadLineAsync();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line != null)
                yield return line;
        }
    }


    // 2.
    public IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() 
        => GetUniqueStatesAsync();

    private async IAsyncEnumerable<string> GetUniqueStatesAsync()
    {
        // Collect all CSV rows asynchronously
        var rows = new List<string>();
        await foreach (var row in CsvRows)
            rows.Add(row);
        foreach (var state in DataHelper.ExtractStates(rows))
            yield return state;
    }

    // 3.
    public async Task<string> GetAggregateSortedListOfStatesUsingCsvRows()
    {
        var rows = new List<string>();

        await foreach (var row in CsvRows)
            rows.Add(row);

        // Reuse your existing sync logic
        var sortedStates = DataHelper.ExtractStates(rows).ToList();

        return sortedStates.Count == 0
            ? string.Empty
            : string.Join(",", sortedStates);
    }

    // 4.
    public IAsyncEnumerable<IPerson> People => GetPeopleAsync();

    private async IAsyncEnumerable<IPerson> GetPeopleAsync()
    {
        var rows = new List<string>();
        await foreach (var row in CsvRows)
            rows.Add(row);
        foreach (var person in DataHelper.ExtractPeople(rows))
            yield return person;
    }


    // 5.
    public IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return FilterByEmailAsync(filter);
    }

    private async IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAsync(Predicate<string> filter)
    {
        await foreach (var person in People)
        {
            if (filter(person.EmailAddress))
                yield return (person.FirstName, person.LastName);
        }
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return GetAggregateStatesAsync(people).GetAwaiter().GetResult();
    }

    private static async Task<string> GetAggregateStatesAsync(IAsyncEnumerable<IPerson> people)
    {
        var states = new List<string>();

        await foreach (var p in people)
        {
            if (!string.IsNullOrWhiteSpace(p.Address.State))
                states.Add(p.Address.State.Trim());
        }

        var uniqueSorted = states
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        if (uniqueSorted.Count == 0)
            return string.Empty;

        return uniqueSorted.Skip(1).Aggregate(uniqueSorted.First(), (acc, state) => $"{acc}, {state}");
    }

    
}
