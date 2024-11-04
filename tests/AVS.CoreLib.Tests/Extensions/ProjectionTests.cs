using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Json;
using AVS.CoreLib.REST.Projections;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.Extensions;

[TestClass]
public class ProjectionTests
{
    [TestMethod]
    public void SimpleMap_Should_Deserialize_Object()
    {
        var order = CreateOrder("#1");
        var json = order.ToJson();

        var proj = new Proj<TestOrder>(json);
        var obj = proj.Map();

        Assert.IsNotNull(obj);
        obj.OrderId.Should().Be(order.OrderId);
        obj.Amount.Should().Be(order.Amount);
        obj.Price.Should().Be(order.Price);
    }

    [TestMethod]
    public void Map_Should_Deserialize_Object()
    {
        var order = CreateOrder("#1");
        var json = order.ToJson();

        var proj = new Proj<ITestOrder>(json);
        var obj = proj.Map<TestOrder>();

        Assert.IsNotNull(obj);
        obj.OrderId.Should().Be(order.OrderId);
        obj.Amount.Should().Be(order.Amount);
        obj.Price.Should().Be(order.Price);
    }

    [TestMethod]
    public void MapArray_Should_Deserialize_ListOfObjects()
    {
        var orders = new List<ITestOrder>() { CreateOrder("#1"), CreateOrder("#2"), CreateOrder("#3") };
        var json = orders.ToJson();

        var proj = new Proj<ITestOrder>(json);
        var list = proj.MapArray<TestOrder>();

        Assert.IsNotNull(list);

        list.Count.Should().Be(orders.Count);
        list[0].OrderId.Should().Be(orders[0].OrderId);
    }

    [TestMethod]
    public void MapDictionary_Should_Deserialize_ListOfKeyValues()
    {
        var orders = new Dictionary<string, ITestOrder>() { {"key1", CreateOrder("#1")}, {"key2", CreateOrder("#2") } };
        var json = orders.ToJson();

        var proj = new Proj<ITestOrder>(json);
        var dictionary = proj.MapDictionary<TestOrder>();

        Assert.IsNotNull(dictionary);

        dictionary.Count.Should().Be(orders.Count);
        dictionary.First().Key.Should().Be(orders.First().Key);
        dictionary.First().Value.OrderId.Should().Be(orders.First().Value.OrderId);
    }

    [TestMethod]
    public void Map_WithProxy_Should_Deserialize_Object()
    {
        var order = CreateOrder("#1");
        var json = order.ToJson();

        var proj = new Proj<ITestOrder>(json);
        var obj = proj.Map<TestOrder, TestOrderProxy>();

        Assert.IsNotNull(obj);
        obj.OrderId.Should().Be(order.OrderId);
        obj.Amount.Should().Be(order.Amount);
        obj.Price.Should().Be(order.Price);
    }

    [TestMethod]
    public void MapArray_WithProxy_Should_Deserialize_Objects_And_Return_Container()
    {
        var orders = new List<ITestOrder>() { CreateOrder("#1"), CreateOrder("#2"), CreateOrder("#3") };
        var json = orders.ToJson();

        var proj = new Proj<IList<ITestOrder>>(json);
        var list = proj.MapArray<TestOrder, TestOrderProxy>();

        Assert.IsNotNull(list);

        list.Count.Should().Be(orders.Count);
        list[0].OrderId.Should().Be(orders[0].OrderId);
    }

    [TestMethod]
    public void MapDictionary_WithProxy_Should_Deserialize_Objects_And_Return_Container()
    {
        var orders = new Dictionary<string, ITestOrder>() { { "key1", CreateOrder("#1") }, { "key2", CreateOrder("#2") } };
        var json = orders.ToJson();

        var proj = new Proj<IDictionary<string,ITestOrder>>(json);
        var dictionary = proj.MapDictionary<TestOrder, TestOrderDictProxy>();

        Assert.IsNotNull(dictionary);

        dictionary.Count.Should().Be(orders.Count);
        dictionary.First().Key.Should().Be(orders.First().Key);
        dictionary.First().Value.OrderId.Should().Be(orders.First().Value.OrderId);
    }

    private TestOrder CreateOrder(string orderId)
    {
        return new TestOrder() { OrderId = orderId, Amount = 100, Price = 1 };
    }
}

public interface ITestOrder
{
    string OrderId { get; set; }
    decimal Price { get; set; }
    decimal Amount { get; set; }
}

public class TestOrderProxy : IProxy<TestOrder, ITestOrder>, IProxy<TestOrder, IList<ITestOrder>>
{
    private readonly List<ITestOrder> _list = new();
    public void Add(TestOrder item)
    {
        _list.Add(item);
    }

    public ITestOrder Create()
    {
        return _list[0];
    }

    IList<ITestOrder> IProxy<IList<ITestOrder>>.Create()
    {
        return _list;
    }
}

public class TestOrderDictProxy : IKeyedCollectionProxy<IDictionary<string, ITestOrder>, TestOrder>
{
    private readonly Dictionary<string, ITestOrder> _dict = new();
    public void Add(string key, TestOrder item)
    {
        _dict.Add(key, item);
    }

    public IDictionary<string, ITestOrder> Create()
    {
        return _dict;
    }
}

public class TestOrder : ITestOrder
{
    public string OrderId { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}