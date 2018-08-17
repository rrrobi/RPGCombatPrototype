using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;

public class BattlefieldController
{
    GameObject[,] FriendlySlots;
    GameObject[,] EnemySlots;
    public GameObject GetFriendlySlot(int column, int row)
    {
        return FriendlySlots[column, row];
    }
    public GameObject GetEnemySlot(int column, int row)
    {
        return EnemySlots[column, row];
    }

    // Unitslot prefab 
    // TODO...may rework this later
    GameObject unitSlotGO;
    // Empty GOs for holding the unitslots
    GameObject friendlyTeamSlots;
    public GameObject GetFriendlyTeamSlotsGO() { return friendlyTeamSlots; }
    GameObject enemyTeamSlots;
    public GameObject GetEnemyTeamGOSlots() { return enemyTeamSlots; }

    int rows = 2;
    int columns = 3;

    List<string> slotOrderList = new List<string>()
    {
        "1_0",
        "0_0",
        "2_0",
        "1_1",
        "0_1",
        "2_1",
    };
    

    public void Setup()
    {
        // Read in MonsterGO template for to use when spawning monsters
        unitSlotGO = Resources.Load("Prefabs/UnitSlot") as GameObject;
        // Initialise Slot arrays
        FriendlySlots = new GameObject[columns, rows];
        EnemySlots = new GameObject[columns, rows];

        // Set up Team objects
        GameObject battlefieldGO = GameObject.FindGameObjectWithTag("Battlefield");
        friendlyTeamSlots = new GameObject("FriendlyTeamSlots");
        friendlyTeamSlots.transform.parent = battlefieldGO.transform;
        enemyTeamSlots = new GameObject("EnemyTeamSlots");
        enemyTeamSlots.transform.parent = battlefieldGO.transform;

        SetUpFriendlySlots();
        SetUpEnemySlots();

        // register callbacks
        RegisterEventCallbacks();
    }

    private void SetUpFriendlySlots()
    {         
        FriendlySlots[0, 0] = GameObject.Instantiate(unitSlotGO,
            new Vector3(-2.5f, -2.0f, 0.0f),
            Quaternion.identity,
            friendlyTeamSlots.transform) as GameObject;
        FriendlySlots[0, 0].name = "UnitSlot_0-0";
        FriendlySlots[1, 0] = GameObject.Instantiate(unitSlotGO,
            new Vector3(0.0f, -2.0f, 0.0f),
            Quaternion.identity,
            friendlyTeamSlots.transform) as GameObject;
        FriendlySlots[1, 0].name = "UnitSlot_1-0";
        FriendlySlots[2, 0] = GameObject.Instantiate(unitSlotGO,
            new Vector3(2.5f, -2.0f, 0.0f),
            Quaternion.identity,
            friendlyTeamSlots.transform) as GameObject;
        FriendlySlots[2, 0].name = "UnitSlot_2-0";

        FriendlySlots[0, 1] = GameObject.Instantiate(unitSlotGO,
            new Vector3(-2.5f, -4.5f, 0.0f),
            Quaternion.identity,
            friendlyTeamSlots.transform) as GameObject;
        FriendlySlots[0, 1].name = "UnitSlot_0-1";
        FriendlySlots[1, 1] = GameObject.Instantiate(unitSlotGO,
            new Vector3(0.0f, -4.5f, 0.0f),
            Quaternion.identity,
            friendlyTeamSlots.transform) as GameObject;
        FriendlySlots[1, 1].name = "UnitSlot_1-1";
        FriendlySlots[2, 1] = GameObject.Instantiate(unitSlotGO,
            new Vector3(2.5f, -4.5f, 0.0f),
            Quaternion.identity,
            friendlyTeamSlots.transform) as GameObject;
        FriendlySlots[2, 1].name = "UnitSlot_2-1";
    }

    private void SetUpEnemySlots()
    {
        EnemySlots[0, 0] = GameObject.Instantiate(unitSlotGO,
            new Vector3(-2.5f, 2.0f, 0.0f),
            Quaternion.identity,
            enemyTeamSlots.transform) as GameObject;
        EnemySlots[0, 0].name = "UnitSlot_0-0";
        EnemySlots[1, 0] = GameObject.Instantiate(unitSlotGO,
            new Vector3(0.0f, 2.0f, 0.0f),
            Quaternion.identity,
            enemyTeamSlots.transform) as GameObject;
        EnemySlots[1, 0].name = "UnitSlot_1-0";
        EnemySlots[2, 0] = GameObject.Instantiate(unitSlotGO,
            new Vector3(2.5f, 2.0f, 0.0f),
            Quaternion.identity,
            enemyTeamSlots.transform) as GameObject;
        EnemySlots[2, 0].name = "UnitSlot_2-0";

        EnemySlots[0, 1] = GameObject.Instantiate(unitSlotGO,
            new Vector3(-2.5f, 4.5f, 0.0f),
            Quaternion.identity,
            enemyTeamSlots.transform) as GameObject;
        EnemySlots[0, 1].name = "UnitSlot_0-1";
        EnemySlots[1, 1] = GameObject.Instantiate(unitSlotGO,
            new Vector3(0.0f, 4.5f, 0.0f),
            Quaternion.identity,
            enemyTeamSlots.transform) as GameObject;
        EnemySlots[1, 1].name = "UnitSlot_1-1";
        EnemySlots[2, 1] = GameObject.Instantiate(unitSlotGO,
            new Vector3(2.5f, 4.5f, 0.0f),
            Quaternion.identity,
            enemyTeamSlots.transform) as GameObject;
        EnemySlots[2, 1].name = "UnitSlot_2-1";
    }

    public int FindUnoccupiedFriendlySlotCount()
    {
        int unoccupiedCount = 0;

        // for each unit slot
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // if the slot is NOT occupied add to the count
                if (!FriendlySlots[x, y].GetComponent<UnitSlot>().GetIsOccupied())
                    unoccupiedCount++;
            }
        }

        return unoccupiedCount;
    }

    public GameObject FindNextUnoccupiedFriendlySlot()
    {
        foreach (var item in slotOrderList)
        {
            string[] s_index = item.Split('_');
            int x = int.Parse(s_index[0]);
            int y = int.Parse(s_index[1]);
            if (!GetFriendlySlot(x, y).GetComponent<UnitSlot>().GetIsOccupied())
                return GetFriendlySlot(x, y);
        }

        // if no unoccupied slots are available, return null
        return null;
    }

    public int FindUnoccupiedEnemySlotCount()
    {
        int unoccupiedCount = 0;

        // for each unit slot
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // if the slot is NOT occupied add to the count
                if (!EnemySlots[x, y].GetComponent<UnitSlot>().GetIsOccupied())
                    unoccupiedCount++;
            }
        }

        return unoccupiedCount;
    }

    public GameObject FindNextUnoccupiedEnemySlot()
    {
        foreach (var item in slotOrderList)
        {
            string[] s_index = item.Split('_');
            int x = int.Parse(s_index[0]);
            int y = int.Parse(s_index[1]);
            if (!GetEnemySlot(x, y).GetComponent<UnitSlot>().GetIsOccupied())
                return GetEnemySlot(x, y);
        }

        // if no unoccupied slots are available, return null
        return null;
    }

    public GameObject FindSlotFromCharacter(GameObject character)
    {
        if (character.GetComponent<Character>().GetTeam == TeamName.Friendly)
        {
            // for each unit slot
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (FriendlySlots[x, y].GetComponent<UnitSlot>().GetIsOccupied())
                        if (FriendlySlots[x, y].GetComponent<UnitSlot>().GetOccupyingCharacter() == character)
                            return FriendlySlots[x, y];
                }
            }
        }
        else if (character.GetComponent<Character>().GetTeam == TeamName.Enemy)
        {
            // for each unit slot
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (EnemySlots[x, y].GetComponent<UnitSlot>().GetIsOccupied())
                        if (EnemySlots[x, y].GetComponent<UnitSlot>().GetOccupyingCharacter() == character)
                            return EnemySlots[x, y];
                }
            }
        }

        // If no matches are found return null
        return null;
    }

    #region Event Callbacks

        void RegisterEventCallbacks()
    {
        UnitSpawnEventInfo.RegisterListener(OnUnitSpawn);
        DeathEventInfo.RegisterListener(OnUnitDied);
    }

    void OnUnitSpawn(UnitSpawnEventInfo unitSpawnEventInfo)
    {
        Debug.Log("BattlefieldController Alerted to unit Spawned: " + unitSpawnEventInfo.UnitGO.name);

        unitSpawnEventInfo.UnitSlotGO.GetComponent<UnitSlot>().SetIsOccupied(true);
        unitSpawnEventInfo.UnitSlotGO.GetComponent<UnitSlot>().SetOccupyingCharacter(unitSpawnEventInfo.UnitGO);
    }

    void OnUnitDied(DeathEventInfo deathEventInfo)
    {
        Debug.Log("BattlefieldController Alerted to Character Death: " + deathEventInfo.UnitGO.name);

        // Find UnitSlot by CharacterGO, 
        // Reset slot back to unoccupied
        GameObject unitSlot = FindSlotFromCharacter(deathEventInfo.UnitGO);
        if (unitSlot != null)
            unitSlot.GetComponent<UnitSlot>().SetIsOccupied(false);

        // If dead unit was an enemy, check how many enemies are left(unoccupied slots), if none we win 
        if (deathEventInfo.UnitGO.GetComponent<Character>().GetTeam == TeamName.Enemy)
        {
            if (FindUnoccupiedEnemySlotCount() >= (rows * columns))
            {
                // All enemies are dead, the player has won the battle
                BattleWonEventInfo bwei = new BattleWonEventInfo();
                bwei.EventDescription = "The player has won the battle";
                bwei.FireEvent();
            }
        }
    }

    #endregion
}
