using Assert = AVS.CoreLib.UnitTesting.xUnit.Assert;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.TA.Calculators.Oscillators;
using AVS.CoreLib.Trading.TA.Indicators;
using AVS.CoreLib.Trading.Extensions;

namespace AVS.CoreLib.Trading.Tests
{
    public class OscillatorsTests
    {
        [Theory]
        [InlineData(14, 3, 5)]
        public void StochCalculator_Returns_K_and_D_After_Period_Iterated(int period1, int period2, int period3)
        {
            // Arrange
            var data = MarketDataGenerator.GenerateAsc(period1 + period3, bullFactor: 2);

            var bearCount = data.Count(x => x.IsBearish());

            var calculator = new StochCalculator(period1, period2, period3);

            // Act
            for (var i = 0; i < data.Length; i++)
            {
                var value = (Stoch?)calculator.Invoke(data[i]);

                if (i < period1 - 1)
                {
                    Assert.Null(value, $"Value supposed to be null (i={i})");
                    continue;
                }

                Assert.NotNull(value, $"Value supposed to be not null (i={i})");
                Assert.WithinRange(value.K, (0m, 100m), $"K must be within [0;100] range (i={i})");
                Assert.WithinRange(value.D, (0m, 100m), $"D must be within [0;100] range (i={i})");
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
                    Assert.Null(value, $"Value supposed to be null (i={i})");
                    continue;
                }

                Assert.NotNull(value, $"Value supposed to be not null (i={i})");
                Assert.WithinRange(value.K, (0m, 100m), $"K must be within [0;100] range (i={i})");
                Assert.WithinRange(value.D, (0m, 100m), $"D must be within [0;100] range (i={i})");
            }
        }


        [Theory]
        [InlineData(12)]
        public void RSICalculator_Return_Value_After_Period_Iterated(int period)
        {
            // Arrange
            var data = MarketDataGenerator.GenerateAsc(period + 3, bullFactor: period);

            var bearCount = data.Count(x => x.IsBearish());

            var calculator = new RSICalculator(period);

            // Act
            for (var i = 0; i < data.Length; i++)
            {
                var rsi = (RSI?)calculator.Invoke(data[i]);

                if (i < period - 1)
                {
                    Assert.Null(rsi, $"RSI({period}) supposed to be null (i={i})");
                    continue;
                }

                Assert.NotNull(rsi, $"RSI({period}) supposed to be not null (i={i})");

                if(i == period - 1)
                {
                    Assert.Equal(100, rsi.Value, $"RSI({period}) must be 100 (i={i})");
                }
                else
                {
                    Assert.WithinInclRange(rsi.Value, (0m, 100m), $"RSI({period}) must be within range [0;100] (i={i})");                    
                }
            }
        }
    }
}