using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        throw new System.NotImplementedException();
    }
}
