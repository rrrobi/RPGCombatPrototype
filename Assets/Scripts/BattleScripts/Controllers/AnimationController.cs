using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;
using System.Linq;

public class AnimationController
{
    public AnimationController()
    {

        RegisterEventCallbacks();
    }

    void RegisterEventCallbacks()
    {
        UseAbilityEventInfo.RegisterListener(OnAbilityUsed);
        AbilityHitEventInfo.RegisterListener(OnAbilityHit);
    }

    public void UnregisterEventCallbacks()
    {
        UseAbilityEventInfo.UnregisterListener(OnAbilityUsed);
        AbilityHitEventInfo.UnregisterListener(OnAbilityHit);
    }

    void OnAbilityUsed(UseAbilityEventInfo useAbilityEventInfo)
    {
        Debug.Log("AnimationController Alerted to Ability used!");
        useAbilityEventInfo.UnitGO.GetComponent<Animator>().SetTrigger("OnAttack");
        
    }

    void OnAbilityHit(AbilityHitEventInfo abilityHitEventInfo)
    {
        Debug.Log("AnimationController Alerted to Ability Hit!");

        if (abilityHitEventInfo.ability.GetEffectList().Any(c => c.abilityType == AbilityType.Attack))
            abilityHitEventInfo.UnitGO.GetComponent<Animator>().SetTrigger("OnGetHit");

    }
}
