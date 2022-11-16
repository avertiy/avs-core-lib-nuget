using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Bootstrap
{
    public interface ITestService
    {
        void Test();
        Task TestAsync();
    }

    public abstract class TestService : ITestService
    {
        public virtual void Test()
        {
        }

        public virtual Task TestAsync()
        {
            return Task.CompletedTask;
        }
    }
}