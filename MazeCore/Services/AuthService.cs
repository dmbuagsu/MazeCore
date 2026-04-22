using System;
using System.Collections.Generic;
using System.Linq;
using MazeCore.Models;
using MazeCore.Common;

namespace MazeCore.Services
{
    public class AuthService
    {
        private const string UsersFileName = "users.json";
        private List<User> _users;

        // Зберігаємо поточного авторизованого користувача, щоб знати, хто зараз у системі
        public static User CurrentUser { get; private set; }

        public AuthService()
        {
            _users = JsonProvider.LoadFromFile<User>(UsersFileName);

            // Якщо база порожня, створюємо дефолтного адміна
            if (!_users.Any())
            {
                Register("admin", "admin123", "Головний Адміністратор", UserRole.Admin);
            }
        }

        public bool Login(string login, string password)
        {
            var user = _users.FirstOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

            if (user == null) return false;

            // Перевіряємо введений пароль з хешем у базі
            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                CurrentUser = user;
                return true;
            }

            return false;
        }

        public bool Register(string login, string password, string fullName, UserRole role = UserRole.User)
        {
            // Перевірка на унікальність логіна
            if (_users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // Такий користувач вже є
            }

            int newId = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;

            var newUser = new User
            {
                Id = newId,
                Login = login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), // Хешуємо пароль
                FullName = fullName,
                Role = role,
                CreatedAt = DateTime.Now
            };

            _users.Add(newUser);
            JsonProvider.SaveToFile(UsersFileName, _users);
            return true;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}