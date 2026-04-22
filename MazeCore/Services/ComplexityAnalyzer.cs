using System;
using MazeCore.Models;

namespace MazeCore.Services
{
    // Аналіз складності лабіринту
    public class ComplexityAnalyzer
    {
        // Підраховує відсоток прохідних клітинок (чим менше — тим важче)
        public static double CalculateComplexity(Maze maze)
        {
            int total = maze.Width * maze.Height;
            int passable = 0;

            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    if (maze.Grid[y][x] == 0)
                        passable++;
                }
            }

            if (total == 0) return 0;
            return Math.Round((double)passable / total * 100, 1);
        }

        // Повертає текстову оцінку складності
        public static string GetComplexityLabel(double percent)
        {
            if (percent < 30) return "Важкий";
            if (percent < 50) return "Середній";
            return "Легкий";
        }
    }
}
