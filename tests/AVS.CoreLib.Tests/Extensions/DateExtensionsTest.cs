using System;
using AVS.CoreLib.Dates.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.Extensions
{
    [TestClass]
    public class DateExtensionsTest
    {
        [TestMethod]
        public void RownDownTest()
        {
            var dt1 = new DateTime(2021,1,1, 8, 48, 37);
            var dt2 = new DateTime(2021,1,1, 12, 16, 37);
            var dt3 = new DateTime(2021,1,1, 2, 05, 50);

            //round down to hour
            var result = dt1.RoundDown(TimeSpan.FromSeconds(60 * 60));
            result.Should().Be(new DateTime(2021, 1, 1, 8, 0, 0));
            result = dt2.RoundDown(TimeSpan.FromSeconds(60 * 60));
            result.Should().Be(new DateTime(2021, 1, 1, 12, 0, 0));
            result = dt3.RoundDown(TimeSpan.FromSeconds(60 * 60));
            result.Should().Be(new DateTime(2021, 1, 1, 2, 0, 0));


            //round down to 30M
            result = dt1.RoundDown(TimeSpan.FromSeconds(60 * 30));
            result.Should().Be(new DateTime(2021, 1, 1, 8, 30, 0));
        }
    }
}
