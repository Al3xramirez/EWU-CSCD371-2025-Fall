using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assignment;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Tests;
[TestClass]
public class SampleDataAsyncTests
{
    private IAsyncSampleData? _sampleDataAsync;

    [TestInitialize]
    public void Setup()
    {
        _sampleDataAsync = new SampleDataAsync();
    }

    private async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
            list.Add(item);
        return list;
    }

    private void VerifyUniqueSortedStates(IEnumerable<string> actual)
    {
        Assert.IsNotNull(actual);
        var expected = actual.Distinct().OrderBy(s => s).ToList();
        CollectionAssert.AreEqual(expected, actual.ToList());
    }

    [TestMethod]
    public async Task CsvRowsAsync_FirstRowSkipped_Success()
    {
        var rows = await ToListAsync(_sampleDataAsync!.CsvRows);

        string firstLine = rows.First();
        string expectedLine = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577";
        Assert.AreEqual(expectedLine, firstLine);
    }

    [TestMethod]
    public async Task GetUniqueSortedListOfStates_Async()
    {
        var actual = await ToListAsync(_sampleDataAsync!.GetUniqueSortedListOfStatesGivenCsvRows());
        VerifyUniqueSortedStates(actual);
    }

}
