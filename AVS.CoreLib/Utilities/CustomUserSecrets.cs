using System;
using System.IO;
using System.Reflection;

namespace AVS.CoreLib.Utilities
{
    public static class CustomUserSecrets
    {
        private static string _userSecretsPath = null;

        public static bool Enabled { get; set; } = true;
        /// <summary>
        /// avoid using common path for secrets due to common location like microsoft/UserSecrets/.. kind of dangerous
        /// malicious software will target that path to get all user secrets for all apps at once
        /// </summary>
        public static string UserSecretsPath
        {
            get => GetUserSecretsPath();
            set => _userSecretsPath = value;
        }

        public static string GetUserSecretsPath(string applicationName = null, bool createPathIfNotExists = true, bool createEmptySecretsFileIfNotExists = false)
        {
            if (_userSecretsPath == null)
            {
                var appName = applicationName ?? Assembly.GetEntryAssembly()?.GetName().Name ??
                    Assembly.GetCallingAssembly().GetName().Name;
                var userFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                _userSecretsPath = Path.Combine(userFolder, ".secrets", appName, "secrets.json");
            }


            if (Enabled && createPathIfNotExists)
            {
                var dirPath = Path.GetDirectoryName(_userSecretsPath);
                if (dirPath != null && !Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }

            if (Enabled && createEmptySecretsFileIfNotExists && !File.Exists(_userSecretsPath))
            {
                using var writer = File.CreateText(_userSecretsPath);
                writer.Write("{}");
                writer.Flush();
            }

            return _userSecretsPath;
        }
    }
}