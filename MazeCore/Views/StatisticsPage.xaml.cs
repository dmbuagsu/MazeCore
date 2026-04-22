using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MazeCore.Common;
using MazeCore.Models;

namespace MazeCore.Views
{
    public partial class StatisticsPage : Page
    {
        // Використовуємо ObservableCollection для Data Binding
        private ObservableCollection<Maze> _mazesCollection;
        private const string MazesFileName = "mazes.json";

        public StatisticsPage()
        {
            InitializeComponent();
            LoadData();
        }
        // Додай цей метод всередину класу StatisticsPage
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text.Trim();

            // Якщо поле порожнє, просто завантажуємо всі дані (скидання пошуку)
            if (string.IsNullOrEmpty(query))
            {
                LoadData();
                return;
            }

            // Завантажуємо всі лабіринти з бази
            List<Maze> allMazes = JsonProvider.LoadFromFile<Maze>(MazesFileName);

            // Очищаємо поточну колекцію в UI
            _mazesCollection.Clear();

            foreach (var maze in allMazes)
            {
                // 1. Точний пошук: перевіряємо, чи містить назва введений текст (без урахування регістру)
                bool exactMatch = maze.Name.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0;

                // 2. Fuzzy Search: перевіряємо відстань Левенштейна
                int distance = FuzzySearch.CalculateLevenshteinDistance(maze.Name, query);
                bool fuzzyMatch = distance <= 2; // Допускаємо 2 опечатки

                // Якщо знайдено точний збіг АБО схоже слово за Левенштейном
                if (exactMatch || fuzzyMatch)
                {
                    _mazesCollection.Add(maze);
                }
            }

            if (_mazesCollection.Count == 0)
            {
                MessageBox.Show("За вашим запитом нічого не знайдено.", "Пошук", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(); // Повертаємо весь список
            }
        }
        // Метод завантаження даних
        private void LoadData()
        {
            // Читаємо з файлу
            List<Maze> loadedMazes = JsonProvider.LoadFromFile<Maze>(MazesFileName);

            // Ініціалізуємо колекцію
            _mazesCollection = new ObservableCollection<Maze>(loadedMazes);

            // Прив'язуємо дані до DataGrid
            MazesDataGrid.ItemsSource = _mazesCollection;
        }

        // Кнопка сортування
        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mazesCollection == null || !_mazesCollection.Any()) return;

            // Перетворюємо в List для нашого алгоритму
            List<Maze> listToSort = _mazesCollection.ToList();

            // Застосовуємо власний QuickSort
            SortProvider.SortMazes(listToSort);

            // Оновлюємо ObservableCollection (інтерфейс оновиться автоматично)
            _mazesCollection.Clear();
            foreach (var maze in listToSort)
            {
                _mazesCollection.Add(maze);
            }

            MessageBox.Show("Дані відсортовано за складністю (Площею), а потім за назвою!", "Сортування", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Кнопка оновлення даних
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // Повернення в меню
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}