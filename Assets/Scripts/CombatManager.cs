using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CombatManager : MonoBehaviour {

    // Later
    List<Monster> PlayerMonsters;
    List<Monster> EnemyMonsters;

    [SerializeField]
    private int FriendlyMonstersNum;
    [SerializeField]
    private GameObject FriendlyTeamGO;
    [SerializeField]
    private GameObject FriendlyMonsterSlots;
    [SerializeField]    
    private int EnemyMonstersNum;
    [SerializeField]
    private GameObject EnemyTeamGO;
    [SerializeField]
    private GameObject EnemyMonsterSlots;

    [SerializeField]
    GameObject MonsterGOTemplate;

    Dictionary<string, Sprite> monsterSprites;

    public List<GameObject> slotList = new List<GameObject>();


    void Awake()
    {
        Assert.IsNotNull(FriendlyTeamGO);
        Assert.IsNotNull(FriendlyMonsterSlots);
        Assert.IsNotNull(EnemyTeamGO);
        Assert.IsNotNull(EnemyMonsterSlots);
        Assert.IsNotNull(MonsterGOTemplate);
    }

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
        slotList = new List<GameObject>();
        foreach (Transform child in FriendlyMonsterSlots.transform)
        {
            slotList.Add(child.gameObject);
        }

        // get number of monsters on this team
        // Check theres room for all of them
        // spawn each monster, unless there are more monsters than available slots.
        int numToSpawn = slotList.Count;
        if (FriendlyMonstersNum <= slotList.Count)
            numToSpawn = FriendlyMonstersNum;

        // for each Monster 
        for (int i = 0; i < numToSpawn; i++)
        {
            // Instaniate GO, Assign location of next available monster slot
            GameObject monsterGO = Instantiate(MonsterGOTemplate, slotList[i].transform.position, Quaternion.identity, FriendlyTeamGO.transform) as GameObject;
            monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites["SimpleMonsterBackBlue"]);
            // Set name of monsters, use sprite name and number (starts from 1)
            monsterGO.name = monsterGO.GetComponent<Monster>().GetMonsterSprite().name + "_" + (i+1);
        }
        
    }

    void AddEnemyMonsters()
    {
        slotList = new List<GameObject>();
        foreach (Transform child in EnemyMonsterSlots.transform)
        {
            slotList.Add(child.gameObject);
        }

        // spawn each monster, unless there are more monsters than available slots.
        int numToSpawn = slotList.Count;
        if (EnemyMonstersNum <= slotList.Count)
            numToSpawn = EnemyMonstersNum;

        for (int i = 0; i < numToSpawn; i++)
        {
            GameObject monsterGO = Instantiate(MonsterGOTemplate, slotList[i].transform.position, Quaternion.identity, EnemyTeamGO.transform) as GameObject;
            monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites["SimpleMonsterRed"]);
            // Set name of monsters, use sprite name and number (starts from 1)
            monsterGO.name = monsterGO.GetComponent<Monster>().GetMonsterSprite().name + "_" + (i + 1);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
