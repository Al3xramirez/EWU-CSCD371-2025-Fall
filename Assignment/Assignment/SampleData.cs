using System;
using System.Collections.Generic;
using System.IO;

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
            return File.ReadAllLines(filePath);
        }
    }

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() 
        => throw new NotImplementedException();

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => throw new NotImplementedException();

    // 4.
    public IEnumerable<IPerson> People => throw new NotImplementedException();

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => throw new NotImplementedException();

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => throw new NotImplementedException();
}
