using System.Windows;

namespace MazeCore.Services
{
    public static class DialogService
    {
        public static void ShowInfo(string messageKey)
        {
            MessageBox.Show(GetText(messageKey), GetText("InfoTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string messageKey)
        {
            MessageBox.Show(GetText(messageKey), GetText("WarningTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(string messageKey)
        {
            MessageBox.Show(GetText(messageKey), GetText("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowErrorText(string message)
        {
            MessageBox.Show(message, GetText("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool Confirm(string message)
        {
            return MessageBox.Show(message, GetText("ConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        private static string GetText(string key)
        {
            return LocalizationService.GetString(key);
        }
    }
}
