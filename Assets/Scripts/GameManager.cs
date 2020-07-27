using System.Collections;
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

    // Variables for Starting Dungeon Scene
    int playerCurrentFloor = -1;
    int playerStartFloor = -1;                                              // <- not sure if this is overkill
    int baseFloorDifficulty = 3;
    public int GetPlayerCurrentFloor { get { return playerCurrentFloor; } }
    Vector2Int playerDungeonPosition = new Vector2Int();
    public Vector2Int GetPlayerDungeonPosition { get { return playerDungeonPosition; } }
    public void SetPlayerDungeonPosition(Vector2Int pos) { playerDungeonPosition = pos; }
    int dungeonMapWidth = 50;
    public int GetDungeonMapWidth { get { return dungeonMapWidth; } }    
    int dungeonMapHeight = 30;
    public int GetDungeonMapHeight { get { return dungeonMapHeight; } }
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

        // Game loads Hero Data from save file,
        // IF NewGame - this save file is set to default in Launch-Menu Scene
        LoadHeroData(); 
        AssignPlayerMonsterParty();

        if (BSP_MapDictionary.Count < 1)
        {
            SetBSPMapForFloor(playerStartFloor, new BSP_MapGen(dungeonMapWidth, dungeonMapHeight, baseFloorDifficulty));
            BSP_MapDictionary[playerStartFloor].GenerateBSPDungeon();
            playerDungeonPosition = BSP_MapDictionary[playerStartFloor].GetMapUpStairs;           
        }
    }

    void AssignPlayerMonsterParty()
    {
        foreach (var mi in heroData.heroWrapper.HeroData.HeroInfo.PlayerDemons)
        {
            AddToPlayerMonsterParty(mi);
        }
    }

    // Load Hero Info
    void LoadHeroData()
    {
        SaveData data = SaveSystem.LoadFull();
        heroData.heroWrapper.HeroData.HeroInfo.UniqueID = data.UniqueID;
        heroData.heroWrapper.HeroData.HeroInfo.PlayerName = data.PlayerName;
        heroData.heroWrapper.HeroData.HeroInfo.FriendlySpriteName = data.FriendlySpriteName;
        heroData.heroWrapper.HeroData.HeroInfo.CombatLevel = data.CombatLevel;
        heroData.heroWrapper.HeroData.HeroInfo.MaxHP = data.MaxHP;
        heroData.heroWrapper.HeroData.HeroInfo.CurrentHP = data.CurrentHP;
        heroData.heroWrapper.HeroData.HeroInfo.StrengthModifier = data.StrengthModifier;
        heroData.heroWrapper.HeroData.HeroInfo.WillModifier = data.WillModifier;
        heroData.heroWrapper.HeroData.HeroInfo.GoldOwned = data.GoldOwned;

        heroData.heroWrapper.HeroData.HeroInfo.baseActions = data.baseActions;
        heroData.heroWrapper.HeroData.HeroInfo.SummonActions = data.SummonActions;
        heroData.heroWrapper.HeroData.HeroInfo.ItemActions = data.ItemActions;
        heroData.heroWrapper.HeroData.HeroInfo.SpellActions = data.SpellActions;

        heroData.heroWrapper.HeroData.HeroInfo.PlayerDemons = data.PlayerDemons;
        heroData.heroWrapper.HeroData.HeroInfo.ActiveDemons = data.ActiveDemons;

        Dictionary<int, DungeonFloorData> dungeonData = data.DungeonFloorList;
        foreach (var floor in dungeonData)
        {
            //BSP_MapGen mapGen = new BSP_MapGen()
            //BSP_MapDictionary.Add(floor.Key, )
        }

    }

    void SaveData()
    {
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
        // Ensure EnemyMonsterParty is empty on leving the battle
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
            SetBSPMapForFloor(DestinationFloor, new BSP_MapGen(dungeonMapWidth, dungeonMapHeight, newFloorDifficulty));
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
