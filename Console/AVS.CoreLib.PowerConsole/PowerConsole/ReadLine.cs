using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVS.CoreLib.PowerConsole.Printers2;

namespace AVS.CoreLib.PowerConsole
{
    using Console = System.Console;
    public static partial class PowerConsole
    {
        public static string ReadLine()
        {
            return Console.ReadLine()!;
        }

        public static IEnumerable<string> ReadLines(int count)
        {
            for (var i = 0; i < count; i++)
                yield return Console.ReadLine()!;
        }

        public static async Task<string> ReadLineAsync(int millisecondsTimeout, string defaultText = null)
        {
            var task = Task.Factory.StartNew(Console.ReadLine);
            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsTimeout));
            var result = object.ReferenceEquals(task, completedTask) ? task.Result : defaultText;
            return result;
        }

        /// <summary>
        /// Writes a message with options and wait for the user input
        /// </summary>
        /// <param name="message">Message text to be written in console output</param>
        /// <param name="options"><see cref="PrintOptions"/></param>
        /// <returns>Returns entered value from the user</returns>
        public static string ReadLine(string message, PrintOptions2 options = PrintOptions2.Default)
        {
            Print(message, options);
            return Console.ReadLine() ?? string.Empty;
        }


        /// <summary>
        /// Writes a message with options and wait for the user input
        /// </summary>
        /// <typeparam name="T">Generic type to validate the input and request for re-entering if eneted value is invalid</typeparam>
        /// <param name="message">Message text to be written in console output</param>
        /// <param name="options"></param>
        /// <returns>Returns entered value from the user</returns>
        public static T ReadLine<T>(string message, PrintOptions2 options = PrintOptions2.Default)
            where T : IConvertible
        {
            Print(message, options);
            var input = Console.ReadLine();
            try
            {
                return (T)Convert.ChangeType(input, typeof(T));
            }
            catch (Exception ex)
            {
                PowerConsole.PrintError(ex, $"{nameof(ReadLine)} failed", true);
                return ReadLine<T>(message, options);
            }
        }
    }
}
