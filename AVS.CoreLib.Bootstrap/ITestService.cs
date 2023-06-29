using System.Diagnostics;

namespace AVS.CoreLib.BootstrapTools;

public interface ITestService
{
    void Test(string[] args);
}

public abstract class TestService : ITestService
{
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