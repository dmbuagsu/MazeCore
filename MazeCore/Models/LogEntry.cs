using System;

namespace MazeCore.Models
{
    // Запис у журналі подій програми
    public class LogEntry
    {
        public string EventType { get; set; }   // "Login", "Logout", "Generate", "Error" тощо
        public string Message { get; set; }
        public string UserLogin { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
