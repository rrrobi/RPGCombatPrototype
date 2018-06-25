using System.Collections;
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
    [SerializeField]
    GameObject MonsterGOTemplate;

    Dictionary<string, Sprite> monsterSprites;

    // Use this for initialization
    void Start () {

        // Load Monster Sprites
        monsterSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Monsters/");
        Debug.Log("LOADED RESOURCE: ");
        foreach (Sprite s in sprites)
        {
            Debug.Log(s);
            monsterSprites.Add(s.name, s);
        }


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
            GameObject monsterGO = Instantiate(MonsterGOTemplate, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
            monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites["SimpleMonsterRed"]);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
