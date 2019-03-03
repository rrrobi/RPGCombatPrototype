using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; protected set; }

    // Variables For starting Battle Scene
    // num of enemies
    int numOfEnemies = 1;
    public int GetNumOfEnemies { get { return numOfEnemies; } }
    public void SetNumOfEnemies(int input) { numOfEnemies = input; }
    // num of Friendlies
    int numOfFriendlies = 1;
    public int GetNumOfFriendlies { get { return numOfFriendlies; } }
    public void SetNumOfFriendlies(int input) { numOfFriendlies = input; }

    // Variables for Starting Dungeon Scene
    int playerCurrentFloor = 1;
    Vector2Int playerDungeonPosition = new Vector2Int();
    public Vector2Int GetPlayerDungeonPosition { get { return playerDungeonPosition; } }
    public void SetPlayerDungeonPosition(Vector2Int pos) { playerDungeonPosition = pos; }
    Dictionary<int, int[,]> dungeonMapDictionary = new Dictionary<int, int[,]>();
    public int[,] GetDungeonFloorMap(int floorNum) { return dungeonMapDictionary[floorNum]; }
    public void SetDungeonFloorMap (int floorNum, int[,] map)
    {
        if (dungeonMapDictionary.ContainsKey(floorNum))
        {
            // floor already exists
            Debug.Log("Overwrite floor: " + floorNum);
            dungeonMapDictionary.Remove(floorNum);
            dungeonMapDictionary.Add(floorNum, map);
        }
        else
            dungeonMapDictionary.Add(floorNum, map);
    }
    int dungeonMapWidth = 50;
    public int GetDungeonMapWidth { get { return dungeonMapWidth; } }    
    int dungeonMapHeight = 30;
    public int GetDungeonMapHeight { get { return dungeonMapHeight; } }
    BSP_MapGen BSP_DungeonGenerator;

    // Use this for initialization
    void Start () {
        if (Instance != null)
        {
            Debug.Log("There should never be more than one GameManager.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);

        if (dungeonMapDictionary.Count < 1)
        {
            BSP_DungeonGenerator = new BSP_MapGen(dungeonMapWidth, dungeonMapHeight);
            SetDungeonFloorMap(1, BSP_DungeonGenerator.GenerateBSPDungeon());
            playerDungeonPosition = new Vector2Int((int)BSP_DungeonGenerator.GetMapEntrance.x, (int)BSP_DungeonGenerator.GetMapEntrance.y);            
        }
    }

    

    // Update is called once per frame
    void Update () {
		
	}

    public void StartBattle()
    {
        //// Temp- beyond temp, to add some UI input to manipulate the battles
        //GameObject panel = GameObject.Find("BattleInfoPanel");
        //InputField[] inputs = panel.GetComponentsInChildren<InputField>();
        //foreach (var input in inputs)
        //{
        //    int value;
        //    if (!int.TryParse(input.text, out value))
        //        value = 2;

        //    if (input.name == "InputFriendlyCount")
        //        numOfFriendlies = value;
        //    if (input.name == "InputEnemyCount")
        //        numOfEnemies = value;
        //}

        SceneManager.LoadScene("CombatPrototype");
    }

    public void ReturnToDungeon()
    {
        // Cache Battle has been won,
        // Replace Cache with Empty Cache
        if (dungeonMapDictionary[playerCurrentFloor][playerDungeonPosition.x, playerDungeonPosition.y] == 2)
        {
            // Ensure current position is a full Cache, else something has gone wrong,
            // Assign Empty Cache tile index instead
            dungeonMapDictionary[playerCurrentFloor][playerDungeonPosition.x, playerDungeonPosition.y] = 5;
        }
        else
        { Debug.Log("Something has gone wrong, we are returning to the dungeon while not standing on a Cache!"); }
        SceneManager.LoadScene("Dungeon");
    }

    public void GameOver()
    {
        // Temp - Quit game from editor - this will NOT be in the game at all later
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
