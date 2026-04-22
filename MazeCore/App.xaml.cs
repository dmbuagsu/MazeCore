using System;
using System.Linq;
using System.Windows;
using MazeCore.Services;

namespace MazeCore
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Завантажуємо налаштування при старті
            var settingsService = new SettingsService();
            var settings = settingsService.LoadSettings();

            ApplyTheme(settings.Theme);
            ApplyLanguage(settings.Language);
            
            LogService.Log("System", "Додаток запущено");
        }

        public static void ApplyTheme(string theme)
        {
            // theme може бути "Light" або "Dark"
            string themePath = $"Resources/{theme}Theme.xaml";
            ChangeDictionary(themePath, "Theme.xaml");
        }

        public static void ApplyLanguage(string lang)
        {
            // lang може бути "uk" або "en"
            string langPath = $"Resources/Strings.{lang}.xaml";
            ChangeDictionary(langPath, "Strings.");
        }

        private static void ChangeDictionary(string newSource, string keyword)
        {
            var appDictionaries = Current.Resources.MergedDictionaries;

            // Знаходимо старий словник (тему або мову) і видаляємо його
            var dictToRemove = appDictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains(keyword));
            if (dictToRemove != null)
            {
                appDictionaries.Remove(dictToRemove);
            }

            // Додаємо новий
            appDictionaries.Add(new ResourceDictionary { Source = new Uri(newSource, UriKind.Relative) });
        }
    }
}
