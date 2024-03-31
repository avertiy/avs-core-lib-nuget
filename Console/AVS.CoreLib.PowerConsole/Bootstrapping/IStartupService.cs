using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Bootstrap;

namespace AVS.CoreLib.PowerConsole.Bootstrapping
{
    /// <summary>
    /// For a convenience startup service is just an entry point like Program.Main
    /// <see cref="Bootstrap.Start{TStartupService}"/>
    /// <code>
    ///  Bootstrap.Start&lt;StartupService&gt;(services => {..register your services..})
    /// </code>
    /// <seealso cref="StartupServiceBase"/>
    /// </summary>
    public interface IStartupService
    {
        void Start();
    }

    /// <summary>
    /// StartupService is a base class that allows effortlessly switch from sync <see cref="Main"/> to async <see cref="MainAsync"/>
    /// Also it implements <see cref="ITestService"/> the idea is to effortlessly switch between main flow and test flow - to test something,
    /// execute some alternative logic you want to verify, switch <see cref="RunTestFlow"/> to true
    /// </summary>
    public abstract class StartupServiceBase : IStartupService, ITestService
    {
        public virtual bool RunTestFlow { get; set; }
        [DebuggerStepThrough]
        public void Start()
        {
            if (RunTestFlow)
            {
                Test();
            }
            else
            {
                Main();
            }
        }

        [DebuggerStepThrough]
        public virtual void Main()
        {
            var task = Task.Run(async () =>
            {
                await MainAsync();
            });
            task.Wait();
        }

        public virtual Task MainAsync()
        {
            return Task.CompletedTask;
        }

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