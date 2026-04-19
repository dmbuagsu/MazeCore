using System.Windows;

namespace MazeCore
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Ви ввели: {TestInput.Text}", "Тест UI");
        }
    }
}