using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MazeCore.Services;
using MazeCore.Models;

namespace MazeCore.Views
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            User currentUser = AuthService.CurrentUser;

            if (currentUser != null)
            {
                WelcomeTextBlock.Text = currentUser.FullName;

                // Звичайний користувач не бачить редактора лабіринтів
                if (currentUser.Role == UserRole.User)
                {
                    AdminEditorButton.Visibility = Visibility.Collapsed;
                    AdminTileBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        // Кнопка в бічному меню
        private void GoToGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new GamePage());
        }

        // Клік по плитці на дашборді
        private void TileGame_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new GamePage());
        }

        private void GoToEditor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MazeEditorPage());
        }

        private void TileEditor_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new MazeEditorPage());
        }

        private void GoToStats_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StatisticsPage());
        }

        private void TileStats_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new StatisticsPage());
        }

        private void GoToSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new SettingsPage());
        }

        private void TileSettings_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new SettingsPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LogService.Log("Logout", "Користувач вийшов з системи");
            AuthService authService = new AuthService();
            authService.Logout();
            NavigationService?.Navigate(new LoginPage());
        }
    }
}