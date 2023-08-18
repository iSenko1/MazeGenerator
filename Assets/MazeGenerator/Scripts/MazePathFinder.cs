using System.Collections.Generic;
using UnityEngine;

public class MazePathfinder
{
    private MazeCell startCell;
    private MazeCell goalCell;
    private BasicMazeGenerator mazeGenerator;

    public MazePathfinder(MazeCell startCell, MazeCell goalCell, BasicMazeGenerator mazeGenerator)
    {
        this.startCell = startCell;
        this.goalCell = goalCell;
        this.mazeGenerator = mazeGenerator;
    }

    public List<MazeCell> FindPath()
    {
        Stack<MazeCell> stack = new Stack<MazeCell>();
        List<MazeCell> visitedCells = new List<MazeCell>();

        stack.Push(startCell);
        visitedCells.Add(startCell);

        while (stack.Count > 0)
        {
            MazeCell cell = stack.Pop();

            if (cell == goalCell)
            {
                // Found the path
                return visitedCells;
            }

            foreach (MazeCell neighbor in GetUnvisitedNeighbors(cell, visitedCells))
            {
                stack.Push(neighbor);
                visitedCells.Add(neighbor);
            }
        }

        // No path found
        return new List<MazeCell>();
    }

    private List<MazeCell> GetUnvisitedNeighbors(MazeCell cell, List<MazeCell> visitedCells)
    {
        List<MazeCell> unvisitedNeighbors = new List<MazeCell>();
        // Here you need to implement the logic for retrieving the unvisited neighbors of a cell.
        // This will depend on how your maze is represented in your MazeGenerator class.
        // You will likely need to check if a wall exists between the current cell and its neighbors.
        return unvisitedNeighbors;
    }


}
