using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    
        List<string> collectedStates = new();

        var asyncEnum = people.GetAsyncEnumerator();
        try
        {
            // Extract states synchronously from async source
            while (true)
            {
                var move = asyncEnum.MoveNextAsync();

                if (!move.AsTask().Result)
                    break;

                var current = asyncEnum.Current;
                var state = current.Address.State?.Trim();

                if (!string.IsNullOrWhiteSpace(state))
                    collectedStates.Add(state);
            }
        }
        finally
        {
            // Proper cleanup
            asyncEnum.DisposeAsync().AsTask().Wait();
        }

        // Process the list: unique + sorted
        var finalStates = collectedStates
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase);

        return string.Join(", ", finalStates);
    }
}
