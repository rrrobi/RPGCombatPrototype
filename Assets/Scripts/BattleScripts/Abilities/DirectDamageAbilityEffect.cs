using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class DirectDamageAbilityEffect : BaseAbilityEffect
    {
        // base damage attibute for the ability effect
        // 'Actual' effect of the ability will be calculate using both this value 
        // and the strengthModifier from the character using the ability
        int baseDamage = 1;

        public DirectDamageAbilityEffect()
            : base()
        {

        }

        // this is where are DO the direct heal
        public override void EffectAction(int strengthModifier, GameObject target)
        {           
            // Attack is a direct damage ability
            // Calculate damage from ability base damage and source str modifier
            int damage = baseDamage * strengthModifier;

            // Deal damage to target
            target.GetComponent<Character>().TakeDamage(damage);            
        }
    }
}
