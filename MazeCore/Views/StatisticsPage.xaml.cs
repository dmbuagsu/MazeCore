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

        // --- CRUD UPDATE ---
        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if (MazesDataGrid.SelectedItem is Maze selectedMaze)
            {
                // Демонстрація операції Update. Замість повноцінного діалогового вікна введення,
                // ми просто додаємо маркер "Ред." до назви, щоб показати як оновлювати запис у файлі.
                List<Maze> allMazes = JsonProvider.LoadFromFile<Maze>(MazesFileName);
                var mazeToUpdate = allMazes.FirstOrDefault(m => m.Id == selectedMaze.Id);
                
                if (mazeToUpdate != null)
                {
                    mazeToUpdate.Name = mazeToUpdate.Name + " (Ред.)";
                    JsonProvider.SaveToFile(MazesFileName, allMazes);
                    
                    // Оновлюємо UI
                    selectedMaze.Name = mazeToUpdate.Name;
                    MazesDataGrid.Items.Refresh(); // Примусове оновлення гріда
                    
                    Services.LogService.Log("Update", $"Лабіринт {selectedMaze.Name} перейменовано");
                    MessageBox.Show("Запис оновлено (додано маркер 'Ред.')", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть лабіринт зі списку.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // --- CRUD DELETE ---
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MazesDataGrid.SelectedItem is Maze selectedMaze)
            {
                var result = MessageBox.Show($"Ви дійсно хочете видалити лабіринт '{selectedMaze.Name}'?", 
                                             "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    List<Maze> allMazes = JsonProvider.LoadFromFile<Maze>(MazesFileName);
                    var mazeToRemove = allMazes.FirstOrDefault(m => m.Id == selectedMaze.Id);
                    
                    if (mazeToRemove != null)
                    {
                        allMazes.Remove(mazeToRemove);
                        JsonProvider.SaveToFile(MazesFileName, allMazes); // Зберігаємо у файл
                        
                        _mazesCollection.Remove(selectedMaze); // Видаляємо з колекції (оновлює UI)
                        
                        Services.LogService.Log("Delete", $"Лабіринт {selectedMaze.Name} видалено");
                        MessageBox.Show("Запис успішно видалено.", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть лабіринт зі списку.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Повернення в меню
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}