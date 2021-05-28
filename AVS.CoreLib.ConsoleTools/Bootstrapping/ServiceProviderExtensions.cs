using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public static class ServiceProviderExtensions
    {
        public static void PressEnterToExit(this ServiceProvider sp)
        {
            Console.Write("Press enter to exit");
            Console.ReadLine();
        }

        public static ServiceProvider Run(this ServiceProvider sp, Action<ServiceProvider> action)
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

        public static ServiceProvider RunTest<TService>(this ServiceProvider sp) where TService : ITestService
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

        public static ServiceProvider RunAllDemo(this ServiceProvider sp)
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

        public static ServiceProvider RunAllTest(this ServiceProvider sp)
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

        public static async Task<ServiceProvider> RunDemoAsync<TService>(this ServiceProvider sp) where TService : IDemoService
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