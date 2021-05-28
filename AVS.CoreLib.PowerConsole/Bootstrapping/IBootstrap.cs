using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.PowerConsole.Bootstrapping
{
    public interface IBootstrap
    {
        void Run<TStartup>(string[] args, bool isWindowsService = false) where TStartup : IStartup;
        Task RunAsync<TStartup>(string[] args, bool isWindowsService = false) where TStartup : IStartup;
        void Run<TStartup>(Action<IServiceProvider> main) where TStartup : IStartup, new();
        Task RunAsync<TStartup>(Func<IServiceProvider, Task> main) where TStartup : IStartup, new();
        IServiceProvider Run<TStartup>() where TStartup : IStartup, new();
    }
}