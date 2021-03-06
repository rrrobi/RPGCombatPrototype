﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Global
{
    static public class MonsterDataReader
    {
        static public MonsterDataWrapper monsterWrapper = new MonsterDataWrapper();

        static string filename = "monsterData.json";
        static string path;

        // This only needs to be called if im am changing machines during devoplment
        // To ensure the Monster Json Config file is correct
        static private void JSONSetUp()
        {
            monsterWrapper.MonsterData.Date = System.DateTime.Now.ToShortDateString();

            MonsterInfo m1 = new MonsterInfo();
            m1.Index = 1;
            m1.DifficultyLevel = 2;
            m1.MonsterName = "Demon";
            m1.FriendlySpriteName = "SimpleMonsterBackBlue";
            m1.EnemySpriteName = "SimpleMonsterRed";
            m1.AnnimationController = "DefaultMonster";
            m1.MaxHP = 50;
            m1.CurrentHP = 50;
            m1.MaxMana = 0;
            m1.CurrentMana = 0;
            m1.StrengthModifier = 10;
            m1.WillModifier = 10;
            m1.AgilityModifier = 10;
            m1.Ability1 = "Slash";
            m1.Ability2 = "Stab";
            monsterWrapper.MonsterData.MonsterList.Add(m1);

            MonsterInfo m2 = new MonsterInfo();
            m2.Index = 2;
            m2.DifficultyLevel = 1;
            m2.MonsterName = "Demon Swarm";
            m2.FriendlySpriteName = "SimpleMonsterSwarmBackBlue";
            m2.EnemySpriteName = "SimpleMonsterSwarmRed";
            m2.AnnimationController = "DefaultMonster";
            m2.MaxHP = 15;
            m2.CurrentHP = 15;
            m2.MaxMana = 0;
            m2.CurrentMana = 0;
            m2.StrengthModifier = 5;
            m2.WillModifier = 5;
            m2.AgilityModifier = 13;
            m2.Ability1 = "Tackle";
            monsterWrapper.MonsterData.MonsterList.Add(m2);

            MonsterInfo m3 = new MonsterInfo();
            m3.Index = 3;
            m3.DifficultyLevel = 3;
            m3.MonsterName = "Heavy Demon";
            m3.FriendlySpriteName = "SimpleTankMonsterBackBlue";
            m3.EnemySpriteName = "SimpleTankMonsterRed";
            m3.AnnimationController = "DefaultMonster";
            m3.MaxHP = 75;
            m3.CurrentHP = 75;
            m3.MaxMana = 0;
            m3.CurrentMana = 0;
            m3.StrengthModifier = 15;
            m3.WillModifier = 15;
            m3.AgilityModifier = 7;
            m3.Ability1 = "Bash";
            m3.Ability2 = "Taunt";
            m3.Ability3 = "Guard";
            monsterWrapper.MonsterData.MonsterList.Add(m3);

            SaveData();
        }

        static public void SetUp()
        {
            Debug.Log("MonsterData Set up started!");
            path = Application.persistentDataPath + "/" + filename;
            Debug.Log("MonsterData Path: " + path);

            JSONSetUp();
        }

        static public void SaveData()
        {
            string contents = JsonUtility.ToJson(monsterWrapper, true);
            System.IO.File.WriteAllText(path, contents);
        }

        static public void ReadData()
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

        /// <summary>
        /// Finds the monster from the config which uses the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>MonsterInfo</returns>
        static public MonsterInfo GetMonsterFromIndex(int index)
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

        /// <summary>
        /// Returns a list of MonsterInfo that is available for use with the given difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns>List<MonsterInfo></returns>
        static public List<MonsterInfo> GetAvailableMonstersForDifficulty(int difficulty)
        {
            List<MonsterInfo> availableMonsters = new List<MonsterInfo>();
            // return a list of all monsters with a dificulty of less/equal to the given difficulty limit
            // TODO... this will later be expanded to include other conditions. e.g. biome type etc
            availableMonsters = monsterWrapper.MonsterData.MonsterList.FindAll(s => s.DifficultyLevel <= difficulty);

            return availableMonsters;
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
        public string UniqueID = "00-000";

        public int Index = 0;
        public int DifficultyLevel = 0;
        public string MonsterName = "";
        public string FriendlySpriteName = "";
        public string EnemySpriteName = "";
        public string AnnimationController = "";
        public int MaxHP = 0;
        public int CurrentHP = 0;
        public int MaxMana = 0;
        public int CurrentMana = 0;

        public int StrengthModifier = 0;
        public int WillModifier = 0;
        public int AgilityModifier = 0;

        public string Ability1 = "";
        public string Ability2 = "";
        public string Ability3 = "";
        public string Ability4 = "";

        // Status Data
        public bool IsSummoned = false;
        public bool IsDead = false;
        // Maybe add other status effects here - Poison/sleep/curse etc
    }
}