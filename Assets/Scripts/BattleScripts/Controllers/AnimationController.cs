using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;

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
        
    }

    void OnAbilityUsed(UseAbilityEventInfo useAbilityEventInfo)
    {
        Debug.Log("AnimationController Alerted to Ability used!");

        
    }

    void OnAbilityHit(AbilityHitEventInfo abilityHitEventInfo)
    {
        Debug.Log("AnimationController Alerted to Ability Hit!");


    }
}
