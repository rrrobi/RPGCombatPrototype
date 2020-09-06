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

            heroWrapper.HeroData.HeroInfo.UniqueID = "01-001";

            heroWrapper.HeroData.HeroInfo.PlayerName = "Rrrobi";
            heroWrapper.HeroData.HeroInfo.FriendlySpriteName = "SimpleHeroBackBlue";
            heroWrapper.HeroData.HeroInfo.MaxHP = 100;
            heroWrapper.HeroData.HeroInfo.CurrentHP = 100;
            heroWrapper.HeroData.HeroInfo.MaxMana = 100;
            heroWrapper.HeroData.HeroInfo.CurrentMana = 100;
            heroWrapper.HeroData.HeroInfo.CombatLevel = "10";
            heroWrapper.HeroData.HeroInfo.StrengthModifier = 10;
            heroWrapper.HeroData.HeroInfo.WillModifier = 10;
            heroWrapper.HeroData.HeroInfo.AgilityModifier = 10;
            heroWrapper.HeroData.HeroInfo.GoldOwned = 0;

            heroWrapper.HeroData.HeroInfo.baseActions.Name = "None";
            heroWrapper.HeroData.HeroInfo.baseActions.Abilities.Add("Slash");
            heroWrapper.HeroData.HeroInfo.baseActions.MenuType = "None";

            heroWrapper.HeroData.HeroInfo.SummonActions.Name = "Summon";
            heroWrapper.HeroData.HeroInfo.SummonActions.Abilities.Add("Summon");
            heroWrapper.HeroData.HeroInfo.SummonActions.MenuType = "Summon";

            heroWrapper.HeroData.HeroInfo.SpellActions.Name = "Spells";
            heroWrapper.HeroData.HeroInfo.SpellActions.Abilities.Add("Heal");
            heroWrapper.HeroData.HeroInfo.SpellActions.Abilities.Add("Fire Wave");
            heroWrapper.HeroData.HeroInfo.SpellActions.MenuType = "Spells";

            heroWrapper.HeroData.HeroInfo.ConsumableActions.Name = "Items";
            heroWrapper.HeroData.HeroInfo.ConsumableActions.Consumables.Add(new Consumable{ Name = "Health Potion", Charges = 3 });
            heroWrapper.HeroData.HeroInfo.ConsumableActions.Consumables.Add(new Consumable { Name = "Mana Potion", Charges = 4 });
            heroWrapper.HeroData.HeroInfo.ConsumableActions.Consumables.Add(new Consumable { Name = "Throwing Axe", Charges = 2 });
            heroWrapper.HeroData.HeroInfo.ConsumableActions.Consumables.Add(new Consumable { Name = "Dynamite", Charges = 1 });
            heroWrapper.HeroData.HeroInfo.ConsumableActions.MenuType = "Items";

            #region player demon List setup
            MonsterInfo m1 = new MonsterInfo();
            m1.UniqueID = "01-002";
            m1.Index = 3;
            m1.MonsterName = "Heavy Demon Gary";
            m1.FriendlySpriteName = "SimpleTankMonsterBackBlue";
            m1.EnemySpriteName = "SimpleTankMonsterRed";
            m1.MaxHP = 75;
            m1.CurrentHP = 75;
            m1.MaxMana = 0;
            m1.CurrentMana = 0;
            m1.StrengthModifier = 15;
            m1.WillModifier = 15;
            m1.AgilityModifier = 7;
            m1.Ability1 = "Bash";
            m1.Ability2 = "Taunt";
            m1.Ability3 = "Guard";
            m1.IsSummoned = true;
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m1);

            MonsterInfo m2 = new MonsterInfo();
            m2.UniqueID = "01-003";
            m2.Index = 3;
            m2.MonsterName = "Heavy Demon Steve";
            m2.FriendlySpriteName = "SimpleTankMonsterBackBlue";
            m2.EnemySpriteName = "SimpleTankMonsterRed";
            m2.MaxHP = 75;
            m2.CurrentHP = 75;
            m2.MaxMana = 0;
            m2.CurrentMana = 0;
            m2.StrengthModifier = 15;
            m2.WillModifier = 15;
            m2.AgilityModifier = 7;
            m2.Ability1 = "Bash";
            m2.Ability2 = "Taunt";
            m2.Ability3 = "Guard";
            m2.IsSummoned = true;
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m2);

            MonsterInfo m3 = new MonsterInfo();
            m3.UniqueID = "01-004";
            m3.Index = 1;
            m3.MonsterName = "Demon Pete";
            m3.FriendlySpriteName = "SimpleMonsterBackBlue";
            m3.EnemySpriteName = "SimpleMonsterRed";
            m3.MaxHP = 50;
            m3.CurrentHP = 50;
            m3.MaxMana = 0;
            m3.CurrentMana = 0;
            m3.StrengthModifier = 10;
            m3.WillModifier = 10;
            m3.AgilityModifier = 10;
            m3.Ability1 = "Slash";
            m3.Ability2 = "Stab";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m3);

            MonsterInfo m4 = new MonsterInfo();
            m4.UniqueID = "01-005";
            m4.Index = 1;
            m4.MonsterName = "Demon Sarah";
            m4.FriendlySpriteName = "SimpleMonsterBackBlue";
            m4.EnemySpriteName = "SimpleMonsterRed";
            m4.MaxHP = 50;
            m4.CurrentHP = 50;
            m4.MaxMana = 0;
            m4.CurrentMana = 0;
            m4.StrengthModifier = 10;
            m4.WillModifier = 10;
            m4.AgilityModifier = 10;
            m4.Ability1 = "Slash";
            m4.Ability2 = "Stab";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m4);

            MonsterInfo m5 = new MonsterInfo();
            m5.UniqueID = "01-006";
            m5.Index = 2;
            m5.MonsterName = "Demon Swarm Jones";
            m5.FriendlySpriteName = "SimpleMonsterSwarmBackBlue";
            m5.EnemySpriteName = "SimpleMonsterSwarmRed";
            m5.MaxHP = 15;
            m5.CurrentHP = 15;
            m5.MaxMana = 0;
            m5.CurrentMana = 0;
            m5.StrengthModifier = 5;
            m5.WillModifier = 5;
            m5.AgilityModifier = 13;
            m5.Ability1 = "Tackle";
            heroWrapper.HeroData.HeroInfo.PlayerDemons.Add(m5);

            MonsterInfo m6 = new MonsterInfo();
            m6.UniqueID = "01-007";
            m6.Index = 2;
            m6.MonsterName = "Demon Swarm Smith";
            m6.FriendlySpriteName = "SimpleMonsterSwarmBackBlue";
            m6.EnemySpriteName = "SimpleMonsterSwarmRed";
            m6.MaxHP = 15;
            m6.CurrentHP = 15;
            m6.MaxMana = 0;
            m6.CurrentMana = 0;
            m6.StrengthModifier = 5;
            m6.WillModifier = 5;
            m6.AgilityModifier = 13;
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
        public string UniqueID = "00-000";

        public string PlayerName = "";
        public string FriendlySpriteName = "";
        public int MaxHP = 0;
        public int CurrentHP = 0;
        public int MaxMana = 0;
        public int CurrentMana = 0;

        public string CombatLevel = "";
        public int StrengthModifier = 0;
        public int WillModifier = 0;
        public int AgilityModifier = 0;

        public int GoldOwned = 0;
        public int CurrentDungeonFloor = 0;
        public int CurrentXPosition = 0;
        public int CurrentYPosition = 0;

        // test - items/spells not used yet
        public AbilityGroup baseActions = new AbilityGroup();
        public AbilityGroup SummonActions = new AbilityGroup();
        public ConsumableGroup ConsumableActions = new ConsumableGroup();
        public AbilityGroup SpellActions = new AbilityGroup();

        public List<MonsterInfo> PlayerDemons = new List<MonsterInfo>();
        // TODO... not sure if 'ActiveDemons' list is the right way to go about this
        public List<int> ActiveDemons = new List<int>();
    }

    [System.Serializable]
    public class AbilityGroup
    {
        public string Name = "";
        public List<string> Abilities = new List<string>();
        public string MenuType = ""; // <- i dont think this is used for anything atm
    }

    [System.Serializable]
    public class ConsumableGroup
    {
        public string Name = "";
        public List<Consumable> Consumables = new List<Consumable>();
        public string MenuType = ""; // <- i dont think this is used for anything atm
    }

    [System.Serializable]
    public class Consumable
    {
        public string Name = "";
        public int Charges = 0;
    }
}