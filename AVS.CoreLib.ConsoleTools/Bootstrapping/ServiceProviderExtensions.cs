using System;
using System.Threading.Tasks;
using AVS.CoreLib.PowerConsole.Bootstrapping;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider Run(this IServiceProvider sp, Action<IServiceProvider> action)
        {
            try
            {
                action(sp);
            }
            catch (Exception ex)
            {
                PowerConsole.PowerConsole.Write($"Run() failed");
                PowerConsole.PowerConsole.WriteError(ex);
            }

            return sp;
        }

        public static IServiceProvider RunTest<TService>(this IServiceProvider sp) where TService : ITestService
        {
            var service = sp.GetService<TService>();
            if (service == null)
            {
                throw new ArgumentException($"{typeof(TService).Name} is null");
            }

            try
            {
                service.Test();
            }
            catch (Exception ex)
            {
                PowerConsole.PowerConsole.Write($"{typeof(TService).Name}.Test() failed");
                PowerConsole.PowerConsole.WriteError(ex);
                throw;
            }

            return sp;
        }

        public static IServiceProvider RunAllDemo(this IServiceProvider sp)
        {
            var services = sp.GetServices<IDemoService>();
            var task = Task.Run(async () =>
            {
                foreach (var demoService in services)
                {
                    try
                    {
                        await demoService.DemoAsync();
                    }
                    catch (Exception ex)
                    {
                        PowerConsole.PowerConsole.Write($"{demoService.GetType().Name}.DemoAsync() failed");
                        PowerConsole.PowerConsole.WriteError(ex);
                    }
                }
            });
            Task.WaitAny(task);
            return sp;
        }

        public static IServiceProvider RunAllTest(this IServiceProvider sp)
        {
            var services = sp.GetServices<ITestService>();
            foreach (var testService in services)
            {
                try
                {
                    testService.Test();
                }
                catch (Exception ex)
                {
                    PowerConsole.PowerConsole.Write($"{testService.GetType().Name}.Test() failed");
                    PowerConsole.PowerConsole.WriteError(ex);
                }
            }
            return sp;
        }

        public static async Task<IServiceProvider> RunDemoAsync<TService>(this IServiceProvider sp) where TService : IDemoService
        {
            var service = sp.GetService<TService>();
            if (service == null)
            {
                throw new ArgumentException($"{typeof(TService).Name} is null");
            }

            try
            {
                await service.DemoAsync();
            }
            catch (Exception ex)
            {
                PowerConsole.PowerConsole.Write($"{typeof(TService).Name}.Test() failed");
                PowerConsole.PowerConsole.WriteError(ex);
            }

            return sp;
        }
    }
}