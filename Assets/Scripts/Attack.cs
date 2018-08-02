using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{

    int Damage;
    public int GetDamage { get { return Damage; } }
    string AttackName;
    public string GetAttackName { get { return AttackName; } }

    public Attack(string attackName, int damage)
    {
        AttackName = attackName;
        Damage = damage;
    }


}
