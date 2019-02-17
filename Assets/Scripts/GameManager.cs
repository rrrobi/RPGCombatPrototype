using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; protected set; }

    // num of enemies
    int numOfEnemies = 1;
    public int GetNumOfEnemies { get { return numOfEnemies; } }
    // num of Friendlies
    int numOfFriendlies = 1;
    public int GetNumOfFriendlies { get { return numOfFriendlies; } }

    // Dungeon Gen
    BSP_MapGen BSP_DungeonGenerator;

    // Use this for initialization
    void Start () {
        if (Instance != null)
        {
            Debug.Log("There should never be more than one GameManager.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);

        BSP_DungeonGenerator = new BSP_MapGen();
        BSP_DungeonGenerator.GenerateBSPDungeon();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartBattle()
    {
        // Temp- beyond temp, to add some UI input to manipulate the battles
        GameObject panel = GameObject.Find("BattleInfoPanel");
        InputField[] inputs = panel.GetComponentsInChildren<InputField>();
        foreach (var input in inputs)
        {
            int value;
            if (!int.TryParse(input.text, out value))
                value = 2;

            if (input.name == "InputFriendlyCount")
                numOfFriendlies = value;
            if (input.name == "InputEnemyCount")
                numOfEnemies = value;
        }

        SceneManager.LoadScene("CombatPrototype");
    }
}
