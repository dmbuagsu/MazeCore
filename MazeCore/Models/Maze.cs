using System;

namespace MazeCore.Models
{
    public class Maze
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Унікальний ID
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        // Сітка лабіринту: 1 - стіна, 0 - прохід
        public int[][] Grid { get; set; }

        public string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Порожній конструктор для JSON
        public Maze() { }

        // Зручний конструктор для створення
        public Maze(string name, int width, int height, string creator)
        {
            Name = name;
            Width = width;
            Height = height;
            CreatorName = creator;
            CreatedAt = DateTime.Now;

            // Ініціалізуємо зубчастий масив стінами (одиницями)
            Grid = new int[height][];
            for (int i = 0; i < height; i++)
            {
                Grid[i] = new int[width];
                for (int j = 0; j < width; j++)
                {
                    Grid[i][j] = 1;
                }
            }
        }
    }
}