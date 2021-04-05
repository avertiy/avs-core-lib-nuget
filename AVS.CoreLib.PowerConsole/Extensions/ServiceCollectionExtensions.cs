using System;
using System.Text;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Sets formatter for PowerConsole.PrintF functionality
        /// use string.Format analog, for example X.Format from AVS.CoreLib.Text
        /// </summary>
        [Obsolete("Use ServiceProviderExtensions UsePowerConsoleCustomFormat instead")]
        public static void AddPowerConsoleFormatter(this IServiceCollection services, Func<FormattableString, string> format)
        {
            PowerConsole.Format = format;
        }
    }

    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Sets formatter for PowerConsole.PrintF functionality
        /// use string.Format analog, for example X.Format from AVS.CoreLib.Text
        /// </summary>
        public static IServiceProvider UsePowerConsoleCustomFormat(this IServiceProvider sp, Func<FormattableString, string> format, Encoding encoding)
        {
            PowerConsole.Format = format;
            PowerConsole.InputEncoding = encoding;
            PowerConsole.OutputEncoding = encoding;
            PowerConsole.ApplyColorScheme(ColorScheme.Default);
            return sp;
        }

        public static IServiceProvider UseDefaultColorScheme(this IServiceProvider sp, ColorScheme scheme)
        {
            PowerConsole.SetDefaultColorScheme(scheme);
            PowerConsole.ApplyColorScheme(scheme);
            return sp;
        }
    }
}