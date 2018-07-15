using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CombatManager : MonoBehaviour {

    // Later - unused
    Dictionary<string, GameObject> playerCharacters;
    Dictionary<string, GameObject> enemyCharacters;

    public Dictionary<string, GameObject> GetPlayerCharacterList { get{  return playerCharacters; }  }
    public Dictionary<string, GameObject> GetEnemyCharacterList { get { return enemyCharacters; } }
    public GameObject GetPlayerCharacterByName(string name)
    {
        return playerCharacters[name];
    }
    public GameObject GetEnemyCharacterByName(string name)
    {
        return enemyCharacters[name];
    }
    private void AddToPlayerCharacterList(GameObject character)
    {
        playerCharacters.Add(character.name, character);
    }
    private void AddToEnemyCharacterList(GameObject character)
    {
        enemyCharacters.Add(character.name, character);
    }
    private void RemoveFromPlayerCharacterList(GameObject character)
    {
        playerCharacters.Remove(character.name);
    }
    private void RemoveFromEnemyCharacterList(GameObject character)
    {
        enemyCharacters.Remove(character.name);
    }


    public static CombatManager Instance { get; protected set; }

    // Monster Setup Objects
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

    // Battle Objects
    Queue<GameObject> actionQueue = new Queue<GameObject>();

    void OnEnable()
    {
        if (Instance != null)
            Debug.LogError("There should never be more than one CombatManager.");
        Instance = this;
    }

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
        playerCharacters = new Dictionary<string, GameObject>();
        enemyCharacters = new Dictionary<string, GameObject>();

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
        List<GameObject> slotList = new List<GameObject>();
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
            // Set Monster's ability
            Attack attack = new Attack("Slash", 10);
            Attack[] abilities = new Attack[1];
            abilities[0] = attack;
            monsterGO.GetComponent<Monster>().SetMonsterAbilities(abilities);

            // add to PlayerCharacterList
            playerCharacters.Add(monsterGO.name, monsterGO);
        }
        
    }

    void AddEnemyMonsters()
    {
        List<GameObject> slotList = new List<GameObject>();
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
            // Set Monster's ability
            Attack attack = new Attack("Slash", 10);
            Attack[] abilities = new Attack[1];
            abilities[0] = attack;
            monsterGO.GetComponent<Monster>().SetMonsterAbilities(abilities);

            // Add to EnemyCharacterList
            enemyCharacters.Add(monsterGO.name, monsterGO);
        }
    }

    public void AddToActionQueue(GameObject monster)
    {
        if (!actionQueue.Contains(monster))
        {
            actionQueue.Enqueue(monster);

            // Temp
            Debug.Log(monster.name + " joined the action queue");
        }
    }

    public GameObject TakeFromActionQueue()
    {
        return actionQueue.Dequeue();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
