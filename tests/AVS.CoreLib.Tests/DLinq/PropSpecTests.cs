using System;
using System.Collections.Generic;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.DLinq.Extensions;
using AVS.CoreLib.DLinq.Specs.BasicBlocks;
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
        var result = source.Select(spec, SelectMode.ToList);

        // assert
        var list = result as List<int>;
        Assert.IsNotNull(list);
        list.Count.Should().Be(3);
    }
}