using System;

namespace MazeCore.Models
{
    // Запис про пройдений лабіринт
    public class SolveRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MazeName { get; set; }
        public string PlayerLogin { get; set; }
        public int SolveTimeSeconds { get; set; }
        public int PathLength { get; set; }
        public string Algorithm { get; set; } // "BFS" або "Ручний"
        public DateTime SolvedAt { get; set; }
    }
}
