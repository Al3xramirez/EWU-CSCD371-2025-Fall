using Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    // Fields
    private ISampleData? _sampleData;

    [TestInitialize]
    public void Setup()
    {
        _sampleData = new SampleData();
    }
    private void VerifyUniqueSortedStates(IEnumerable<string> actual)
    {
        Assert.IsNotNull(actual);
        var expected = actual.Distinct().OrderBy(s => s).ToList();
        CollectionAssert.AreEqual(expected, actual.ToList());
    }


    [TestMethod]
    public void CvsRows_FirstRowSkipped_Success()
    {
        var rows = _sampleData!.CsvRows.ToList();
        string firstLine = rows.First();
        string expectedLine = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577";
        Assert.AreEqual<string>(expectedLine, firstLine);
    }

    [TestMethod]
    public void CvsRows_TotalLines_Success()
    {
        
        var allRows = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "People.csv")).ToList();
        var dataRows = _sampleData!.CsvRows.ToList();

        // Total lines in CSV file = 51
        Assert.HasCount(51, allRows);

        // Data rows after skipping header = 50
        Assert.HasCount(50, dataRows);
    }



    [TestMethod]
    // Using hardcoded CSV rows to verify unique sorted list of states
    public void GetUniqueSortedListOfStatesGivenCsvRows_HardcodedRows_Success()
    {

        var rows = new[]
        {
            "1,Leonel,Messi,LeoMessi@ewu.edu,123 Main St,CityA,WA,98823",
            "2,Cristiano,Ronaldo,CrisRon@ewu.edu,456 Oak St,CityB,CA,90210",
            "3,Neymar,Junior,NeyJr@ewu.edu,789 Pine St,CityC,FL,33101",
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
    public void GetUniqueSortedListOfStatesGivenCsvRows_LinqVerification_Success()
    {

        SampleData data = new();

        var statesLinqActual = data.CsvRows
            .Select(row =>
            {
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
            .Select(row =>
            {
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
    public void People_RecordsSortedByStateCityZip_Success()
    {
        // Arrange
        SampleData data = new();
        var people = data.People.ToList();

        // Act
        var expectedOrder = people
            .OrderBy(p => p.Address.State)
            .ThenBy(p => p.Address.City)
            .ThenBy(p => p.Address.Zip)
            .ToList();

        // Assert
        CollectionAssert.AreEqual(expectedOrder, people);
    }

    [TestMethod]
    public void People_ValidCsvRows_ReturnsCorrectFirstPerson()
    {
        // Arrange
        SampleData data = new();
        var people = data.People.ToList();

        // Act
        var firstPerson = people.First();

        // Assert
        Assert.AreEqual<string>("Arthur", firstPerson.FirstName);
        Assert.AreEqual<string>("Myles", firstPerson.LastName);
        Assert.AreEqual<string>("amyles1c@miibeian.gov.cn", firstPerson.EmailAddress);
        Assert.AreEqual<string>("Mobile", firstPerson.Address.City);
        Assert.AreEqual<string>("AL", firstPerson.Address.State);
        Assert.AreEqual<string>("37308", firstPerson.Address.Zip);
    }

    [TestMethod]
    public void People_ValidCsvRows_ReturnsCorrectLastPerson()
    {
        // Arrange
        SampleData data = new();
        var people = data.People.ToList();

        // Act
        var firstPerson = people.Last();

        // Assert
        Assert.AreEqual<string>("Amelia", firstPerson.FirstName);
        Assert.AreEqual<string>("Toal", firstPerson.LastName);
        Assert.AreEqual<string>("atoall@fema.gov", firstPerson.EmailAddress);
        Assert.AreEqual<string>("Huntington", firstPerson.Address.City);
        Assert.AreEqual<string>("WV", firstPerson.Address.State);
        Assert.AreEqual<string>("44302", firstPerson.Address.Zip);
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
            .Select(row =>
            {
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

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_EmptyList_ReturnsEmptyString()
    {
        SampleData data = new();
        string result = data.GetAggregateListOfStatesGivenPeopleCollection(new List<IPerson>());
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_DuplicateStates_ReturnsUniqueStates()
    {
        SampleData data = new();

        var people = new List<IPerson>
        {
            new Person("John", "Doe", new Address("123 Main St", "CityA", "CA", "90001"), "email@gmail.com"),
            new Person("Jane", "Smith", new Address("456 Oak St", "CityB", "CA", "90002"), "C#@gmail.com"),
            new Person("Alice", "Johnson", new Address("789 Pine St", "CityC", "NY", "10001"), "Java@gmail.com")

        };
        
       string result = data.GetAggregateListOfStatesGivenPeopleCollection(people);
       Assert.AreEqual<string>("CA,NY", result);

    }
}
