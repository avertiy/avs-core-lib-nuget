using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.FormatPreprocessors;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    internal class ConsoleFeaturesTestService : TestService
    {
        public override async Task TestAsync(string[] args)
        {
            try
            {
                PowerConsole.SwitchColorMode(ColorMode.AnsiCodes);
                //format features tests
                PrintHeader();
                PrintArrayTests();

                PrintUtilitiesTests();
                PrintTest();
                PrintAllColors();


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
            PowerConsole.PrintF($"Supposed to be DarkGray: {ColorMode.AnsiCodes}");
            PowerConsole.PrintF($"Supposed to be Cyan: {ColorMode.PlainText}");
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

        public void PrintArrayTests()
        {
            PowerConsole.PrintHeader($"PrintArray");
            var array = new List<double> { 1.0, 2.999, -3000.001, 0, 10.99, 0.1234567890 };
            PowerConsole.PrintArray(array);
            PowerConsole.PrintHeader($"PrintArray with formatter");
            PowerConsole.PrintArray(array, formatter: x => x + "m");
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

        

        public void PrintUtilitiesTests()
        {
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

        //Colors Colors => new Colors(ConsoleColor.DarkBlue, ConsoleColor.DarkYellow);
        

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

            //PowerConsole.PrintF($"4) <square>5x5:`abc`</square>");
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
