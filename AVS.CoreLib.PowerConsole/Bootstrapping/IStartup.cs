using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.PowerConsole.Bootstrapping
{
    public interface IStartup
    {
        IConfiguration Configuration { get; }
        void ConfigureServices(IServiceProvider services);
        void RegisterServices(IServiceCollection services);
    }
}