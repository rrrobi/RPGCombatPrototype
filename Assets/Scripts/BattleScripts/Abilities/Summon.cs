using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
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

            // Summon new Demon
            CombatManager.Instance.AddSummonedPlayerMonster(SummonIndex,
                target);

            // Resolve Afteraction Effects
            AfterAction(source);
        }
    }
}