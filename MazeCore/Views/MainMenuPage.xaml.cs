using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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
                WelcomeTextBlock.Text = $"Вітаємо, {currentUser.FullName}!";

              // Перевірка ролей: звичайний користувач не бачить редактора лабіринтів [cite: 27]
                if (currentUser.Role == UserRole.User)
                {
                    AdminEditorButton.Visibility = Visibility.Collapsed;
                    // Або можна зробити AdminEditorButton.IsEnabled = false; за бажанням
                }
            }
        }

        private void GoToGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new GamePage());
        }

        private void GoToEditor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MazeEditorPage());
        }

        private void GoToStats_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StatisticsPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Виходимо з акаунту
            AuthService authService = new AuthService();
            authService.Logout();

            // Повертаємось на екран авторизації
            NavigationService?.Navigate(new LoginPage());
        }
    }
}