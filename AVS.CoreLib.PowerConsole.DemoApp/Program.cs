using System.Threading.Tasks;
using AVS.CoreLib.PowerConsole.DemoApp.Services;

namespace AVS.CoreLib.PowerConsole.DemoApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var demo = new ConsoleFeaturesDemoService();
            await demo.DemoAsync();
            var demo2 = new XFormatDemoService();
            await demo2.DemoAsync();
        }
    }
}
