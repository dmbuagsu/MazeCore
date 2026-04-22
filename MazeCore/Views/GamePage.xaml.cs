using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MazeCore.Models;
using MazeCore.Services;
using MazeCore.Common;

namespace MazeCore.Views
{
    public partial class GamePage : Page
    {
        private List<Maze> _allMazes;
        private Maze _currentMaze;
        private const string MazesFileName = "mazes.json";
        private const string SolvesFileName = "solves.json";
        private double _cellSize = 20;

        public GamePage()
        {
            InitializeComponent();
            LoadMazes();
        }

        private void LoadMazes()
        {
            _allMazes = JsonProvider.LoadFromFile<Maze>(MazesFileName);
            MazeComboBox.ItemsSource = _allMazes;
            if (_allMazes.Any())
            {
                MazeComboBox.SelectedIndex = 0;
            }
        }

        private void MazeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MazeComboBox.SelectedItem is Maze selectedMaze)
            {
                _currentMaze = selectedMaze;
                DrawMaze();
                UpdateComplexity();
            }
        }

        private void QuickGameButton_Click(object sender, RoutedEventArgs e)
        {
            var generator = new MazeGenerator();
            _currentMaze = generator.GenerateMaze("Швидка гра", 31, 31, "System");
            MazeComboBox.SelectedItem = null;
            DrawMaze();
            UpdateComplexity();
        }

        private void UpdateComplexity()
        {
            if (_currentMaze == null) return;
            double pct = ComplexityAnalyzer.CalculateComplexity(_currentMaze);
            string label = ComplexityAnalyzer.GetComplexityLabel(pct);
            ComplexityTextBlock.Text = $"{label} ({pct}%)";
            PathLengthTextBlock.Text = "-";
        }

        private void DrawMaze()
        {
            if (_currentMaze == null) return;

            MazeCanvas.Children.Clear();
            MazeCanvas.Width = _currentMaze.Width * _cellSize;
            MazeCanvas.Height = _currentMaze.Height * _cellSize;

            for (int y = 0; y < _currentMaze.Height; y++)
            {
                for (int x = 0; x < _currentMaze.Width; x++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = _cellSize,
                        Height = _cellSize,
                        // Використовуємо кольори, що підходять до Viewbox
                        Fill = _currentMaze.Grid[y][x] == 1 ? Brushes.Black : Brushes.White
                    };

                    if (y == 1 && x == 0) rect.Fill = Brushes.LightGreen; // Вхід
                    if (y == _currentMaze.Height - 2 && x == _currentMaze.Width - 1) rect.Fill = Brushes.Coral; // Вихід

                    Canvas.SetLeft(rect, x * _cellSize);
                    Canvas.SetTop(rect, y * _cellSize);
                    MazeCanvas.Children.Add(rect);
                }
            }

            SolveButton.IsEnabled = true;
            ClearButton.IsEnabled = false;
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMaze == null) return;

            var startTime = DateTime.Now;
            var path = PathFinder.FindPath(_currentMaze);
            var endTime = DateTime.Now;

            if (path.Count > 0)
            {
                PathLengthTextBlock.Text = path.Count.ToString();
                DrawPath(path);

                // Збереження результату
                SaveSolveRecord(path.Count, (int)(endTime - startTime).TotalMilliseconds);
                LogService.Log("Solve", $"Лабіринт {_currentMaze.Name} вирішено алгоритмом BFS. Довжина: {path.Count}");
            }
            else
            {
                MessageBox.Show("Шлях не знайдено! Можливо лабіринт неможливо пройти.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            SolveButton.IsEnabled = false;
            ClearButton.IsEnabled = true;
        }

        private void DrawPath(List<(int Row, int Col)> path)
        {
            // Пропускаємо вхід і вихід (перший і останній елемент), щоб не перекривати кольори
            for (int i = 1; i < path.Count - 1; i++)
            {
                var point = path[i];
                Rectangle pathRect = new Rectangle
                {
                    Width = _cellSize - 6, // Робимо лінію тоншою за клітинку
                    Height = _cellSize - 6,
                    Fill = Brushes.DodgerBlue,
                    RadiusX = 2,
                    RadiusY = 2
                };

                Canvas.SetLeft(pathRect, point.Col * _cellSize + 3);
                Canvas.SetTop(pathRect, point.Row * _cellSize + 3);
                MazeCanvas.Children.Add(pathRect);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DrawMaze();
        }

        private void SaveSolveRecord(int pathLength, int timeMs)
        {
            try
            {
                var records = JsonProvider.LoadFromFile<SolveRecord>(SolvesFileName);
                records.Add(new SolveRecord
                {
                    MazeName = _currentMaze.Name,
                    PlayerLogin = AuthService.CurrentUser?.Login ?? "Guest",
                    SolveTimeSeconds = timeMs, // Зберігаємо в мілісекундах для більшої точності
                    PathLength = pathLength,
                    Algorithm = "BFS",
                    SolvedAt = DateTime.Now
                });
                JsonProvider.SaveToFile(SolvesFileName, records);
            }
            catch { }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
