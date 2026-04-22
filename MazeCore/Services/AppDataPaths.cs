using System;
using System.IO;

namespace MazeCore.Services
{
    public static class AppDataPaths
    {
        private static readonly string BaseDirectory = AppContext.BaseDirectory;
        private static readonly string LegacyDataFolder = Path.Combine(BaseDirectory, "Data");

        public static string DataFolder { get; } = ResolveDataFolder();

        public static void Initialize()
        {
            Directory.CreateDirectory(DataFolder);
            MoveLegacyFiles();
            EnsureFileExists("mazes.json");
            EnsureFileExists("users.json");
            EnsureFileExists("solves.json");
            EnsureFileExists("logs.json");
            EnsureFileExists("settings.json");
        }

        public static string GetFilePath(string fileName)
        {
            return Path.Combine(DataFolder, fileName);
        }

        private static string ResolveDataFolder()
        {
            if (BaseDirectory.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFullPath(Path.Combine(BaseDirectory, "..", "..", "..", "Data"));
            }

            return LegacyDataFolder;
        }

        private static void MoveLegacyFiles()
        {
            if (string.Equals(LegacyDataFolder, DataFolder, StringComparison.OrdinalIgnoreCase) || !Directory.Exists(LegacyDataFolder))
            {
                return;
            }

            foreach (var filePath in Directory.GetFiles(LegacyDataFolder, "*.json"))
            {
                string targetPath = Path.Combine(DataFolder, Path.GetFileName(filePath));
                if (!File.Exists(targetPath))
                {
                    File.Copy(filePath, targetPath);
                }
            }
        }

        private static void EnsureFileExists(string fileName)
        {
            string filePath = GetFilePath(fileName);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }
        }
    }
}
