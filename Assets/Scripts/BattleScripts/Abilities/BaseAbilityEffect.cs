using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbilityEffect
{
    public BaseAbilityEffect()
    {

    }

    /// <summary>
    /// Will be overridden for each Effect class, e.g. DirectDamamge will handle the EffectAction differently to DirectHeal
    /// </summary>
    /// <param name="strengthModifier"> Made up of the ability strength AND the characters relvant modifier</param>
    /// <param name="target"></param>
    public abstract void EffectAction(int strengthModifier, GameObject target);
}
