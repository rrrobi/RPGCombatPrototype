using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class SummonAbilityEffect : BaseAbilityEffect
    {
        Global.MonsterInfo MonsterInfo;

        public SummonAbilityEffect(Global.MonsterInfo mi)
            : base()
        {
            MonsterInfo = mi;
        }

        // this is where are DO the direct heal
        public override void EffectAction(int strengthModifier, GameObject target)
        {
            CombatManager.Instance.AddSummonedPlayerMonster(MonsterInfo, target);
        }
    }
}
