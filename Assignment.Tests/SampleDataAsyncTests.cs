using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assignment;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Assignment.Tests;
[TestClass]
public class SampleDataAsyncTests
{
    private SampleDataAsync? _sampleDataAsync;

    [TestInitialize]
    public void Setup()
    {
        _sampleDataAsync = new SampleDataAsync();
    }

    //Helper method to convert IAsyncEnermerable to List
    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
            list.Add(item);
        return list;
    }

    //Helper method to verify CSV rows
    private static async Task VerifyCsvRowsAsync(IAsyncEnumerable<string> rows) { 
    
        var list = await ToListAsync(rows);

        Assert.IsNotNull(list);
        Assert.IsNotEmpty(list);

        string firstLine = list.First();
        string expectedFirstLine = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577";
        Assert.AreEqual(expectedFirstLine, firstLine);

    }

    //Helper method to verify unique sorted state
    private static void VerifyUniqueSortedStates(IEnumerable<string> actual)
    {
        Assert.IsNotNull(actual);
        var expected = actual
            .Distinct()
            .OrderBy(s => s)
            .ToList();
        CollectionAssert.AreEqual(expected, actual.ToList());
    }

    private async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            await Task.Yield(); // Simulate asynchronous operation
            yield return item;
        }
    }

    [TestMethod]
    public async Task CsvRowsAsync_FirstRowSkipped_Success()
    {
        await VerifyCsvRowsAsync(_sampleDataAsync!.CsvRows);
    }

    [TestMethod]
    public async Task GetUniqueSortedListOfStatesGivenCsvRows_Async()
    {
        var actual = await ToListAsync(_sampleDataAsync!.GetUniqueSortedListOfStatesGivenCsvRows());
        VerifyUniqueSortedStates(actual);
    }

    [TestMethod]
    public async Task GetAggregateSortedListOfStatesUsingCsvRows_Async()
    {
        var result = await _sampleDataAsync!.GetAggregateSortedListOfStatesUsingCsvRows();
        Assert.IsNotNull(result);
        
        var states = result.Split(',')
            .Select(s => s.Trim())
            .ToList();

        VerifyUniqueSortedStates(states);
    }

    [TestMethod]
    public async Task People_ValidCsvRows_ReturnsSortedPeopleAsync()
    {
        var people = await ToListAsync(_sampleDataAsync!.People);
        

        var sortedPeople = people
            .OrderBy(p => p.Address.State)
            .ThenBy(p => p.Address.City)
            .ThenBy(p => p.Address.Zip)
            .ToList();

        Assert.HasCount(sortedPeople.Count, people);
    }

    [TestMethod]
    public async Task FilterByEmailAddress_ValidPredicate_ReturnsFilteredNamesAsync() { 
    
        Predicate<string> filter = email => email.EndsWith(".com", StringComparison.OrdinalIgnoreCase);

        var csvRows = DataHelper.CsvRows(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "people.csv"));
        
        var expected = DataHelper.ExtractPeople(csvRows)
            .Where(Person => filter(Person.EmailAddress))
            .Select(Person => (Person.FirstName, Person.LastName))
            .ToList();

        var actual = await ToListAsync(_sampleDataAsync!.FilterByEmailAddress(filter));

        Assert.HasCount(expected.Count, actual);

    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_ReturnsStatesAsync() {

        var csvRows = DataHelper.CsvRows(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "people.csv"));
        var people = DataHelper.ExtractPeople(csvRows)
            .OrderBy(p => p.Address.State)
            .ThenBy(p => p.Address.City)
            .ThenBy(p => p.Address.Zip)
            .ToList();

        var expectedStates = people
            .Select(p => p.Address.State)
            .Where(state => !string.IsNullOrWhiteSpace(state))
            .Distinct()

            .OrderBy(state => state)
            .ToList();

        var result = await Task.Run(() => 
            _sampleDataAsync!.GetAggregateListOfStatesGivenPeopleCollection(ToAsyncEnumerable(people))
        );

        var actualStates = result.Split(',')
            .Select(s => s.Trim())
            .ToList();

        VerifyUniqueSortedStates(actualStates);

        CollectionAssert.AreEqual(expectedStates, actualStates);
        
    }

}
