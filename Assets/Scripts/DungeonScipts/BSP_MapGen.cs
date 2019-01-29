using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP_MapGen
{
    // BSP - partition the space up into randomly sized areas
    #region

    #endregion

    // Room placement - Asign a randomly sized room to each of the BSP segments
    // The room can not be larger than the segemnt, and is positioned randomly within the segment, 
    // it can not be placed so that it 'spills' out of the segment
    #region

    #endregion

    // First pass of corridor connections - Pair up a number of the rooms previously created
    // Use A* pathfinding to create a path between each pair.
    #region

    #endregion

    // Second pass of corridor connections - Assign an entrance tile in one of the rooms,
    // Ensure there is a path from this location to every other room
    // Assign of of the distant rooms to contain the exit.
    #region

    #endregion
}
