using System;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Abstractions.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Call action wrapped in try catch block 
        /// </summary>
        public static IServiceProvider Run(this IServiceProvider sp, Action<IServiceProvider> action)
        {
            try
            {
                action(sp);
            }
            catch (Exception ex)
            {
                PowerConsole.PowerConsole.Write($"Run() failed");
                PowerConsole.PowerConsole.PrintError(ex);
            }

            return sp;
        }

        /// <summary>
        /// Get an enumeration of services of type <see cref="IDemoService"/> registered in DI container
        /// and call their DemoAsync() methods within a try catch block 
        /// </summary>
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
                        PowerConsole.PowerConsole.PrintError(ex);
                    }
                }
            });
            Task.WaitAny(task);
            return sp;
        }

        /// <summary>
        /// Get an enumeration of services of type <see cref="ITestService"/> registered in DI container
        /// and call their Test() methods within a try catch block 
        /// </summary>
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
                    PowerConsole.PowerConsole.PrintError(ex);
                }
            }
            return sp;
        }

        /// <summary>
        /// Get service of type <see cref="IDemoService"/> registered in DI container
        /// and call Test() method within a try catch block 
        /// </summary>
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
                PowerConsole.PowerConsole.PrintError(ex);
            }

            return sp;
        }

        /// <summary>
        /// Get service of type <see cref="ITestService"/> registered in DI container
        /// and call Test() method within a try catch block 
        /// </summary>
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
                PowerConsole.PowerConsole.PrintError(ex);
                throw;
            }

            return sp;
        }
    }
}