using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.Utilities;
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
            throw;
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
        // arrange
        var dictItem = new Dictionary<string, long> { { "KeY#1", 10L }, { "kEy.123", 20L }, { "key@1", 30L } };
        var listOfDict = new List<Dictionary<string, long>> { dictItem, dictItem };
        var source = new[] { new { Prop = listOfDict }, new { Prop = listOfDict }, new { Prop = listOfDict } };

        // act
        var result = _engine.Process(source, "prop[0][\"kEy.123\"],prop[1][KeY#1],prop[1][key@1]", null);
        var list = result as List<IDictionary<string, long>>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(3);
        list[0].Keys.Count.Should().Be(3);
        list[0].ContainsKey("kEy.123").Should().BeTrue();
        list[0].ContainsKey("KeY#1").Should().BeTrue();
        list[0].ContainsKey("key@1").Should().BeTrue();
        list[0]["KeY#1"].Should().Be(10L);
    }

    [TestMethod]
    public void Select_Multiple_Values_Should_Return_Object_Dictionary()
    {

        // arrange
        var dictItem = new Dictionary<string, long> { { "KeY#1", 10L }, { "kEy.123", 20L }, { "key@1", 30L } };
        var listOfDict = new List<Dictionary<string, long>> { dictItem, dictItem };
        var source = new[]
        {
            new { Prop = listOfDict, Close = 1 }, new { Prop = listOfDict, Close = 2 },
            new { Prop = listOfDict, Close = 3 }
        };

        // act
        var result = _engine.Process(source, "prop[0][\"kEy.123\"],prop[1][KeY#1],close", null);
        var list = result as List<IDictionary<string, object>>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(3);
        list[0].Keys.Count.Should().Be(3);
        list[0].ContainsKey("kEy.123").Should().BeTrue();
        list[0].ContainsKey("KeY#1").Should().BeTrue();
        list[0].ContainsKey("close").Should().BeTrue();
        list[0]["KeY#1"].Should().Be(10L);
        list[1]["close"].Should().Be(2);
    }

    [TestMethod]
    public void Select_With_Where_Simple_Condition_Should_Work()
    {

        // arrange
        var dict1 = new Dictionary<string, long> { { "key1", 10L }, { "key2", 20L }, { "key3", 30L } };
        var dict2 = new Dictionary<string, long> { { "key1", 15L }, { "key2", 20L }, { "key3", 30L } };

        var source = new[]
        {
            new { Prop = dict1, Close = 1 }, new { Prop = dict2, Close = 2 }, new { Prop = dict2, Close = 3 }
        };

        // act
        var result = _engine.Process(source, "prop[key1],close WHERE prop[key1] > 10", null);
        var list = result as List<IDictionary<string, object>>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(2);
        list[0].Keys.Count.Should().Be(2);
        list[0].ContainsKey("key1").Should().BeTrue();
        list[0].ContainsKey("close").Should().BeTrue();
        list[0]["key1"].Should().Be(15L);
        list[0]["close"].Should().Be(2);
        list[1]["key1"].Should().Be(15L);
        list[1]["close"].Should().Be(3);

    }

    [TestMethod]
    public void Select_With_Where_Value_Expression_Condition_Should_Work()
    {

        // arrange
        var dict1 = new Dictionary<string, long> { { "key1", 10L }, { "key2", 20L }, { "key3", 30L } };
        var dict2 = new Dictionary<string, long> { { "key1", 15L }, { "key2", 20L }, { "key3", 30L } };
        var dict3 = new Dictionary<string, long> { { "key1", 25L }, { "key2", 20L }, { "key3", 30L } };
        var listOfDict1 = new List<Dictionary<string, long>> { dict1, dict1 };
        var listOfDict2 = new List<Dictionary<string, long>> { dict2, dict2 };
        var listOfDict3 = new List<Dictionary<string, long>> { dict3, dict3 };
        var source = new[]
        {
            new { Prop = listOfDict1, Close = 1 }, new { Prop = listOfDict2, Close = 2 },
            new { Prop = listOfDict3, Close = 3 }
        };


        // act
        var result = _engine.Process(source, "prop[0][key1],close WHERE prop[0][key1] > 10", null);
        var list = result as List<IDictionary<string, object>>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(2);
        list[0].Keys.Count.Should().Be(2);
        list[0].ContainsKey("key1").Should().BeTrue();
        list[0].ContainsKey("close").Should().BeTrue();
        list[0]["key1"].Should().Be(15L);
        list[0]["close"].Should().Be(2);
        list[1]["key1"].Should().Be(25L);
        list[1]["close"].Should().Be(3);
    }

    [TestMethod]
    public void Select_With_Where_Expression_Should_Work()
    {

        // arrange
        var dict1 = new Dictionary<string, long> { { "key1", 10L }, { "key2", 2L }, { "key3", 30L } };
        var dict2 = new Dictionary<string, long> { { "key1", 15L }, { "key2", 20L }, { "key3", 30L } };
        var dict3 = new Dictionary<string, long> { { "key1", 25L }, { "key2", 200L }, { "key3", 30L } };
        var listOfDict1 = new List<Dictionary<string, long>> { dict1, dict1 };
        var listOfDict2 = new List<Dictionary<string, long>> { dict2, dict2 };
        var listOfDict3 = new List<Dictionary<string, long>> { dict3, dict3 };
        var source = new[]
        {
            new { Prop = listOfDict1, Close = 1 }, new { Prop = listOfDict2, Close = 2 },
            new { Prop = listOfDict3, Close = 3 }
        };


        // act
        var result = _engine.Process(source, "prop[0][key1],close WHERE prop[0][key1] > 10 AND prop[0][key2] >= 200", null);
        var list = result as List<IDictionary<string, object>>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(1);
        list[0].Keys.Count.Should().Be(2);
        list[0].ContainsKey("key1").Should().BeTrue();
        list[0].ContainsKey("close").Should().BeTrue();
        list[0]["key1"].Should().Be(25L);
        list[0]["close"].Should().Be(3);
    }

    [TestMethod]
    public void Select_With_OrderBy_Expression_Should_Work()
    {

        // arrange
        var dict1 = new Dictionary<string, long> { { "key1", 10L }, { "key2", 2L }, { "key3", 30L } };
        var dict2 = new Dictionary<string, long> { { "key1", 15L }, { "key2", 20L }, { "key3", 30L } };
        var dict3 = new Dictionary<string, long> { { "key1", 25L }, { "key2", 200L }, { "key3", 30L } };
        var listOfDict1 = new List<Dictionary<string, long>> { dict1, dict1 };
        var listOfDict2 = new List<Dictionary<string, long>> { dict2, dict2 };
        var listOfDict3 = new List<Dictionary<string, long>> { dict3, dict3 };
        var source = new[]
        {
            new { Prop = listOfDict1, Close = 1 },
            new { Prop = listOfDict2, Close = 2 },
            new { Prop = listOfDict3, Close = 3 }
        };

        // act
        var result = _engine.Process(source, "prop[0][key1] ORDER BY prop[0][key1] DESC", null);
        var list = result as List<long>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(3);
        list[0].Should().Be(25L);
        list[1].Should().Be(15L);
        list[2].Should().Be(10L);
    }

    [TestMethod]
    public void Select_With_OrderBy_And_ThenBy_Expression_Should_Work()
    {

        // arrange
        var dict1 = new Dictionary<string, long> { { "key1", 10L }, { "key2", 2L }, { "key3", 5L } };
        var dict2 = new Dictionary<string, long> { { "key1", 25L }, { "key2", 20L }, { "key3", 50L } };
        var dict3 = new Dictionary<string, long> { { "key1", 25L }, { "key2", 200L }, { "key3", 15L } };
        var dict4 = new Dictionary<string, long> { { "key1", 25L }, { "key2", 2000L }, { "key3", 500L } };
        var list1 = new List<Dictionary<string, long>> { dict1, dict1 };
        var list2 = new List<Dictionary<string, long>> { dict2, dict2 };
        var list3 = new List<Dictionary<string, long>> { dict3, dict3 };
        var list4 = new List<Dictionary<string, long>> { dict4, dict4 };
        var source = new[]
        {
            new { Prop = list1, Close = 1 },
            new { Prop = list2, Close = 2 },
            new { Prop = list3, Close = 3 },
            new { Prop = list4, Close = 4 }
        };

        // act
        var result = _engine.Process(source, "prop[0][key2] ORDER BY prop[0][key1],prop[0][key3] DESC", null);
        var list = result as List<long>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(4);
        list[0].Should().Be(2000L);
        list[1].Should().Be(20L);
        list[2].Should().Be(200L);
        list[3].Should().Be(2L);
    }

    [TestMethod]
    public void Select_With_Where_And_OrderBy_Expressions_Should_Work()
    {

        // arrange
        var dict1 = new Dictionary<string, long> { { "key1", 10L }, { "key2", 2L }, { "key3", 100L } };
        var dict2 = new Dictionary<string, long> { { "key1", 15L }, { "key2", 20L }, { "key3", 30L } };
        var dict3 = new Dictionary<string, long> { { "key1", 25L }, { "key2", 200L }, { "key3", 3L } };
        var listOfDict1 = new List<Dictionary<string, long>> { dict1, dict1 };
        var listOfDict2 = new List<Dictionary<string, long>> { dict2, dict2 };
        var listOfDict3 = new List<Dictionary<string, long>> { dict3, dict3 };
        var source = new[]
        {
            new { Prop = listOfDict1, Close = 1 },
            new { Prop = listOfDict2, Close = 2 },
            new { Prop = listOfDict3, Close = 3 }
        };

        // act
        var result = _engine.Process(source, "prop[0][key1],close WHERE prop[0][key2] > 2 ORDER BY  prop[0][key3] ASC", null);
        var list = result as List<IDictionary<string, object>>;

        // assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(2);
        list[0].Keys.Count.Should().Be(2);
        list[0].ContainsKey("key1").Should().BeTrue();
        list[0].ContainsKey("close").Should().BeTrue();
        list[0]["key1"].Should().Be(25L);
        list[0]["close"].Should().Be(3);
        list[1]["key1"].Should().Be(15L);
        list[1]["close"].Should().Be(2);
    }
}