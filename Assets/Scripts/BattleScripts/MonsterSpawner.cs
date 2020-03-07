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
        AbilityDataReader abilityData = new AbilityDataReader();

        Dictionary<string, int> monsterCounts = new Dictionary<string, int>();
        Dictionary<string, List<string>> UniqueIDLog = new Dictionary<string, List<string>>();

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

            abilityData.SetUp();
            abilityData.ReadData();
        }

        public GameObject SpawnHero(TeamName team, GameObject teamGroup, GameObject unitSlot)
        {
            // Get monster data from index
            HeroInfo heroInfo = GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo;

            GameObject heroGO = GameObject.Instantiate(heroTemplateGO, unitSlot.transform.position, Quaternion.identity, teamGroup.transform) as GameObject;
            heroGO.name = heroInfo.PlayerName;
            if (team == TeamName.Friendly)
                heroGO.GetComponent<Hero>().SetMonsterSprite(monsterSprites[heroInfo.FriendlySpriteName]);
            else
                Debug.Log("TEAM name not correct!!!!");
            heroGO.GetComponent<Hero>().SetTeam(team);
            // Set Monster's ability
            List<Ability> abilities = new List<Ability>();
            if (heroInfo.baseActions.Abilities.Count > 0)
            {
                foreach (var action in heroInfo.baseActions.Abilities)
                {
                    abilities.Add(CreateAbilityFromData(action));
                }
            }
            else
            {
                Debug.LogError("No Base abilities set for the hero, something may have gone wrong!");
            }
            heroGO.GetComponent<Hero>().SetMonsterAbilities(abilities);

            #region Set up Hero's Menus
            List<ActionMenu> menus = new List<ActionMenu>();
            // Summon Menu - Should only contain 1 ability
            if (heroInfo.SummonActions.Abilities.Count == 1)
            {
                menus.Add(PopulateHeroSummonMenu(heroInfo));
            }
            // Item Menu
            if (heroInfo.ItemActions.Abilities.Count > 0)
            {
                List<Ability> menuAbilities = new List<Ability>();
                foreach (var ability in heroInfo.ItemActions.Abilities)
                {
                    menuAbilities.Add(CreateAbilityFromData(ability));
                }
                ActionMenu ItemMenu = new ActionMenu(heroInfo.ItemActions.Name, menuAbilities);
                menus.Add(ItemMenu);
            }
            // Spells Menu
            if (heroInfo.SpellActions.Abilities.Count > 0)
            {
                List<Ability> menuAbilities = new List<Ability>();
                foreach (var ability in heroInfo.SpellActions.Abilities)
                {
                    menuAbilities.Add(CreateAbilityFromData(ability));
                }
                ActionMenu spellMenu = new ActionMenu(heroInfo.SpellActions.Name, menuAbilities);
                menus.Add(spellMenu);
            }
            // TODO... add this
            heroGO.GetComponent<Hero>().SetMenus(menus);
            #endregion

            // Set monster HP
            heroGO.GetComponent<Hero>().SetMaxHP(heroInfo.MaxHP);
            heroGO.GetComponent<Hero>().SetHP(heroInfo.CurrentHP); // Must be set here or the UI at start doesn't update with correct HP
            // Set Character Unique ID
            heroGO.GetComponent<Hero>().SetUniqueID(heroInfo.UniqueID);

            // Trigger Unit Spawn Event Callback
            EventCallbacks.UnitSpawnEventInfo usei = new EventCallbacks.UnitSpawnEventInfo();
            usei.EventDescription = "Unit " + heroGO.name + " has spawned.";
            usei.UnitGO = heroGO;
            usei.UnitSlotGO = unitSlot;
            usei.FireEvent();

            return heroGO;
        }

        public ActionMenu PopulateHeroSummonMenu(HeroInfo heroInfo)
        {
            List<Ability> menuAbilities = new List<Ability>();
            foreach (var monster in heroInfo.PlayerDemons)
            {
                if (!monster.IsDead && !monster.IsSummoned)
                    menuAbilities.Add(CreateAbilityFromData(heroInfo.SummonActions.Abilities[0], monster));
            }
            ActionMenu summonMenu = new ActionMenu(heroInfo.SummonActions.Name, menuAbilities);

            return summonMenu;
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
                monsterInfo = MonsterDataReader.GetMonsterFromIndex(index);
            }
            // Keep track of count of each monster type in this fight
            TrackCharacterCount(monsterInfo, team);

            GameObject monsterGO = GameObject.Instantiate(monsterTemplateGO, unitSlot.transform.position, Quaternion.identity, teamGroup.transform) as GameObject;
            monsterGO.name = monsterInfo.MonsterName;// + " " + monsterCounts[team + monsterInfo.MonsterName];
            if (team == TeamName.Friendly)
            {
                monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.FriendlySpriteName]);
                monsterGO.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("BattleResources\\Animations\\Default_Animations\\Monster") as RuntimeAnimatorController;
            }
            else if (team == TeamName.Enemy)
            {
                monsterGO.GetComponent<Monster>().SetMonsterSprite(monsterSprites[monsterInfo.EnemySpriteName]);
                monsterGO.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("BattleResources\\Animations\\Default_Animations\\Enemy\\EnemyMonster") as RuntimeAnimatorController;
            }
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
            monsterGO.GetComponent<Monster>().SetHP(monsterInfo.CurrentHP); // Must be set here or the UI at start doesn't update with correct HP
            // Set Character Unique ID
            monsterGO.GetComponent<Monster>().SetUniqueID(AssignUniqueID(monsterInfo, team));

            // Trigger Unit Spawn Event Callback
            EventCallbacks.UnitSpawnEventInfo usei = new EventCallbacks.UnitSpawnEventInfo();
            usei.EventDescription = "Unit " + monsterGO.name + " has spawned.";
            usei.UnitGO = monsterGO;
            usei.UnitSlotGO = unitSlot;
            usei.FireEvent();

            return monsterGO;
        }

        Ability CreateAbilityFromData(string abilityName, MonsterInfo mi = null)
        {
            AbilityInfo abilityInfo = abilityData.GetAbilityByName(abilityName);

            string givenName = abilityInfo.Name;
            if (mi != null)
                givenName = mi.MonsterName;
            Ability ability = new Ability(givenName, abilityInfo.AbilityCD);
            ability.SetTargetType(abilityInfo.targetType);
            ability.SetAbilityEffectType(abilityInfo.abilityEffectType);
            foreach (var effect in abilityInfo.abilityEffects)
            {
                AbilityEffect abilityEffect = new AbilityEffect()
                {
                    abilityType = effect.abilityType,
                    damageType = effect.damageType,
                    BaseAbilityStrength = effect.BaseAbilityStrength
                };
                if (mi != null)
                    abilityEffect.monsterInfo = mi;
                ability.AddToEffectList(abilityEffect);
            }
            return ability;

            // TODO... should never be allowed to happen, look inot more robust error handling
            return null;
        }

        string AssignUniqueID(MonsterInfo mi, TeamName team)
        {
            string teamID = string.Empty;
            string unitID = string.Empty;
            string fullUniqueID = string.Empty;
            switch(team)
            {
                case TeamName.Friendly:
                    teamID = "01";
                    break;
                case TeamName.Enemy:
                    teamID = "02";
                    break;
                default:
                    Debug.LogError("Something has gone wrong here, Team name is wrong");
                    break;
            }
            // Ensure Log contains a list for this Team
            if (!UniqueIDLog.ContainsKey(teamID))
                UniqueIDLog.Add(teamID, new List<string>());

            // Check if MonsterID already has Unique ID (all friendlies will have one pre-set)
            if (mi.UniqueID.StartsWith(teamID))
                fullUniqueID = mi.UniqueID;
            // Else, calculate new ID
            else
            {
                unitID = (UniqueIDLog[teamID].Count + 1).ToString("000");
                fullUniqueID = teamID + "-" + unitID;
            }
            // Add to dictionary, 
            if (!UniqueIDLog[teamID].Contains(fullUniqueID))
                UniqueIDLog[teamID].Add(fullUniqueID);
            // add Unique ID to monster's GO
            return fullUniqueID;
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
