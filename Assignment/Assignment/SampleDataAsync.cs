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
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        throw new NotImplementedException();
    }

    // 4.
    public IAsyncEnumerable<IPerson> People => throw new NotImplementedException();

    // 5.
    public IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        throw new NotImplementedException();
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        throw new NotImplementedException();
    }
}
