using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamName
{
    Friendly,
    Enemy
}

public class MonsterSpawner
{
    MonsterDataReader monsterData = new MonsterDataReader();
    AbilityDataReader abilityData = new AbilityDataReader();

    Dictionary<string, int> monsterCounts = new Dictionary<string, int>();

    GameObject monsterTemplateGO;
    Dictionary<string, Sprite> monsterSprites = new Dictionary<string, Sprite>();

    public void Setup()
    {
        // Read in MonsterGO template for to use when spawning monsters
        monsterTemplateGO = Resources.Load("Prefabs/Monster") as GameObject;
        // Read in Monster sprites ready to assign to New monsters
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Monsters/");
        Debug.Log("LOADED RESOURCE: ");
        foreach (Sprite s in sprites)
        {
            Debug.Log(s);
            monsterSprites.Add(s.name, s);
        }

        // Read in monster data, ready for spawning specific monsters from the config file
        monsterData.SetUp();
        monsterData.SaveData();
        monsterData.ReadData();

        abilityData.SetUp();
        abilityData.SaveData();
        abilityData.ReadData();
    }

    public GameObject SpawnMonster(int index, TeamName team, GameObject teamGroup, Vector3 pos)
    {
        // Get monster data from index
        MonsterInfo monsterInfo = monsterData.GetMonsterFromIndex(index);
        // Keep track of count of each monster type in this fight
        TrackMonsterCount(monsterInfo, team);

        GameObject monsterGO = GameObject.Instantiate(monsterTemplateGO, pos, Quaternion.identity, teamGroup.transform) as GameObject;
        monsterGO.name = monsterInfo.MonsterName + " " + monsterCounts[team + monsterInfo.MonsterName];
        if (team == TeamName.Friendly)
            monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.FriendlySpriteName]);
        else if (team == TeamName.Enemy)
            monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.EnemySpriteName]);
        else
            Debug.Log("TEAM name not correct!!!!");
        monsterGO.GetComponent<Monster>().SetTeam(team);
        // Set Monster's ability
        List<Attack> abilities = new List<Attack>();
        if (!string.IsNullOrEmpty(monsterInfo.Ability1))
            abilities.Add(CreateAbilityFromData(monsterInfo.Ability1));
        if (!string.IsNullOrEmpty(monsterInfo.Ability2))
            abilities.Add(CreateAbilityFromData(monsterInfo.Ability2));
        if (!string.IsNullOrEmpty(monsterInfo.Ability3))
            abilities.Add(CreateAbilityFromData(monsterInfo.Ability3));
        if (!string.IsNullOrEmpty(monsterInfo.Ability4))
            abilities.Add(CreateAbilityFromData(monsterInfo.Ability4));
        // TODO... what if no abilities?!?
        monsterGO.GetComponent<Monster>().SetMonsterAbilities(abilities);

        // Set monster HP
        monsterGO.GetComponent<Monster>().SetMaxHP(monsterInfo.MaxHP);
        monsterGO.GetComponent<Monster>().SetHP(monsterInfo.MaxHP); // TODO... because of some strange ordering, if this isnt set here the UI at start doesn't update with correct HP

        return monsterGO;
    }

    Attack CreateAbilityFromData(string abilityName)
    {
        AbilityInfo abilityInfo = abilityData.GetAbilityByName(abilityName);

        Attack ability = new Attack(abilityInfo.Name, abilityInfo.BaseAbilityStrength);
        return ability;
    }

    void TrackMonsterCount(MonsterInfo monsterInfo, TeamName team)
    {
        // Keep track and increment number of each Monster type in this battle,
        // This is for the Naming convenntion, to ensure no two names are the same
        if (monsterCounts != null)
        {
            if (!monsterCounts.ContainsKey(team + monsterInfo.MonsterName))
            {
                monsterCounts.Add(team + monsterInfo.MonsterName, 1);
            }
            else
                monsterCounts[team + monsterInfo.MonsterName]++;
        }
    }

}
