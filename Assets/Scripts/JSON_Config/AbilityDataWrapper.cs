﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    None,
    Damage,
    Heal,
    Buff,
    Debuff
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

public class AbilityDataReader
{
    public AbilityDataWrapper abilityWrapper = new AbilityDataWrapper();

    string filename = "abilityData.json";
    string path;

    public void SetUp()
    {
        path = Application.persistentDataPath + "/" + filename;
        Debug.Log("AbilityData Path: " + path);

        AbilityInfo ability1 = new AbilityInfo();
        ability1.Name = "Slash";
        ability1.abilityType = AbilityType.Damage;
        ability1.abilityEffectType = AbilityEffectType.Direct;
        ability1.abilityDamageType = AbilityDamageType.Physical;
        ability1.BaseAbilityStrength = 10;
        ability1.AbilityCD = 10f;
        abilityWrapper.AbilityData.AbilityList.Add(ability1);

        AbilityInfo ability2 = new AbilityInfo();
        ability2.Name = "Stab";
        ability2.abilityType = AbilityType.Damage;
        ability2.abilityEffectType = AbilityEffectType.Direct;
        ability2.abilityDamageType = AbilityDamageType.Physical;
        ability2.BaseAbilityStrength = 15;
        ability2.AbilityCD = 10f;
        abilityWrapper.AbilityData.AbilityList.Add(ability2);

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

}
