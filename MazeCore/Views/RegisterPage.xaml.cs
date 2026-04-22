using System.Windows;
using System.Windows.Controls;
using MazeCore.Services;
using MazeCore.Models;

namespace MazeCore.Views
{
    public partial class RegisterPage : Page
    {
        private readonly AuthService _authService;

        public RegisterPage()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // 1. Перевірка на порожнечу (Валідація вводу)
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowError("Всі поля є обов'язковими для заповнення.");
                return;
            }

            // 2. Перевірка мінімальної довжини логіну/пароля
            if (login.Length < 3 || password.Length < 4)
            {
                ShowError("Логін має бути від 3 символів, а пароль - від 4.");
                return;
            }

            // 3. Перевірка збігу паролів
            if (password != confirmPassword)
            {
                ShowError("Паролі не співпадають!");
                return;
            }

            // 4. Спроба реєстрації через сервіс (за замовчуванням даємо роль User)
            bool isRegistered = _authService.Register(login, password, fullName, UserRole.User);

            if (isRegistered)
            {
                MessageBox.Show("Реєстрація пройшла успішно! Тепер ви можете увійти.",
                                "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                // Повертаємось на сторінку входу
                NavigationService?.Navigate(new LoginPage());
            }
            else
            {
                ShowError("Користувач з таким логіном вже існує!");
            }
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack(); // Або NavigationService?.Navigate(new LoginPage());
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}