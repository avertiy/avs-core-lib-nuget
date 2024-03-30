using System;
using System.Collections.Generic;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq.LambdaSpec;
using AVS.CoreLib.DLinq0.LambdaSpec0;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.DLinq;

[TestClass]
public class PropSpecTests
{
    [TestMethod]
    public void Test_SimpleSpec()
    {
        //arrange
        var time = DateTime.Now;
        var source = new[] { time, time.AddDays(1), time.AddDays(2) };
        var spec = new PropSpec() { Name = "Day"};

        // act
        var lambda = spec.Build<DateTime>();
        var fn = lambda.Compile();
        var result = fn(source);

        // assert
        var list = result as List<int>;
        Assert.IsNotNull(list);
        list.Count.Should().Be(3);
    }

    [TestMethod]
    public void Test_SimpleSpec_With_GetSelectFn()
    {
        //arrange
        var time = DateTime.Now;
        var source = new[] { time, time.AddDays(1), time.AddDays(2) };
        var spec = new PropSpec() { Name = "Day" };
        var bag = new LambdaBag();

        // act
        var fn = bag.GetSelectFn<DateTime>(spec);
        var result = fn(source);

        // assert
        var list = result as List<int>;
        Assert.IsNotNull(list);
        list.Count.Should().Be(3);
    }
}