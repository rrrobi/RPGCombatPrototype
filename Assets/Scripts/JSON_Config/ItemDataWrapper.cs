using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public class ItemDataReader
    {
        public ItemDataWrapper itemWrapper = new ItemDataWrapper();

        string itemFilename = "itemData.json";
        string itemPath;

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

                item.AbilityCD = 5f;
                item.ManaCost = 0;
                item.Charges = 0;
                itemWrapper.ItemData.ItemList.Add(item);
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
                item.Charges = 0;
                itemWrapper.ItemData.ItemList.Add(item);
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
                item.Charges = 0;
                itemWrapper.ItemData.ItemList.Add(item);
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
                item.Charges = 0;
                itemWrapper.ItemData.ItemList.Add(item);
            }
             
            SaveData();
        }

        public void SetUp()
        {
            itemPath = Application.persistentDataPath + "/" + itemFilename;
            Debug.Log("ItemData Path: " + itemPath);

            JSONSetUp();
        }

        public void SaveData()
        {
            string contents = JsonUtility.ToJson(itemWrapper, true);
            System.IO.File.WriteAllText(itemPath, contents);
        }

        public void ReadData()
        {
            try
            {
                if (System.IO.File.Exists(itemPath))
                {
                    string contents = System.IO.File.ReadAllText(itemPath);
                    itemWrapper = JsonUtility.FromJson<ItemDataWrapper>(contents);
                }
                else
                {
                    Debug.Log("File: '" + itemPath + "' does not exist.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("File not as expected at " + itemPath);
            }
        }

        public AbilityInfo GetItemByName(string name)
        {
            List<AbilityInfo> itemInfoGroup = itemWrapper.ItemData.ItemList.FindAll(s => s.Name == name);

            if (itemInfoGroup.Count < 1)
            {
                // TODO... Need more robust error handling
                Debug.Log("No Abilities in Config using Name of: " + name);
                return null;
            }
            else if (itemInfoGroup.Count > 1)
            {
                // TODO... Need more robust error handling
                Debug.Log("Too Many Abilities in Config using Name of: " + name);
                return itemInfoGroup[0];
            }
            // Return first from list. After error handleing above, there should only be one ability in the list.
            return itemInfoGroup[0];
        }
    }

    [System.Serializable]
    public class ItemDataWrapper
    {
        public ItemData ItemData = new ItemData();
    }

    // Item Data is identical in format and layout to Ability Data
    // This is a separate class to keep the config json files separate
    [System.Serializable]
    public class ItemData
    {
        public List<AbilityInfo> ItemList = new List<AbilityInfo>();
    }

    
}