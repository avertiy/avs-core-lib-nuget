using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.PowerConsole
{
    using Console = System.Console;
    public static partial class PowerConsole
    {
        public static int Read()
        {
            return Console.Read();
        }

        #region ReadKey methods
        public static ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            return Console.ReadKey(intercept);
        }

        public static Task<ConsoleKeyInfo> ReadKeyAsync(bool intercept = false)
        {
            return intercept ? Task.Factory.StartNew(() => Console.ReadKey(true)) : Task.Factory.StartNew(Console.ReadKey);
        }

        public static async Task<ConsoleKeyInfo> ReadKeyAsync(bool intercept, int millisecondsTimeout)
        {
            var task = ReadKeyAsync(intercept);
            
            if (millisecondsTimeout <= 0) 
                return await task;

            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsTimeout));
            var result = object.ReferenceEquals(task, completedTask) ? task.Result : new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false);
            return result;

        }

        public static async Task<ConsoleKeyInfo> ReadKeyAsync(bool intercept, int millisecondsTimeout, ConsoleKeyInfo defaultValue)
        {
            var task = ReadKeyAsync(intercept);
            if (millisecondsTimeout <= 0)
                return await task;

            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsTimeout));
            var result = object.ReferenceEquals(task, completedTask) ? task.Result : defaultValue;
            return result;
        }

        public static async Task<bool> PromptYesNoAsync(string message, int millisecondsTimeout = 0, ConsoleKey defaultAnswer = ConsoleKey.Y)
        {
            //Do you agree? (y - yes/n - no):
            // 5 sec. to answer
            Write($"\r\n{message} (y - yes/n - no): ");
            int left = Console.CursorLeft;
            if (millisecondsTimeout > 0)
            {
                Write($"   [{millisecondsTimeout / 1000} sec. to answer]");
            }
            
            Console.SetCursorPosition(left, Console.CursorTop);
            //, new ConsoleKeyInfo(defaultAnswer, defaultAnswer == 'y'? ConsoleKey.Y: ConsoleKey.N, false,false,false)
            var keyInfo = await ReadKeyAsync(false, millisecondsTimeout);

            if (keyInfo.Key == ConsoleKey.Escape)
                Write(defaultAnswer.ToString());

            ClearLine(left);
            if (keyInfo.Key == ConsoleKey.Y || (keyInfo.Key == ConsoleKey.Escape && defaultAnswer == ConsoleKey.Y))
            {
                //default answer
                Print("Yes", ConsoleColor.DarkGreen);
                return true;
            }
            else
            {
                Print("No", ConsoleColor.DarkRed);
                return false;
            }
        }

        public static bool PromptYesNo(string message, ConsoleKey defaultAnswer = ConsoleKey.Y)
        {
            //Do you agree? (y - yes/n - no):
            // 5 sec. to answer
            Write($"\r\n{message} (y - yes/n - no): ");
            int left = Console.CursorLeft;
            //, new ConsoleKeyInfo(defaultAnswer, defaultAnswer == 'y'? ConsoleKey.Y: ConsoleKey.N, false,false,false)
            var keyInfo= Console.ReadKey(false);

            if (keyInfo.Key == ConsoleKey.Escape)
                Write(defaultAnswer.ToString());

            ClearLine(left);
            if (keyInfo.Key == ConsoleKey.Y || (keyInfo.Key == ConsoleKey.Escape && defaultAnswer == ConsoleKey.Y))
            {
                //default answer
                Print("Yes", ConsoleColor.DarkGreen);
                return true;
            }
            else
            {
                Print("No", ConsoleColor.DarkRed);
                return false;
            }
        }
        #endregion
    }
}
