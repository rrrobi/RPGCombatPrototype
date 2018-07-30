using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDataReader
{
    public MonsterDataWrapper monsterWrapper = new MonsterDataWrapper();

    string filename = "monsterData.json";
    string path;

    public void SetUp()
    {
        path = Application.persistentDataPath + "/" + filename;
        Debug.Log(path);

        // Temp - this will be removed
        monsterWrapper.MonsterData.Date = System.DateTime.Now.ToShortDateString();

        MonsterInfo m1 = new MonsterInfo();
        m1.index = 1;
        m1.MonsterName = "Demon";
        m1.FriendlySpriteName = "BlueDemon";
        m1.EnemySpriteName = "RedDemon";
        m1.MaxHP = 50;
        m1.Ability1 = "Slash";
        m1.Ability2 = "Stab";
        monsterWrapper.MonsterData.MonsterList.Add(m1);

        MonsterInfo m2 = new MonsterInfo();
        m2.index = 2;
        m2.MonsterName = "Demon Swarm";
        m2.FriendlySpriteName = "BlueDemonSwarm";
        m2.EnemySpriteName = "RedDemonSwarm";
        m2.MaxHP = 15;
        m2.Ability1 = "Tackle";
        monsterWrapper.MonsterData.MonsterList.Add(m2);

        MonsterInfo m3 = new MonsterInfo();
        m3.index = 3;
        m3.MonsterName = "Heavy Demon";
        m3.FriendlySpriteName = "HeavyBlueDemon";
        m3.EnemySpriteName = "HeavyRedDemon";
        m3.MaxHP = 75;
        m3.Ability1 = "Taunt";
        m3.Ability2 = "Bash";
        m3.Ability3 = "Guard";
        monsterWrapper.MonsterData.MonsterList.Add(m3);
    }

    public void SaveData()
    {
        string contents = JsonUtility.ToJson(monsterWrapper, true);
        System.IO.File.WriteAllText(path, contents);
    }

    public void ReadData()
    {
        try
        {
            if (System.IO.File.Exists(path))
            {
                string contents = System.IO.File.ReadAllText(path);
                monsterWrapper = JsonUtility.FromJson<MonsterDataWrapper>(contents);
                Debug.Log(monsterWrapper.MonsterData.Date);
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
public class MonsterDataWrapper
{
    public MonstersData MonsterData = new MonstersData();  
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
