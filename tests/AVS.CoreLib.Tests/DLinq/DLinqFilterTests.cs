using System;
using System.Collections.Generic;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.Extensions.Dynamic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.DLinq;

[TestClass]
public class DLinqFilterTests
{
    [TestMethod]
    public void Filter_Single_Property_Should_Return_List()
    {
        //arrange
        var time = DateTime.Now;
        var source = new[] { time, time.AddDays(1), time.AddHours(2) };

        // act
        var result = source.Filter("Day");
        var list = result as List<int>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);
        list[0].Should().Be(time.Day);
    }

    [TestMethod]
    public void Filter_Single_Property_Should_Not_Be_Case_Sensitive()
    {
        //arrange
        var time = DateTime.Now;
        var source = new[] { time, time.AddDays(1), time.AddHours(2) };

        // act
        var result = source.Filter("day");
        var list = result as List<int>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);
        list[0].Should().Be(time.Day);
    }

    [TestMethod]
    public void Filter_Multiple_Same_Type_Properties_Should_Return_List_Of_Typed_Dictionaries()
    {
        //arrange
        var time = DateTime.Now;
        var source = new[] { time, time.AddDays(1), time.AddHours(2) };

        // act
        var result = source.Filter("day,hour");
        var list = result as List<IDictionary<string,int>>;
        
        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);

        var dict = list[0];
        dict.Keys.Count.Should().Be(2);
        dict.ContainsKey("day").Should().BeTrue();
        dict.ContainsKey("hour").Should().BeTrue();
        dict["day"].Should().Be(time.Day);
        dict["hour"].Should().Be(time.Hour);
    }

    [TestMethod]
    public void Filter_Multiple_Different_Type_Properties_Should_Return_List_Of_Object_Dictionaries()
    {
        //arrange
        var time = DateTime.Now;
        var source = new[] { time, time.AddDays(1), time.AddHours(2) };

        // act
        var result = source.Filter("day,ticks,DayOfWeek");
        var list = result as List<IDictionary<string, object>>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);

        var dict = list[0];
        dict.Keys.Count.Should().Be(3);
        dict.ContainsKey("day").Should().BeTrue();
        dict.ContainsKey("ticks").Should().BeTrue();
        dict.ContainsKey("DayOfWeek").Should().BeTrue();
        dict["day"].Should().Be(time.Day);
        dict["ticks"].Should().Be(time.Ticks);
        dict["DayOfWeek"].Should().Be(time.DayOfWeek);
    }

    [TestMethod]
    public void Filter_By_Index_For_Single_Property_Of_Array_Type_Should_Return_List_Of_Values()
    {
        //arrange

        var source = new object[] { new { Prop = new []{1,2} }, new { Prop = new[] { 3, 4 } }, new { Prop = new[] { 5, 6 } } };

        // act
        var result = source.Filter("prop[1]");
        var list = result as List<int>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);
        list[0].Should().Be(2);
        list[1].Should().Be(4);
        list[2].Should().Be(6);
    }

    [TestMethod]
    public void Filter_By_Index_Skip_Expression_Should_Be_Handled_Properly()
    {
        //arrange
        var source = new object[] { new { Prop = new[] { 1, 2 } }, new { Prop = new[] { 3, 4 } }, new { Prop = new[] { 5, 6 } } };

        // act
        var result = source.Filter("prop[1] SKIP 2");
        var list = result as List<int>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(1);
        list[0].Should().Be(6);
    }

    [TestMethod]
    public void Filter_By_Index_Skip_And_Take_Expressions_Should_Be_Handled_Properly()
    {
        //arrange
        var source = new object[] { new { Prop = new[] { 1, 2 } }, new { Prop = new[] { 3, 4 } }, new { Prop = new[] { 5, 6 } }, new { Prop = new[] { 7, 8 } } };

        // act
        var result = source.Filter("prop[1] SKIP 2 TAKE 1");
        var list = result as List<int>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(1);
        list[0].Should().Be(6);
    }

    [TestMethod]
    public void Filter_By_Index_For_Single_Property_Of_List_Type_Should_Return_List_Of_Values()
    {
        //arrange
        var source = new[] { new { Prop = new List<int>()}, new { Prop = new List<int>() }, new { Prop = new List<int>() } };
        source[0].Prop.Add(1);
        source[0].Prop.Add(10);
        source[1].Prop.Add(2);
        source[1].Prop.Add(20);
        source[2].Prop.Add(3);
        source[2].Prop.Add(30);

        // act
        var result = source.Filter("prop[1]");
        var list = result as List<int>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);
        list[0].Should().Be(10);
        list[1].Should().Be(20);
        list[2].Should().Be(30);
    }

    [TestMethod]
    public void Filter_By_Key_For_Single_Property_Should_Return_List_Of_Values()
    {
        //arrange
        var source = new[] { new { Prop = new Dictionary<string, int>()}, new { Prop = new Dictionary<string, int>() }, new { Prop = new Dictionary<string, int>() } };
        source[0].Prop.Add("key1", 1);
        source[0].Prop.Add("key2", 10);
        source[1].Prop.Add("key1", 2);
        source[1].Prop.Add("key2", 20);
        source[2].Prop.Add("key1", 3);
        source[2].Prop.Add("key2", 30);
 
        // act
        var result = source.Filter("prop[\"key1\"]");
        var result2 = source.Filter("prop[\"key2\"]");
        var list = result as List<int>;
        var list2 = result2 as List<int>;

        //assert
        Assert.IsNotNull(list);
        Assert.IsNotNull(list2);
        list.Count.Should().Be(source.Length);
        list2.Count.Should().Be(source.Length);
        list[0].Should().Be(1);
        list[1].Should().Be(2);
        list[2].Should().Be(3);

        list2[0].Should().Be(10);
        list2[1].Should().Be(20);
        list2[2].Should().Be(30);
    }

    [TestMethod]
    public void Filter_Multiple_Properties_By_Key_Should_Return_List_Of_Typed_Dictionaries()
    {
        //arrange
        var source = new[] { new { Prop = new Dictionary<string, int>() }, new { Prop = new Dictionary<string, int>() }, new { Prop = new Dictionary<string, int>() } };
        source[0].Prop.Add("key1", 1);
        source[0].Prop.Add("key2", 10);
        source[0].Prop.Add("key3", 100);
        source[1].Prop.Add("key1", 2);
        source[1].Prop.Add("key2", 20);
        source[1].Prop.Add("key3", 200);
        source[2].Prop.Add("key1", 3);
        source[2].Prop.Add("key2", 30);
        source[2].Prop.Add("key3", 300);

        // act
        var result = source.Filter("prop[\"key2\"],prop[\"key3\"]");
        
        var list = result as List<IDictionary<string, int>>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);
        list[0].ContainsKey("key1").Should().BeFalse();
        list[0].ContainsKey("key2").Should().BeTrue();
        list[0].ContainsKey("key3").Should().BeTrue();

        list[0]["key2"].Should().Be(10);
        list[1]["key2"].Should().Be(20);
        list[2]["key2"].Should().Be(30);
    }

    [TestMethod]
    public void Filter_Multiple_Properties_By_Key_Should_Return_List_Of_Typed_Dictionaries2()
    {
        //arrange
        var source = new[] { new { Prop = new Dictionary<string, DateTime>() }, new { Prop = new Dictionary<string, DateTime>() }, new { Prop = new Dictionary<string, DateTime>() } };
        var time = DateTime.Now;
        var time1 = time.AddDays(1);
        var time2 = time.AddDays(2).AddHours(1);
        var time3 = time.AddDays(10).AddHours(2);
        source[0].Prop.Add("key1", time);
        source[0].Prop.Add("key2", time1);
        source[0].Prop.Add("key3", time.AddDays(10));
        source[1].Prop.Add("key1", time.AddHours(1));
        source[1].Prop.Add("key2", time2);
        source[1].Prop.Add("key3", time.AddDays(10).AddHours(1));
        source[2].Prop.Add("key1", time.AddHours(2));
        source[2].Prop.Add("key2", time3);
        source[2].Prop.Add("key3", time.AddDays(10).AddHours(2));

        // act
        var result = source.Filter("prop[\"key2\"].day,prop[\"key3\"].hour");

        var list = result as List<IDictionary<string, int>>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);
        list[0].ContainsKey("key2").Should().BeFalse();
        list[0].ContainsKey("day").Should().BeTrue();
        list[0].ContainsKey("hour").Should().BeTrue();

        list[0]["day"].Should().Be(time1.Day);
        list[1]["day"].Should().Be(time2.Day);
        list[2]["day"].Should().Be(time3.Day);
    }

    [TestMethod]
    public void Filter_Multiple_Properties_By_Key_Should_Return_List_Of_Object_Dictionaries()
    {
        //arrange
        var source = new[] { new { Prop = new Dictionary<string, DateTime>() }, new { Prop = new Dictionary<string, DateTime>() }, new { Prop = new Dictionary<string, DateTime>() } };
        var time = DateTime.Now;
        var time2 = time.AddDays(2).AddHours(1);
        var time3 = time.AddDays(10).AddHours(2);
        source[0].Prop.Add("key1", time);
        source[0].Prop.Add("key2", time.AddDays(1));
        source[0].Prop.Add("key3", time.AddDays(10));
        source[1].Prop.Add("key1", time.AddHours(1));
        source[1].Prop.Add("key2", time2);
        source[1].Prop.Add("key3", time.AddDays(10).AddHours(1));
        source[2].Prop.Add("key1", time.AddHours(2));
        source[2].Prop.Add("key2", time3);
        source[2].Prop.Add("key3", time3);

        // act
        var result = source.Filter("prop[\"key2\"].day,prop[\"key3\"].ticks,prop.keys.count");

        var list = result as List<IDictionary<string, object>>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);
        list[0].ContainsKey("key2").Should().BeFalse();
        list[0].ContainsKey("day").Should().BeTrue();
        list[0].ContainsKey("ticks").Should().BeTrue();
        list[0].ContainsKey("count").Should().BeTrue();

        list[0]["day"].Should().Be(time.AddDays(1).Day);
        list[1]["day"].Should().Be(time2.Day);
        list[2]["day"].Should().Be(time3.Day);
    }

    [TestMethod]
    public void Filter_Multiple_Properties_By_Key_When_Some_Item_Contains_Null_Should_Return_Default_Value()
    {
        //arrange
        var source = new[] { new { Prop = new Dictionary<string, DateTime>() }, new { Prop = new Dictionary<string, DateTime>() }, new { Prop = new Dictionary<string, DateTime>() } };
        var time = DateTime.Now;
        source[0].Prop.Add("key1", time);
        source[0].Prop.Add("key2", time.AddDays(1));
        source[0].Prop.Add("key3", time.AddDays(10));
        source[1].Prop.Add("key1", time.AddHours(1));
        //source[1].Prop.Add("key2", time.AddDays(2).AddHours(1)); //null value
        source[1].Prop.Add("key3", time.AddDays(10).AddHours(1));
        source[2].Prop.Add("key1", time.AddHours(2));
        source[2].Prop.Add("key2", time.AddDays(3).AddHours(2));
        source[2].Prop.Add("key3", time.AddDays(10).AddHours(2));

        //TODO ideally add nullable operator ? e.g. prop["key2"]?.day even prop["key2"]?.day ?? 0

        // act
        var result = source.Filter("prop[\"key2\"].day,prop[\"key3\"].ticks,prop.keys.count", mode: SelectMode.ToListSafe);

        var list = result as List<IDictionary<string, object>>;

        //assert
        Assert.IsNotNull(list);
        list.Count.Should().Be(source.Length);
        list[0].ContainsKey("key2").Should().BeFalse();
        list[0].ContainsKey("day").Should().BeTrue();
        list[0].ContainsKey("ticks").Should().BeTrue();
        list[0].ContainsKey("count").Should().BeTrue();
        list[0].Count.Should().Be(3);

        list[1].Count.Should().Be(3);
        list[1].ContainsKey("day").Should().BeTrue();
        list[1]["day"].Should().Be(0);
        list[1].ContainsKey("ticks").Should().BeTrue();
        list[1].ContainsKey("count").Should().BeTrue();
    }
}