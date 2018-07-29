using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataWrapper
{
    public GameData gameData = new GameData();
}

// check what this means
[System.Serializable]
public class GameData
{ 

    public string date = "";
    public string time = "";
    public List<MonsterData> MonsterList = new List<MonsterData>();
}

