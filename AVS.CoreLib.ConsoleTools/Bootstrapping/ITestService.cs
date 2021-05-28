using System.Threading.Tasks;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
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