using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List <Vector3> gridPositions = new List<Vector3>();

    //Populate the gridPositions List with all possible positions within the outerwalls
    void InitialiseList()
    {
        gridPositions.Clear();

        for(int x = 1; x < columns - 1; x++)
        {
            for(int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for(int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                //Pick a random tile to be instantiated from the floorTiles[] array
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                //If the current tile is on the outer border, pick from the outerWallTiles array instead
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //Instantiate tile as a GameObject at the current tile position
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Place tile inside the boardHolder 
                instance.transform.SetParent(boardHolder);

            }
        }
    }

    Vector3 RandomPosition()
    {
        //Grab a random grid position from within the gridPositions List
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        //Remove this position from the List
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    //Exact number of objects to layout can be specified by setting the minimum and the maximum equal
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //Determine the number of objects to layout
        int objectCount = Random.Range(minimum, maximum+1);

        //Generate the determined number of objects
        for(int i = 0; i < objectCount; i++)
        {
            //Determine a random position to generate the object
            Vector3 randomPosition = RandomPosition();
            //Grab the object to generate from the given array
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            //Instantiate the object at the random position 
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }


    }


    //Public function to be called by the GameManager
   public void SetupScene(int level)
    {
        //Call initialization functions
        BoardSetup();
        InitialiseList();

        //Layout the walls
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        //Layout the food
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        //Determine the number of enemies to spawn, based on the current level
        int enemyCount = (int)Mathf.Log(level, 2f);
        //Layout the determined number of enemies
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        //Create the exit Tile in the upper right corner of the map
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);
    }


}
