using Assert = AVS.CoreLib.UnitTesting.xUnit.Assert;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.TA.Calculators.Oscillators;
using AVS.CoreLib.Trading.TA.Indicators;

namespace AVS.CoreLib.Trading.Tests
{
    public class OscillatorsTests
    {
        [Theory]
        [InlineData(14, 3, 5)]
        public void StochCalculator_Returns_K_and_D_After_Period_Iterated(int period1, int period2, int period3)
        {
            var period = 14;
            // Arrange
            var data = MarketDataGenerator.GenerateAsc(period + 2, bullFactor: 2);

            var bearCount = data.Count(x => x.IsBearish());

            var calculator = new StochCalculator(period1, period2, period3);

            // Act
            for (var i = 0; i < data.Length; i++)
            {
                var value = (Stoch?)calculator.Invoke(data[i]);

                if (i < period - 1)
                {
                    Assert.Null(value, $"Value supposed to be null at bar[{i}]");
                    continue;
                }

                Assert.NotNull(value, $"Value supposed to be not null at bar[{i}]");
                Xunit.Assert.True(value.K > 0, $"K supposed to be greater than 0 at bar[{i}]");
                Xunit.Assert.True(value.D > 0, $"D supposed to be greater than 0 at bar[{i}]");
            }
        }

        [Theory]
        [InlineData(14, 3, 5)]
        public void StochCalculator_Returns_K_and_D_Correct_Values(int period1, int period2, int period3)
        {
            var atr = 8m;
            // Arrange
            var data = MarketDataGenerator.Generate(atr, period1 + 2);

            var bearCount = data.Count(x => x.IsBearish());

            var calculator = new StochCalculator(period1, period2, period3);

            //data.Take(period1).

            // Act
            for (var i = 0; i < data.Length; i++)
            {
                var value = (Stoch?)calculator.Invoke(data[i]);

                if (i < period1 - 1)
                {
                    Assert.Null(value, $"Value supposed to be null at bar[{i}]");
                    continue;
                }

                Assert.NotNull(value, $"Value supposed to be not null at bar[{i}]");
                Xunit.Assert.True(value.K > 0, $"K supposed to be greater than 0 at bar[{i}]");
                Xunit.Assert.True(value.D > 0, $"D supposed to be greater than 0 at bar[{i}]");
            }
        }
    }
}