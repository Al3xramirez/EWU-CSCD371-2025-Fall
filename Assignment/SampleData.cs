using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    private List<string>? _csvRows;
    public IEnumerable<string> CsvRows
    {
        get
        {
            if (_csvRows == null)
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "People.csv");
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"CSV file not found: {filePath}");
                }
                _csvRows = File.ReadAllLines(filePath).Skip(1).ToList();
            }
            return _csvRows;
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
        List<string> statesArray = uniqueStates.ToList();
        string result = statesArray.Aggregate((current, next) => current + "," + next);
        return result;
    }

    // 4.
    public IEnumerable<IPerson> People
    {
        get
        {
            List<IPerson> people = CsvRows
                .Where(row => row.Split(',').Length >= 8) 
                .Select(row =>
                {
                    string[] parts = row.Split(',');
                    if (parts.Length < 8)
                    {
                        return null;
                    }

                    IAddress address = new Address(
                        parts[4].Trim(),
                        parts[5].Trim(),
                        parts[6].Trim(),
                        parts[7].Trim()
                    );

                    IPerson person = new Person(
                        parts[1].Trim(),
                        parts[2].Trim(),
                        address,
                        parts[3].Trim()
                    );

                    return person;
                })
                .OfType<IPerson>()        // tell compiler that after filtering, it's safe
                .OrderBy(p => p.Address.State)
                .ThenBy(p => p.Address.City)
                .ThenBy(p => p.Address.Zip)
                .ToList();

            return people;
        }
    }


    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter)
    {

        return People
            .Where(p => filter(p.EmailAddress)) //filters by email address
            .Select(p => (p.FirstName, p.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people)
    {

        IEnumerable<string> uniqueStates = people
            .Select(p => p.Address.State)
            .Where(state => !string.IsNullOrWhiteSpace(state))
            .Distinct()
            .OrderBy(state => state);

        var statesArray = uniqueStates.ToList();
        return statesArray.Count == 0 
            ? string.Empty 
            : statesArray.Aggregate((current, next) => current + "," + next);
        
    }
}
