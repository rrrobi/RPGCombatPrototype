using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroDataReader
    {
        public HeroDataWrapper heroWrapper = new HeroDataWrapper();

        string filename = "HeroData.json";
        string path;

        // This only needs to be called if im am changing machines during devoplment
        // To ensure the Hero Json Config file is correct
        private void JSONSetUp()
        {
            // Temp - this will be removed
            heroWrapper.HeroData.Date = System.DateTime.Now.ToShortDateString();
            heroWrapper.HeroData.Time = System.DateTime.Now.ToShortTimeString();

            heroWrapper.HeroData.HeroInfo.PlayerName = "Rrrobi";
            heroWrapper.HeroData.HeroInfo.FriendlySpriteName = "SimpleHeroBackBlue";
            heroWrapper.HeroData.HeroInfo.MaxHP = 100;
            heroWrapper.HeroData.HeroInfo.CombatLevel = "10";
            heroWrapper.HeroData.HeroInfo.StrengthModifier = "10";
            heroWrapper.HeroData.HeroInfo.WillModifier = "10";
            heroWrapper.HeroData.HeroInfo.Ability1 = "Slash";
            heroWrapper.HeroData.HeroInfo.Ability2 = "Stab";
            heroWrapper.HeroData.HeroInfo.Ability3 = "Summon Demon";
            heroWrapper.HeroData.HeroInfo.Ability4 = "Summon Demon Swarm";

            SaveData();
        }

        public void Setup()
        {
            path = Application.persistentDataPath + "/" + filename;
            Debug.Log("HeroData Path: " + path);

            //JSONSetUp();
        }

        public void SaveData()
        {
            string contents = JsonUtility.ToJson(heroWrapper, true);
            System.IO.File.WriteAllText(path, contents);
        }

        public void ReadData()
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    string contents = System.IO.File.ReadAllText(path);
                    heroWrapper = JsonUtility.FromJson<HeroDataWrapper>(contents);
                    Debug.Log(heroWrapper.HeroData.Date);
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
    public class HeroDataWrapper
    {
        public HeroData HeroData = new HeroData();

    }

    [System.Serializable]
    public class HeroData
    {
        public string Date = "";
        public string Time = "";
        public HeroInfo HeroInfo = new HeroInfo();
    }

    [System.Serializable]
    public class HeroInfo
    {
        public string PlayerName = "";
        public string FriendlySpriteName = "";
        public int MaxHP = 0;

        public string CombatLevel = "";
        public string StrengthModifier = "";
        public string WillModifier = "";

        public string Ability1 = "";
        public string Ability2 = "";
        public string Ability3 = "";
        public string Ability4 = "";
    }
}