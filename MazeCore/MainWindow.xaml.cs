using System.Windows;
using MazeCore.Views; // Підключаємо твою папку Views

namespace MazeCore
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Завантажуємо головне меню при старті
            MainFrame.Navigate(new MainMenuPage());
        }
    }
}