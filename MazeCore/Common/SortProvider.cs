using System;
using System.Collections.Generic;
using MazeCore.Models;

namespace MazeCore.Common
{
    public static class SortProvider
    {
        // Головний метод, який викликається ззовні
        public static void SortMazes(List<Maze> list)
        {
            if (list == null || list.Count <= 1) return;
            QuickSort(list, 0, list.Count - 1);
        }

        // Рекурсивний алгоритм QuickSort
        private static void QuickSort(List<Maze> list, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(list, low, high);
                QuickSort(list, low, pivotIndex - 1);
                QuickSort(list, pivotIndex + 1, high);
            }
        }

        // Розбиття масиву
        private static int Partition(List<Maze> list, int low, int high)
        {
            Maze pivot = list[high];
            int i = (low - 1);

            for (int j = low; j < high; j++)
            {
                // Викликаємо наш метод порівняння
                if (CompareMazes(list[j], pivot) <= 0)
                {
                    i++;
                    // Обмін елементів
                    Maze temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }

            // Ставимо pivot на правильне місце
            Maze temp1 = list[i + 1];
            list[i + 1] = list[high];
            list[high] = temp1;

            return i + 1;
        }

        // Логіка порівняння за двома критеріями
        private static int CompareMazes(Maze a, Maze b)
        {
            // 1 критерій: Площа (Складність)
            int areaA = a.Width * a.Height;
            int areaB = b.Width * b.Height;

            if (areaA != areaB)
            {
                return areaA.CompareTo(areaB);
            }

            // 2 критерій: Назва лабіринту (якщо розміри однакові)
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}