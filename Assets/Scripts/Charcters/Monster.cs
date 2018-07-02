﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Monster : MonoBehaviour {

    [SerializeField]
    GameObject SpeedBarGO;

    [SerializeField]
    float attackCD;
    [SerializeField]
    float attackTimer;

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

    void Awake()
    {
        Assert.IsNotNull(SpeedBarGO);
        Assert.IsNotNull(monsterSprite);
    }

	// Use this for initialization
	void Start () {
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
        //Debug.Log("CD: " + attackCD + ", time past: " + timePast + ", scale: " + scale);

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
            CombatManager.Instance.AddToActionQueue(this.gameObject);
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

            Debug.Log(this.name + " Attacks " + mobList[targetIndex].name);
            mobList[targetIndex].GetComponent<Monster>().TakeAttack(10);
        }

        attackTimer = attackCD;
    }

    public void TakeAttack(int damage)
    {
        Debug.Log(this.name + " got hit for " + damage);
    }
}
