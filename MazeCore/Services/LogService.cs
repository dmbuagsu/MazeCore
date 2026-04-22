using System;
using System.Collections.Generic;
using MazeCore.Common;
using MazeCore.Models;

namespace MazeCore.Services
{
    // Статичний сервіс логування подій у програмі
    public static class LogService
    {
        private const string FileName = "logs.json";

        public static void Log(string eventType, string message)
        {
            try
            {
                var logs = JsonProvider.LoadFromFile<LogEntry>(FileName);

                logs.Add(new LogEntry
                {
                    EventType = eventType,
                    Message = message,
                    Timestamp = DateTime.Now,
                    UserLogin = AuthService.CurrentUser?.Login ?? "system"
                });

                // Зберігаємо лише останні 500 записів
                if (logs.Count > 500)
                    logs = logs.GetRange(logs.Count - 500, 500);

                JsonProvider.SaveToFile(FileName, logs);
            }
            catch
            {
                // Якщо помилка логування — просто ігноруємо, щоб не ламати програму
            }
        }
    }
}
