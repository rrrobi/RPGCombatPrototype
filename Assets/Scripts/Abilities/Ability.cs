using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    protected string abilityName;
    public string GetAbilityName { get { return abilityName; } }

    protected AbilityType abilityType;
    public AbilityType GetAbilityType() { return abilityType; }
    public void SetAbilityType(AbilityType type) { abilityType = type; }

    public Ability(string name)
    {
        abilityName = name;
    }

    public virtual void Action()
    {
        // must be overridden
    }

}
