using System;
using AVS.CoreLib.Guards;
using AVS.CoreLib.Utilities;
using Microsoft.Extensions.Configuration;

namespace AVS.CoreLib.Configuration
{
    /// <summary>
    /// helps to load configuration
    /// (i) appsetting.json,
    /// (ii) appsettings.{environment}.json,
    /// (iii) user secrets by default path is %userprofile%/.secrets/{AppName}/secrets.json 
    /// </summary>
    public static class ConfigurationHelper
    {
        public static IConfigurationRoot LoadConfiguration(string? appName = null, string? environment = null, bool reloadOnChange = false)
        {
            var builder = new ConfigurationBuilder();
            builder.AddAppSettingsJson(environment, reloadOnChange);
            builder.AddCustomUserSecrets(appName, reloadOnChange);
            return builder.Build();
        }

        public static IConfigurationRoot LoadConfiguration(ConfigurationAttribute attribute)
        {
            Guard.Against.Null(attribute);
            var builder = new ConfigurationBuilder();
            builder.AddAppSettingsJson(attribute.Environment, attribute.ReloadOnChange);

            if (attribute.UseCustomUserSecrets)
                builder.AddCustomUserSecrets(attribute.AppName, attribute.ReloadOnChange);

            return builder.Build();
        }

        public static ConfigurationBuilder AddCustomUserSecrets(this ConfigurationBuilder builder, string? appName = null, bool reloadOnChange = false)
        {
            var path = CustomUserSecrets.GetUserSecretsPath(appName);
            Console.WriteLine($"ConfigurationManager: add {path} (reloadOnChange: {reloadOnChange})");
            builder.AddJsonFile(path, optional: true, reloadOnChange);
            return builder;
        }

        /// <summary>
        /// load (add json file) custom user secrets to configuration
        /// in .NET 6 ConfigurationBuilder was replaced with ConfigurationManager
        /// </summary>
        public static ConfigurationManager AddCustomUserSecrets(this ConfigurationManager configuration,
            string? appName = null, bool reloadOnChange = false)
        {
            var path = CustomUserSecrets.GetUserSecretsPath(appName);
            Console.WriteLine($"ConfigurationManager: add {path} (reloadOnChange: {reloadOnChange})");
            configuration.AddJsonFile(path, optional: true, reloadOnChange);
            return configuration;
        }

        public static ConfigurationBuilder AddAppSettingsJson(this ConfigurationBuilder builder, string? environment = null,
            bool reloadOnChange = false)
        {
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange);
            if (environment != null)
            {
                Console.WriteLine($"ConfigurationManager: add appsettings.{environment}.json (reloadOnChange: {reloadOnChange})");
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange);
            }

            return builder;
        }
    }

    /// <summary>
    /// Allows to set app name, environment etc. parameters, the attribute is used by <see cref="ConfigurationHelper.LoadConfiguration(ConfigurationAttribute)"/>
    /// most likely you will use it with unit tests
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ConfigurationAttribute : Attribute
    {
        public string AppName { get; set; } = null!;
        public bool UseCustomUserSecrets { get; set; } = true;
        public string Environment { get; set; } = null!;
        public bool ReloadOnChange { get; set; }
    }
}