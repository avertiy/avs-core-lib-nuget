#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Abstractions.XBars;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Extensions;
using AVS.CoreLib.Trading.TA.Indicators;

namespace AVS.CoreLib.Trading.TA
{
    public interface ICalc
    {
        object? Process(decimal source);
        string GetName();
    }
    public class BarMapper
    {
        private readonly List<ICalc> _calcList = new();
        public Dictionary<string, object?> Process(decimal source)
        {
            var output = new Dictionary<string, object?>(_calcList.Count);
            foreach (var calc in _calcList)
            {
                var result = calc.Process(source);
                var key = calc.GetName();
                output.Add(key, result);
            }

            return output;
        }
    }

    public static class BarExtensions
    {
        public static IEnumerable<T> MapTo<T>(this IEnumerable<IBar> items, TimeFrame timeframe,
            Func<IBar, decimal>? selector = null, TAInputs? inputs = null) where T: class, IXBar, IPropertyBag, new()
        {
            inputs ??= new TAInputs();
            var avgLengthCalc = new AvgCalculator(inputs.AvgVolumeLength);
            var avgVolumeCalc = new AvgCalculator(inputs.AvgVolumeLength);
            var ma14calc = new SMACalculator<MA>(inputs.MA14Length);
            var ma21calc = new SMACalculator<MA>(inputs.MA21Length);
            var ma50calc = new SMACalculator<MA>(inputs.MA50Length);
            var ma100calc = new SMACalculator<MA>(inputs.MA100Length);
            var bb21calc = new BBCalculator(inputs.BBLength, inputs.StdDevMul, inputs.StdDevMulNarrow);

            var barMapper = new BarMapper();
            var bar = new T();
            
            var src = items.First().Close;
            var values = barMapper.Process(src);
            foreach (var kp in values)
            {
                bar[kp.Key] = kp.Value;
                yield return bar;
            } 
        }

        public static IEnumerable<XBar> MapToXBar(this IEnumerable<IBar> source, TimeFrame timeframe, Func<IBar, decimal>? selector = null, TAInputs? inputs = null)
        {
            inputs ??= new TAInputs();
            var avgLengthCalc = new AvgCalculator(inputs.AvgVolumeLength);
            var avgVolumeCalc = new AvgCalculator(inputs.AvgVolumeLength);
            var ma14calc = new SMACalculator<MA>(inputs.MA14Length);
            var ma21calc = new SMACalculator<MA>(inputs.MA21Length);
            var ma50calc = new SMACalculator<MA>(inputs.MA50Length);
            var ma100calc = new SMACalculator<MA>(inputs.MA100Length);
            var bb21calc = new BBCalculator(inputs.BBLength, inputs.StdDevMul, inputs.StdDevMulNarrow);

            foreach (var item in source)
            {
                var src = selector?.Invoke(item) ?? item.Close;
                var decPlaces = src.GetDecimalPlaces();
                var bar = new XBar()
                {
                    //bar props
                    Open = item.Open,
                    Close = item.Close,
                    Low = item.Low,
                    High = item.High,
                    Volume = item.Volume,
                    Time = item.Time,
                    Total = item.Total,

                    //x-bar props
                    Length = item.GetLength(),
                    BodyLength = item.GetBodyLength(),
                    Type = item.GetBarType(),
                    Avg = item.GetAvgPrice(decPlaces),
                    Mid = item.GetMediana(decPlaces),
                    SizeAbs = item.GetBarSizeAbs(timeframe),
                };

                var avgLength = avgLengthCalc.Process(bar.Length);
                bar.Size = item.GetBarSize(avgLength);

                var avgVolume = avgVolumeCalc.Process(bar.Volume);
                bar.IncreasedVolume = bar.Volume > avgVolume * 2;

                //TA-props 
                bar.MA14 = ma14calc.Process(src);
                bar.MA21 = ma21calc.Process(src);
                bar.MA50 = ma50calc.Process(src);
                bar.MA100 = ma100calc.Process(src);
                bar.BB21 = bb21calc.Process(src);

                yield return bar;
            } 
        }
    }
}