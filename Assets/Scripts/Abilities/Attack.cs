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

    public override void Action(GameObject source, GameObject target)
    {
        Debug.Log(source.name + " Has used Direct attack on " + target.name);

        // Attack is a direct damage ability
        // Calculate damage from ability base damage and source str modifier
        int damage = Damage;

        // Calculate Cooldown from ability cooldown and source agi 
        float cd = abilityCD;

        // Deal damage to target
        target.GetComponent<Character>().TakeDamage(damage);

        // Restart the source's cooldown
        source.GetComponent<Character>().UpdateAttackTimer(cd);
    }
}
