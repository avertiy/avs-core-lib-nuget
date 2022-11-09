using System;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    public class ArgsParserTestService : ITestService
    {
        public void Test()
        {
            var args = new[] { "" };
            PowerConsole.Print("args: ", args, ConsoleColor.DarkGreen);
            var dict = ArgsParser.Parse(args);
            //PowerConsole.PrintDictionary();
        }
    }
}