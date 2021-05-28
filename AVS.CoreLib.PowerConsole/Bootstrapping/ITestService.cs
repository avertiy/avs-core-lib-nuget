using System.Threading.Tasks;

namespace AVS.CoreLib.PowerConsole.Bootstrapping
{
    public interface ITestService
    {
        void Test();
    }

    public interface IDemoService
    {
        Task DemoAsync();
    }
}