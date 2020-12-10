using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    static public class TargetingHelper
    {
        static public List<GameObject> GetPossibleTargets(List<GameObject> activeCharacters)
        {
            // This Only handles standard target options, if the future allows exceptions, this will need to be modified.
            List<GameObject> output = new List<GameObject>();
            foreach (var character in activeCharacters)
            {
                // If charcater is on front row, we can always target this.
                if (character.GetComponent<Character>().GetUnitSlot().name.EndsWith("0"))
                {
                    output.Add(character);
                    continue;
                }
                // if the character is on the back row, BUT there is no character DIRECTLY in front, we can target this
                // Find the character position
                string charPos = character.GetComponent<Character>().GetUnitSlot().name.Split('_')[1].Split('-')[0];
                bool isblocked = false;
                foreach (var charCompare in activeCharacters)
                {
                    // if any other available character is 'blocking' the charcater, move on to the next,
                    // this is not a targetable character
                    if (charCompare.GetComponent<Character>().GetUnitSlot().name.Split('_')[1] == $"{charPos}-0")
                    {
                        isblocked = true;
                        break;
                    }
                }
                if (!isblocked)
                    output.Add(character);
            }

            return output;
        }
    }
}
