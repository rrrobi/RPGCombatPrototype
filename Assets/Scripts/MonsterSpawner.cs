using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner
{
    MonsterDataReader monsterData = new MonsterDataReader();

    GameObject monsterTemplateGO;
    Dictionary<string, Sprite> monsterSprites = new Dictionary<string, Sprite>();

    void Setup()
    {
        // Read in MonsterGO template for to use when spawning monsters
        monsterTemplateGO = Resources.Load("Prefab/Monster") as GameObject;
        // Read in Monster sprites ready to assign to New monsters
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Monsters/");

        // Read in monster data, ready for spawning specific monsters from the config file
        monsterData.SetUp();
        monsterData.ReadData();
    }

    public GameObject SpawnMonster(int index)
    {
        // Get monster data from index
        MonsterInfo monsterInfo = monsterData.GetMonsterFromIndex(index);

        GameObject monsterGO = monsterTemplateGO;
        monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.FriendlySpriteName]);
        monsterGO.name = monsterInfo.MonsterName;
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
