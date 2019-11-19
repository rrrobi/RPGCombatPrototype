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

    public /*abstract*/ class Ability
    {
        protected string abilityName;
        public string GetAbilityName { get { return abilityName; } }

        protected float abilityCD;
        public float GetAbilityCD { get { return abilityCD; } }

        protected TargetType targetType;
        public TargetType GetTargetType() { return targetType; }
        public void SetTargetType(TargetType type) { targetType = type; }


        // unfinished
        ////////////////////////////////////////////////////////////////////////////////
        protected List<AbilityEffect> effectList = new List<AbilityEffect>();
        public void AddToEffectList(AbilityEffect effect)
        {
            effectList.Add(effect);
        }
        public List<AbilityEffect> GetEffectList() { return effectList; }
        //////////////////////////////////////////////////////////////////////////////////

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
            // for each target, apply each effect

            // Ensure each effect of the ability is carried out
            foreach (var effect in effectList)
            {
                int strengthModifier = effect.BaseAbilityStrength; // TODO... needs to also take into account the strength modifier of the charcater using the ability.
                switch(effect.abilityType)
                {
                    case AbilityType.Attack:
                        DirectDamageAbilityEffect ddEffect = new DirectDamageAbilityEffect();
                        ddEffect.EffectAction(strengthModifier, target);
                        break;
                    case AbilityType.Summon:
                        SummonAbilityEffect summonEffect = new SummonAbilityEffect(effect.monsterInfo);
                        summonEffect.EffectAction(strengthModifier, target);
                        break;
                    case AbilityType.DirectHeal:
                        DirectHealAbilityEffect dhEffect = new DirectHealAbilityEffect();
                        dhEffect.EffectAction(strengthModifier, target);
                        break;
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