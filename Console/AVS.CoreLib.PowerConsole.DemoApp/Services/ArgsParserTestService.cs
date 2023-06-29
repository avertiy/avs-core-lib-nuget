using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    public class ArgsParserTestService : TestService
    {
        public override void Test(string[] args)
        {
            args = new[] { "" };
            PowerConsole.PrintArray(args);
            var dict = ArgsParser.Parse(args);
        }
    }
}