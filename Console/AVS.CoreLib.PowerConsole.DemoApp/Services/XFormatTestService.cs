using System;
using System.Threading.Tasks;
using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.Text;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    internal class XFormatTestService : TestService
    {
        public override Task TestAsync(string[] args)
        {
            try
            {
                //X.FormatProvider.AppendFormatter(new OhlcFormatter());
                //X.FormatProvider.AppendFormatter(new PriceFormatter());
                //X.FormatProvider.AppendFormatter(new PairStringFormatter());
                //X.FormatProvider.AppendFormatter(new TradingEnumsFormatter());
                //X.FormatProvider.AppendFormatter(new CurrencySymbolFormatter());
            }
            catch (Exception ex)
            {
                PowerConsole.PrintError(ex);
            }
            return Task.CompletedTask;
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

            PowerConsole.Print($"a|amount format:",colors: ConsoleColor.DarkYellow);

            foreach (var d in arr)
            {
                var formatted = X.Format($"{d:amount}");
                PowerConsole.Print($"{d:G} => {formatted}");
            }

            PowerConsole.Print($"\r\nt|total format:", colors: ConsoleColor.Green);
            foreach (var d in arr)
            {
                var formatted = X.Format($"{d:total}");
                PowerConsole.Print($"{d:G} => {formatted}");
            }

            PowerConsole.Print($"\r\np|price format:", colors: ConsoleColor.Cyan);
            foreach (var d in arr)
            {
                var formatted = X.Format($"{d:price}");
                PowerConsole.Print($"{d:G} => {formatted}");
            }
        }
    }
}