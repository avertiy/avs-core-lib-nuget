using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.PowerConsole.Bootstrapping
{
    /// <summary>
    /// SimpleBootstrap provides only basic bootstrap's functionality
    /// the rest is foreseen but not implemented
    /// to use RunAsHost or RunAsService check out avs.corelib.hosting package 
    /// </summary>
    public class SimpleBootstrap : IBootstrap
    {
        public IServiceProvider Run<TStartup>() where TStartup : IStartup, new()
        {
            try
            {
                var startup = new TStartup();
                var services = new ServiceCollection();
                startup.RegisterServices(services);
                var serviceProvider = services.BuildServiceProvider();
                startup.ConfigureServices(serviceProvider);
                return serviceProvider;
            }
            catch (Exception ex)
            {
                PowerConsole.Print($"Bootstrap.Run<{typeof(TStartup).Name}>() failed", ConsoleColor.Red);
                PowerConsole.WriteError(ex);
                throw;
            }
        }

        public void Run<TStartup>(string[] args, bool isWindowsService = false) where TStartup : IStartup
        {
            throw new NotImplementedException();
            //var host = CreateHost<TStartup>(args, isService);
            //host.Run();
        }

        public Task RunAsync<TStartup>(string[] args, bool isWindowsService = false) where TStartup : IStartup
        {
            throw new NotImplementedException();
        }

        public void Run<TStartup>(Action<IServiceProvider> action) where TStartup : IStartup, new()
        {
            try
            {
                var startup = new TStartup();
                var services = new ServiceCollection();
                startup.RegisterServices(services);
                var serviceProvider = services.BuildServiceProvider();
                startup.ConfigureServices(serviceProvider);
                action(serviceProvider);
            }
            catch (Exception ex)
            {
                PowerConsole.Print($"Bootstrap.Run<{typeof(TStartup).Name}>() failed", ConsoleColor.Red);
                PowerConsole.WriteError(ex);
                throw;
            }
        }

        public Task RunAsync<TStartup>(Func<IServiceProvider, Task> action) where TStartup : IStartup, new()
        {
            try
            {
                var startup = new TStartup();
                var services = new ServiceCollection();
                startup.RegisterServices(services);
                var serviceProvider = services.BuildServiceProvider();
                startup.ConfigureServices(serviceProvider);
                return action(serviceProvider);
            }
            catch (Exception ex)
            {
                PowerConsole.Print($"Bootstrap.Run<{typeof(TStartup).Name}>() failed", ConsoleColor.Red);
                PowerConsole.WriteError(ex);
                throw;
            }
        }
    }
}