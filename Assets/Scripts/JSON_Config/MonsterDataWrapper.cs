using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterDataReader
{
    public MonsterDataWrapper monsterWrapper = new MonsterDataWrapper();

    string filename = "monsterData.json";
    string path;

    public void SetUp()
    {
        path = Application.persistentDataPath + "/" + filename;
        Debug.Log("MonsterData Path: " + path);

        // Temp - this will be removed
        monsterWrapper.MonsterData.Date = System.DateTime.Now.ToShortDateString();

        MonsterInfo m1 = new MonsterInfo();
        m1.Index = 1;
        m1.MonsterName = "Demon";
        m1.FriendlySpriteName = "SimpleMonsterBackBlue";
        m1.EnemySpriteName = "SimpleMonsterRed";
        m1.MaxHP = 50;
        m1.Ability1 = "Slash";
        m1.Ability2 = "Stab";
        monsterWrapper.MonsterData.MonsterList.Add(m1);

        MonsterInfo m2 = new MonsterInfo();
        m2.Index = 2;
        m2.MonsterName = "Demon Swarm";
        m2.FriendlySpriteName = "SimpleMonsterSwarmBackBlue";
        m2.EnemySpriteName = "SimpleMonsterSwarmRed";
        m2.MaxHP = 15;
        m2.Ability1 = "Tackle";
        monsterWrapper.MonsterData.MonsterList.Add(m2);

        MonsterInfo m3 = new MonsterInfo();
        m3.Index = 3;
        m3.MonsterName = "Heavy Demon";
        m3.FriendlySpriteName = "SimpleTankMonsterBackBlue";
        m3.EnemySpriteName = "SimpleTankMonsterRed";
        m3.MaxHP = 75;
        m3.Ability1 = "Bash";
        m3.Ability2 = "Taunt";
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

    public MonsterInfo GetMonsterFromIndex(int index)
    {
        List<MonsterInfo> monsterInfoGroup = monsterWrapper.MonsterData.MonsterList.FindAll(s => s.Index == index);

        if (monsterInfoGroup.Count < 1)
        {
            // TODO... Need more robust error handling
            Debug.Log("No monsters in Config using Index of: " + index);
            return null;
        }
        else if (monsterInfoGroup.Count > 1)
        {
            // TODO... Need more robust error handling
            Debug.Log("Too Many monsters in Config using Index of: " + index);
            return monsterInfoGroup[0];
        }
        // Return first from list. After error handleing above, there should only be one monster in the list.
        return monsterInfoGroup[0];
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
    public int Index = 0;
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
