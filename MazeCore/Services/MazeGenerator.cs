using System;
using System.Collections.Generic;
using MazeCore.Models;

namespace MazeCore.Services
{
    public class MazeGenerator
    {
        private static Random rng = new Random();

        // Допоміжна структура для координат клітинки
        public struct Cell
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Cell(int x, int y) { X = x; Y = y; }
        }

        public Maze GenerateMaze(string name, int width, int height, string creatorName)
        {
            // Розміри мають бути непарними для правильної генерації стін
            if (width % 2 == 0) width++;
            if (height % 2 == 0) height++;

            Maze newMaze = new Maze(name, width, height, creatorName);
            int[][] grid = newMaze.Grid;

            Stack<Cell> stack = new Stack<Cell>();

            // Починаємо з координати (1, 1)
            Cell current = new Cell(1, 1);
            grid[current.Y][current.X] = 0; // 0 = прохід
            stack.Push(current);

            while (stack.Count > 0)
            {
                current = stack.Peek();
                List<Cell> unvisitedNeighbors = GetUnvisitedNeighbors(current, grid, width, height);

                if (unvisitedNeighbors.Count > 0)
                {
                    // Вибираємо випадкового сусіда
                    Cell next = unvisitedNeighbors[rng.Next(unvisitedNeighbors.Count)];

                    // Ламаємо стіну між поточною клітинкою та сусідньою
                    int wallX = current.X + (next.X - current.X) / 2;
                    int wallY = current.Y + (next.Y - current.Y) / 2;
                    grid[wallY][wallX] = 0; // Робимо прохід

                    // Відмічаємо сусіда як прохід і додаємо в стек
                    grid[next.Y][next.X] = 0;
                    stack.Push(next);
                }
                else
                {
                    // Якщо сусідів немає (тупик), повертаємося назад (Pop)
                    stack.Pop();
                }
            }

            // Робимо вхід і вихід
            grid[1][0] = 0; // Вхід зліва зверху
            grid[height - 2][width - 1] = 0; // Вихід справа знизу

            return newMaze;
        }

        private List<Cell> GetUnvisitedNeighbors(Cell current, int[][] grid, int width, int height)
        {
            List<Cell> neighbors = new List<Cell>();

            // Перевіряємо через одну клітинку (бо стіни займають 1 клітинку)
            // Вгору
            if (current.Y - 2 > 0 && grid[current.Y - 2][current.X] == 1)
                neighbors.Add(new Cell(current.X, current.Y - 2));
            // Вниз
            if (current.Y + 2 < height - 1 && grid[current.Y + 2][current.X] == 1)
                neighbors.Add(new Cell(current.X, current.Y + 2));
            // Вліво
            if (current.X - 2 > 0 && grid[current.Y][current.X - 2] == 1)
                neighbors.Add(new Cell(current.X - 2, current.Y));
            // Вправо
            if (current.X + 2 < width - 1 && grid[current.Y][current.X + 2] == 1)
                neighbors.Add(new Cell(current.X + 2, current.Y));

            return neighbors;
        }
    }
}