using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Global;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; protected set; }

    // Globaly Used Variables
    HeroDataReader heroData = new HeroDataReader();
    public HeroDataReader GetHeroData { get { return heroData; } }

    // Variables For starting Battle Scene
    List<MonsterInfo> enemyMonsterParty = new List<MonsterInfo>();
    public List<MonsterInfo> GetEnemyMonsterParty { get { return enemyMonsterParty; } }
    public void AddToEnemyMonsterParty(MonsterInfo mi)
    {
        enemyMonsterParty.Add(mi);
    }
    Dictionary<string, MonsterInfo> playerMonsterParty = new Dictionary<string, MonsterInfo>();
    public Dictionary<string, MonsterInfo> GetPlayerMonsterParty { get { return playerMonsterParty; } }
    public void AddToPlayerMonsterParty(MonsterInfo mi)
    {
        if (playerMonsterParty.ContainsKey(mi.UniqueID))
        {
            playerMonsterParty[mi.UniqueID] = mi;
        }
        else
            playerMonsterParty.Add(mi.UniqueID, mi);
    }
    public void RemoveFromPlayerMonsterParty(MonsterInfo mi)
    {
        if (playerMonsterParty.ContainsKey(mi.UniqueID))
            playerMonsterParty.Remove(mi.UniqueID);
    }
    int cacheBattleBounty;
    public int GetCacheBattleBounty { get { return cacheBattleBounty; } }
    public void SetCacheBattleBounty(int bounty) { cacheBattleBounty = bounty; }

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
    // TODO... The BSP_DungeonGenerator will need to be amended to store a list of them, one for each floor of a dungeon.
    BSP_MapGen BSP_DungeonGenerator;
    public BSP_MapGen GetBSP { get { return BSP_DungeonGenerator; } }

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

        heroData.Setup();
        heroData.ReadData();
        AssignPlayerMonsterParty();

        if (dungeonMapDictionary.Count < 1)
        {
            BSP_DungeonGenerator = new BSP_MapGen(dungeonMapWidth, dungeonMapHeight, 3);
            BSP_DungeonGenerator.GenerateBSPDungeon();
            SetDungeonFloorMap(1, BSP_DungeonGenerator.GetMap);
            playerDungeonPosition = new Vector2Int((int)BSP_DungeonGenerator.GetMapEntrance.x, (int)BSP_DungeonGenerator.GetMapEntrance.y);            
        }
    }

    void AssignPlayerMonsterParty()
    {
        foreach (var mi in heroData.heroWrapper.HeroData.HeroInfo.PlayerDemons)
        {
            AddToPlayerMonsterParty(mi);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void StartBattle()
    {
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
        // Ensure EnemyMonsterParty is empty on leving the battle
        enemyMonsterParty.Clear();

        SceneManager.LoadScene("Dungeon");
    }

    public void GameOver()
    {
        // Temp - Quit game from editor - this will NOT be in the game at all later
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
