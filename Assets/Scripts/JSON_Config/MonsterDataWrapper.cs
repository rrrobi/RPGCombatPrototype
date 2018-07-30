using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterDataWrapper
{
    //public MonsterDataWrapper wrapper = new MonsterDataWrapper();

    public MonstersData MonsterData = new MonstersData();

    string filename = "monsterData.json";
    string path;

    public void SetUp()
    {
        path = Application.persistentDataPath + "/" + filename;
        Debug.Log(path);
    }

    public void SaveData()
    {
        string contents = JsonUtility.ToJson(MonsterData, true);
        System.IO.File.WriteAllText(path, contents);
    }

    public void ReadData()
    {
        try
        {
            if (System.IO.File.Exists(path))
            {
                string contents = System.IO.File.ReadAllText(path);
                MonsterData = JsonUtility.FromJson<MonstersData>(contents);
                Debug.Log(MonsterData.Date);
            }
            else
            {
                Debug.Log("File: '" + path + "' does not exist.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("File not as expected at " + path);
        }
    }
}

[System.Serializable]
public class MonstersData
{
    public string Date = "";
    public List<MonsterInfo> MonsterList = new List<MonsterInfo>();
}

[System.Serializable]
public class MonsterInfo
{
    public int index = 0;
    public string MonsterName = "";
    public string FriendlySpriteName = "";
    public string EnemySpriteName = "";
    public int MaxHP = 0;

    public string StrengthModifier = "";
    public string WillModifier = "";

    public string Ability1 = "";
    public string Ability2 = "";
    public string Ability3 = "";
    public string Ability4 = "";
}
