﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.UnitTesting.Extensions
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection Replace<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            ServiceLifetime lifetime)
            where TService : class
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));

            services.Remove(descriptorToRemove);

            var descriptorToAdd = new ServiceDescriptor(typeof(TService), implementationFactory, lifetime);

            services.Add(descriptorToAdd);

            return services;
        }
    }
}
