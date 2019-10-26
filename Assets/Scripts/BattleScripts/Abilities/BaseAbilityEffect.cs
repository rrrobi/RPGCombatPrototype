using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbilityEffect
{
    public BaseAbilityEffect()
    {

    }

    /// <summary>
    /// Will be overridden for each Effect class, e.g. DirectDamamge will hand the EffectAction differently to DirectHeal
    /// </summary>
    /// <param name="strengthMod"></param>
    /// <param name="target"></param>
    public abstract void EffectAction(int strengthMod, GameObject target);
}
