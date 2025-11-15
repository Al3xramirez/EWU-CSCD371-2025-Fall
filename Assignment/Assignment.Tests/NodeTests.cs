using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class NodeTests
{
    [TestMethod]
    public void GetEnumerator_SingleNode_Succes()
    {
        Node<int> node = new(10);
        List<int> list = node.ToList();
        Assert.AreEqual(1, list.Count());
        Assert.AreEqual(10, list[0]);
    }

    [TestMethod]
    public void GetEnumerator_MultipleNodes_Success()
    {
        Node<string> node = new("apple");
        node.Append("orange");
        node.Append("banana");
        node.Append("cherry");
        List<string> list = node.ToList();
        List<string> expectedList = new() { "apple", "cherry", "banana", "orange" };
        CollectionAssert.AreEqual(expectedList, list);
    }

    [TestMethod]
    public void GetEnumerator_LinqQueries_Success()
    {
        Node<string> node = new("apple");
        node.Append("orange");
        node.Append("blackberry");
        node.Append("mango");
        node.Append("strawberry");
        node.Append("rasberry");

        var longFruits = node.Where(f => f.Length > 5).ToList();
        List<string> expectedList = new() { "rasberry", "strawberry", "blackberry", "orange" };
        CollectionAssert.AreEqual (expectedList, longFruits);
    }

    [TestMethod]
    public void ChildItems_Max2_ReturnsCorrect()
    {
        Node<int> node = new(1);
        node.Append(2);
        node.Append(3);
        node.Append(4);
        node.Append(5);
        node.Append(6);
        node.Append(7);

        List<int> result = node.ChildItems(2).ToList();
        List<int> expectedList = new() { 7, 6 };

        CollectionAssert.AreEqual(expectedList, result); 
    }

    [TestMethod]
    public void ChildItems_Max10_ReturnsAllRemaining()
    {
        Node<int> node = new(1);
        node.Append(2);
        node.Append(3);
        node.Append(4);
        node.Append(5);

        List<int> result = node.ChildItems(10).ToList();
        List<int> expectedList = new() { 5, 4, 3, 2 };

        CollectionAssert.AreEqual(expectedList, result);
    }

    
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    //public void Node_ChildItems_MaxNegative_ThrowsException()
    //{
        //Node<int> node = new(1);
        //node.Append(2);

        //var result = node.ChildItems(-5).ToList();
    //}

    [TestMethod]
    public void Append_List_Success()
    {
        Node<int> node = new(1);
        node.Append(2);
        node.Append(3);
        List<int> result = node.ToList();
        List<int> expectedList = new() { 1, 3, 2 };
        CollectionAssert.AreEqual(expectedList, result);
    }

    [TestMethod]
    public void Append_DuplicateValue_ThrowsException() { 
        
        Node<string> node = new("apple");
        node.Append("banana");
        node.Append("cherry");
        try
        {
            node.Append("banana");
            Assert.Fail("Expected InvalidOperationException was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual<string>("Duplicate value detected", ex.Message);
        }
    }

    [TestMethod]
    public void Clear_List_Success()
    {
        Node<int> node = new(1);
        node.Append(2);
        node.Append(3);
        node.Clear();
        List<int> result = node.ToList();
        List<int> expectedList = new() { 1 };
        CollectionAssert.AreEqual(expectedList, result);
    }

}
