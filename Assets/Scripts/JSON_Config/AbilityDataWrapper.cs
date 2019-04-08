using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    None,
    Attack,
    Support,
    Summon
}

public enum AbilityEffectType
{
    None,
    Direct,
    Cleave,
    Pierce,
    Cone
}

// This may be moved into the Config file, to improve modibility
public enum AbilityDamageType
{
    None,
    Physical,
    Fire,
    Ice,
    Lightning,
    Light,
    Shadow
}

namespace Global
{
    public class AbilityDataReader
    {
        public AbilityDataWrapper abilityWrapper = new AbilityDataWrapper();

        string filename = "abilityData.json";
        string path;

        // This only needs to be called if im am changing machines during devoplment
        // To ensure the Ability Json Config file is correct
        private void JSONSetUp()
        {
            AbilityInfo ability1 = new AbilityInfo();
            ability1.Name = "Slash";
            ability1.abilityType = AbilityType.Attack;
            ability1.abilityEffectType = AbilityEffectType.Direct;
            ability1.abilityDamageType = AbilityDamageType.Physical;
            ability1.BaseAbilityStrength = 10;
            ability1.AbilityCD = 10f;
            abilityWrapper.AbilityData.AbilityList.Add(ability1);

            AbilityInfo ability2 = new AbilityInfo();
            ability2.Name = "Stab";
            ability2.abilityType = AbilityType.Attack;
            ability2.abilityEffectType = AbilityEffectType.Direct;
            ability2.abilityDamageType = AbilityDamageType.Physical;
            ability2.BaseAbilityStrength = 15;
            ability2.AbilityCD = 12f;
            abilityWrapper.AbilityData.AbilityList.Add(ability2);

            AbilityInfo ability3 = new AbilityInfo();
            ability3.Name = "Tackle";
            ability3.abilityType = AbilityType.Attack;
            ability3.abilityEffectType = AbilityEffectType.Direct;
            ability3.abilityDamageType = AbilityDamageType.Physical;
            ability3.BaseAbilityStrength = 10;
            ability3.AbilityCD = 5f;
            abilityWrapper.AbilityData.AbilityList.Add(ability3);

            AbilityInfo ability4 = new AbilityInfo();
            ability4.Name = "Bash";
            ability4.abilityType = AbilityType.Attack;
            ability4.abilityEffectType = AbilityEffectType.Direct;
            ability4.abilityDamageType = AbilityDamageType.Physical;
            ability4.BaseAbilityStrength = 25;
            ability4.AbilityCD = 15f;
            abilityWrapper.AbilityData.AbilityList.Add(ability4);

            // TODO.. Add tech ability functionality
            AbilityInfo ability5 = new AbilityInfo();
            ability5.Name = "Taunt";
            ability5.abilityType = AbilityType.Attack;
            ability5.abilityEffectType = AbilityEffectType.Direct;
            ability5.abilityDamageType = AbilityDamageType.Physical;
            ability5.BaseAbilityStrength = 0;
            ability5.AbilityCD = 10f;
            abilityWrapper.AbilityData.AbilityList.Add(ability5);

            // TODO.. Add tech ability functionality
            AbilityInfo ability6 = new AbilityInfo();
            ability6.Name = "Guard";
            ability6.abilityType = AbilityType.Attack;
            ability6.abilityEffectType = AbilityEffectType.Direct;
            ability6.abilityDamageType = AbilityDamageType.Physical;
            ability6.BaseAbilityStrength = 0;
            ability6.AbilityCD = 10f;
            abilityWrapper.AbilityData.AbilityList.Add(ability6);

            AbilityInfo ability7 = new AbilityInfo();
            ability7.Name = "Summon Demon";
            ability7.abilityType = AbilityType.Summon;
            ability7.abilityEffectType = AbilityEffectType.Direct;
            ability7.abilityDamageType = AbilityDamageType.None;
            ability7.BaseAbilityStrength = 0;
            ability7.AbilityCD = 20f;
            ability7.summonIndex = 1;
            abilityWrapper.AbilityData.AbilityList.Add(ability7);

            AbilityInfo ability8 = new AbilityInfo();
            ability8.Name = "Summon Demon Swarm";
            ability8.abilityType = AbilityType.Summon;
            ability8.abilityEffectType = AbilityEffectType.Direct;
            ability8.abilityDamageType = AbilityDamageType.None;
            ability8.BaseAbilityStrength = 0;
            ability8.AbilityCD = 20f;
            ability8.summonIndex = 2;
            abilityWrapper.AbilityData.AbilityList.Add(ability8);

            //AbilityInfo ability9 = new AbilityInfo();
            //ability9.Name = "Summon";
            //ability9.abilityType = AbilityType.Menu;
            //ability9.AbilityCD = 0f;
            //ability9.menuList = new List<AbilityInfo>();

            //AbilityInfo summon1 = new AbilityInfo()
            //{
            //    Name = "Summon Demon",
            //    abilityType = AbilityType.Summon,
            //    abilityEffectType = AbilityEffectType.Direct,
            //    abilityDamageType = AbilityDamageType.None,
            //    BaseAbilityStrength = 0,
            //    AbilityCD = 20f,
            //    summonIndex = 1,
            //};
            //ability9.menuList.Add(summon1);
            //AbilityInfo summon2 = new AbilityInfo()
            //{
            //    Name = "Summon Demon Swarm",
            //    abilityType = AbilityType.Summon,
            //    abilityEffectType = AbilityEffectType.Direct,
            //    abilityDamageType = AbilityDamageType.None,
            //    BaseAbilityStrength = 0,
            //    AbilityCD = 20f,
            //    summonIndex = 2,
            //};
            //ability9.menuList.Add(summon2);
            //abilityWrapper.AbilityData.AbilityList.Add(ability9);

            // TODO.. Add tech ability functionality
#warning Supports DO NOT work yet
            AbilityInfo ability10 = new AbilityInfo();
            ability10.Name = "Heal";
            ability10.abilityType = AbilityType.Support;
            ability10.abilityEffectType = AbilityEffectType.Direct;
            ability10.abilityDamageType = AbilityDamageType.Physical;
            ability10.BaseAbilityStrength = -25;
            ability10.AbilityCD = 10f;
            abilityWrapper.AbilityData.AbilityList.Add(ability10);

            SaveData();
        }

        public void SetUp()
        {
            path = Application.persistentDataPath + "/" + filename;
            Debug.Log("AbilityData Path: " + path);

            JSONSetUp();
        }

        public void SaveData()
        {
            string contents = JsonUtility.ToJson(abilityWrapper, true);
            System.IO.File.WriteAllText(path, contents);
        }

        public void ReadData()
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    string contents = System.IO.File.ReadAllText(path);
                    abilityWrapper = JsonUtility.FromJson<AbilityDataWrapper>(contents);
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

        public AbilityInfo GetAbilityByName(string name)
        {
            List<AbilityInfo> abilityInfoGroup = abilityWrapper.AbilityData.AbilityList.FindAll(s => s.Name == name);

            if (abilityInfoGroup.Count < 1)
            {
                // TODO... Need more robust error handling
                Debug.Log("No Abilities in Config using Name of: " + name);
                return null;
            }
            else if (abilityInfoGroup.Count > 1)
            {
                // TODO... Need more robust error handling
                Debug.Log("Too Many Abilities in Config using Name of: " + name);
                return abilityInfoGroup[0];
            }
            // Return first from list. After error handleing above, there should only be one ability in the list.
            return abilityInfoGroup[0];
        }
    }

    [System.Serializable]
    public class AbilityDataWrapper
    {
        public AbilitiesData AbilityData = new AbilitiesData();
    }

    [System.Serializable]
    public class AbilitiesData
    {
        public List<AbilityInfo> AbilityList = new List<AbilityInfo>();
    }

    [System.Serializable]
    public class AbilityInfo
    {
        public string Name = "";
        public AbilityType abilityType = AbilityType.None;
        public AbilityEffectType abilityEffectType = AbilityEffectType.None;
        public AbilityDamageType abilityDamageType = AbilityDamageType.None;
        // strength of ability = damage delt, health healed of stat buffed/debuffed depending on ability type
        public int BaseAbilityStrength = 0;
        // AbilityCD - time in battle for this ability to take effect
        public float AbilityCD = 0f;
        // index for the character to be summoned
        public int summonIndex = 0;

        // List of Menu Abilities
        public List<AbilityInfo> menuList = new List<AbilityInfo>();
    }
}