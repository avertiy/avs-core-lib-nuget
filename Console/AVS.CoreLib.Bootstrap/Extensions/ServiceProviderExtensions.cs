﻿using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.BootstrapTools.Extensions
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Run <see cref="IStartupService"/>
        /// <seealso cref="StartupServiceBase"/>
        /// </summary>
        public static IServiceProvider Run<TService>(this IServiceProvider sp, string[]? args = null) where TService : IStartupService
        {
            try
            {
                var service = sp.GetRequiredService<TService>();
                service.Start(args ?? Array.Empty<string>());
            }
            catch (Exception ex)
            {
                Bootstrap.PrintError(ex, $"{nameof(Run)} failed");
                throw;
            }

            return sp;
        }

        /// <summary>
        /// Run custom service
        /// </summary>
        public static IServiceProvider Run<TService>(this IServiceProvider sp, Action<TService> action) where TService : notnull
        {
            try
            {
                var service = sp.GetRequiredService<TService>();
                action(service);
            }
            catch (Exception ex)
            {
                Bootstrap.PrintError(ex, $"{nameof(Run)} failed");
                throw;
            }

            return sp;
        }

        /// <summary>
        /// Run custom service in async way
        /// </summary>
        public static IServiceProvider RunAsync<TService>(this IServiceProvider sp, Func<TService, Task> action) where TService : notnull
        {
            var task = Task.Run(async () =>
            {
                try
                {
                    var service = sp.GetRequiredService<TService>();
                    await action(service);
                }
                catch (Exception ex)
                {
                    Bootstrap.PrintError(ex, $"{nameof(RunAsync)} failed");
                    throw;
                }
            });

            Task.WaitAny(task);
            return sp;
        }

        /// <summary>
        /// Run <see cref="ITestService"/>
        /// </summary>
        public static IServiceProvider RunTest<TService>(this IServiceProvider sp, string[]? args = null) where TService : ITestService
        {
            try
            {
                var service = sp.GetRequiredService<TService>();
                service.Test(args ?? Array.Empty<string>());
            }
            catch (Exception ex)
            {
                Bootstrap.PrintError(ex, $"{nameof(RunTest)} failed");
                throw;
            }

            return sp;
        }

        /// <summary>
        /// Run all services registered as <see cref="ITestService"/>
        /// </summary>
        public static IServiceProvider RunAllTest(this IServiceProvider sp, int delay = 1000, string[]? args = null)
        {
            var services = sp.GetServices<ITestService>();
            foreach (var testService in services)
                try
                {
                    testService.Test(args ?? Array.Empty<string>());
                    Thread.Sleep(delay);
                }
                catch (Exception ex)
                {
                    Bootstrap.PrintError(ex, $"{nameof(RunTest)}:{testService.GetType().Name} failed");
                    throw;
                }
            return sp;
        }
    }
}