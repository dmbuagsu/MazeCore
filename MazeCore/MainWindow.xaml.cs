using System.Windows;
using MazeCore.Views;

namespace MazeCore
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Завантажуємо вітальну сторінку при старті замість головного меню
            MainFrame.Navigate(new WelcomePage());
        }
    }
}