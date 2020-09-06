using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public class ConsumableDataReader
    {
        public ConsumableDataWrapper consumableWrapper = new ConsumableDataWrapper();

        string consumableFilename = "itemData.json";
        string consumablePath;

        // This only needs to be called if im am changing machines during devoplment
        // To ensure the Item Json Config file is correct
        private void JSONSetUp()
        {
            {
                AbilityInfo item = new AbilityInfo();
                item.Name = "Health Potion";
                item.targetType = TargetType.Friendly;
                item.abilityEffectType = AbilityEffectType.Direct;

                item.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.DirectHeal,
                    damageType = AbilityDamageType.Light,
                    BaseAbilityStrength = 25
                };
                item.abilityEffects.Add(effect1);

                item.AbilityCD = 1f;
                item.ManaCost = 0;
                consumableWrapper.ConsumableData.ConsumableList.Add(item);
            }

            // TODO... still need to add mana regen functionallity
            {
                AbilityInfo item = new AbilityInfo();
                item.Name = "Mana Potion";
                item.targetType = TargetType.Friendly;
                item.abilityEffectType = AbilityEffectType.Direct;

                item.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.Support,
                    damageType = AbilityDamageType.Light,
                    BaseAbilityStrength = 25
                };
                item.abilityEffects.Add(effect1);

                item.AbilityCD = 5f;
                item.ManaCost = 0;
                consumableWrapper.ConsumableData.ConsumableList.Add(item);
            }

            {
                AbilityInfo item = new AbilityInfo();
                item.Name = "Throwing Axe";
                item.targetType = TargetType.Enemy;
                item.abilityEffectType = AbilityEffectType.Direct;

                item.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.DirectDamage,
                    damageType = AbilityDamageType.Physical,
                    BaseAbilityStrength = 10
                };
                item.abilityEffects.Add(effect1);

                item.AbilityCD = 5f;
                item.ManaCost = 0;
                consumableWrapper.ConsumableData.ConsumableList.Add(item);
            }

            {
                AbilityInfo item = new AbilityInfo();
                item.Name = "Dynamite";
                item.targetType = TargetType.Enemy;
                item.abilityEffectType = AbilityEffectType.Nova;

                item.abilityEffects = new List<Effect>();
                Effect effect1 = new Effect()
                {
                    abilityType = AbilityType.DirectDamage,
                    damageType = AbilityDamageType.Fire,
                    BaseAbilityStrength = 20
                };
                item.abilityEffects.Add(effect1);

                item.AbilityCD = 15f;
                item.ManaCost = 0;
                consumableWrapper.ConsumableData.ConsumableList.Add(item);
            }
             
            SaveData();
        }

        public void SetUp()
        {
            consumablePath = Application.persistentDataPath + "/" + consumableFilename;
            Debug.Log("ItemData Path: " + consumablePath);

            JSONSetUp();
        }

        public void SaveData()
        {
            string contents = JsonUtility.ToJson(consumableWrapper, true);
            System.IO.File.WriteAllText(consumablePath, contents);
        }

        public void ReadData()
        {
            try
            {
                if (System.IO.File.Exists(consumablePath))
                {
                    string contents = System.IO.File.ReadAllText(consumablePath);
                    consumableWrapper = JsonUtility.FromJson<ConsumableDataWrapper>(contents);
                }
                else
                {
                    Debug.Log("File: '" + consumablePath + "' does not exist.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("File not as expected at " + consumablePath);
            }
        }

        public AbilityInfo GetConsumableByName(string name)
        {
            List<AbilityInfo> consumableInfoGroup = consumableWrapper.ConsumableData.ConsumableList.FindAll(s => s.Name == name);

            if (consumableInfoGroup.Count < 1)
            {
                // TODO... Need more robust error handling
                Debug.Log("No Abilities in Config using Name of: " + name);
                return null;
            }
            else if (consumableInfoGroup.Count > 1)
            {
                // TODO... Need more robust error handling
                Debug.Log("Too Many Abilities in Config using Name of: " + name);
                return consumableInfoGroup[0];
            }
            // Return first from list. After error handleing above, there should only be one ability in the list.
            return consumableInfoGroup[0];
        }
    }

    [System.Serializable]
    public class ConsumableDataWrapper
    {
        public ConsumableData ConsumableData = new ConsumableData();
    }

    // Item Data is identical in format and layout to Ability Data
    // This is a separate class to keep the config json files separate
    [System.Serializable]
    public class ConsumableData
    {
        public List<AbilityInfo> ConsumableList = new List<AbilityInfo>();
    }

    
}