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

        public Ability(string name, float cd)
        {
            abilityName = name;
            abilityCD = cd;
        }

//        public abstract void Action(GameObject source, GameObject target);

        public void NewAction(GameObject source, GameObject target)
        {
            Debug.Log($"{source.name} Has used {abilityName} on {target.name}");

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
                    // To find the AoE targets I must:
                    // 1) find the 'Y' of the slot containg the main target
                    // 2) Find all other slots in line with the target's 'Y' - using the parent of the main target to find search only other 'siblings'
                    // 3) Find target charcater occupying each slot we have selected
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
                    /// should now have all 3 targets in the cleaved row
                    break;
                case AbilityEffectType.Pierce:
                    break;
                case AbilityEffectType.Nova:
                    break;
                case AbilityEffectType.None:
                default:
                    Debug.LogError("AbilityEffectType is not working!!!");
                    break;
            }

            // for each target, apply each effect

            foreach (var t in targetList)
            {
                // Ensure each effect of the ability is carried out
                foreach (var effect in effectList)
                {
                    int strengthModifier = effect.BaseAbilityStrength; // TODO... needs to also take into account the strength modifier of the charcater using the ability.
                    switch (effect.abilityType)
                    {
                        case AbilityType.Attack:
                            DirectDamageAbilityEffect ddEffect = new DirectDamageAbilityEffect();
                            ddEffect.EffectAction(strengthModifier, t);
                            break;
                        case AbilityType.Summon:
                            SummonAbilityEffect summonEffect = new SummonAbilityEffect(effect.monsterInfo);
                            summonEffect.EffectAction(strengthModifier, t);
                            break;
                        case AbilityType.DirectHeal:
                            DirectHealAbilityEffect dhEffect = new DirectHealAbilityEffect();
                            dhEffect.EffectAction(strengthModifier, t);
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
            // Restart the source's cooldown
            source.GetComponent<Character>().UpdateAttackTimer(cd);
            source.GetComponent<Character>().SetIsReady(false);

            // Reset Sprite Shader to Dprite Default (not Glowing)
            //source.GetComponent<Character>().MakeUnclickable();
        }
    }
}