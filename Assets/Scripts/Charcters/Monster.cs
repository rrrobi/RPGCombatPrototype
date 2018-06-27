using System.Collections;
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

        MonsterSprite = this.gameObject.AddComponent<SpriteRenderer>();
        // Set Correct Monster sprite
       // MonsterSprite = this.gameObject.GetComponent<SpriteRenderer>();
        MonsterSprite.sprite = monsterSprite;
        MonsterSprite.sortingLayerName = "Characters";
    }
	
	// Update is called once per frame
	void Update ()
    {
        attackTimer -= Time.deltaTime;
        ScaleSpeedBar();
        if (attackTimer <= 0)
        {
            MonsterAttack();
            attackTimer = attackCD;
        }

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

    private void MonsterAttack()
    {
        // get team this monster is on
        GameObject myTeam = this.gameObject.transform.parent.gameObject;
        // get opposing team
        GameObject enemyTeam;
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

        // get list of enemies
        List<GameObject> mobList = new List<GameObject>();
        foreach (Transform child in enemyTeam.transform)
        {
            mobList.Add(child.gameObject);
        }
        // attack 1st enemy in list
        if (mobList.Count > 0)
        {
            Debug.Log(this.name + " Attacks " + mobList[0].name);
            mobList[0].GetComponent<Monster>().TakeAttack(10);
        }

    }

    public void TakeAttack(int damage)
    {
        Debug.Log(this.name + " got hit for " + damage);
    }
}
