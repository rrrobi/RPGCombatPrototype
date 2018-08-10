using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine.UI;

public class Monster : Character {

    Dictionary <string, Attack> Abilities;
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

    void Awake()
    {
        Assert.IsNotNull(SpeedBarGO);
    }

	// Use this for initialization
	protected override void Start () {
        Debug.Log("Monster, start method for: " + team.ToString() + "_" + this.name);
        base.Start();
        
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

    // TODO....
    // Split AI/Player attack code
    private void TakeTurn()
    {
        if (team == TeamName.Enemy)
            MonsterAttack();
        else
        {
            // TODO... ReThink
            // I HATE this
            GameObject.Find(this.team + "_" + this.name + "_Button").GetComponent<Button>().interactable = true;

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
    
}
