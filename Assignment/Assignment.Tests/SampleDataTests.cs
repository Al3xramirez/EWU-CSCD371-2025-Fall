using Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
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
}
