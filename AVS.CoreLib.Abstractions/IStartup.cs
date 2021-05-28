using System;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Abstractions
{
    public interface IStartup
    {
        void ConfigureServices(IServiceProvider services);
        void RegisterServices(IServiceCollection services);
    }
}