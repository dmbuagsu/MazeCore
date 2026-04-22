using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        // Розмір клітинки на канвасі в пікселях
        private const int CellSize = 20;

        // Стан малювання перетягуванням
        private bool _isDragging = false;
        // true = малюємо стіни, false = стираємо стіни (малюємо проходи)
        private bool _drawingWall = true;

        public MazeEditorPage()
        {
            InitializeComponent();
            _mazeGenerator = new MazeGenerator();
        }

        // --- Кнопки генерації / створення ---

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string name, out int width, out int height, maxSize: 101))
                return;

            string creator = AuthService.CurrentUser?.Login ?? "Unknown";
            _currentMaze = _mazeGenerator.GenerateMaze(name, width, height, creator);

            DrawMaze(_currentMaze);
            SaveButton.IsEnabled = true;
            SetStatus("Лабіринт згенеровано! Можна редагувати кліками або перетягуванням.", success: true);
        }

        private void CreateEmptyButton_Click(object sender, RoutedEventArgs e)
        {
            // Для ручного малювання обмежуємо 51x51 — більше буде незручно малювати
            if (!ValidateInputs(out string name, out int width, out int height, maxSize: 51))
                return;

            string creator = AuthService.CurrentUser?.Login ?? "Unknown";
            _currentMaze = new Maze(name, width, height, creator);

            // Все заповнюємо проходами, лише межі — стіни
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    _currentMaze.Grid[y][x] = (x == 0 || y == 0 || x == width - 1 || y == height - 1) ? 1 : 0;

            DrawMaze(_currentMaze);
            SaveButton.IsEnabled = true;
            SetStatus("Порожнє поле створено. Малюйте стіни кліком або перетягуванням миші.", success: false);
        }

        // --- Перемикачі режиму малювання ---

        private void DrawWallBtn_Click(object sender, RoutedEventArgs e)
        {
            _drawingWall = true;
            // Виділяємо активну кнопку візуально
            DrawWallBtn.Style = (Style)FindResource("PrimaryButtonStyle");
            EraseBtn.Style = (Style)FindResource("SecondaryButtonStyle");
        }

        private void EraseBtn_Click(object sender, RoutedEventArgs e)
        {
            _drawingWall = false;
            DrawWallBtn.Style = (Style)FindResource("SecondaryButtonStyle");
            EraseBtn.Style = (Style)FindResource("PrimaryButtonStyle");
        }

        // --- Кнопка очищення (скидаємо до порожнього поля) ---

        private void ClearMazeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMaze == null) return;

            int h = _currentMaze.Height;
            int w = _currentMaze.Width;

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    _currentMaze.Grid[y][x] = (x == 0 || y == 0 || x == w - 1 || y == h - 1) ? 1 : 0;

            // Перемальовуємо
            DrawMaze(_currentMaze);
            SetStatus("Поле очищено.", success: false);
        }

        // --- Малювання лабіринту на Canvas ---

        private void DrawMaze(Maze maze)
        {
            MazeCanvas.Children.Clear();
            MazeCanvas.Width  = maze.Width  * CellSize;
            MazeCanvas.Height = maze.Height * CellSize;

            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    var rect = CreateCell(x, y, maze.Grid[y][x]);
                    Canvas.SetLeft(rect, x * CellSize);
                    Canvas.SetTop(rect, y * CellSize);
                    MazeCanvas.Children.Add(rect);
                }
            }

            // Показуємо канвас, ховаємо підказку
            EmptyHintText.Visibility = Visibility.Collapsed;
            MazeViewbox.Visibility = Visibility.Visible;
        }

        private Rectangle CreateCell(int x, int y, int cellValue)
        {
            Brush fill;
            // Вхід — зелений, Вихід — жовтогарячий, Стіна — чорна, Прохід — білий
            if (y == 1 && x == 0)
                fill = Brushes.LightGreen;
            else if (y == (_currentMaze?.Height ?? 0) - 2 && x == (_currentMaze?.Width ?? 0) - 1)
                fill = Brushes.Coral;
            else
                fill = cellValue == 1 ? Brushes.Black : Brushes.White;

            var rect = new Rectangle
            {
                Width  = CellSize,
                Height = CellSize,
                Fill   = fill,
                Stroke = Brushes.DimGray,
                StrokeThickness = 0.3,
                Tag = new Point(x, y)
            };

            return rect;
        }

        // --- Обробники подій миші на Canvas (drag-малювання) ---

        private void MazeCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentMaze == null) return;
            _isDragging = true;
            MazeCanvas.CaptureMouse(); // захоплюємо мишу, щоб drag працював за межами клітинки
            PaintCellAt(e.GetPosition(MazeCanvas));
        }

        private void MazeCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            MazeCanvas.ReleaseMouseCapture();
        }

        private void MazeCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Малюємо тільки якщо ліва кнопка натиснута
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                PaintCellAt(e.GetPosition(MazeCanvas));
            }
        }

        private void PaintCellAt(Point mousePos)
        {
            if (_currentMaze == null) return;

            int col = (int)(mousePos.X / CellSize);
            int row = (int)(mousePos.Y / CellSize);

            // Перевіряємо межі
            if (col < 0 || row < 0 || col >= _currentMaze.Width || row >= _currentMaze.Height)
                return;

            // Не чіпаємо межі, вхід та вихід
            bool isBorder = (col == 0 || row == 0 || col == _currentMaze.Width - 1 || row == _currentMaze.Height - 1);
            bool isEntrance = (row == 1 && col == 0);
            bool isExit = (row == _currentMaze.Height - 2 && col == _currentMaze.Width - 1);
            if (isBorder || isEntrance || isExit) return;

            int newValue = _drawingWall ? 1 : 0;

            // Не перемальовуємо якщо значення не змінилось (оптимізація для drag)
            if (_currentMaze.Grid[row][col] == newValue) return;

            _currentMaze.Grid[row][col] = newValue;

            // Знаходимо прямокутник на Canvas і змінюємо його колір
            int index = row * _currentMaze.Width + col;
            if (index < MazeCanvas.Children.Count && MazeCanvas.Children[index] is Rectangle rect)
            {
                rect.Fill = newValue == 1 ? Brushes.Black : Brushes.White;
            }
        }

        // --- Збереження ---

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMaze == null) return;

            try
            {
                List<Maze> existingMazes = JsonProvider.LoadFromFile<Maze>(MazesFileName);
                existingMazes.Add(_currentMaze);
                JsonProvider.SaveToFile(MazesFileName, existingMazes);

                LogService.Log("Create", $"Лабіринт '{_currentMaze.Name}' збережено");
                MessageBox.Show("Лабіринт успішно збережено в базу!", "Успіх",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                // Після збереження скидаємо, щоб не зберегли двічі
                SaveButton.IsEnabled = false;
                SetStatus("Збережено!", success: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        // --- Допоміжні методи ---

        // Перевірка введених даних (назва + розміри)
        private bool ValidateInputs(out string name, out int width, out int height, int maxSize)
        {
            name = MazeNameTextBox.Text.Trim();
            width = 0;
            height = 0;

            if (string.IsNullOrEmpty(name))
            {
                SetStatus("Введіть назву лабіринту!", success: false);
                return false;
            }

            if (!int.TryParse(WidthTextBox.Text, out width) || !int.TryParse(HeightTextBox.Text, out height))
            {
                SetStatus("Ширина та висота повинні бути цілими числами!", success: false);
                return false;
            }

            if (width < 5 || width > maxSize || height < 5 || height > maxSize)
            {
                SetStatus($"Розміри мають бути від 5 до {maxSize}.", success: false);
                return false;
            }

            return true;
        }

        private void SetStatus(string message, bool success)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Foreground = success
                ? new SolidColorBrush(Color.FromRgb(39, 174, 96))   // зелений
                : (Brush)FindResource("SubtleText");
        }
    }
}