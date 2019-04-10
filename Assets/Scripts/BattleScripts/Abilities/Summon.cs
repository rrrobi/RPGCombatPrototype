using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Summon : Ability
    {
        int SummonIndex;

        Global.MonsterInfo MonsterInfo;

        // Index used when summoning a NEW monster - Most likely only used for Enemy 'Call help' style abilities
        public Summon(string name, float cd, int index) : base(name, cd)
        {
            SummonIndex = index;
        }

        public Summon(string name, float cd, Global.MonsterInfo monsterInfo) : base(name, cd)
        {            
            MonsterInfo = monsterInfo;
        }

        public override void Action(GameObject source, GameObject target)
        {
            Debug.Log("A Demon Should be summoned in UnitSlot: " + target.name);

            // Summon new Demon
            if (MonsterInfo == null)
            CombatManager.Instance.AddSummonedPlayerMonster(SummonIndex,
                target);
            else
                CombatManager.Instance.AddSummonedPlayerMonster(MonsterInfo,
                target);

            // Resolve Afteraction Effects
            AfterAction(source);
        }
    }
}