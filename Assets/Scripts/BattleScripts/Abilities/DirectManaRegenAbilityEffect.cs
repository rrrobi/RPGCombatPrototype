using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class DirectManaRegenAbilityEffect : BaseAbilityEffect
    {
        // base strength attibute for the ability effect
        // 'Actual' effect of the ability will be calculate using both this value 
        // and the strengthModifier from the character using the ability
        int baseStrength = 1;

        public DirectManaRegenAbilityEffect()
            : base()
        {

        }

        // this is where are DO the direct mana regen
        public override void EffectAction(int strengthModifier, GameObject target)
        {
            // Attack is a direct damage ability
            // Calculate damage from ability base damage and source str modifier
            int mana = baseStrength * strengthModifier;

            // Restore mana to the target
            target.GetComponent<Character>().RecoverMana(mana);
        }
    }
}
