using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Abstractions.Bootstrap;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.FormatPreprocessors;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    internal class ConsoleFeaturesDemoService : DemoService
    {
        public override async Task DemoAsync()
        {
            try
            {
                //FormatPreprocessorTest();
                WriteTests();
                CTagsTests();
                ColorSchemeTests();

                //format features tests
                PrintHeader();
                PrintArrayTests();
                PrintTableTests();
                //PrintColorPaletteTest();


                PrintAllColors();
                
                PrintTest();
                PrintTimeElapsed();
                PrintFTests();
                
                PrintColorString();
                PrintTableFormattedString();
                ClearLine();
                
                await PromptAsync();
                await ReadLineAsync();
                Fonts();
            }
            catch (Exception ex)
            {
                PowerConsole.PrintError(ex);
            }
        }

        private void FormatPreprocessorTest()
        {
            PowerConsole.PrintHeader($"Format Preprocessor Test");
            X.FormatPreprocessor.Add(new EnumFormatPreprocessor());
            PowerConsole.PrintF($"Supposed to be DarkGray: {TradeType.Buy}");
            PowerConsole.PrintF($"Supposed to be Cyan: {TradeType.Sell}");
        }


        private void CTagsTests()
        {
            PowerConsole.PrintHeader($" CTags Test");

            PowerConsole.Print("text colorized by yellow ctag", CTag.Yellow);
            PowerConsole.Print("text with color tags <Cyan>cyan</Cyan>", PrintOptions.CTags());
            PowerConsole.Print("text with <Red>a few tags </Red><Cyan>cyan</Cyan>", PrintOptions.CTags());
            PowerConsole.Print("text with <Red>a few tags <Cyan>cyan</Cyan></Red>", PrintOptions.CTags());
        }

        private void PrintColorPaletteTest()
        {
            PowerConsole.PrintHeader($"ColorPalette print test");
            PowerConsole.PrintF($"some text {TradeType.Buy} some other text {TradeType.Sell}", ColorPalette.RedGreen);
            PowerConsole.PrintF($"some text {TradeType.Buy} some other text {TradeType.Sell}", new[] { ConsoleColor.DarkYellow, ConsoleColor.Cyan });
        }

        public void PrintColorString()
        {
            PowerConsole.PrintHeader($"PrintF(ColoredString)  [format: {{text:-Color}}]");
            PowerConsole.PrintF($"{{text in red background:--Red$}} ### some other text {{green text::-Green}}");
        }

        public void PrintHeader()
        {
            PowerConsole.PrintHeader($"This is a header");
            PowerConsole.Print("this is regular text");
        }

        public void ColorSchemeTests()
        {
            PowerConsole.Print("this is (dark blue / white) color scheme", new ColorScheme(ConsoleColor.DarkBlue, ConsoleColor.White));
        }

        public void PrintArrayTests()
        {
            PowerConsole.PrintHeader($"PrintArray");
            var array = new List<double> { 1.0, 2.999, -3000.001, 0, 10.99, 0.1234567890 };
            PowerConsole.PrintArray(array);
            PowerConsole.PrintHeader($"PrintArray with formatter");
            PowerConsole.PrintArray(array, formatter:x => x + "m");
        }

        public void PrintTest()
        {
            PowerConsole.PrintHeader($"PrintTest");
            PowerConsole.PrintTest(true, "true condition", 30);
            PowerConsole.PrintTest(false, "false condition", 30);
        }

        public void PrintTimeElapsed()
        {
            PowerConsole.PrintTimeElapsed(DateTime.Now.AddMilliseconds(-1999), "time elapsed test");
        }

        public void ClearLine()
        {
            PowerConsole.PrintHeader($"ClearLine");
            PowerConsole.Print("The next line will write 123456789 than clear leaving only 4 symbols");
            PowerConsole.Write("123456789");
            PowerConsole.ClearLine(4);
        }

        public void WriteTests()
        {
            PowerConsole.Write("simple write ");
            PowerConsole.Write(" ConsoleColor.Cyan ", ConsoleColor.Cyan);
            PowerConsole.Write(" ColorScheme.DarkRed ", ColorScheme.DarkRed);

            PowerConsole.PrintHeader($"WriteLine tests");
            PowerConsole.WriteLine("write line");
            PowerConsole.WriteLine("write line ConsoleColor.DarkYellow with timestamp", ConsoleColor.DarkYellow);
            PowerConsole.WriteLine("write line ColorScheme.DarkYellow", ColorScheme.DarkYellow);


            PowerConsole.PrintHeader($"WriteLine message status tests");
            PowerConsole.WriteLine("write line debug", MessageLevel.Debug);
            PowerConsole.WriteLine("write line default", MessageLevel.Default);
            PowerConsole.WriteLine("write line info", MessageLevel.Info);
            PowerConsole.WriteLine("write line warning", MessageLevel.Warning);
            PowerConsole.WriteLine("write line error", MessageLevel.Error);
            
            

            PowerConsole.PrintDebug("this is a debug message");
            try
            {
                throw new Exception("exception");
            }
            catch (Exception ex)
            {
                PowerConsole.PrintError(ex);
            }
        }

        public async Task ReadLineAsync()
        {
            PowerConsole.PrintHeader($"ReadLineAsync");
            PowerConsole.Print($"read line timeout 4 sec with a default text");
            var read = await PowerConsole.ReadLineAsync(4000, "default text");
            PowerConsole.Print($"read result: {read}");
        }

        public void Fonts()
        {
            PowerConsole.PrintHeader($"Fonts");
            var fontInfo = PowerConsole.GetFont();
            PowerConsole.Print($"the default font is: {fontInfo}");
            PowerConsole.Print($"setting font size to 28");
            Thread.Sleep(1000);
            PowerConsole.SetFont(fontInfo.FontName, 28, 400);
            PowerConsole.Print($"delay 2 second");
            Thread.Sleep(2000);
            PowerConsole.RestoreDefaultFont();
        }

        public void PrintAllColors()
        {
            PowerConsole.PrintAllColors();
        }

        public void PrintFTests()
        {
            //sorry not supported yet
            //PrintF requires format provider which is not brought here yet
            var arguments = new object[] { "abc", 0, DateTime.Today };
            PowerConsole.PrintHeader($"PrintF tests:");
            PowerConsole.Print("1. PrintF($\"{100500:-Red} {abc:--DarkGray} {DateTime.Today:-DarkGreen dd/MM/yyyy}\":");
            PowerConsole.PrintF($"{100500:-Red} {"abc":--DarkGray} {DateTime.Today:-DarkGreen dd/MM/yyyy}");
            PowerConsole.Print("2. PrintF($\"{arguments[0]:-Green}; {arguments[0]:--Green}; {arguments[0]:!-Blue}\":");
            PowerConsole.PrintF($"{arguments[0]:-Green}; {arguments[0]:--Green}; {arguments[0]:!-Blue}");

            var tradingTypes = new object[] { OrderSide.Buy, TradeType.Sell, PositionType.Long };

            PowerConsole.PrintF($"{tradingTypes[0]}; {tradingTypes[0]:-Yellow c}; {tradingTypes[0]:-Blue --Yellow n}");


            //PowerConsole.PrintF($"2) @text before `arg[0]: {arguments[0]:--DarkYellow};``arg[1]: {arguments[1]:!-DarkRed};``arg[2]: {arguments[2]:!}`");

            //PowerConsole.PrintF($"4) <square>5x5:`abc`</square>");
        }

        public void PrintTableTests()
        {
            PowerConsole.PrintHeader($"PrintTable");
            var arr = new[] {
                new
                {
                    Price = 0.051132156m,
                    Column2 = DateTime.Now,
                    Amount = 10.50m,
                    Column3="asd ads d qwd qw",
                    Column4 = new { Prop1 = 1, Prop2 = 100}
                },
                new
                {
                    Price = 0.003m,
                    Column2 = DateTime.Now,
                    Amount = 120.09879877m,
                    Column3 = "asd ads d qwd qw 213 21321 321 3213",
                    Column4 = new { Prop1 = 2, Prop2 = 200}
                } };

            PowerConsole.PrintTable(arr, ConsoleColor.DarkCyan);
            PowerConsole.WriteLine();

            var table = Table.Create(new[] { "Buys", "Sells", "Column 3", "Column 4" }, new[] { ColorScheme.DarkGreen, ColorScheme.DarkRed });
            table.AddRow().AddCell("row 1 colspan = 4", 4);
            table.AddRow().AddCell("row 2 colspan = 3", 3).AddCell("Cell 4");
            table.AddRow().AddCell("row 3 cell 1").AddCell("Cell 2       -        3", 2).AddCell("cell 4");
            table.AddRow().AddCell("row 4 cell 1").AddCell("Cell 2    -    3", 2);
            table.AddRow(new object[] { "buys trade2", "sell trade2", "adasdasd", "sadasd adqw 23 2323" });
            table.AddRow(new object[] { "cell 3 asdas dasdasd das d", "", "adasdasd", "sadasd adqw 23 2323" });
            table.AddRow(new object[] { "", "cell 4 sdfdsf eewewer", "adasdasd", "sadasd adqw 23 2323" });

            PowerConsole.PrintTable(table);
        }

        public void PrintTableFormattedString()
        {
            //table formatter not implemented yet
            //PowerConsole.PrintHeader($"PrintF table formatted string test:");
            //PowerConsole.PrintF($"<table><header>Column1|Col2|EmptyColumn</header><body><cell>$$text:-Red$</cell>... </table>");
            //$"@any text {arr:table}" => "@any text\r\n<table><header>Price|Amount|EmptyColumn</header><body><cell>0.051</cell><cell>10.50</cell>\r\n<cell>0.003</cell><cell>120</cell></body></table>" 
            //PowerConsole.PrintF($"{arr:-Yellow table}"); //$$<table>...</table>:-Red$
            //PowerConsole.PrintTable(arr, (column, cell, scheme)=>{...}, ConsoleColor.Red);
        }

        public async Task PromptAsync()
        {
            PowerConsole.PrintHeader($"PromptYesNo");
            var answer = await PowerConsole.PromptYesNoAsync("Do you confirm ...?", 5000);
            PowerConsole.Print($"Got an answer: {answer}");
        }
    }
}
