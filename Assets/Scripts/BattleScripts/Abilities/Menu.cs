using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public class Menu : Ability
    {
        // List of abilities which will populate the menu
        private List<Ability> actionList = new List<Ability>();
        // Ability type - what type of abilities will be used to populate the ActionList
        private AbilityType actionType;

        public Menu(string name, float cd, AbilityType abilityType) : base(name, cd)
        {
            actionType = abilityType;
        }

        public override void Action(GameObject source, GameObject target)
        {
            
        }

    }
}
