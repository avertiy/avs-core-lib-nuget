using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static string ReadLine()
        {
            return Console.ReadLine();
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
        /// <param name="color">Console color to be used to write only tis line of text</param>
        /// <param name="timeFormat">Display format for the date and time</param>
        /// <returns>Returns entered value from the user</returns>
        public static string ReadLine(string message, ConsoleColor color = ConsoleColor.Gray, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            WriteLine(message, color, timeFormat);
            return Console.ReadLine();
        }

        /// <summary>
        /// Writes a message with options and wait for the user input
        /// </summary>
        /// <param name="message">Message text to be written in console output</param>
        /// <param name="status">Message status to be used to output message text</param>
        /// <param name="timeFormat">Display format for the date and time</param>
        /// <returns>Returns entered value from the user</returns>
        public static string ReadLine(string message, MessageStatus status = MessageStatus.Default, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            WriteLine(message, status, timeFormat);
            return Console.ReadLine();
        }


        /// <summary>
        /// Writes a message with options and wait for the user input
        /// </summary>
        /// <typeparam name="T">Generic type to validate the input and request for re-entering if eneted value is invalid</typeparam>
        /// <param name="message">Message text to be written in console output</param>
        /// <param name="color">Console color to be used to write only tis line of text</param>
        /// <param name="timeFormat">Display format for the date and time</param>
        /// <returns>Returns entered value from the user</returns>
        public static T ReadLine<T>(string message, ConsoleColor color = ConsoleColor.Gray, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff") 
            where T : IConvertible
        {
            WriteLine(message, color, timeFormat);
            string input = Console.ReadLine();
                    try
                    {
                        return (T)Convert.ChangeType(input, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        PowerConsole.WriteError(ex, true, timeFormat);
                        return ReadLine<T>(message, color, timeFormat);
                    }
        }
    }
}
