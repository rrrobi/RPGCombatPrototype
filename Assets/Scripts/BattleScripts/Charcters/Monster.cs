using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine.UI;

namespace Battle
{
    public class Monster : Character
    {
        

        //Global.MonsterInfo mi;  // allows MonsterGO to be used to keep track of Player's monster's stats, even if they are un-summoned, or between battles
        //bool IsSummonable;      // allows the UI to know if the monster needs to have a summon button on the summon menu    // these two may be replaced by a 
        //bool IsActive;          // may not be needed                                                                        //          'status' enum

        void Awake()
        {
        }

        // Use this for initialization
        protected override void Start()
        {
            Debug.Log("Monster, start method for: " + team.ToString() + "_" + this.name);
            base.Start();

        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

        }

    }
}