﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Global;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; protected set; }

    #region SaveGame Values
    //ActiveSaveSlot activeSaveSlot;
    //public ActiveSaveSlot GetActiveSaveSlot() { return activeSaveSlot; }
    //public void SetActiveSaveSlot(ActiveSaveSlot slot) { activeSaveSlot = slot; }
    #endregion

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
    List<AbilityInfo> cacheBattleConsumableLoot;
    public List<AbilityInfo> GetCacheBattleConsumableLoot { get { return cacheBattleConsumableLoot; } }
    public void SetCacheBattleConsumableLoot(List<AbilityInfo> consumableLoot) { cacheBattleConsumableLoot = consumableLoot; }

    // Variables for Starting Dungeon Scene
    int playerCurrentFloor = -1;
    int playerStartFloor = -1;                                              // <- not sure if this is overkill
    int baseFloorDifficulty = 3;
    public int GetPlayerCurrentFloor { get { return playerCurrentFloor; } }
    Vector2Int playerDungeonPosition = new Vector2Int();
    public Vector2Int GetPlayerDungeonPosition { get { return playerDungeonPosition; } }
    public void SetPlayerDungeonPosition(Vector2Int pos) { playerDungeonPosition = pos; }
    const int DUNGEON_MAP_WIDTH = 50;
    public int GetDungeonMapWidth { get { return DUNGEON_MAP_WIDTH; } }    
    const int DUNGEON_MAP_HEIGHT = 30;
    public int GetDungeonMapHeight { get { return DUNGEON_MAP_HEIGHT; } }
    Dictionary<int, BSP_MapGen> BSP_MapDictionary = new Dictionary<int, BSP_MapGen>();
    public BSP_MapGen GetBSPMapForFloor(int floor) { return BSP_MapDictionary[floor]; }
    public BSP_MapGen GetBSPMapForCurrentFloor { get { return BSP_MapDictionary[playerCurrentFloor]; } }
    public void SetBSPMapForFloor(int floor, BSP_MapGen BSPmap)
    {
        if (BSP_MapDictionary.ContainsKey(floor))
        {
            // floor already exists, 
            // we don't want to replace the floor already produced
            // log an error
            Debug.LogError("Attempt made to replace BSPMap for floor: " + floor + ", No Action taken.");
        }
        else
            BSP_MapDictionary.Add(floor, BSPmap);
    }

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

        // Set up MonsterData config file for use        
        MonsterDataReader.SetUp();
        MonsterDataReader.ReadData();
        ConsumableDataReader.SetUp();
        ConsumableDataReader.ReadData();

        // Game loads Hero Data from save file,
        // IF NewGame - this save file is set to default in Launch-Menu Scene
        LoadGameData(); 
        AssignPlayerMonsterParty();


        //if (BSP_MapDictionary.Count < 1)
        //{
        //    SetBSPMapForFloor(playerStartFloor, new BSP_MapGen(dungeonMapWidth, dungeonMapHeight, baseFloorDifficulty));
        //    BSP_MapDictionary[playerStartFloor].GenerateBSPDungeon();
        //    playerDungeonPosition = BSP_MapDictionary[playerStartFloor].GetMapUpStairs;           
        //}
    }

    void AssignPlayerMonsterParty()
    {
        foreach (var mi in heroData.heroWrapper.HeroData.HeroInfo.PlayerDemons)
        {
            AddToPlayerMonsterParty(mi);
        }
    }

    // Load Game Info
    void LoadGameData()
    {
        SaveData data = SaveSystem.LoadFull();
        heroData.heroWrapper.HeroData.HeroInfo = data.HeroInfoData;

        Dictionary<int, DungeonFloorData> dungeonData = data.DungeonFloorList;
        // if new Game
        if (dungeonData.Count == 0)
        {
            // Generate new bungeon floor
            SetBSPMapForFloor(playerStartFloor, new BSP_MapGen(DUNGEON_MAP_WIDTH, DUNGEON_MAP_HEIGHT, baseFloorDifficulty));
            BSP_MapDictionary[playerStartFloor].GenerateBSPDungeon();
            playerDungeonPosition = BSP_MapDictionary[playerStartFloor].GetMapUpStairs;
        }
        else
        {
            playerCurrentFloor = data.HeroInfoData.CurrentDungeonFloor;
            playerDungeonPosition.x = data.HeroInfoData.CurrentXPosition;
            playerDungeonPosition.y = data.HeroInfoData.CurrentYPosition;

            foreach (var floor in dungeonData)
            {
                int newFloorDifficulty = baseFloorDifficulty + Mathf.Abs(0 - floor.Key);

                BSP_MapGen mapGen = new BSP_MapGen(DUNGEON_MAP_WIDTH, DUNGEON_MAP_HEIGHT, newFloorDifficulty);
                mapGen.LoadBSPDungeon(floor.Value.map, floor.Value.cacheList, floor.Value.upStairsPos, floor.Value.downStairsPos);
                BSP_MapDictionary.Add(floor.Key, mapGen);
            }
        }

    }

    void SaveData()
    {
        heroData.heroWrapper.HeroData.HeroInfo.CurrentDungeonFloor = playerCurrentFloor;
        heroData.heroWrapper.HeroData.HeroInfo.CurrentXPosition = playerDungeonPosition.x;
        heroData.heroWrapper.HeroData.HeroInfo.CurrentYPosition = playerDungeonPosition.y;

        Dictionary<int, DungeonFloorData> dungeonData = new Dictionary<int, DungeonFloorData>();
        foreach (var floor in BSP_MapDictionary)
        {
            DungeonFloorData floorData = new DungeonFloorData();
            floorData.map = floor.Value.GetMap;
            floorData.upStairsPos = floor.Value.GetMapUpStairs;
            floorData.downStairsPos = floor.Value.GetMapDownStairs;
            floorData.cacheList = floor.Value.GetCacheList;

            dungeonData.Add(floor.Key, floorData);
        }

        SaveSystem.FullSave(heroData.heroWrapper.HeroData.HeroInfo, dungeonData);
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
        int[,] map = BSP_MapDictionary[playerCurrentFloor].GetMap;
        // Cache Battle has been won,
        // Replace Cache with Empty Cache
        if (map[playerDungeonPosition.x, playerDungeonPosition.y] == 2)
        {
            // Ensure current position is a full Cache, else something has gone wrong,
            // Assign Empty Cache tile index instead
            BSP_MapDictionary[playerCurrentFloor].AmendMap(playerDungeonPosition, 5);
        }
        else
        { Debug.Log("Something has gone wrong, we are returning to the dungeon while not standing on a Cache!"); }
        // Ensure EnemyMonsterParty is empty on leaving the battle
        enemyMonsterParty.Clear();

        // auto save
        SaveData();
        SceneManager.LoadScene("Dungeon");
    }

    public void TravelToNextDugeonFloor(int DestinationFloor)
    {
        // check if destination floor already has BSP created for it
        if (!BSP_MapDictionary.ContainsKey(DestinationFloor))
        {
            int newFloorDifficulty = baseFloorDifficulty + Mathf.Abs(0 - DestinationFloor);
            SetBSPMapForFloor(DestinationFloor, new BSP_MapGen(DUNGEON_MAP_WIDTH, DUNGEON_MAP_HEIGHT, newFloorDifficulty));
            BSP_MapDictionary[DestinationFloor].GenerateBSPDungeon();            
        }
        // Set player position to the new 'Up Stairs' tile
        if (playerCurrentFloor > DestinationFloor)
            playerDungeonPosition = BSP_MapDictionary[DestinationFloor].GetMapUpStairs;
        else
            playerDungeonPosition = BSP_MapDictionary[DestinationFloor].GetMapDownStairs;

        // update current floor to destination floor
        playerCurrentFloor = DestinationFloor;

        // load new scene
        SceneManager.LoadScene("Dungeon");
    }

    public void GameOver()
    {
        // Temp - Quit game from editor - this will NOT be in the game at all later
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
