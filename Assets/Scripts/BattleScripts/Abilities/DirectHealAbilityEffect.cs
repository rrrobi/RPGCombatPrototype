using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class DirectHealAbilityEffect : BaseAbilityEffect
    {
        // base strength attibute for the ability effect
        // 'Actual' effect of the ability will be calculate using both this value 
        // and the strengthModifier from the character using the ability
        int baseStrength = 1;

        public DirectHealAbilityEffect()
            : base()
        {

        }

        // this is where are DO the direct heal
        public override void EffectAction(int strengthModifier, GameObject target)
        {
            Debug.Log("Heal effect triggered!");

            // Attack is a direct damage ability
            // Calculate damage from ability base damage and source str modifier
            int hitPoints = baseStrength * strengthModifier;

            // Deal damage to target
            target.GetComponent<Character>().RecoverHitPoints(hitPoints);
        }
    }
}
