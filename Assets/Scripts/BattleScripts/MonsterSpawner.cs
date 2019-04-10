using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public enum TeamName
{
    Friendly,
    Enemy
}

namespace Battle
{
    public class MonsterSpawner
    {
        //HeroDataReader heroData = new HeroDataReader();
        MonsterDataReader monsterData = new MonsterDataReader();
        AbilityDataReader abilityData = new AbilityDataReader();

        Dictionary<string, int> monsterCounts = new Dictionary<string, int>();

        GameObject monsterTemplateGO;
        GameObject heroTemplateGO;
        Dictionary<string, Sprite> monsterSprites = new Dictionary<string, Sprite>();

        public void Setup()
        {
            // Read in MonsterGO template for to use when spawning monsters
            monsterTemplateGO = Resources.Load("BattleResources/Prefabs/Monster") as GameObject;
            heroTemplateGO = Resources.Load("BattleResources/Prefabs/Hero") as GameObject;
            // Read in Monster sprites ready to assign to New monsters
            Sprite[] sprites = Resources.LoadAll<Sprite>("BattleResources/Sprites/Monsters/");
            Debug.Log("LOADED RESOURCE: ");
            foreach (Sprite s in sprites)
            {
                Debug.Log(s);
                monsterSprites.Add(s.name, s);
            }

            // Read in monster data, ready for spawning specific monsters from the config file
            //heroData.Setup();
            //heroData.ReadData();

            monsterData.SetUp();
            monsterData.ReadData();

            abilityData.SetUp();
            abilityData.ReadData();
        }

        public GameObject SpawnHero(TeamName team, GameObject teamGroup, GameObject unitSlot)
        {
            // Get monster data from index
            HeroInfo HeroInfo = GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo;
            // Keep track of count of each monster type in this fight
            //   TrackCharacterCount(HeroInfo, team);

            GameObject heroGO = GameObject.Instantiate(heroTemplateGO, unitSlot.transform.position, Quaternion.identity, teamGroup.transform) as GameObject;
            heroGO.name = HeroInfo.PlayerName;
            if (team == TeamName.Friendly)
                heroGO.GetComponent<Hero>().SetMonsterSprite(monsterSprites[HeroInfo.FriendlySpriteName]);
            else
                Debug.Log("TEAM name not correct!!!!");
            heroGO.GetComponent<Hero>().SetTeam(team);
            // Set Monster's ability
            List<Ability> abilities = new List<Ability>();
            if (!string.IsNullOrEmpty(HeroInfo.Ability1))
                abilities.Add(CreateAbilityFromData(HeroInfo.Ability1));
            if (!string.IsNullOrEmpty(HeroInfo.Ability2))
                abilities.Add(CreateAbilityFromData(HeroInfo.Ability2));
            //if (!string.IsNullOrEmpty(HeroInfo.Ability3))
            //    abilities.Add(CreateAbilityFromData(HeroInfo.Ability3));
            //if (!string.IsNullOrEmpty(HeroInfo.Ability4))
            //    abilities.Add(CreateAbilityFromData(HeroInfo.Ability4));
            // TODO... what if no abilities?!?
            heroGO.GetComponent<Hero>().SetMonsterAbilities(abilities);

            // Set up Hero's Menus
            List<ActionMenu> menus = new List<ActionMenu>();
            if (!string.IsNullOrEmpty(HeroInfo.Menu1))
            {
                List<Ability> menuAbilities = new List<Ability>();
                if (!string.IsNullOrEmpty(HeroInfo.Ability3))
                    menuAbilities.Add(CreateAbilityFromData(HeroInfo.Ability3));
                if (!string.IsNullOrEmpty(HeroInfo.Ability4))
                    menuAbilities.Add(CreateAbilityFromData(HeroInfo.Ability4));


                ActionMenu menu = new ActionMenu(HeroInfo.Menu1, AbilityType.Summon, menuAbilities);
                menus.Add(menu);
            }
            heroGO.GetComponent<Hero>().SetMenus(menus);

            // Set monster HP
            heroGO.GetComponent<Hero>().SetMaxHP(HeroInfo.MaxHP);
            heroGO.GetComponent<Hero>().SetHP(HeroInfo.CurrentHP); // TODO... because of some strange ordering, if this isnt set here the UI at start doesn't update with correct HP

            // Trigger Unit Spawn Event Callback
            EventCallbacks.UnitSpawnEventInfo usei = new EventCallbacks.UnitSpawnEventInfo();
            usei.EventDescription = "Unit " + heroGO.name + " has spawned.";
            usei.UnitGO = heroGO;
            usei.UnitSlotGO = unitSlot;
            usei.FireEvent();

            return heroGO;
        }

        public GameObject SpawnMonster(int index, MonsterInfo mi, TeamName team, GameObject teamGroup, GameObject unitSlot)
        {
            // TODO... the index will not be needed once all monster spawns are driven by player and map driven teams
            MonsterInfo monsterInfo;
            if (index == 0)
            {
                monsterInfo = mi;
            }
            else
            {
                // Get monster data from index
                monsterInfo = monsterData.GetMonsterFromIndex(index);
            }
            // Keep track of count of each monster type in this fight
            TrackCharacterCount(monsterInfo, team);

            GameObject monsterGO = GameObject.Instantiate(monsterTemplateGO, unitSlot.transform.position, Quaternion.identity, teamGroup.transform) as GameObject;
            monsterGO.name = monsterInfo.MonsterName + " " + monsterCounts[team + monsterInfo.MonsterName];
            if (team == TeamName.Friendly)
                monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.FriendlySpriteName]);
            else if (team == TeamName.Enemy)
                monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.EnemySpriteName]);
            else
                Debug.Log("TEAM name not correct!!!!");
            monsterGO.GetComponent<Monster>().SetTeam(team);
            // Set Monster's ability
            List<Ability> abilities = new List<Ability>();
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
            monsterGO.GetComponent<Monster>().SetHP(monsterInfo.CurrentHP); // TODO... because of some strange ordering, if this isnt set here the UI at start doesn't update with correct HP

            // Trigger Unit Spawn Event Callback
            EventCallbacks.UnitSpawnEventInfo usei = new EventCallbacks.UnitSpawnEventInfo();
            usei.EventDescription = "Unit " + monsterGO.name + " has spawned.";
            usei.UnitGO = monsterGO;
            usei.UnitSlotGO = unitSlot;
            usei.FireEvent();

            return monsterGO;
        }

        Ability CreateAbilityFromData(string abilityName)
        {
            AbilityInfo abilityInfo = abilityData.GetAbilityByName(abilityName);

            switch (abilityInfo.abilityType)
            {
                case AbilityType.Attack:
                    Attack attack = new Attack(abilityInfo.Name, abilityInfo.AbilityCD, abilityInfo.BaseAbilityStrength);
                    attack.SetAbilityType(abilityInfo.abilityType);
                    return attack;
                case AbilityType.Summon:
                    Summon summon;
                    if (abilityInfo.summonIndex == 0)
                        summon = new Summon(abilityInfo.Name, abilityInfo.AbilityCD, abilityInfo.summonIndex);
                    else
                        summon = new Summon(abilityInfo.Name, abilityInfo.AbilityCD, abilityInfo.summonIndex);
                    summon.SetAbilityType(abilityInfo.abilityType);
                    return summon;
                case AbilityType.Support:
                    // TODO... Support not implemented yet - temp returning attack
                    Attack support = new Attack(abilityInfo.Name, abilityInfo.AbilityCD, abilityInfo.BaseAbilityStrength);
                    support.SetAbilityType(abilityInfo.abilityType);
                    return support;
            }

            // TODO... should never be allowed to happen, look inot more robust error handling
            return null;
        }

        void TrackCharacterCount(MonsterInfo monsterInfo, TeamName team)
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
}
