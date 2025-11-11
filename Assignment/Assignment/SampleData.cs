using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows
    {
        get
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "People.csv");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CVS file not found: {filePath}");
            }
            return File.ReadAllLines(filePath).Skip(1);
        }
    }

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows
        .Select(row => { 
            var parts = row.Split(',');
            return parts.Length > 6 ? parts[6].Trim() : string.Empty;
        })
        .Where(state => !string.IsNullOrWhiteSpace(state))
        .Distinct()
        .OrderBy(state => state);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        IEnumerable<string> uniqueStates = GetUniqueSortedListOfStatesGivenCsvRows();
        string[] statesArray = uniqueStates.ToArray();
        string result = string.Join(",", statesArray);
        return result;
    }

    // 4.
    public IEnumerable<IPerson> People => throw new NotImplementedException();

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => throw new NotImplementedException();

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => throw new NotImplementedException();
}
