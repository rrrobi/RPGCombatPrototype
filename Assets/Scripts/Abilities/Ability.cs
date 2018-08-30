using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    protected string abilityName;
    public string GetAbilityName { get { return abilityName; } }

    protected float abilityCD;
    public float GetAbilityCD { get { return abilityCD; } }

    protected AbilityType abilityType;
    public AbilityType GetAbilityType() { return abilityType; }
    public void SetAbilityType(AbilityType type) { abilityType = type; }

    public Ability(string name, float cd)
    {
        abilityName = name;
        abilityCD = cd;
    }

    public abstract void Action(GameObject source, GameObject target);

}
