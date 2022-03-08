using System.IO;
using System.Reflection;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public static class CustomUserSecrets
    {
        public static bool Enabled { get; set; }
        public static void CreateSecretsDirectory()
        {
            var path = UserSecretsPath;
            var dirPath = Path.GetDirectoryName(path);
            if (dirPath != null && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            //if (!File.Exists(path))
            //{
            //    using var writer = File.CreateText(path);
            //    writer.Write("{}");
            //    writer.Flush();
            //}
        }

        /// <summary>
        /// avoid using common path for secrets due to common location like microsoft/UserSecrets/.. kind of dangerous
        /// malicious software will target that path to get all user secrets for all apps at once
        /// </summary>
        public static string UserSecretsPath
        {
            get
            {
                var userFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                return Path.Combine(userFolder, ".secrets", AppName, "secrets.json");
            }
        }

        private static string AppName => Assembly.GetEntryAssembly()?.GetName().Name ?? Assembly.GetCallingAssembly().GetName().Name;
    }
}