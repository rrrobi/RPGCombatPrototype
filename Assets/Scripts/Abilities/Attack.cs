using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Ability
{

    int Damage;
    public int GetDamage { get { return Damage; } }    

    public Attack(string abilityName, int damage) : base (abilityName)
    {
        Damage = damage;
    }


}
