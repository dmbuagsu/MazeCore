using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MazeCore.Models;
using MazeCore.Services;
using MazeCore.Common;

namespace MazeCore.Views
{
    public partial class MazeEditorPage : Page
    {
        private readonly MazeGenerator _mazeGenerator;
        private Maze _currentMaze;
        private const string MazesFileName = "mazes.json";

        public MazeEditorPage()
        {
            InitializeComponent();
            _mazeGenerator = new MazeGenerator();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";

            string name = MazeNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                StatusTextBlock.Text = "Введіть назву лабіринту!";
                return;
            }

            // Валідація вводу: перевіряємо, чи ввів користувач саме числа
            if (!int.TryParse(WidthTextBox.Text, out int width) || !int.TryParse(HeightTextBox.Text, out int height))
            {
                StatusTextBlock.Text = "Ширина та висота повинні бути цілими числами!";
                return;
            }

            // Обмеження розмірів, щоб не "повісити" програму величезними масивами
            if (width < 5 || width > 101 || height < 5 || height > 101)
            {
                StatusTextBlock.Text = "Розміри мають бути від 5 до 101.";
                return;
            }

            // Отримуємо ім'я поточного користувача з AuthService
            string creator = AuthService.CurrentUser != null ? AuthService.CurrentUser.Login : "Unknown";

            // Генеруємо лабіринт
            _currentMaze = _mazeGenerator.GenerateMaze(name, width, height, creator);

            // Малюємо лабіринт на екрані
            DrawMaze(_currentMaze);

            // Робимо кнопку збереження активною
            SaveButton.IsEnabled = true;
            StatusTextBlock.Foreground = Brushes.Green;
            StatusTextBlock.Text = "Лабіринт успішно згенеровано!";
        }

        private void DrawMaze(Maze maze)
        {
            // Налаштовуємо сітку
            MazeCanvas.Rows = maze.Height;
            MazeCanvas.Columns = maze.Width;
            MazeCanvas.Children.Clear();

            // Проходимо по двовимірному масиву
            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    // Створюємо квадратик для кожної клітинки
                    Rectangle rect = new Rectangle
                    {
                        Width = 15, // Базовий розмір (Viewbox його масштабує)
                        Height = 15,
                        // Якщо 1 - стіна (чорний), якщо 0 - прохід (білий)
                        Fill = maze.Grid[y][x] == 1 ? Brushes.Black : Brushes.White
                    };

                    // Вхід (зелений) та Вихід (червоний) для краси
                    if (y == 1 && x == 0) rect.Fill = Brushes.LightGreen;
                    if (y == maze.Height - 2 && x == maze.Width - 1) rect.Fill = Brushes.Coral;

                    MazeCanvas.Children.Add(rect);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMaze == null) return;

            try
            {
                // Зчитуємо вже існуючі лабіринти
                List<Maze> existingMazes = JsonProvider.LoadFromFile<Maze>(MazesFileName);

                // Додаємо новий
                existingMazes.Add(_currentMaze);

                // Зберігаємо назад у файл
                JsonProvider.SaveToFile(MazesFileName, existingMazes);

                MessageBox.Show("Лабіринт успішно збережено в базу!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                // Скидаємо стан
                SaveButton.IsEnabled = false;
                StatusTextBlock.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}