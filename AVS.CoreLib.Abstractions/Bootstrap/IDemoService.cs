using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Bootstrap
{
    public interface IDemoService
    {
        void Demo();
        Task DemoAsync();
    }

    public abstract class DemoService : IDemoService
    {
        public virtual void Demo()
        {
        }

        public virtual Task DemoAsync()
        {
            return Task.CompletedTask;
        }
    }
}