﻿using UnityEngine;
using System.Collections;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeSpawner : MonoBehaviour
{
	public enum MazeGenerationAlgorithm
	{
		PureRecursive,
		RecursiveTree,
		RandomTree,
		OldestTree,
		RecursiveDivision,
	}

	public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
	public bool FullRandom = false;
	public int RandomSeed = 12345;
	public GameObject Floor = null;
	public GameObject Wall = null;
	public GameObject Pillar = null;
	public GameObject GoalPrefab = null;
	public GameObject ExitPrefab = null;  // Added exit prefab
	public int Rows = 5;
	public int Columns = 5;
	public float CellWidth = 5;
	public float CellHeight = 5;
	public bool AddGaps = true;

	private BasicMazeGenerator mMazeGenerator = null;

	void Start()
	{
		if (!FullRandom)
		{
			Random.seed = RandomSeed;
		}
		switch (Algorithm)
		{
			case MazeGenerationAlgorithm.PureRecursive:
				mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
				break;
			case MazeGenerationAlgorithm.RecursiveTree:
				mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
				break;
			case MazeGenerationAlgorithm.RandomTree:
				mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
				break;
			case MazeGenerationAlgorithm.OldestTree:
				mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
				break;
			case MazeGenerationAlgorithm.RecursiveDivision:
				mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
				break;
		}
		mMazeGenerator.GenerateMaze();


		// After maze generation
		int exitCorner = Random.Range(0, 3); // Gives a random number: 0, 1 or 2
		MazeCell exitCell;
		Vector3 exitPosition;

		switch (exitCorner)
		{
			case 0:
				exitCell = mMazeGenerator.GetMazeCell(0, Columns - 1); // top right cell
				exitCell.WallFront = false; // Remove the front wall of the cell
				exitPosition = new Vector3((Columns - 1) * (CellWidth + (AddGaps ? .2f : 0)), 1, 0);
				break;
			case 1:
				exitCell = mMazeGenerator.GetMazeCell(Rows - 1, 0); // bottom left cell
				exitCell.WallLeft = false; // Remove the left wall of the cell
				exitPosition = new Vector3(0, 1, (Rows - 1) * (CellHeight + (AddGaps ? .2f : 0)));
				break;
			default: // or case 2:
				exitCell = mMazeGenerator.GetMazeCell(Rows - 1, Columns - 1); // bottom right cell
				exitCell.WallRight = false; // Remove the right wall of the cell
				exitPosition = new Vector3((Columns - 1) * (CellWidth + (AddGaps ? .2f : 0)), 1, (Rows - 1) * (CellHeight + (AddGaps ? .2f : 0)));
				break;
		}
		// Instantiate exit prefab at the exit position
		if (ExitPrefab != null)
		{
			GameObject tmp = Instantiate(ExitPrefab, exitPosition, Quaternion.Euler(0, 0, 0)) as GameObject;
			tmp.transform.parent = transform;
		}



		for (int row = 0; row < Rows; row++)
		{
			for (int column = 0; column < Columns; column++)
			{
				float x = column * (CellWidth + (AddGaps ? .2f : 0));
				float z = row * (CellHeight + (AddGaps ? .2f : 0));
				MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
				GameObject tmp;
				tmp = Instantiate(Floor, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0)) as GameObject;
				tmp.transform.parent = transform;
				if (cell.WallRight)
				{
					tmp = Instantiate(Wall, new Vector3(x + CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;// right
					tmp.transform.parent = transform;
				}
				if (cell.WallFront)
				{
					tmp = Instantiate(Wall, new Vector3(x, 0, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;// front
					tmp.transform.parent = transform;
				}
				if (cell.WallLeft)
				{
					tmp = Instantiate(Wall, new Vector3(x - CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 270, 0)) as GameObject;// left
					tmp.transform.parent = transform;
				}
				if (cell.WallBack)
				{
					tmp = Instantiate(Wall, new Vector3(x, 0, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;// back
					tmp.transform.parent = transform;
				}
				if (cell.IsGoal && GoalPrefab != null)
				{
					tmp = Instantiate(GoalPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
					tmp.transform.parent = transform;
				}
			}
		}
		if (Pillar != null)
		{
			for (int row = 0; row < Rows + 1; row++)
			{
				for (int column = 0; column < Columns + 1; column++)
				{
					float x = column * (CellWidth + (AddGaps ? .2f : 0));
					float z = row * (CellHeight + (AddGaps ? .2f : 0));
					GameObject tmp = Instantiate(Pillar, new Vector3(x - CellWidth / 2, 0, z - CellHeight / 2), Quaternion.identity) as GameObject;
					tmp.transform.parent = transform;
				}
			}
		}
	}
}
