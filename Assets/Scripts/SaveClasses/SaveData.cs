using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Global;

public struct DungeonFloorData
{
    public int[,] map;
    List<RoomCache> cacheList;
    public Vector2Int upStairsPos;
    public Vector2Int downStairsPos;
}

[System.Serializable]
public class SaveData 
{
    // Encrypted Binary save data
    // Persists between game sessions
    public string Date;
    public string Time;

    #region Hero Data
    // TODO... implement a check to ensure Hero info is not different in layout to hero save info - 
    // This could be a check, or it could simply use the same resource... look into it
    public string UniqueID = "00-000";
    public string PlayerName = "";
    public string FriendlySpriteName = "";
    public string CombatLevel = "";
    public int MaxHP = 0;
    public int CurrentHP = 0;
        
    public string StrengthModifier = "";
    public string WillModifier = "";

    public int GoldOwned = 0;

    public AbilityGroup baseActions = new AbilityGroup();
    public AbilityGroup SummonActions = new AbilityGroup();
    public AbilityGroup ItemActions = new AbilityGroup();
    public AbilityGroup SpellActions = new AbilityGroup();

    public List<MonsterInfo> PlayerDemons = new List<MonsterInfo>();
    public List<int> ActiveDemons = new List<int>();
    #endregion

    #region Dungeon Data
    
    #endregion

    #region World Data - outside world does not yet exist!
    #endregion

    public SaveData(HeroInfo hi, string dungeonData_placeholder)
    {
        Date = DateTime.Now.Date.ToString("dd-MM-yyyy");
        //Time = DateTime.Now.TimeOfDay.ToString("hh:mm:ss");
        UniqueID = hi.UniqueID;
        PlayerName = hi.PlayerName;
        FriendlySpriteName = hi.FriendlySpriteName;
        CombatLevel = hi.CombatLevel;
        MaxHP = hi.MaxHP;
        CurrentHP = hi.CurrentHP;
        StrengthModifier = hi.StrengthModifier;
        WillModifier = hi.WillModifier;
        GoldOwned = hi.GoldOwned;

        baseActions = hi.baseActions;
        SummonActions = hi.SummonActions;
        ItemActions = hi.ItemActions;
        SpellActions = hi.SpellActions;

        PlayerDemons = hi.PlayerDemons;
        ActiveDemons = hi.ActiveDemons;
    // Dungeon Data


}
}
