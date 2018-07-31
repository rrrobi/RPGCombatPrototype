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
        monsterData.ReadData();
    }

    public GameObject SpawnMonster(int index, TeamName team)
    {
        // Get monster data from index
        MonsterInfo monsterInfo = monsterData.GetMonsterFromIndex(index);

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

        GameObject monsterGO = monsterTemplateGO;
        monsterGO.name = monsterInfo.MonsterName + " " + monsterCounts[team + monsterInfo.MonsterName];
        if (team == TeamName.Friendly)
            monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.FriendlySpriteName]);
        else if (team == TeamName.Enemy)
            monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.EnemySpriteName]);
        else
            Debug.Log("TEAM name not correct!!!!");
        // Set Monster's ability
        // TODO... REwork this to use Ability Config
        Attack slash = new Attack("Slash", 10);
        Attack stab = new Attack("Stab", 15);
        List<Attack> abilities = new List<Attack>();
        abilities.Add(slash);
        abilities.Add(stab);
        monsterGO.GetComponent<Monster>().SetMonsterAbilities(abilities);
        // Set monster HP
        monsterGO.GetComponent<Monster>().SetMaxHP(monsterInfo.MaxHP);
        monsterGO.GetComponent<Monster>().SetHP(monsterInfo.MaxHP); // TODO... because of some strange ordering, if this isnt set here the UI at start doesn't update with correct HP

        return monsterGO;
    }

}
