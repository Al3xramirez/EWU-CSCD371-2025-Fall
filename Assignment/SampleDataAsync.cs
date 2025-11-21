using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment;

public class SampleDataAsync : IAsyncSampleData
{
    // 1.
    private readonly string _filePath = 
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "People.csv");
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
        var rows = new List<string>();
        await foreach (var row in CsvRows)
            rows.Add(row);
        foreach (var state in DataHelper.ExtractStates(rows))
            yield return state;
    }

    // 3.
    public async Task<string> GetAggregateSortedListOfStatesUsingCsvRows()
    {
        List<string> rows = new();

        await foreach (var row in CsvRows)
        {
            rows.Add(row);
        }

        var states = DataHelper.ExtractStates(rows).ToList();

        return states.Count == 0
            ? string.Empty
            : string.Join(", ", states);
    }

    // 4.
    public IAsyncEnumerable<IPerson> People => GetPeopleAsync();

    private async IAsyncEnumerable<IPerson> GetPeopleAsync()
    {
        List<string> rows = new();
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
        return AggregateStatesAsync(people).GetAwaiter().GetResult();
    }

    private async Task<string> AggregateStatesAsync(IAsyncEnumerable<IPerson> people)
    {
        List<string> states = new();

        await foreach (var person in people)
        {
            string? state = person.Address.State?.Trim();

            if (!string.IsNullOrWhiteSpace(state))
                states.Add(state);
        }

        var final = states
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase);

        return string.Join(", ", final);
    }

       
}
