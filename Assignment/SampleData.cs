using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows =>
        DataHelper.CsvRows(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "People.csv"));

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        DataHelper.ExtractStates(CsvRows);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows() =>
        string.Join(",", GetUniqueSortedListOfStatesGivenCsvRows());
    

    // 4.
    public IEnumerable<IPerson> People =>
       DataHelper.ExtractPeople(CsvRows)
           .OrderBy(p => p.Address.State)
           .ThenBy(p => p.Address.City)
           .ThenBy(p => p.Address.Zip)
           .ToList();


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

        string[] statesArray = uniqueStates.ToArray();
        return statesArray.Length == 0 
            ? string.Empty 
            : statesArray.Aggregate((current, next) => current + "," + next);
    }
}
