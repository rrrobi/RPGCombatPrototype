using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData 
{
    // Encrypted Binary save data
    // Persists between game sessions


    #region Hero Data
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
}
