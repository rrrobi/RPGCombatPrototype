using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public class Menu : Ability
    {
        // List of abilities which will populate the menu
        private List<Ability> actionList = new List<Ability>();
        public List<Ability> GetActionList { get { return actionList; } }
        // Ability type - what type of abilities will be used to populate the ActionList
        private AbilityType actionType;

        public Menu(string name, float cd, AbilityType abilityType, List<Ability> abilityList) : base(name, cd)
        {
            actionType = abilityType;
            foreach (var ability in abilityList)
            {
                actionList.Add(ability);
            }
        }

        public override void Action(GameObject source, GameObject target)
        {
            
        }

    }
}
