using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions
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