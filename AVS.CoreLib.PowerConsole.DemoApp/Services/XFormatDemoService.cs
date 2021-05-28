using System;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Text;
using AVS.CoreLib.Trading.FormatProviders;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    internal class XFormatDemoService : IDemoService
    {
        public async Task DemoAsync()
        {
            try
            {
                X.FormatProvider.AppendFormatter(new PriceFormatter());
                X.FormatProvider.AppendFormatter(new PairStringFormatter());
                X.FormatProvider.AppendFormatter(new TradingEnumsFormatter());
                X.FormatProvider.AppendFormatter(new CurrencySymbolFormatter());
                PowerConsole.Format = X.Format;
                PriceFormatterDemo();

            }
            catch (Exception ex)
            {
                PowerConsole.WriteError(ex);
            }
        }

        private void PriceFormatterDemo()
        {
            PowerConsole.PrintHeader($"PriceFormatter decimals");
            var arr = new decimal[]
            {
                1234567890.123456789m,
                123456789.123456789m, 12345678.123456789m,
                1234567.54321m, 123456.54321m, 12345.54321m,
                1222.12345m,
                1111.12345m, 
                998.54321m, 
                99.54321m, 
                9.54321m,
                1.0123456789m,
                0.987654321m,
                0.012345678m,
                0.001234567m,
                0.000123456m,
                0.000012345m,
                0.000001234m,
                0.000000123m,
                0.000000069m,
            };

            PowerConsole.Print($"a|amount format:", ConsoleColor.DarkYellow);

            foreach (var d in arr)
            {
                var formatted = X.Format($"{d:amount}");
                PowerConsole.Print($"{d:G} => {formatted}");
            }

            PowerConsole.Print($"\r\nt|total format:", ConsoleColor.Green);
            foreach (var d in arr)
            {
                var formatted = X.Format($"{d:total}");
                PowerConsole.Print($"{d:G} => {formatted}");
            }

            PowerConsole.Print($"\r\np|price format:", ConsoleColor.Cyan);
            foreach (var d in arr)
            {
                var formatted = X.Format($"{d:price}");
                PowerConsole.Print($"{d:G} => {formatted}");
            }
        }
    }
}