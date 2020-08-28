using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct AbilityEffect
    {
        public AbilityType abilityType;
        public AbilityDamageType damageType;
        public int BaseAbilityStrength;

        public Global.MonsterInfo monsterInfo;
    }

    public class Ability
    {
        protected string abilityName;
        public string GetAbilityName { get { return abilityName; } }

        protected float abilityCD;
        public float GetAbilityCD { get { return abilityCD; } }

        protected int abilityManaCost;
        public int GetAbilityManaCost { get { return abilityManaCost; } }

        protected TargetType targetType;
        public TargetType GetTargetType() { return targetType; }
        public void SetTargetType(TargetType type) { targetType = type; }

        protected AbilityEffectType abilityEffectType;
        public AbilityEffectType GetAbilityEffectType() { return abilityEffectType; }
        public void SetAbilityEffectType(AbilityEffectType aet) { abilityEffectType = aet; }

        protected List<AbilityEffect> effectList = new List<AbilityEffect>();
        public void AddToEffectList(AbilityEffect effect)
        {
            effectList.Add(effect);
        }
        public List<AbilityEffect> GetEffectList() { return effectList; }

        public Ability(string name, float cd, int manaCost)
        {
            abilityName = name;
            abilityCD = cd;
            abilityManaCost = manaCost;
        }

        // TODO... I don't think AOE will work for Summon Abilities right now.
        public List<GameObject> GetAbilityTargetList(GameObject target)
        {
            // Handle AOE, 
            // Get all targets based on 'AbilityEffectType'            
            List<GameObject> targetList = new List<GameObject>();

            // slot name 'UnitSlot_0-0'
            //                   (_X-Y)            
            switch (abilityEffectType)
            {
                case AbilityEffectType.Direct:
                    // Direct - Target list ONLY contains main target
                    targetList.Add(target);
                    break;
                case AbilityEffectType.Cleave:
                    {
                        // To find the AoE targets I must:
                        // 1) find the 'Y' of the slot containg the main target
                        // 2) Find all other slots in line with the target's 'Y' - using the parent of the main target to find search only other 'siblings'
                        // 3) Find target character occupying each slot we have selected
                        // 4) Add each target character to a list
                        GameObject targetUnitSlot = CombatManager.Instance.battlefieldController.FindSlotFromCharacter(target);
                        string y = targetUnitSlot.name.Substring(targetUnitSlot.name.Length - 1);
                        for (int x = 0; x < 3; x++)
                        {
                            GameObject slot = targetUnitSlot.transform.parent.Find($"UnitSlot_{x}-{y}").gameObject;
                            GameObject aoeTarget = slot.GetComponent<UnitSlot>().GetOccupyingCharacter();
                            if (aoeTarget != null)
                                targetList.Add(aoeTarget);
                        }
                        // should now have all 3 targets in the cleaved row
                    }
                    break;
                case AbilityEffectType.Pierce:
                    {
                        // To find the AoE targets I must:
                        // 1) find the 'X' of the slot containg the main target
                        // 2) Find all other slots in line with the target's 'X' - using the parent of the main target to find search only other 'siblings'
                        // 3) Find target character occupying each slot we have selected
                        // 4) Add each target character to a list
                        GameObject targetUnitSlot = CombatManager.Instance.battlefieldController.FindSlotFromCharacter(target);
                        string x = targetUnitSlot.name.Substring(targetUnitSlot.name.Length - 3, 1);
                        for (int y = 0; y < 2; y++)
                        {
                            GameObject slot = targetUnitSlot.transform.parent.Find($"UnitSlot_{x}-{y}").gameObject;
                            GameObject aoeTarget = slot.GetComponent<UnitSlot>().GetOccupyingCharacter();
                            if (aoeTarget != null)
                                targetList.Add(aoeTarget);
                        }
                        // should now have all 2 targets in the cleaved row
                    }
                    break;
                case AbilityEffectType.Nova:
                    {
                        // To find the AoE targets I must:
                        // 1) Find all slots in the target's team - using the parent of the main target to find search only other 'siblings'
                        // 3) Find target character occupying each slot we have selected
                        // 4) Add each target character to a list
                        GameObject targetUnitSlot = CombatManager.Instance.battlefieldController.FindSlotFromCharacter(target);
                        for (int y = 0; y < 2; y++)
                        {
                            for (int x = 0; x < 3; x++)
                            {
                                GameObject slot = targetUnitSlot.transform.parent.Find($"UnitSlot_{x}-{y}").gameObject;
                                GameObject aoeTarget = slot.GetComponent<UnitSlot>().GetOccupyingCharacter();
                                if (aoeTarget != null)
                                    targetList.Add(aoeTarget);
                            }
                        }
                        // should now have all 6 targets in the cleaved row
                    }
                    break;
                case AbilityEffectType.None:
                default:
                    Debug.LogError("AbilityEffectType is not working!!!");
                    break;
            }

            return targetList;
        }

        public void NewAction(GameObject source, GameObject target)
        {
            Debug.Log($"{source.name} Has used {abilityName} on {target.name}");
            // Trigger AbilityUsed Event Callback
            EventCallbacks.UseAbilityEventInfo uaei = new EventCallbacks.UseAbilityEventInfo();
            uaei.EventDescription = $"{source.name} Has used {abilityName} on {target.name}";
            uaei.UnitGO = source;
            uaei.FireEvent();

            // Handle AOE, 
            // Get all targets based on 'AbilityEffectType'            
            List<GameObject> targetList = GetAbilityTargetList(target);                        

            // for each target, apply each effect
            foreach (var t in targetList)
            {
                // Trigger AbilityHit Event Callback
                EventCallbacks.AbilityHitEventInfo ahei = new EventCallbacks.AbilityHitEventInfo();
                ahei.EventDescription = $"{abilityName} has hit {target.name}";
                ahei.UnitGO = target;
                ahei.ability = this;
                ahei.FireEvent();

                // Ensure each effect of the ability is carried out
                foreach (var effect in effectList)
                {
                    int modifiedAbilityPower = effect.BaseAbilityStrength;
                    switch(effect.damageType)
                    {
                        case AbilityDamageType.Physical:
                            modifiedAbilityPower = effect.BaseAbilityStrength + source.GetComponent<Character>().GetStrengthModifer;
                            break;
                        case AbilityDamageType.Light:
                        case AbilityDamageType.Shadow:
                        case AbilityDamageType.Fire:
                        case AbilityDamageType.Ice:
                        case AbilityDamageType.Lightning:
                            modifiedAbilityPower = effect.BaseAbilityStrength + source.GetComponent<Character>().GetWillModifer;
                            break;
                        default:
                            break;
                    }

                    //int strengthModified = effect.BaseAbilityStrength + source.GetComponent<Character>().GetStrengthModifer;
                    //int willModified = effect.BaseAbilityStrength + source.GetComponent<Character>().GetWillModifer;
                    switch (effect.abilityType)
                    {
                        case AbilityType.Attack:
                            DirectDamageAbilityEffect ddEffect = new DirectDamageAbilityEffect();
                            ddEffect.EffectAction(modifiedAbilityPower, t);
                            break;
                        case AbilityType.Summon:
                            SummonAbilityEffect summonEffect = new SummonAbilityEffect(effect.monsterInfo);
                            summonEffect.EffectAction(modifiedAbilityPower, t);
                            break;
                        case AbilityType.DirectHeal:
                            DirectHealAbilityEffect dhEffect = new DirectHealAbilityEffect();
                            dhEffect.EffectAction(modifiedAbilityPower, t);
                            break;
                    }
                }
            }
            

            // Resolve Afteraction Effects
            AfterAction(source);
        }

        protected void AfterAction(GameObject source)
        {
            // Calculate Cooldown from ability cooldown and source agi 
            float cd = abilityCD;
            // Subtract Mana cost of the ability from the source Character's Mana pool
            source.GetComponent<Character>().SetMP(source.GetComponent<Character>().GetMP - abilityManaCost);
            EventCallbacks.MPChangedEventInfo mpcei = new EventCallbacks.MPChangedEventInfo();
            mpcei.EventDescription = $"Unit {source.name} has had the mana cost({abilityManaCost}) of ability {abilityName} deducted from its mana pool.";
            mpcei.UnitGO = source;
            mpcei.FireEvent();

            // Restart the source's cooldown
            source.GetComponent<Character>().UpdateAttackTimer(cd);
            source.GetComponent<Character>().SetIsReady(false);

            // Trigger AbilityOver Event callback
            EventCallbacks.CharacterTurnOverEventInfo ctoei = new EventCallbacks.CharacterTurnOverEventInfo();
            ctoei.EventDescription = $"Unit {source.name} has finished its turn";
            ctoei.UnitGO = source;
            ctoei.FireEvent();

            // Reset Sprite Shader to Sprite Default (not Glowing)
            //source.GetComponent<Character>().MakeUnclickable();
        }
    }
}