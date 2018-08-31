using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : Ability
{
    int SummonIndex;

	public Summon(string name, float cd, int index) : base(name, cd)
    {
        SummonIndex = index;
    }

    public override void Action(GameObject source, GameObject target)
    {
        Debug.Log("A Demon Should be summoned in UnitSlot: " + target.name);

        // Calculate Cooldown from ability cooldown and source agi 
        float cd = abilityCD;

        // Summon new Demon
        CombatManager.Instance.AddSummonedPlayerMonster(SummonIndex,
            target);

        // Restart the source's cooldown
        source.GetComponent<Character>().UpdateAttackTimer(cd);
    }
}
