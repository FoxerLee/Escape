using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public int rows = 0;
    public int columns = 0;
    public int seed = 2333;
    public GameObject[] floorTiles;
    public GameObject lightFloor;
    public GameObject[] outerWalTiles;
    public GameObject exitTile;

    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;

    Transform boardHolder;
    List<Vector3> gridPositions = new List<Vector3>();
    Text seed_show;

    
    public void SetupScene(int level)
    {
        // TODO: Here we need to update "seed" variable based on player's input
        seed_show = GameObject.Find("seed_show").GetComponent<Text>();
        seed_show.text = "Seed: " + seed;
        Random.seed = seed + GameController.instance.level;
        BoardSetup();
        ExitSetup();

        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
    }

    private void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                // GameObject toInstantiate = floorTiles[(seed + x + y)%floorTiles.Length];
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || y == -1 || x == columns || y == rows)
                {
                    // toInstantiate = outerWalTiles[(seed + x + y)%outerWalTiles.Length];
                    toInstantiate = outerWalTiles[Random.Range(0, outerWalTiles.Length)];
                }
                if (x == 0 && y == 0)
                    toInstantiate = lightFloor;

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    private void ExitSetup()
    {
        Instantiate(exitTile, new Vector3(columns-1, rows-1, 0f), Quaternion.identity);
    }

    private void InitialiseList()
    {
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    private Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

}
