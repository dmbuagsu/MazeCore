using System.Windows;

namespace MazeCore.Services
{
    public static class LocalizationService
    {
        public static string GetString(string key)
        {
            return Application.Current.TryFindResource(key)?.ToString() ?? key;
        }
    }
}
