using System.Windows;
using System.Windows.Controls;

namespace MazeCore.Views
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
        }

        private void GoToGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage());
        }
    }
}