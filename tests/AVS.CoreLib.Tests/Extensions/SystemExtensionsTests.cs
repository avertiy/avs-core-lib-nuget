using System;
using System.Linq;
using AVS.CoreLib.Dates;
using AVS.CoreLib.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.Extensions;

[TestClass]
public class SystemExtensionsTests
{
    [TestMethod]
    public void GetStackTraceLines_Should_Return_Lines()
    {
        // Arrange
        var error = GetTestException();

        // Act
        var lines = error.GetStackTraceLines();

        // Assert

        Assert.IsNotNull(lines);
        lines.Length.Should().BeGreaterThan(5);
        lines.Any(x => x.StartsWith("InnerException:")).Should().BeTrue();
        //lines.Any(x => x)
    }

    private Exception GetTestException()
    {
        try
        {
            ThrowTestException();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private void ThrowTestException()
    {
        try
        {
            var dt = DateRange.Create(DateTime.Now, DateTime.Today);
        }
        catch(Exception ex)
        {
            throw new ApplicationException("Test exception", ex);
        }
    }
}