using System.Linq;
using AVS.CoreLib.Collections;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.Extensions;

[TestClass]
public class FixedListTests
{
    [TestMethod]
    public void Should_Add_Items()
    {
        //arrange
        var list = new FixedList<int>(2);

        // act
        list.Add(1);
        list.Add(2);

        var is3Added = list.TryAdd(3);

        //assert
        list.Count.Should().Be(2);
        list[0].Should().Be(1);
        list[1].Should().Be(2);

        is3Added.Should().BeFalse();

    }

    [TestMethod]
    public void Should_Throw_When_Adding_Over_Capacity()
    {
        //arrange
        var list = new FixedList<int>(3) { 1, 2, 3 };

        // act
        Assert.ThrowsException<ExceedCapacityException>(() => list.Add(4));

        //assert
        list.Count.Should().Be(3);
    }

    [TestMethod]
    public void Should_Add_Removing_Older_Items()
    {
        //arrange
        var list = new FixedList<int>(3) { 1, 2, 3 };

        // act
        list.Put(4);

        //assert
        list.Count.Should().Be(3);
        var arr = list.ToArray();

        arr.Should().BeEquivalentTo(new int[] { 2, 3, 4 });

        arr[0].Should().Be(2);
        arr[1].Should().Be(3);
        arr[2].Should().Be(4);
    }

    [TestMethod]
    public void Should_Put_Item_On_Top()
    {
        //arrange
        var list = new FixedList<int>(3) { 1, 2, 3 };

        // act
        list.Put(2);

        //assert
        list.Count.Should().Be(3);
        var arr = list.ToArray();

        arr.Should().BeEquivalentTo(new int[] { 1, 3, 2 });
    }

    [TestMethod]
    public void Should_Add_To_The_End_And_Put_On_Top_Correctly()
    {
        //arrange
        var list = new FixedList<int>(5) { 1, 2, 3, 4, 5 };

        // act
        list.Put(6);
        list.Put(2);

        // assert
        var arr1 = list.ToArray();
        arr1.Should().BeEquivalentTo(new[] { 3, 4, 5, 6, 2 });

        // act
        list.Put(7);
        list.Put(2);

        //assert
        list.Count.Should().Be(5);
        var arr2 = list.ToArray();
        arr2.Should().BeEquivalentTo(new[] { 4, 5, 6, 7, 2 });
    }
}