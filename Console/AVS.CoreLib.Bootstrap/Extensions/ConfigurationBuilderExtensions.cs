using AVS.CoreLib.BootstrapTools.Configuration;
using Microsoft.Extensions.Configuration;

namespace AVS.CoreLib.BootstrapTools.Extensions;

public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Add custom user secrets configuration
    /// </summary>
    public static void AddCustomUserSecrets(this IConfigurationBuilder builder, string? appName = null, string secretsFolder =".secrets", string secretsFile ="secrets.json", bool reloadOnChange = false)
    {
        var path = CustomUserSecrets.GetUserSecretsPath(appName, secretsFolder, secretsFile);
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