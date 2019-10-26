using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class Ability
    {
        protected string abilityName;
        public string GetAbilityName { get { return abilityName; } }

        protected float abilityCD;
        public float GetAbilityCD { get { return abilityCD; } }

        protected AbilityType abilityType;
        public AbilityType GetAbilityType() { return abilityType; }
        public void SetAbilityType(AbilityType type) { abilityType = type; }

        // unfinished
        //protected List<AbilityEffect> effectList;
        //public void AddToEffectList(AbilityEffect effect)
        //{
        //    effectList.Add(effect);
        //}
        //public List<AbilityEffect> GetEffectList() { return effectList; }


        public Ability(string name, float cd)
        {
            abilityName = name;
            abilityCD = cd;
        }

        public abstract void Action(GameObject source, GameObject target);

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