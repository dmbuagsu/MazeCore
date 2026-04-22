using System.Collections.Generic;
using MazeCore.Models;

namespace MazeCore.Services
{
    // Алгоритм BFS (пошук у ширину) для знаходження найкоротшого шляху в лабіринті
    public class PathFinder
    {
        public static List<(int Row, int Col)> FindPath(Maze maze)
        {
            int[][] grid = maze.Grid;
            int rows = maze.Height;
            int cols = maze.Width;

            // Вхід зліва зверху, вихід справа знизу
            var start = (Row: 1, Col: 0);
            var end = (Row: rows - 2, Col: cols - 1);

            // Черга для BFS
            var queue = new Queue<(int Row, int Col)>();
            queue.Enqueue(start);

            // Запам'ятовуємо звідки прийшли до кожної клітинки
            var cameFrom = new Dictionary<(int, int), (int, int)>();
            cameFrom[start] = (-1, -1);

            // Чотири напрями: вгору, вниз, вліво, вправо
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            bool found = false;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == end)
                {
                    found = true;
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    int nr = current.Row + dr[i];
                    int nc = current.Col + dc[i];

                    // Перевіряємо межі
                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols)
                        continue;

                    // Стіни пропускаємо
                    if (grid[nr][nc] == 1)
                        continue;

                    var next = (nr, nc);
                    if (!cameFrom.ContainsKey(next))
                    {
                        cameFrom[next] = current;
                        queue.Enqueue(next);
                    }
                }
            }

            if (!found)
                return new List<(int, int)>(); // Шлях не знайдено

            // Відновлюємо шлях від виходу до входу, потім реверсуємо
            var path = new List<(int, int)>();
            var node = end;
            while (node != (-1, -1))
            {
                path.Add(node);
                node = cameFrom[node];
            }
            path.Reverse();

            return path;
        }
    }
}
