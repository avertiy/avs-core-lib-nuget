using System.Diagnostics;

namespace AVS.CoreLib.BootstrapTools;

/// <summary>
/// StartupService is a base class that allows effortlessly switch from sync <see cref="Main"/> to async <see cref="MainAsync"/>
/// Also it implements <see cref="ITestService"/> the idea is to effortlessly switch between main flow and test flow - to test something,
/// execute some alternative logic you want to verify, switch <see cref="RunTestFlow"/> to true
/// </summary>
public abstract class StartupServiceBase : IStartupService, ITestService
{
    public virtual bool RunTestFlow { get; set; }
    [DebuggerStepThrough]
    public void Start(string[] args)
    {
        if (RunTestFlow)
        {
            Test(args);
        }
        else
        {
            Main(args);
        }
    }

    [DebuggerStepThrough]
    public virtual void Main(string[] args)
    {
        var task = Task.Run(async () =>
        {
            await MainAsync(args);
        });
        task.Wait();
    }

    public virtual Task MainAsync(string[] args)
    {
        return Task.CompletedTask;
    }

    [DebuggerStepThrough]
    public virtual void Test(string[] args)
    {
        var task = Task.Run(async () =>
        {
            await TestAsync(args);
        });
        task.Wait();
    }

    public virtual Task TestAsync(string[] args)
    {
        return Task.CompletedTask;
    }
}