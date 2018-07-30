using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonData : MonoBehaviour {

    string filename = "data.json";
    string path;

    MonsterDataWrapper gameData = new MonsterDataWrapper();

	// Use this for initialization
	void Start () {
        path = Application.persistentDataPath + "/" + filename;
        Debug.Log(path);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.S))
        {
            gameData.MonsterData.Date = System.DateTime.Now.ToShortDateString();
            //gameData.time = System.DateTime.Now.ToShortTimeString();

            MonsterInfo m1 = new MonsterInfo();
            m1.index = 1;
            m1.MonsterName = "Demon";
            m1.FriendlySpriteName = "BlueDemon";
            m1.EnemySpriteName = "RedDemon";
            m1.MaxHP = 50;
            m1.Ability1 = "Slash";
            m1.Ability2 = "Stab";
            gameData.MonsterData.MonsterList.Add(m1);

            MonsterInfo m2 = new MonsterInfo();
            m2.index = 2;
            m2.MonsterName = "Demon Swarm";
            m2.FriendlySpriteName = "BlueDemonSwarm";
            m2.EnemySpriteName = "RedDemonSwarm";
            m2.MaxHP = 15;
            m2.Ability1 = "Tackle";
            gameData.MonsterData.MonsterList.Add(m2);

            MonsterInfo m3 = new MonsterInfo();
            m3.index = 3;
            m3.MonsterName = "Heavy Demon";
            m3.FriendlySpriteName = "HeavyBlueDemon";
            m3.EnemySpriteName = "HeavyRedDemon";
            m3.MaxHP = 75;
            m3.Ability1 = "Taunt";
            m3.Ability2 = "Bash";
            m3.Ability3 = "Guard";
            gameData.MonsterData.MonsterList.Add(m3);

           // SaveData();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
           // ReadData();
        }
	}

    void SaveData()
    {
        string contents = JsonUtility.ToJson(gameData, true);
        System.IO.File.WriteAllText(path, contents);
    }

    void ReadData()
    {
        try
        {
            if (System.IO.File.Exists(path))
            {
                string contents = System.IO.File.ReadAllText(path);
                gameData = JsonUtility.FromJson<MonsterDataWrapper>(contents);
                Debug.Log(gameData.MonsterData.Date);
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
