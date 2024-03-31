using System.Diagnostics;
using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Bootstrap
{
    public interface IDemoService
    {
        void Demo();
    }

    public abstract class DemoService : IDemoService
    {
        [DebuggerStepThrough]
        public virtual void Demo()
        {
            var task = Task.Run(async () =>
            {
                await DemoAsync();
            });
            task.Wait();
        }

        public virtual Task DemoAsync()
        {
            return Task.CompletedTask;
        }
    }
}