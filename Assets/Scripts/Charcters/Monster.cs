using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine.UI;

public class Monster : MonoBehaviour {

    [SerializeField]
    GameObject SpeedBarGO;

    [SerializeField]
    float attackCD;
    [SerializeField]
    float attackTimer;
    [SerializeField]
    int hP;
    public void SetHP(int hp) { hP = hp; }
    public int GetHP { get { return hP; } }
    int maxHP;
    public void SetMaxHP(int hp) { maxHP = hp; }
    public int GetMaxHP { get { return maxHP; } }
    // int numOfAbilities = 2;
    Dictionary <string, Attack> Abilities;
    public Dictionary<string, Attack> GetAbilities { get { return Abilities; } }
    public Attack GetAbilityByName(string name)
    {
        return Abilities[name];
    }

    [SerializeField]
    Sprite monsterSprite;

    GameObject myTeam;
    GameObject enemyTeam;

    private SpriteRenderer MonsterSprite;

    public void SetMonsterSprite(Sprite sprite)
    {
        monsterSprite = sprite;
    }
    public Sprite GetMonsterSprite()
    {
        return monsterSprite;
    }
    public void SetMonsterAbilities(List<Attack> abilities)
    {
        Abilities = new Dictionary<string, Attack>();
        for (int i = 0; i < abilities.Count; i++)
        {
            Abilities.Add(abilities[i].GetAttackName, abilities[i]);
        }
    }

    void Awake()
    {
        Assert.IsNotNull(SpeedBarGO);
        Assert.IsNotNull(monsterSprite);
    }

	// Use this for initialization
	void Start () {
        hP = maxHP;
        attackTimer = attackCD;        

        // Set this monster's Sprite
        AssignSprite();

        // Set monster's colider, (Make it clickable)
        CircleCollider2D col = this.gameObject.AddComponent<CircleCollider2D>();
        col.radius = 1.0f;

        // Discove whcih teams are friend or foe
        DiscoverTeams();
    }

    void AssignSprite()
    {
        MonsterSprite = this.gameObject.AddComponent<SpriteRenderer>();
        // Set Correct Monster sprite
        MonsterSprite.sprite = monsterSprite;
        MonsterSprite.sortingLayerName = "Characters";
    }

    void DiscoverTeams()
    {
        // TODO... refactor i don't like how this decides which team its on, and how it selects its target
        // should use setup from Combat manager instead

        // get team this monster is on
        myTeam = this.gameObject.transform.parent.gameObject;
        // get opposing team
        switch (myTeam.tag)
        {
            case "FriendlyTeam":
                enemyTeam = GameObject.FindGameObjectWithTag("EnemyTeam");
                break;
            case "EnemyTeam":
                enemyTeam = GameObject.FindGameObjectWithTag("FriendlyTeam");
                break;
            default:
                enemyTeam = myTeam;
                Debug.LogError("Issue with team tags, this shouldn't happen!");
                break;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // TODO...
        // Change this code so that when a monster is waiting in the action Queue, 
        // its attack times does not continue to count down past zero
        attackTimer -= Time.deltaTime;        
        if (attackTimer <= 0)
        {
            TakeTurn();            
        }
        else
            ScaleSpeedBar();
    }

    private void ScaleSpeedBar()
    {
        float timePast = attackCD - attackTimer;
        float scale = (timePast / attackCD);

        SpeedBarGO.transform.localScale = new Vector3(1.1f, scale, 1.0f);

        float barLength = 2;
        float offset = (barLength / 2) - (scale);
        SpeedBarGO.transform.localPosition = new Vector3(0.0f, offset, 0.0f);
    }

    // TODO....
    // Split AI/Player attack code
    private void TakeTurn()
    {
        if (myTeam.tag == "EnemyTeam")
            MonsterAttack();
        else
        {
            // TODO... ReThink
            // I HATE this
            GameObject.Find(this.name + "_Button").GetComponent<Button>().interactable = true;

            CombatManager.Instance.AddToActionQueue(this.gameObject);
        }
    }

    private void MonsterAttack()
    {        
        // get list of enemies
        List<GameObject> mobList = new List<GameObject>();
        foreach (Transform child in enemyTeam.transform)
        {
            mobList.Add(child.gameObject);
        }
        // attack Random enemy in list - AI (temp solution)
        // TODO....
        // Split AI/Player attack code
        if (mobList.Count > 0)
        {
            int targetIndex = Random.Range(0, mobList.Count);
            int abilityIndex = Random.Range(0, Abilities.Count);

            UseAbilityOn(Abilities.ElementAt(abilityIndex).Value, mobList[targetIndex]);
        }
    }
    
    public void UseAbilityOn(Attack ability, GameObject target)
    {
        Debug.Log(this.gameObject.name + " Uses '" + ability.GetAttackName + "' On " + target.name);

        target.GetComponent<Monster>().TakeAttack(ability);

        attackTimer = attackCD;
    }

    public void TakeAttack(Attack ability)
    {
        TakeDamage(ability.GetDamage);

        // hceck for death
        if (hP < 0)
            MonsterDies();
    }

    private void TakeDamage(int damage)
    {
        Debug.Log(this.name + " got hit for " + damage);
        hP -= damage;

        // Trigger Attacked Event callback
        EventCallbacks.TakeDamageEventInfo tdei = new EventCallbacks.TakeDamageEventInfo();
        tdei.EventDescription = "Unit " + gameObject.name + " Has taken " + damage + " damage";
        tdei.Damage = damage;
        tdei.UnitGO = gameObject;
        tdei.FireEvent();
    }

    private void MonsterDies()
    {
        Debug.Log(this.name + " has died!");

        // Trigger Death Event callback
        EventCallbacks.DeathEventInfo dei = new EventCallbacks.DeathEventInfo();
        dei.EventDescription = "Unit " + gameObject.name + " has died.";
        dei.UnitGO = gameObject;
        dei.TeamName = myTeam.tag;
        dei.FireEvent();

        // Monster has died, and so needs to do the following
        // Play any death sounds
        // Play any death animation        
        // update the Character list Maintained by Combat manager
        // Update the UI, so buttons belonging to this character no longer exist
        // Remove itself from the battlefield
    }
}
