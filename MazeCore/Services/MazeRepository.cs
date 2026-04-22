using System;
using System.Collections.Generic;
using System.Linq;
using MazeCore.Common;
using MazeCore.Models;

namespace MazeCore.Services
{
    public class MazeRepository
    {
        private const string FileName = "mazes.json";

        public List<Maze> GetAll()
        {
            return JsonProvider.LoadFromFile<Maze>(FileName)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();
        }

        public Maze? GetById(string? mazeId)
        {
            if (string.IsNullOrWhiteSpace(mazeId))
            {
                return null;
            }

            return GetAll().FirstOrDefault(m => string.Equals(m.Id, mazeId, StringComparison.OrdinalIgnoreCase));
        }

        public List<Maze> Search(string query)
        {
            var allMazes = GetAll();
            if (string.IsNullOrWhiteSpace(query))
            {
                return allMazes;
            }

            query = query.Trim();

            return allMazes
                .Where(maze =>
                    maze.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    FuzzySearch.CalculateLevenshteinDistance(maze.Name, query) <= 2)
                .ToList();
        }

        public void Save(Maze maze)
        {
            var mazes = GetAll();
            var existingMaze = mazes.FirstOrDefault(m => m.Id == maze.Id);

            if (existingMaze == null)
            {
                mazes.Add(maze);
            }
            else
            {
                mazes[mazes.IndexOf(existingMaze)] = maze;
            }

            JsonProvider.SaveToFile(FileName, mazes);
        }

        public bool Delete(string mazeId)
        {
            var mazes = GetAll();
            var mazeToDelete = mazes.FirstOrDefault(m => m.Id == mazeId);
            if (mazeToDelete == null)
            {
                return false;
            }

            mazes.Remove(mazeToDelete);
            JsonProvider.SaveToFile(FileName, mazes);
            return true;
        }

        public bool Rename(string mazeId, string newName)
        {
            var mazes = GetAll();
            var mazeToUpdate = mazes.FirstOrDefault(m => m.Id == mazeId);
            if (mazeToUpdate == null)
            {
                return false;
            }

            mazeToUpdate.Name = newName.Trim();
            JsonProvider.SaveToFile(FileName, mazes);
            return true;
        }

        public List<Maze> SortByComplexityAndName(IEnumerable<Maze> mazes)
        {
            List<Maze> listToSort = mazes.ToList();
            SortProvider.SortMazes(listToSort);
            return listToSort;
        }
    }
}
