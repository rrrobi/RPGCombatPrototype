using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public class ActionMenu
    {
        string menuName;
        public string GetMenuName { get { return menuName; } }
        // List of abilities which will populate the menu
        private List<Ability> actionList = new List<Ability>();
        public List<Ability> GetActionList { get { return actionList; } }
        public void SetActionList(List<Ability> abilityList)
        {
            actionList.Clear();
            foreach (var ability in abilityList)
            {
                actionList.Add(ability);
            }
        }

        public ActionMenu(string name, List<Ability> abilityList)
        {
            menuName = name;
            foreach (var ability in abilityList)
            {
                actionList.Add(ability);
            }
        }
    }
}
