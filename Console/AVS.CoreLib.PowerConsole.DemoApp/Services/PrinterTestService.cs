using System;
using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Printers2;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    public class PrinterTestService : TestService
    {
        private IPowerConsolePrinter2 Printer2 { get; set; } = PowerConsole.Printer2;
        public override void Test(string[] args)
        {
            try
            {
                WriteTests();
                WriteLineTests();
                MessageLevelTests();
                PrintOptionsTests();
                CTagsTests();
                FormattableStringTests();
            }
            catch (Exception ex)
            {
                PowerConsole.PrintError(ex, $"{nameof(PrinterTestService)} Test failed");
            }
            return;
        }

        public void WriteTests()
        {
            PowerConsole.PrintHeader($"Write tests");

            //printer2
            Printer2.Write("simple write ");
            Printer2.Write("continue should be the same line.\r\n");
            Printer2.Write("write + ConsoleColor.Cyan ", ConsoleColor.Cyan);
            Printer2.Write("dark-red in gray\r\n", new Colors(ConsoleColor.DarkRed, ConsoleColor.Gray));
        }
        public void WriteLineTests()
        {
            PowerConsole.PrintHeader($"WriteLine tests");
            Printer2.WriteLine("1st line", ConsoleColor.Green);
            Printer2.WriteLine("2nd line", ConsoleColor.Blue);
            Printer2.WriteLine();
            Printer2.WriteLine();
            Printer2.WriteLine();
            Printer2.WriteLine("3rd line", ConsoleColor.Yellow);
            Printer2.WriteLine();
        }

        private void MessageLevelTests()
        {
            PowerConsole.PrintHeader($"Message level tests");
            Printer2.WriteLine(MessageLevel.Debug, "write debug message");
            Printer2.WriteLine(MessageLevel.Default, "write default message");
            Printer2.WriteLine(MessageLevel.Info, "write info message");
            Printer2.WriteLine(MessageLevel.Success, "write success message");
            Printer2.WriteLine(MessageLevel.Important, "write important message");
            Printer2.WriteLine(MessageLevel.Warning, "write warning message");
            Printer2.WriteLine(MessageLevel.Error, "write error message");
            Printer2.WriteLine(MessageLevel.Critical, "write critical error");
        }

        private void CTagsTests()
        {
            PowerConsole.PrintHeader($"Color CTags tests");
            Printer2.Print("<Cyan>text in cyan</Cyan>");
            Printer2.Print("<Red>red text </Red><Cyan>cyan text</Cyan>");
            Printer2.Print("<Red> red text <bgCyan>bg cyan</bgCyan></Red>");
        }

        private void PrintOptionsTests()
        {
            PowerConsole.PrintHeader($"Print options tests");

            Printer2.Print("text with ctags <bgYellow>should be yellow background and <Blue>blue text</Blue></bgYellow> ", PrintOptions2.Default);
            Printer2.Print("2nd line no timestamp", PrintOptions2.NoTimestamp);
            Printer2.Print("3rd line ", PrintOptions2.Inline);
            Printer2.Print("continue 3rd line in dark yellow", PrintOptions2.Default, ConsoleColor.DarkYellow);
            Printer2.Print("<bgGray><Blue>text should contain ctags as is</Blue></bgGray> ", PrintOptions2.NoCTags);
        }

        private void FormattableStringTests()
        {
            PowerConsole.PrintHeader($"Print formattable string tests");

            Printer2.Print($"text with ctags <bgYellow>should be yellow background and <Blue>blue text</Blue></bgYellow> ", PrintOptions2.Default);
            Printer2.Print($"int: {11}; dec: {12.22}; str: {"my-str"}; datetime: {DateTime.Now}");
            Printer2.Print($"int: {11}; str: {"my-str"}; datetime: {DateTime.Now}", PrintOptions2.Default, ConsoleColor.Cyan);
            Printer2.Print($"continue 3rd line in dark yellow", PrintOptions2.Default, ConsoleColor.DarkYellow);
            Printer2.Print($"<bgGray><Blue>text should contain ctags as is</Blue></bgGray> ", PrintOptions2.NoCTags);
        }
    }
}