using Microsoft.Extensions.Configuration;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public static class ConfigurationBuilderExtensions
    {
        public static void AddCustomUserSecrets(this IConfigurationBuilder builder, bool reloadOnChange = false)
        {
            if (CustomUserSecrets.Enabled)
            {
                CustomUserSecrets.CreateSecretsDirectory();
                builder.AddJsonFile(CustomUserSecrets.UserSecretsPath, optional: true, reloadOnChange);
            }
        }

        public static void AddAppSettings(this IConfigurationBuilder builder, string environment, bool reloadOnChange = false)
        {
            builder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange)
                .AddEnvironmentVariables();
        }
    }
}