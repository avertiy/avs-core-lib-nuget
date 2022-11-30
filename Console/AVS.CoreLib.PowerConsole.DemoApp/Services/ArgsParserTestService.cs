using System;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Abstractions.Bootstrap;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    public class ArgsParserTestService : TestService
    {
        public override void Test()
        {
            var args = new[] { "" };
            PowerConsole.PrintArray(args);
            var dict = ArgsParser.Parse(args);
        }
    }
}