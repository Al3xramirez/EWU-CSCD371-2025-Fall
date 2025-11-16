using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment;

public class SampleDataAsync : IAsyncSampleData
{
    // 1.
    public IAsyncEnumerable<string> CsvRows => throw new NotImplementedException();

    // 2.
    public IAsyncEnumerable<IPerson> People => throw new NotImplementedException();

    // 3.
    public IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        throw new NotImplementedException();
    }

    // 4.
    public string GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        throw new NotImplementedException();
    }

    // 5.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        throw new NotImplementedException();
    }

    // 6.
    public IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        throw new NotImplementedException();
    }
}
