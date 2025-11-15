using Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void CvsRows_FirstRowSkipped_Success()
    {
        SampleData data = new();
        var rows = data.CsvRows.ToList();
        string firstLine = rows.First();
        string expectedLine = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577";
        Assert.AreEqual<string>(expectedLine, firstLine);
    }

    [TestMethod]
    public void CvsRows_TotalLines_Success()
    {
        SampleData data = new();
        var rows = data.CsvRows.ToList();
        var allRows = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "People.csv")).ToList();
        var dataRows = data.CsvRows.ToList();

        // Total lines in CSV file = 51
        Assert.HasCount(51, allRows);

        // Data rows after skipping header = 50
        Assert.HasCount(50, dataRows);
    }

    

    [TestMethod]
    // Using hardcoded CSV rows to verify unique sorted list of states
    public void GetUniqueSortedListOfStatesGivenCsvRows_HardcodedRows_Success() {
        
        var rows = new[]
        {
            "1,Leonel,Messi,LeoMessi@ewu.edu,123 Main St,CityA,WA,98823",
            "2,Cristiano,Ronaldo,CrisRon@ewu.edu,456 Oak St,CityB,CA,90210",
            "3,Neymar,Junior,NeyJr@ewu.edu,789 Pine St,CityC,FL,33101",
            "4,Mark,Zuckerberg, MarkTheLizard@ewu.edu,101 Maple St,CityD,NY,10001",
        };

        var actualStates = rows
            .Select(row => {
                var parts = row.Split(',');
                return parts.Length > 6 ? parts[6].Trim() : string.Empty;
            })
            .Where(state => !string.IsNullOrWhiteSpace(state))
            .Distinct()
            .OrderBy(state => state)
            .ToList();

        var expectedStates = new[]
        {
            "CA","FL","NY","WA"
        };

        //use collections assert to compare two collections instead of Assert.AreEqual 
        CollectionAssert.AreEqual(expectedStates, actualStates);

    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_Duplicates_ReturnsUniqueStates()
    {

        var rows = new[]
        {
            "1,Leonel,Messi,LeoMessi@ewu.edu,123 Main St,CityA,NY,98823",
            "2,Cristiano,Ronaldo,CrisRon@ewu.edu,456 Oak St,CityB,CA,90210",
            "3,Neymar,Junior,NeyJr@ewu.edu,789 Pine St,CityC,NY,33101",
            "4,Mark,Zuckerberg, MarkTheLizard@ewu.edu,101 Maple St,CityD,NY,10001",
        };

        var actualStates = rows
            .Select(row =>
            {
                var parts = row.Split(',');
                return parts.Length > 6 ? parts[6].Trim() : string.Empty;
            })
            .Where(state => !string.IsNullOrWhiteSpace(state))
            .Distinct()
            .OrderBy(state => state)
            .ToList();

        var expectedStates = new[]
        {
            "CA","NY"
        };

        CollectionAssert.AreEqual(expectedStates, actualStates);
    }

    [TestMethod]
    // Using LINQ to verify the method GetUniqueSortedListOfStatesGivenCsvRows
    public void GetUniqueSortedListOfStatesGivenCsvRows_LinqVerification_Success() {
        
        SampleData data = new();

        var statesLinqActual = data.CsvRows
            .Select(row => {
                var parts = row.Split(',');
                return parts.Length > 6 ? parts[6].Trim() : string.Empty;
            })
            .Where(state => !string.IsNullOrWhiteSpace(state))
            .Distinct()
            .OrderBy(state => state)
            .ToList();
        
        var statesExpected = data.GetUniqueSortedListOfStatesGivenCsvRows().ToList();
        CollectionAssert.AreEqual(statesLinqActual, statesExpected);
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_HardcodedRows_Success()
    {
        var rows = new[]
        {
            "1,Leonel,Messi,LeoMessi@ewu.edu,123 Main St,CityA,WA,98823",
            "2,Cristiano,Ronaldo,CrisRon@ewu.edu,456 Oak St,CityB,CA,90210",
            "3,Neymar,Junior,NeyJr@ewu.edu,789 Pine St,CityC,FL,33101",
            "4,Mark,Zuckerberg, MarkTheLizard@ewu.edu,101 Maple St,CityD,NY,10001",
        };

        var uniqueStates = rows
            .Select(row => {
                var parts = row.Split(',');
                return parts.Length > 6 ? parts[6].Trim() : string.Empty;
            })
            .Where(state => !string.IsNullOrWhiteSpace(state))
            .Distinct()
            .OrderBy(state => state)
            .ToList();
        string[] statesArray = uniqueStates.ToArray();
        string result = string.Join(",", statesArray);

        Assert.AreEqual<string>("CA,FL,NY,WA", result);

    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_Duplicates_ReturnsUniqueStates()
    {
        var rows = new[]
        {
            "1,Leonel,Messi,LeoMessi@ewu.edu,123 Main St,CityA,WA,98823",
            "2,Cristiano,Ronaldo,CrisRon@ewu.edu,456 Oak St,CityB,CA,90210",
            "3,Neymar,Junior,NeyJr@ewu.edu,789 Pine St,CityC,WA,33101",
            "4,Mark,Zuckerberg, MarkTheLizard@ewu.edu,101 Maple St,CityD,WA,10001",
        };

        var uniqueStates = rows
            .Select(row => {
                var parts = row.Split(',');
                return parts.Length > 6 ? parts[6].Trim() : string.Empty;
            })
            .Where(state => !string.IsNullOrWhiteSpace(state))
            .Distinct()
            .OrderBy(state => state)
            .ToList();
        string[] statesArray = uniqueStates.ToArray();
        string result = string.Join(",", statesArray);

        Assert.AreEqual<string>("CA,WA", result);

    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_LinqVerification_Success()
    {
        SampleData data = new();
        string result = data.GetAggregateSortedListOfStatesUsingCsvRows();

        List<string> statesList = result.Split(',').ToList();
        List<string> uniqueSortedStates = data.GetUniqueSortedListOfStatesGivenCsvRows().ToList();

        CollectionAssert.AreEqual(uniqueSortedStates, statesList);

    }

    [TestMethod]
    public void FilterByEmailAddress_ValidFilter_Success()
    {
        SampleData data = new();
        Predicate<string> filter = email => email.EndsWith("@Stanford.edu");
        var filteredNames = data.FilterByEmailAddress(filter).ToList();

        // Verify that all returned email addresses end with @Stanford.edu
        foreach (var (FirstName, LastName) in filteredNames)
        {
            var person = data.People.FirstOrDefault(p => p.FirstName == FirstName && p.LastName == LastName);
            
            Assert.IsNotNull(person);
            Assert.IsTrue(filter(person.EmailAddress));
            Assert.AreEqual<string>("Stanford.edu", person.EmailAddress.Split('@')[1]);
          
        }
    }

    [TestMethod]
    public void FilterByEmailAddress_NoMatches_ReturnsEmpty()
    {
        SampleData data = new();
        Predicate<string> filter = email => email.EndsWith("@67.com");
        var filteredNames = data.FilterByEmailAddress(filter).ToList();
        Assert.IsEmpty(filteredNames);
    }

    [TestMethod]

    public void GetAggregateListOfStatesGivenPeopleCollection_Sucess()
    {

        SampleData data = new();

        string result = data.GetAggregateListOfStatesGivenPeopleCollection(data.People);

        List<string> aggregateState = data.GetUniqueSortedListOfStatesGivenCsvRows().ToList();

        Assert.AreEqual<string>(result, string.Join(",", aggregateState));
    }

    }
