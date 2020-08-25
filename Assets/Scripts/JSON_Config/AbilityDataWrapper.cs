using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    None,
    Attack,
    DirectHeal,
    Support,
    Summon
}

public enum TargetType
{
    None,
    Enemy,
    Friendly,
    SummonSlot
}

// I dont like the name of this!!!!
public enum AbilityEffectType
{
    None,
    Direct,
    Cleave,
    Pierce,
    Nova
    //Global?
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
            {
                AbilityInfo ability1 = new AbilityInfo();
                ability1.Name = "Slash";
                ability1.targetType = TargetType.Enemy;
                ability1.abilityEffectType = AbilityEffectType.Cleave;

                ability1.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Attack,
                    damageType = AbilityDamageType.Physical,
                    BaseAbilityStrength = 10
                };
                ability1.abilityEffects.Add(effect1);

                ability1.AbilityCD = 10f;
                ability1.ManaCost = 0;
                abilityWrapper.AbilityData.AbilityList.Add(ability1);
            }

            {
                AbilityInfo ability2 = new AbilityInfo();
                ability2.Name = "Stab";
                ability2.targetType = TargetType.Enemy;
                ability2.abilityEffectType = AbilityEffectType.Pierce;

                ability2.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Attack,
                    damageType = AbilityDamageType.Physical,
                    BaseAbilityStrength = 15
                };
                ability2.abilityEffects.Add(effect1);

                ability2.AbilityCD = 12f;
                ability2.ManaCost = 0;
                abilityWrapper.AbilityData.AbilityList.Add(ability2);
            }

            {
                AbilityInfo ability3 = new AbilityInfo();
                ability3.Name = "Tackle";
                ability3.targetType = TargetType.Enemy;
                ability3.abilityEffectType = AbilityEffectType.Direct;

                ability3.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Attack,
                    damageType = AbilityDamageType.Physical,
                    BaseAbilityStrength = 10
                };
                ability3.abilityEffects.Add(effect1);

                ability3.AbilityCD = 5f;
                ability3.ManaCost = 0;
                abilityWrapper.AbilityData.AbilityList.Add(ability3);
            }

            {
                AbilityInfo ability4 = new AbilityInfo();
                ability4.Name = "Bash";
                ability4.targetType = TargetType.Enemy;
                ability4.abilityEffectType = AbilityEffectType.Direct;

                ability4.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Attack,
                    damageType = AbilityDamageType.Physical,
                    BaseAbilityStrength = 25
                };
                ability4.abilityEffects.Add(effect1);

                ability4.AbilityCD = 15f;
                ability4.ManaCost = 0;
                abilityWrapper.AbilityData.AbilityList.Add(ability4);
            }

            // TODO.. Add tech ability functionality
            {
                AbilityInfo ability5 = new AbilityInfo();
                ability5.Name = "Taunt";
                ability5.targetType = TargetType.Enemy;
                ability5.abilityEffectType = AbilityEffectType.Direct;

                ability5.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Attack,
                    damageType = AbilityDamageType.Physical,
                    BaseAbilityStrength = 0
                };
                ability5.abilityEffects.Add(effect1);

                ability5.AbilityCD = 10f;
                ability5.ManaCost = 0;
                abilityWrapper.AbilityData.AbilityList.Add(ability5);
            }

            // TODO.. Add tech ability functionality
            {
                AbilityInfo ability6 = new AbilityInfo();
                ability6.Name = "Guard";
                ability6.targetType = TargetType.Friendly;
                ability6.abilityEffectType = AbilityEffectType.Direct;

                ability6.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Attack,
                    damageType = AbilityDamageType.Physical,
                    BaseAbilityStrength = 0
                };
                ability6.abilityEffects.Add(effect1);

                ability6.AbilityCD = 10f;
                ability6.ManaCost = 0;
                abilityWrapper.AbilityData.AbilityList.Add(ability6);
            }

            {
                AbilityInfo ability9 = new AbilityInfo();
                ability9.Name = "Summon";
                ability9.targetType = TargetType.SummonSlot;
                ability9.abilityEffectType = AbilityEffectType.Direct;

                ability9.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Summon,
                    damageType = AbilityDamageType.None,
                    BaseAbilityStrength = 0
                };
                ability9.abilityEffects.Add(effect1);

                ability9.AbilityCD = 20f;
                ability9.ManaCost = 0;
                ability9.summonIndex = 0;                                   // <- index of 0, means it will use characters pool of demons to summon from
                abilityWrapper.AbilityData.AbilityList.Add(ability9);
            }

            {
                AbilityInfo ability10 = new AbilityInfo();
                ability10.Name = "Heal";
                ability10.targetType = TargetType.Friendly;
                ability10.abilityEffectType = AbilityEffectType.Direct;

                ability10.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.DirectHeal,
                    damageType = AbilityDamageType.Light,
                    BaseAbilityStrength = 25
                };
                ability10.abilityEffects.Add(effect1);

                ability10.AbilityCD = 10f;
                ability10.ManaCost = 10;
                abilityWrapper.AbilityData.AbilityList.Add(ability10);
            }

            {
                AbilityInfo ability11 = new AbilityInfo();
                ability11.Name = "Fire Wave";
                ability11.targetType = TargetType.Enemy;
                ability11.abilityEffectType = AbilityEffectType.Nova;

                ability11.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Attack,
                    damageType = AbilityDamageType.Fire,
                    BaseAbilityStrength = 25
                };
                ability11.abilityEffects.Add(effect1);

                ability11.AbilityCD = 10f;
                ability11.ManaCost = 25;
                abilityWrapper.AbilityData.AbilityList.Add(ability11);
            }

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
        public TargetType targetType = TargetType.None;
        public AbilityEffectType abilityEffectType = AbilityEffectType.None;
        // Each Ability will have one to Many effects, on use it will cycle through each effect for eacvh target
        public List<Effect> abilityEffects = new List<Effect>(); 

        // AbilityCD - time in battle needed to recover after using this ability
        public float AbilityCD = 0f;
        public int ManaCost = 0;
        // index for the character to be summoned
        public int summonIndex = 0;
    }

    // Still in progress... not final
    /// <summary>
    /// Effect - One to Many within an ability
    /// contains: 
    /// Effect type info - e.g. direct damage, DoT, Buff etc
    /// Damage type - for use with weaknesses/resistances (not used yet)
    /// Base strength of the effect - used with 'users' strength to determin how powerful the effect is
    /// </summary>
    [System.Serializable]
    public class Effect
    {
        public AbilityType abilityType = AbilityType.None;
        public AbilityDamageType damageType = AbilityDamageType.None;
        // strength of ability = damage delt, health healed of stat buffed/debuffed depending on ability type
        public int BaseAbilityStrength = 0;

    }
}