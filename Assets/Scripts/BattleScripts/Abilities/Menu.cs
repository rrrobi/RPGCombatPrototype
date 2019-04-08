using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public class Menu
    {
        string menuName;
        // List of abilities which will populate the menu
        private List<Ability> actionList = new List<Ability>();
        public List<Ability> GetActionList { get { return actionList; } }
        // Ability type - what type of abilities will be used to populate the ActionList
        private AbilityType actionType;

        public Menu(string name, AbilityType abilityType, List<Ability> abilityList)
        {
            menuName = name;
            actionType = abilityType;
            foreach (var ability in abilityList)
            {
                actionList.Add(ability);
            }
        }

        public void Action(GameObject source, GameObject target)
        {
            
        }

    }
}
