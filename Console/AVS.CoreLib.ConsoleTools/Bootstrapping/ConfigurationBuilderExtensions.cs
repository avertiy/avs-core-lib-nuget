using Microsoft.Extensions.Configuration;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public static class ConfigurationBuilderExtensions
    {
        public static void AddCustomUserSecrets(this IConfigurationBuilder builder, string appName = null,  bool reloadOnChange = false)
        {
            var path = CustomUserSecrets.GetUserSecretsPath(appName);
            builder.AddJsonFile(path, optional: true, reloadOnChange);
        }

        /// <summary>
        /// require 2 packages: Microsoft.Extensions.Configuration.EnvironmentVariables and Microsoft.Extensions.Configuration.Json
        /// </summary>
        public static void AddAppSettings(this IConfigurationBuilder builder, string environment, bool reloadOnChange = false)
        {
            builder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange)
                .AddEnvironmentVariables();
        }
    }
}