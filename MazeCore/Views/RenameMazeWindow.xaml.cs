using System.Windows;

namespace MazeCore.Views
{
    public partial class RenameMazeWindow : Window
    {
        public string MazeName => NameTextBox.Text.Trim();

        public RenameMazeWindow(string currentName)
        {
            InitializeComponent();
            NameTextBox.Text = currentName;
            NameTextBox.SelectAll();
            NameTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
