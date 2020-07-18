using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Global;

[System.Serializable]
public class SaveData 
{
    // Encrypted Binary save data
    // Persists between game sessions
    string Date;
    string Time;

    #region Hero Data
        // TODO... implement a check to ensure Hero info is not different in layout to hero save info - 
        // This could be a check, or it could simply use the same resource... look into it
    public string PlayerName = "";
    public string CombatLevel = "";
    public int MaxHP = 0;
    public int CurrentHP = 0;
        
    public string StrengthModifier = "";
    public string WillModifier = "";

    public int GoldOwned = 0;
    #endregion

    #region Dungeon Data

    #endregion

    #region World Data - outside world does not yet exist!
    #endregion

    public SaveData(HeroInfo hi, string dungeonData_placeholder)
    {
        Date = DateTime.Now.Date.ToString("DD-MM-YYYY");
        Time = DateTime.Now.TimeOfDay.ToString("HH:mm:ss");

        PlayerName = hi.PlayerName;
        CombatLevel = hi.CombatLevel;
        MaxHP = hi.MaxHP;
        CurrentHP = hi.CurrentHP;
        StrengthModifier = hi.StrengthModifier;
        WillModifier = hi.WillModifier;
        GoldOwned = hi.GoldOwned;

        // Dungeon Data
    }
}
