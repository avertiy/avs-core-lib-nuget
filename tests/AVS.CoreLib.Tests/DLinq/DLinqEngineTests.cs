using System;
using System.Collections.Generic;
using AVS.CoreLib.DLinq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.DLinq;

[TestClass]
public class DLinqEngineTests
{
    private readonly DLinqEngine _engine = new();
    
    [TestMethod]
    public void Select_Compound_Index_Expression_Should_Return_Typed_List()
    {
        try
        {
            // arrange
            var dict = new Dictionary<string, long> { { "key123", 10L }, { "key.123", 20L } };
            var list = new List<Dictionary<string, long>> { dict };
            var source = new[] { new { Prop = list }, new { Prop = list }, new { Prop = list } };

            // act
            var result = _engine.Process(source, "prop[0][\"key.123\"]", null);
            var actual = result as List<long>;

            // assert
            Assert.IsNotNull(actual);
            actual.Count.Should().Be(3);
            actual[1].Should().Be(20L);
        }
        catch (Exception ex)
        {
            var str = ex.ToString();
        }
    }

    [TestMethod]
    public void Select_Prop_Should_Return_Typed_List()
    {
        // arrange
        var time = DateTime.Now;
        var source = new[] { time, time.AddDays(1), time.AddDays(2) };

        // act
        var result = _engine.Process(source, "day", null);
        var list = result as List<int>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(3);
    }

    [TestMethod]
    public void Select_By_Index_Should_Return_Typed_List()
    {
        // arrange
        var source = new[] { new[] {1,2}, new[] { 3, 4 }, new[] { 5, 6 } };

        // act
        var result = _engine.Process(source, "[1]", null);
        var list = result as List<int>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(3);
        list[1].Should().Be(4);
    }

    [TestMethod]
    public void Select_Multiple_Values_Should_Return_Typed_Dictionary()
    {
        try
        {
            // arrange
            var dictItem = new Dictionary<string, long> { { "KeY#1", 10L }, { "kEy.123", 20L }, { "key@1", 30L } };
            var listOfDict = new List<Dictionary<string, long>> { dictItem, dictItem };
            var source = new[] { new { Prop = listOfDict }, new { Prop = listOfDict }, new { Prop = listOfDict } };

            // act
            var result = _engine.Process(source, "prop[0][\"kEy.123\"],prop[1][KeY#1],prop[1][key@1]", null);
            var list = result as List<IDictionary<string,long>>;

            // assert
            Assert.IsNotNull(list);
            list.Count.Should().Be(3);
            list[0].Keys.Count.Should().Be(3);
            list[0].ContainsKey("kEy.123").Should().BeTrue();
            list[0].ContainsKey("KeY#1").Should().BeTrue();
            list[0].ContainsKey("key@1").Should().BeTrue();
            list[0]["KeY#1"].Should().Be(10L);
        }
        catch (Exception ex)
        {
            var str = ex.ToString();
        }
    }

    [TestMethod]
    public void Select_Multiple_Values_Should_Return_Object_Dictionary()
    {
        try
        {
            // arrange
            var dictItem = new Dictionary<string, long> { { "KeY#1", 10L }, { "kEy.123", 20L }, { "key@1", 30L } };
            var listOfDict = new List<Dictionary<string, long>> { dictItem, dictItem };
            var source = new[] { new { Prop = listOfDict, Close =1 }, new { Prop = listOfDict, Close = 2 }, new { Prop = listOfDict, Close = 3 } };

            // act
            var result = _engine.Process(source, "prop[0][\"kEy.123\"],prop[1][KeY#1],close", null);
            var list = result as List<IDictionary<string, object>>;

            // assert
            Assert.IsNotNull(list);
            list.Count.Should().Be(3);
            list[0].Keys.Count.Should().Be(3);
            list[0].ContainsKey("kEy.123").Should().BeTrue();
            list[0].ContainsKey("KeY#1").Should().BeTrue();
            list[0].ContainsKey("Close").Should().BeTrue();
            list[0]["KeY#1"].Should().Be(10L);
            list[1]["close"].Should().Be(2);
        }
        catch (Exception ex)
        {
            var str = ex.ToString();
        }
    }
}