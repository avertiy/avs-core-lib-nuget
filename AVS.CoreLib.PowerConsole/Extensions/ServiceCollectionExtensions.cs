﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Sets formatter for PowerConsole.PrintF functionality
        /// use string.Format analog, for example X.Format from AVS.CoreLib.Text
        /// </summary>
        public static void AddPowerConsoleFormatter(this IServiceCollection services, Func<FormattableString, string> format)
        {
            PowerConsole.Format = format;
        }
    }
}