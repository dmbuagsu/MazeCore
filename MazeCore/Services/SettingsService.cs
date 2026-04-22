using System.Collections.Generic;
using MazeCore.Models;

namespace MazeCore.Services
{
    // Сервіс для збереження та завантаження налаштувань програми
    public class SettingsService
    {
        private const string FileName = "settings.json";

        public AppSettings LoadSettings()
        {
            var list = MazeCore.Common.JsonProvider.LoadFromFile<AppSettings>(FileName);
            if (list.Count > 0)
                return list[0];

            return new AppSettings(); // Повертаємо налаштування за замовчуванням
        }

        public void SaveSettings(AppSettings settings)
        {
            var list = new List<AppSettings> { settings };
            MazeCore.Common.JsonProvider.SaveToFile(FileName, list);
        }
    }
}
