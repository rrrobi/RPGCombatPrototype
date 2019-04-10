using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global
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
            heroWrapper.HeroData.Date = System.DateTime.Now.ToShortDateString();
            heroWrapper.HeroData.Time = System.DateTime.Now.ToShortTimeString();

            heroWrapper.HeroData.HeroInfo.PlayerName = "Rrrobi";
            heroWrapper.HeroData.HeroInfo.FriendlySpriteName = "SimpleHeroBackBlue";
            heroWrapper.HeroData.HeroInfo.MaxHP = 100;
            heroWrapper.HeroData.HeroInfo.CurrentHP = 90;
            heroWrapper.HeroData.HeroInfo.CombatLevel = "10";
            heroWrapper.HeroData.HeroInfo.StrengthModifier = "10";
            heroWrapper.HeroData.HeroInfo.WillModifier = "10";
            heroWrapper.HeroData.HeroInfo.Ability1 = "Slash";
            heroWrapper.HeroData.HeroInfo.Ability2 = "Stab";
            heroWrapper.HeroData.HeroInfo.Ability3 = "Summon";
            heroWrapper.HeroData.HeroInfo.Ability4 = "Summon Demon Swarm";

            heroWrapper.HeroData.HeroInfo.Menu1 = "Summon";

            #region player demon List setup
            MonsterInfo m1 = new MonsterInfo();
            m1.Index = 3;
            m1.MonsterName = "Heavy Demon";
            m1.FriendlySpriteName = "SimpleTankMonsterBackBlue";
            m1.EnemySpriteName = "SimpleTankMonsterRed";
            m1.MaxHP = 75;
            m1.CurrentHP = 75;
            m1.Ability1 = "Bash";
            m1.Ability2 = "Taunt";
            m1.Ability3 = "Guard";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m1);

            MonsterInfo m2 = new MonsterInfo();
            m2.Index = 3;
            m2.MonsterName = "Heavy Demon";
            m2.FriendlySpriteName = "SimpleTankMonsterBackBlue";
            m2.EnemySpriteName = "SimpleTankMonsterRed";
            m2.MaxHP = 75;
            m2.CurrentHP = 75;
            m2.Ability1 = "Bash";
            m2.Ability2 = "Taunt";
            m2.Ability3 = "Guard";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m2);

            MonsterInfo m3 = new MonsterInfo();
            m3.Index = 1;
            m3.MonsterName = "Demon";
            m3.FriendlySpriteName = "SimpleMonsterBackBlue";
            m3.EnemySpriteName = "SimpleMonsterRed";
            m3.MaxHP = 50;
            m3.CurrentHP = 50;
            m3.Ability1 = "Slash";
            m3.Ability2 = "Stab";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m3);

            MonsterInfo m4 = new MonsterInfo();
            m4.Index = 1;
            m4.MonsterName = "Demon";
            m4.FriendlySpriteName = "SimpleMonsterBackBlue";
            m4.EnemySpriteName = "SimpleMonsterRed";
            m4.MaxHP = 50;
            m4.CurrentHP = 50;
            m4.Ability1 = "Slash";
            m4.Ability2 = "Stab";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m4);

            MonsterInfo m5 = new MonsterInfo();
            m5.Index = 2;
            m5.MonsterName = "Demon Swarm";
            m5.FriendlySpriteName = "SimpleMonsterSwarmBackBlue";
            m5.EnemySpriteName = "SimpleMonsterSwarmRed";
            m5.MaxHP = 15;
            m5.CurrentHP = 15;
            m5.Ability1 = "Tackle";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m5);

            MonsterInfo m6 = new MonsterInfo();
            m6.Index = 2;
            m6.MonsterName = "Demon Swarm";
            m6.FriendlySpriteName = "SimpleMonsterSwarmBackBlue";
            m6.EnemySpriteName = "SimpleMonsterSwarmRed";
            m6.MaxHP = 15;
            m6.CurrentHP = 15;
            m6.Ability1 = "Tackle";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m6);
            #endregion
            heroWrapper.HeroData.HeroInfo.ActiveDemons.Add(0);
            heroWrapper.HeroData.HeroInfo.ActiveDemons.Add(1);

            SaveData();
        }

        public void Setup()
        {
            path = Application.persistentDataPath + "/" + filename;
            Debug.Log("HeroData Path: " + path);

            JSONSetUp();
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

        // thinking out load so to speak.... not used
        //////
        public void AmendCurrentHP(int newHP)
        {
            heroWrapper.HeroData.HeroInfo.CurrentHP = newHP;
        }

        public void AmendData(string varName, int input)
        {
            this.GetType().GetField(varName).SetValue(this, input);
        }
        //////
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
        public int CurrentHP = 0;

        public string CombatLevel = "";
        public string StrengthModifier = "";
        public string WillModifier = "";

        public string Ability1 = "";
        public string Ability2 = "";
        public string Ability3 = "";
        public string Ability4 = "";

        public string Menu1 = "";

        public List<MonsterInfo> PlayerDemons = new List<MonsterInfo>();
        // TODO... not sure if 'ActiveDemons' list is the right way to go about this
        public List<int> ActiveDemons = new List<int>();
    }
}