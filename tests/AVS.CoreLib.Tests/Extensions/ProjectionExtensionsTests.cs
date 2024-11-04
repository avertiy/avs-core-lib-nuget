#nullable enable
using System.Collections.Generic;
using System.Net;
using AVS.CoreLib.Json;
using AVS.CoreLib.REST;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestResponseProjectionExtensions = AVS.CoreLib.REST.Extensions.RestResponseProjectionExtensions;

namespace AVS.CoreLib.Tests.Extensions;

[TestClass]
public class ProjectionExtensionsTests
{
    [TestMethod]
    public void ToResponse_WithProxy_Should_Deserialize()
    {
        var order = CreateOrder("#1");
        var json = order.ToJson();
        var restResponse = CreateResponse(json);

        var response = RestResponseProjectionExtensions.ToResponse<ITestOrder>(restResponse, x => x.Map<TestOrder, TestOrderProxy>());

        Assert.IsTrue(response.Success);
        Assert.IsNotNull(response.Data);
        
        response.Data.OrderId.Should().Be(order.OrderId);
        response.Data.Amount.Should().Be(order.Amount);
        response.Data.Price.Should().Be(order.Price);
    }

    [TestMethod]
    public void ToResponse_Should_Return_Failure()
    {
        var error = "Bad Request";
        var restResponse = CreateResponse(string.Empty, HttpStatusCode.BadRequest, error);

        var response = RestResponseProjectionExtensions.ToResponse<ITestOrder>(restResponse, x => x.Map<TestOrder>());

        Assert.IsFalse(response.Success);
        Assert.IsNull(response.Data);
        response.Error.Should().Be(error);
    }

    [TestMethod]
    public void ToResponse_WithProxy_Should_Deserialize_ListOfObjects()
    {
        var orders = new List<ITestOrder>() { CreateOrder("#1"), CreateOrder("#2"), CreateOrder("#3") };
        var json = orders.ToJson();
        var restResponse = CreateResponse(json);
        var response = RestResponseProjectionExtensions.ToResponse<IList<ITestOrder>>(restResponse, x => 
            x.MapArray<TestOrder, TestOrderProxy>()
            );

        Assert.IsTrue(response.Success);
        Assert.IsNotNull(response.Data);

        response.Data.Count.Should().Be(orders.Count);
        response.Data[0].OrderId.Should().Be(orders[0].OrderId);
        response.Data[1].Amount.Should().Be(orders[1].Amount);
    }

    private RestResponse CreateResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK, string? error = null)
    {
        var source = "source";
        var restResponse = new RestResponse(source, statusCode) { Content = content, Error = error };
        return restResponse;
    }

    private TestOrder CreateOrder(string orderId)
    {
        return new TestOrder() { OrderId = orderId, Amount = 100, Price = 1 };
    }
}