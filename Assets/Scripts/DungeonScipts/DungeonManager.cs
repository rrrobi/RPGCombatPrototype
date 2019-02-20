using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour {

    // Dungeon Gen
    public Sprite sampleFloor;
    public Sprite sampleEntranceTile;
    public Sprite sampleExitTile;
    public Sprite sampleCache;
    public Sprite sampleWall;
    public Sprite PlayerCharacter;
    int dungeonWidth = 50;
    int dungeonHeight = 30;
    BSP_MapGen BSP_DungeonGenerator;
    int[,] dungeonMap;

    GameObject player;

    // Use this for initialization
    void Start () {
        BSP_DungeonGenerator = new BSP_MapGen(dungeonWidth, dungeonHeight);
        dungeonMap = BSP_DungeonGenerator.GenerateBSPDungeon();
        DrawMap();

        // Add Playable Character
        player = new GameObject();
        player.name = "PlayerCharacter";
        player.transform.position = FindEntrancePosition();
        player.AddComponent<SpriteRenderer>().sprite = PlayerCharacter;
        player.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
        // fix camera to player
        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
    }

    Vector3 FindEntrancePosition()
    {
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (dungeonMap[x, y] == 3)
                    return new Vector3(x, y, 0.0f);
            }
        }

        Debug.Log("No Entrance tile found!");
        return new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void DrawMap()
    {
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                GameObject tile;
                string name = "Tile_" + x + "_" + y;
                if (GameObject.Find(name) == null)
                {
                    tile = new GameObject();
                    tile.transform.parent = GameObject.Find("DungeonMap").transform;
                    tile.name = name;
                    tile.transform.position = new Vector3(x, y);
                    if (dungeonMap[x, y] == 0)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleWall;
                    else if (dungeonMap[x, y] == 1)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleFloor;
                    else if (dungeonMap[x, y] == 2)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleCache;
                    else if (dungeonMap[x, y] == 3)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleEntranceTile;
                    else if (dungeonMap[x, y] == 4)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleExitTile;

                    tile.GetComponent<SpriteRenderer>().sortingLayerName = "BackGround";
                }
                else
                {
                    tile = GameObject.Find(name);
                    if (dungeonMap[x, y] == 0)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleWall;
                    else if (dungeonMap[x, y] == 1)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleFloor;
                    else if (dungeonMap[x, y] == 2)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleCache;
                    else if (dungeonMap[x, y] == 3)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleEntranceTile;
                    else if (dungeonMap[x, y] == 4)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleExitTile;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        ReadInput();
	}

    private void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Move Up");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Move Down");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Move Left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Move Right");
        }
    }
}
