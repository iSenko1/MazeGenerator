using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortestPathAlgorithm : MonoBehaviour
{
    public GameObject breadcrumbPrefab;
    public MazeSpawner mazeSpawner;  // Drag and drop the GameObject with MazeSpawner script here through Unity Inspector
    public Transform exitPosition;
    private float breadcrumbsShowTime = 5.0f;  // Initial time duration for breadcrumbs to be shown
    private List<GameObject> breadcrumbs = new List<GameObject>();

    private int rows;
    private int columns;
    private Vector3 cellSize;

    public GameObject playerGameObject;

    // Use this for initialization
    void Start()
    {
        playerController playerCtrl = playerGameObject.GetComponent<playerController>();

        if (mazeSpawner == null)
        {
            mazeSpawner = GetComponent<MazeSpawner>();
        }

        rows = mazeSpawner.Rows;
        columns = mazeSpawner.Columns;
        cellSize = new Vector3(mazeSpawner.CellWidth, 0, mazeSpawner.CellHeight);

        exitPosition = mazeSpawner.ExitTransform;
        Debug.Log("Starting breadcrumb generation");

        CreateBreadcrumbs(playerCtrl.startPosition);
    }

    void CreateBreadcrumbs(Vector3 startCell)
    {
        Debug.Log("Start Cell: " + startCell);
        Vector3 exitCell = new Vector3(36.0f, 0.0f, 36.0f);
        Debug.Log("Exit Cell: " + exitCell);

        bool[,] visited = new bool[rows, columns];
        Vector3?[,] parent = new Vector3?[rows, columns];
        Queue<Vector3> queue = new Queue<Vector3>();

        queue.Enqueue(startCell);
        visited[(int)(startCell.z / cellSize.z), (int)(startCell.x / cellSize.x)] = true;

        while (queue.Count > 0)
        {
            Vector3 currentCell = queue.Dequeue();

            // Debug Log
            Debug.Log("Current Cell: " + currentCell);

            if (Vector3.Distance(currentCell, exitCell) < 0.01f)
            {
                Debug.Log("Found the exit cell, creating breadcrumbs.");
                while (Vector3.Distance(currentCell, startCell) > 0.01f)
                {
                    GameObject breadcrumb = Instantiate(breadcrumbPrefab, currentCell + new Vector3(0, 0.5f, 0), Quaternion.identity);
                    breadcrumb.SetActive(false);  // Hide breadcrumb
                    breadcrumbs.Add(breadcrumb); // Add to the list

                    // Debug Log
                    Debug.Log("Placing breadcrumb at: " + currentCell);

                    int row = (int)(currentCell.z / cellSize.z);
                    int col = (int)(currentCell.x / cellSize.x);
                    currentCell = parent[row, col].Value;
                }
                // Place the breadcrumb at the start cell too
                GameObject startBreadcrumb = Instantiate(breadcrumbPrefab, startCell + new Vector3(0, 0.5f, 0), Quaternion.identity);
                startBreadcrumb.SetActive(false);  // Hide breadcrumb
                breadcrumbs.Add(startBreadcrumb); // Add to the list
                return;
            }

            List<Vector3> adjacentCells = GetAdjacentCells(currentCell);
            foreach (var adjacentCell in adjacentCells)
            {
                int row = (int)(adjacentCell.z / cellSize.z);
                int col = (int)(adjacentCell.x / cellSize.x);
                    
                if (row >= 0 && row < rows && col >= 0 && col < columns && !visited[row, col])
                {
                    visited[row, col] = true;
                    parent[row, col] = currentCell;
                    queue.Enqueue(adjacentCell);
                }
            }
        }
    }


    public void ShowBreadcrumbs()
    {
        Debug.Log("ShowBreadcrumbs function called.");

        if (breadcrumbs.Count == 0)
        {
            Debug.Log("No breadcrumbs to show!");
            return;
        }

        StartCoroutine(ShowAndHideBreadcrumbs(breadcrumbsShowTime));

        breadcrumbsShowTime += 5.0f;  // Increase time by 5 seconds for each subsequent call
    }

    IEnumerator ShowAndHideBreadcrumbs(float duration)
    {
        // Activate breadcrumbs
        foreach (GameObject breadcrumb in breadcrumbs)
        {
            if (breadcrumb != null)
            {
                breadcrumb.SetActive(true);  // Show breadcrumb
            }
            else
            {
                Debug.Log("Found a null breadcrumb in the list");
            }
        }

        // Wait for 'duration' seconds
        yield return new WaitForSeconds(duration);

        // Deactivate breadcrumbs
        foreach (GameObject breadcrumb in breadcrumbs)
        {
            if (breadcrumb != null)
            {
                breadcrumb.SetActive(false);  // Hide breadcrumb
            }
        }
    }



    List<Vector3> GetAdjacentCells(Vector3 cell)
    {
        List<Vector3> adjacentCells = new List<Vector3>();
        Vector3 right = cell + new Vector3(cellSize.x, 0, 0);
        Vector3 left = cell - new Vector3(cellSize.x, 0, 0);
        Vector3 up = cell + new Vector3(0, 0, cellSize.z);
        Vector3 down = cell - new Vector3(0, 0, cellSize.z);

        Debug.DrawLine(cell, right, Color.red, 1.0f);  // Debug Line
        Debug.DrawLine(cell, left, Color.red, 1.0f);  // Debug Line
        Debug.DrawLine(cell, up, Color.red, 1.0f);  // Debug Line
        Debug.DrawLine(cell, down, Color.red, 1.0f);  // Debug Line

        int layerMask = LayerMask.GetMask("Wall");

        if (!Physics.Linecast(cell, right, layerMask))
        {
            Debug.Log("Right is clear");
            adjacentCells.Add(right);
        }
        if (!Physics.Linecast(cell, left, layerMask))
        {
            Debug.Log("Left is clear");
            adjacentCells.Add(left);
        }
        if (!Physics.Linecast(cell, up, layerMask))
        {
            Debug.Log("Up is clear");
            adjacentCells.Add(up);
        }
        if (!Physics.Linecast(cell, down, layerMask))
        {
            Debug.Log("Down is clear");
            adjacentCells.Add(down);
        }

        return adjacentCells;
    }


}
