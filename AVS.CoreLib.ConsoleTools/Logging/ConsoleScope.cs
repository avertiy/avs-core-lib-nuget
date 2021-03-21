using System;
using Console = AVS.CoreLib.PowerConsole.PowerConsole;

namespace AVS.CoreLib.ConsoleTools.Logging
{
    public class ConsoleScope : IDisposable
    {
        private ConsoleColor Color { get; set; }
        public bool UseCurlyBrackets { get; set; } = true;
        public bool PrintLoggerName { get; set; }
        public int HashCode { get; private set; }
        public string Logger { get; set; }

        public void SetLogger(string logger)
        {
            if (Logger != logger)
            {
                Close();
                Logger = logger;
                if (PrintLoggerName && Logger != null)
                {
                    Console.WriteLine(false);
                    Console.Print(Logger, ConsoleColor.DarkGray);
                }
            }
        }

        public void Begin(int hashcode, string scope, ConsoleColor color = ConsoleColor.Magenta)
        {
            if (HashCode == hashcode)
                return;

            Close();
            Console.WriteLine(false);
            Console.Print(UseCurlyBrackets ? $"\t{scope}\r\n {{" : $"\t{scope}", color);
            Color = color;
            HashCode = hashcode;
        }

        public void Close()
        {
            if (HashCode != 0)
            {
                if (UseCurlyBrackets)
                    Console.Print(" }", Color);
                else
                    Console.WriteLine();

                HashCode = 0;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}