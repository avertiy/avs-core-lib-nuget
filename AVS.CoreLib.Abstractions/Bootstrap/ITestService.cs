using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Bootstrap
{
    public interface ITestService
    {
        void Test();
    }

    public abstract class TestService : ITestService
    {
        [DebuggerStepThrough]
        public virtual void Test()
        {
            var task = Task.Run(async () =>
            {
                await TestAsync();
            });
            task.Wait();
        }

        public virtual Task TestAsync()
        {
            return Task.CompletedTask;
        }
    }
}