using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MazeCore.Services;

namespace MazeCore.Views
{
    public partial class LoginPage : Page
    {
        private readonly AuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = LoginPasswordBox.Password; // Використовуємо нове ім'я!

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Будь ласка, заповніть усі поля.");
                return;
            }

            if (_authService.Login(login, password))
            {
                LogService.Log("Login", $"Користувач {login} успішно увійшов у систему");
                NavigationService?.Navigate(new MainMenuPage());
            }
            else
            {
                LogService.Log("LoginFailed", $"Невдала спроба входу для: {login}");
                ShowError("Невірний логін або пароль!");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterPage());
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}