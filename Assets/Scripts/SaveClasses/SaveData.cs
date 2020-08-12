using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Global;

[System.Serializable]
public struct DungeonFloorData
{
    public int[,] map;
    public List<RoomCache> cacheList;
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
    public HeroInfo HeroInfoData;

    #endregion

    #region Dungeon Data    
    public Dictionary<int, DungeonFloorData> DungeonFloorList = new Dictionary<int, DungeonFloorData>();
    #endregion

    #region World Data - outside world does not yet exist!
    #endregion

    public SaveData(HeroInfo hi, Dictionary<int, DungeonFloorData> dungeonData)
    {
        Date = DateTime.Now.Date.ToString("dd-MM-yyyy");
        //Time = DateTime.Now.TimeOfDay.ToString("hh:mm:ss");
        HeroInfoData = hi;

        // Dungeon Data
        DungeonFloorList = dungeonData;

    }
}
