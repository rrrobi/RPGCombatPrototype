using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Character : MonoBehaviour {

    // Will be changed to be Ability specific
    protected float attackCD = 10;
    protected float attackTimer;

    [SerializeField]
    protected GameObject SpeedBarGO;

    protected int hP;
    public void SetHP(int hp) { hP = hp; }
    public int GetHP { get { return hP; } }
    protected int maxHP;
    public void SetMaxHP(int hp) { maxHP = hp; }
    public int GetMaxHP { get { return maxHP; } }

    protected Dictionary<string, Attack> Abilities;
    public Dictionary<string, Attack> GetAbilities { get { return Abilities; } }
    public Attack GetAbilityByName(string name)
    {
        return Abilities[name];
    }
    public void SetMonsterAbilities(List<Attack> abilities)
    {
        Abilities = new Dictionary<string, Attack>();
        for (int i = 0; i < abilities.Count; i++)
        {
            Abilities.Add(abilities[i].GetAttackName, abilities[i]);
        }
    }

    protected TeamName team;
    public void SetTeam(TeamName teamName) { team = teamName; }
    public TeamName GetTeam { get { return team; } }
    protected GameObject myTeam;
    protected GameObject enemyTeam;

    protected Sprite monsterSprite;
    public void SetMonsterSprite(Sprite sprite) { monsterSprite = sprite; }
    public Sprite GetMonsterSprite() { return monsterSprite; }

    void Awake()
    {
        Assert.IsNotNull(SpeedBarGO);
    }

    protected virtual void Start()
    {
        Debug.Log("Character, start method for: " + team.ToString() + "_" + this.name);

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

    // Update is called once per frame
    protected virtual void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            TakeTurn();
        }
        else
            ScaleSpeedBar();
    }

    protected void AssignSprite()
    {
        SpriteRenderer MonsterSprite = this.gameObject.AddComponent<SpriteRenderer>();
        // Set Correct Monster sprite
        MonsterSprite.sprite = monsterSprite;
        MonsterSprite.sortingLayerName = "Characters";
    }

    protected void DiscoverTeams()
    {
        // TODO... refactor i don't like how this decides which team its on, and how it selects its target
        // should use setup from Combat manager instead

        // get team this monster is on
        myTeam = this.gameObject.transform.parent.gameObject;
        // get opposing team
        switch (team)
        {
            case TeamName.Friendly:
                enemyTeam = GameObject.FindGameObjectWithTag("EnemyTeam");
                break;
            case TeamName.Enemy:
                enemyTeam = GameObject.FindGameObjectWithTag("FriendlyTeam");
                break;
            default:
                enemyTeam = myTeam;
                Debug.LogError("Issue with team tags, this shouldn't happen!");
                break;
        }
    }

    protected void ScaleSpeedBar()
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
    protected void TakeTurn()
    {
        if (team == TeamName.Enemy)
            Attack();
        else
        {
            // TODO... ReThink
            // I HATE this
            GameObject.Find(this.team + "_" + this.name + "_Button").GetComponent<Button>().interactable = true;

            CombatManager.Instance.AddToActionQueue(this.gameObject);
        }
    }

    protected void Attack()
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
            CharacterDies();
    }

    protected void TakeDamage(int damage)
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

    protected virtual void CharacterDies()
    {
        Debug.Log(this.name + " has died!");

        // Trigger Death Event callback
        EventCallbacks.DeathEventInfo dei = new EventCallbacks.DeathEventInfo();
        dei.EventDescription = "Unit " + gameObject.name + " has died.";
        dei.UnitGO = gameObject;
        dei.TeamName = team;
        dei.FireEvent();
    }
}
