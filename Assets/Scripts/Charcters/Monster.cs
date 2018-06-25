using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Monster : MonoBehaviour {

    [SerializeField]
    GameObject SpeedBarGO;

    [SerializeField]
    float attackCD = 10;
    [SerializeField]
    float attackTimer;

    [SerializeField]
    Sprite monsterSprite;
    private SpriteRenderer MonsterSprite;

    void Awake()
    {
        Assert.IsNotNull(SpeedBarGO);
        Assert.IsNotNull(monsterSprite);
    }

	// Use this for initialization
	void Start () {
        attackTimer = attackCD;

        // Set Correct Monster sprite
        //MonsterSprite = this.gameObject.GetComponent<SpriteRenderer>();
        //MonsterSprite.sprite = monsterSprite;
        //MonsterSprite.sortingLayerName = "Characters";
        //this.gameObject.AddComponent<SpriteRenderer>()
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
        Debug.Log("Monster Attacks!");
    }
}
