using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.PowerConsole.Utilities;
using Console = AVS.CoreLib.PowerConsole.PowerConsole;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    internal class ConsoleFeaturesDemoService : IDemoService
    {
        public async Task DemoAsync()
        {
            try
            {
                PrintHeader();
                PrintColorScheme();
                PrintAllColors();
                PrintArray();
                PrintTest();
                PrintTimeElapsed();
                PrintFormattedString();
                PrintTable();
                PrintTableFormattedString();
                ClearLine();
                WriteDebug();
                WriteError();
                await PromptAsync();
                await ReadLineAsync();
                Fonts();
            }
            catch (Exception ex)
            {
                Console.WriteError(ex);
            }
        }

        public void PrintHeader()
        {
            Console.PrintHeader($"This is a header");
            Console.Print("this is regular text");
        }

        public void PrintColorScheme()
        {
            Console.Print("this is (dark blue / white) color scheme", new ColorScheme(ConsoleColor.DarkBlue, ConsoleColor.White));
        }

        public void PrintArray()
        {
            Console.PrintHeader($"PrintArray");
            var array = new List<double> { 1.0, 2.999, -3000.001, 0, 10.99, 0.1234567890 };
            Console.PrintArray(array);
            Console.PrintHeader($"PrintArray with formatter");
            Console.PrintArray(array, x => x + "m");
        }

        public void PrintTest()
        {
            Console.PrintHeader($"PrintTest");
            Console.PrintTest("true condition", true, 30);
            Console.PrintTest("false condition", false, 30);
        }

        public void PrintTimeElapsed()
        {
            Console.PrintHeader($"PrintTimeElapsed");
            Console.PrintTimeElapsed(DateTime.Now.AddMilliseconds(-1999), "time elapsed");
        }

        public void ClearLine()
        {
            Console.PrintHeader($"ClearLine");
            Console.Print("The next line will write 123456789 than clear leaving only 4 symbols");
            Console.Write("123456789");
            Console.ClearLine(4);
        }
        public void WriteDebug()
        {
            Console.PrintHeader($"WriteDebug");
            Console.WriteDebug("this is a debug message");
        }

        public void WriteError()
        {
            Console.PrintHeader($"WriteError");
            try
            {
                throw new Exception("test exception");
            }
            catch (Exception ex)
            {
                Console.WriteError(ex);
            }
        }

        public async Task ReadLineAsync()
        {
            Console.PrintHeader($"ReadLineAsync");
            Console.Print($"read line timeout 4 sec with a default text");
            var read = await Console.ReadLineAsync(4000, "default text");
            Console.Print($"read result: {read}");
        }

        public void Fonts()
        {
            Console.PrintHeader($"Fonts");
            var fontInfo = Console.GetFont();
            Console.Print($"the default font is: {fontInfo}");
            Console.Print($"setting font size to 28");
            Thread.Sleep(1000);
            Console.SetFont(fontInfo.FontName, 28, 400);
            Console.Print($"delay 2 second");
            Thread.Sleep(2000);
            Console.RestoreDefaultFont();
        }

        public void PrintAllColors()
        {
            Console.PrintHeader($"PrintAllColors");
            Console.PrintAllColors();
        }

        public void PrintFormattedString()
        {
            //sorry not supported yet
            //PrintF requires format provider which is not brought here yet
            //object[] arguments = new object[] { "abc", 0, DateTime.Today };
            //Console.PrintHeader($"PrintF tests:");
            //Console.PrintF($"1) arg[0]: {arguments[0]:!-Green}; arg[1]: {arguments[1]:!};arg[2]: {arguments[2]:!}");
            //Console.PrintF($"2) @text before `arg[0]: {arguments[0]:--DarkYellow};``arg[1]: {arguments[1]:!-DarkRed};``arg[2]: {arguments[2]:!}`");
            //Console.PrintF($"3) {100500:-Red} {"abc":--DarkGray} {DateTime.Today:-DarkGreen dd/MM/yyyy}");
            //Console.PrintF($"4) <square>5x5:`abc`</square>");
        }

        public void PrintTable()
        {
            Console.PrintHeader($"PrintTable");
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

            Console.PrintTable(arr, ConsoleColor.DarkCyan);
        }

        public void PrintTableFormattedString()
        {
            //table formatter not implemented yet
            //Console.PrintHeader($"PrintF table formatted string test:");
            //Console.PrintF($"<table><header>Column1|Col2|EmptyColumn</header><body><cell>$$text:-Red$</cell>... </table>");
            //$"@any text {arr:table}" => "@any text\r\n<table><header>Price|Amount|EmptyColumn</header><body><cell>0.051</cell><cell>10.50</cell>\r\n<cell>0.003</cell><cell>120</cell></body></table>" 
            //Console.PrintF($"{arr:-Yellow table}"); //$$<table>...</table>:-Red$
            //Console.PrintTable(arr, (column, cell, scheme)=>{...}, ConsoleColor.Red);
        }

        public async Task PromptAsync()
        {
            Console.PrintHeader($"PromptYesNo");
            var answer = await Console.PromptYesNo("Do you confirm ...?", 5000);
            Console.Print($"Got an answer: {answer}");
        }
    }
}
