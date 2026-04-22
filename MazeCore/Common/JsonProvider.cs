using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MazeCore.Common
{
    public static class JsonProvider
    {
        // Шлях до папки з даними (створиться у bin/Debug/net8.0/Data)
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        // Універсальний метод збереження
        public static void SaveToFile<T>(string fileName, List<T> data)
        {
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }

            string filePath = Path.Combine(DataFolder, fileName);
            var options = new JsonSerializerOptions { WriteIndented = true }; // Щоб JSON був красивим
            string jsonString = JsonSerializer.Serialize(data, options);

            File.WriteAllText(filePath, jsonString);
        }

        // Універсальний метод зчитування
        public static List<T> LoadFromFile<T>(string fileName)
        {
            string filePath = Path.Combine(DataFolder, fileName);

            if (!File.Exists(filePath))
            {
                return new List<T>(); // Якщо файлу ще немає, повертаємо порожній список
            }

            string jsonString = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return new List<T>();
            }

            return JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
        }
    }
}