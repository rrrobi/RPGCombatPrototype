﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

    // Later
    List<Monster> PlayerMonsters;
    List<Monster> EnemyMonsters;

    [SerializeField]
    private int FriendlyMonstersNum;
    [SerializeField]
    private int EnemyMonstersNum;

	// Use this for initialization
	void Start () {
        // Spawn Enemy monsters
        AddEnemyMonsters();

        // Spawn Players Monsters
        AddPlayerMonsters();

    }
	
    void AddPlayerMonsters()
    {
        // get number of Monster slots available

        // get number of monsters on this team

        // Check theres room for all of them

        // for each Monster 

            // Instaniate GO, Assign location of next available monster slot
    }

    void AddEnemyMonsters()
    {
        for (int i = 0; i < EnemyMonstersNum; i++)
        {
            GameObject monsterGO = new GameObject();
            //monsterGO = Instantiate("Monster", new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
