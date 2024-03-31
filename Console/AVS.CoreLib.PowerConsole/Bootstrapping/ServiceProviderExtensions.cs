using System;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.PowerConsole.Bootstrapping
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider RunAsync<TService>(this IServiceProvider sp, Func<TService, Task> action) where TService : notnull
        {
            var service = sp.GetRequiredService<TService>();

            var task = Task.Run(async () =>
            {
                try
                {
                    await action(service);
                }
                catch (Exception ex)
                {
                    PowerConsole.Write($"RunAsync<{typeof(TService).Name}>() failed");
                    PowerConsole.PrintError(ex);
                }
            });

            Task.WaitAny(task);
            return sp;
        }

        public static IServiceProvider Run<TService>(this IServiceProvider sp, Action<TService> action) where TService : notnull
        {
            var service = sp.GetRequiredService<TService>();
            try
            {
                action(service);
            }
            catch (Exception ex)
            {
                PowerConsole.Write($"Run<{typeof(TService).Name}>() failed");
                PowerConsole.PrintError(ex);
            }

            return sp;
        }

        public static IServiceProvider Run(this IServiceProvider sp, Action<IServiceProvider> action)
        {
            try
            {
                action(sp);
            }
            catch (Exception ex)
            {
                PowerConsole.Write("Run() failed");
                PowerConsole.PrintError(ex);
            }

            return sp;
        }

        public static IServiceProvider RunTest<TService>(this IServiceProvider sp) where TService : ITestService
        {
            try
            {
                var service = sp.GetRequiredService<TService>();
                service.Test();
            }
            catch (Exception ex)
            {
                PowerConsole.Write($"{typeof(TService).Name}.Test() failed");
                PowerConsole.PrintError(ex);
                throw;
            }

            return sp;
        }

        public static IServiceProvider RunAllDemo(this IServiceProvider sp)
        {
            var services = sp.GetServices<IDemoService>();
            foreach (var demoService in services)
            {
                try
                {
                    demoService.Demo();
                }
                catch (Exception ex)
                {
                    PowerConsole.Write($"{demoService.GetType().Name}.DemoAsync() failed");
                    PowerConsole.PrintError(ex);
                }
            }
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
                    PowerConsole.Write($"{testService.GetType().Name}.Test() failed");
                    PowerConsole.PrintError(ex);
                }
            }
            return sp;
        }

        public static IServiceProvider RunDemo<TService>(this IServiceProvider sp) where TService : IDemoService
        {
            try
            {
                var service = sp.GetRequiredService<TService>();
                service.Demo();
            }
            catch (Exception ex)
            {
                PowerConsole.Write($"{typeof(TService).Name}.Test() failed");
                PowerConsole.PrintError(ex);
            }

            return sp;
        }
    }
}