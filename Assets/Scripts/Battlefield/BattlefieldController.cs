using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UnitSlot
{
    Vector3 Position;
    bool IsOccupied;
}

public class BattlefieldController
{
    UnitSlot[,] FriendlySlots;
    UnitSlot[,] EnemySlots;

    public UnitSlot GetFriendlySlot(int column, int row)
    {
        return FriendlySlots[row, column];
    }
    public UnitSlot GetEnemySlot(int column, int row)
    {
        return EnemySlots[row, column];
    }

    int rows = 2;
    int columns = 3;

    public void Setup()
    {
        FriendlySlots = new UnitSlot[columns, rows];
        EnemySlots = new UnitSlot[columns, rows];
    }

}
