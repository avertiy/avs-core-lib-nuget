using System;
using System.Reflection;
using AVS.CoreLib.Guards;
using AVS.CoreLib.Utilities;
using Microsoft.Extensions.Configuration;

namespace AVS.CoreLib.UnitTesting.Helpers
{
    public static class ConfigurationHelper
    {
        public static ConfigurationBuilder WithUserSecrets(this ConfigurationBuilder builder, string appname = null, bool reloadOnChange = false)
        {
            builder.AddJsonFile(CustomUserSecrets.GetUserSecretsPath(appname), optional: true, reloadOnChange);
            return builder;
        }

        public static ConfigurationBuilder AddAppSettingsJson(this ConfigurationBuilder builder, string environment = null, bool reloadOnChange = false)
        {
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange);
            if (environment != null)
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange);
            return builder;
        }

        public static IConfigurationRoot LoadConfiguration(string appName = null, string environment = null, bool reloadOnChange = false)
        {
            var builder = new ConfigurationBuilder();
            builder.AddAppSettingsJson(environment, reloadOnChange);
            builder.WithUserSecrets(appName, reloadOnChange);
            return builder.Build();
        }

        public static IConfigurationRoot LoadConfiguration(ConfigurationAttribute attribute)
        {
            Guard.AgainstNull(attribute);
            var builder = new ConfigurationBuilder();
            builder.AddAppSettingsJson(attribute.Environment, attribute.ReloadOnChange);

            if(attribute.UseCustomUserSecrets)
                builder.WithUserSecrets(attribute.AppName, attribute.ReloadOnChange);

            return builder.Build();
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ConfigurationAttribute : Attribute
    {
        public string AppName { get; set; }
        public bool UseCustomUserSecrets { get; set; } = true;
        public string Environment { get; set; }
        public bool ReloadOnChange { get; set; }
    }
}