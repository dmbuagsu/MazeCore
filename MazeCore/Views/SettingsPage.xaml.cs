using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MazeCore.Models;
using MazeCore.Services;

namespace MazeCore.Views
{
    public partial class SettingsPage : Page
    {
        private readonly SettingsService _settingsService;

        public SettingsPage()
        {
            InitializeComponent();
            _settingsService = new SettingsService();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            var settings = _settingsService.LoadSettings();

            if (settings.Theme == "Dark")
                ThemeComboBox.SelectedIndex = 1;
            else
                ThemeComboBox.SelectedIndex = 0;

            if (settings.Language == "en")
                LanguageComboBox.SelectedIndex = 1;
            else
                LanguageComboBox.SelectedIndex = 0;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTheme = (ThemeComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? "Light";
            var selectedLang = (LanguageComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? "uk";

            var settings = new AppSettings
            {
                Theme = selectedTheme,
                Language = selectedLang
            };

            _settingsService.SaveSettings(settings);

            // Застосовуємо відразу
            App.ApplyTheme(selectedTheme);
            App.ApplyLanguage(selectedLang);

            MessageBox.Show("Налаштування збережено! / Settings saved!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            LogService.Log("Settings", $"Налаштування змінено: Тема={selectedTheme}, Мова={selectedLang}");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
