using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Hero : Character
    {

        // Use this for initialization
        protected override void Start()
        {
            Debug.Log("Hero, start method for: " + team.ToString() + "_" + this.name);
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        protected override void CharacterDies()
        {
            Debug.Log(this.name + " has died!");

            // Trigger Death Event callback
            EventCallbacks.HeroDeathEventInfo hdei = new EventCallbacks.HeroDeathEventInfo();
            hdei.EventDescription = "Hero has died!";
            hdei.FireEvent();
        }
    }
}