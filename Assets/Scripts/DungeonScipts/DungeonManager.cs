using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour {

    BSP_MapGen currentBSPMap;
    // on start up BSP for current floor should be passed into this class from the GameManager
    // (GM stores a BSP_MapGen for each floor in a dungeon, so that it can persist between scenes)
    // Only this class should interact with BSP, GM only used for persistance
    // On interaction with cache, check BSP for enemies + reward

    // Dungeon Gen
    public Sprite sampleFloor;
    public Sprite sampleEntranceTile;
    public Sprite sampleExitTile;
    public Sprite sampleCache;
    public Sprite sampleEmptyCache;
    public Sprite sampleWall;
    public Sprite PlayerCharacter;
    int dungeonWidth;// = 50;
    int dungeonHeight;// = 30;
    int[,] dungeonMap;

    GameObject player;

    // Use this for initialization
    void Start () {
    }

    void FloorSetUp(int floorNum)
    {
        currentBSPMap = GameManager.Instance.GetBSP;
        dungeonWidth = currentBSPMap.MAP_WIDTH;
        dungeonHeight = currentBSPMap.MAP_HEIGHT;
        dungeonMap = currentBSPMap.GetMap;
        DrawMap();

        // Add Playable Character
        player = new GameObject();
        player.name = "PlayerCharacter";
        player.transform.position = new Vector3(GameManager.Instance.GetPlayerDungeonPosition.x, GameManager.Instance.GetPlayerDungeonPosition.y, 0.0f);
        player.AddComponent<SpriteRenderer>().sprite = PlayerCharacter;
        player.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
        player.AddComponent<Rigidbody2D>().gravityScale = 0;
        // fix camera to player
        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
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
                    else if (dungeonMap[x, y] == 5)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleEmptyCache;

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
                    else if (dungeonMap[x, y] == 5)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleEmptyCache;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (dungeonMap == null)
        {
            FloorSetUp(1);
            UISetup();
        }

        ReadInput();
        
    }

    private void ReadInput()
    {
        // Move around
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Move Up");
            MoveCharacter(new Vector3(0.0f, 1.0f, 0.0f));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Move Down");
            MoveCharacter(new Vector3(0.0f, -1.0f, 0.0f));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Move Left");
            MoveCharacter(new Vector3(-1.0f, 0.0f, 0.0f));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Move Right");
            MoveCharacter(new Vector3(1.0f, 0.0f, 0.0f));
        }
        // Action 
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Return pressed! do something!");
            switch (dungeonMap[(int)player.transform.position.x, (int)player.transform.position.y])
            {
                // Cache tile
                case 2:
                    StartBattle();
                    break;
                default:
                    Debug.Log("Nothing to interact with at: " + (int)player.transform.position.x + ", " + (int)player.transform.position.y);
                    break;
            }            
        }
    }

    void MoveCharacter(Vector3 dir)
    {
        if (dungeonMap[(int)player.transform.position.x + (int)dir.x, (int)player.transform.position.y + (int)dir.y] != 0)
        {
            player.transform.Translate(dir, Space.Self);
            GameManager.Instance.SetPlayerDungeonPosition(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y));
            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
        }        
    }

    public void StartBattle()
    {      
        // Get Enemy + Reward info from BSP Cache info
        Vector2Int playerPos = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y);
        RoomCache roomCache = currentBSPMap.FindCache(playerPos);

        Vector2Int cachePos = roomCache.position;
        if (cachePos == playerPos)
            Debug.Log("We have info from the cache at: X: " + cachePos.x + ", Y: " + cachePos.y);

        foreach (var mi in roomCache.guardians)
        {
            GameManager.Instance.AddToEnemyMonsterParty(mi);
        }

        GameManager.Instance.SetCacheBattleBounty(roomCache.goldLoot);

        GameManager.Instance.StartBattle();
    }

    private void UISetup()
    {
        GameObject goldInfo = GameObject.Find("GoldText");
        goldInfo.GetComponent<Text>().text = "Gold: " + GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo.GoldOwned;
    }
}
