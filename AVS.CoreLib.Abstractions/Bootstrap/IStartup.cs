using System;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Abstractions.Bootstrap
{
    public interface IStartup
    {
        void RegisterServices(IServiceCollection services);
        void ConfigureServices(IServiceProvider sp);
    }
}