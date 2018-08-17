using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    protected string AbilityName;
    public string GetAbilityName { get { return AbilityName; } }

    public Ability(string name)
    {
        AbilityName = name;
    }

}
