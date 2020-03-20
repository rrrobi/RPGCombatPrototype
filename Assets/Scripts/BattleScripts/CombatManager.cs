using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using EventCallbacks;

namespace Battle
{
    public class CombatManager : MonoBehaviour
    {
        // Battle loot
        int battleLoot;

        public static CombatManager Instance { get; protected set; }
        MonsterSpawner monsterSpawner = new MonsterSpawner();
        public BattleUIController battleUIController = new BattleUIController();
        public BattlefieldController battlefieldController = new BattlefieldController();
        public AnimationController animationController = new AnimationController();

        // Character lists, and function to maintain these lists
        Dictionary<string, GameObject> playerCharacters;
        public Dictionary<string, GameObject> GetPlayerCharacterList { get { return playerCharacters; } }
        public GameObject GetPlayerCharacterByName(string name) { return playerCharacters[name]; }
        private void AddToPlayerCharacterList(GameObject character)
        {
            playerCharacters.Add(character.name, character);
        }
        private void RemoveFromPlayerCharacterList(GameObject character)
        {
            playerCharacters.Remove(character.name);
        }

        Dictionary<string, Global.MonsterInfo> playerMonsterinfoList;
        public Dictionary<string, Global.MonsterInfo> GetPlayerMonsterInfoList { get { return playerMonsterinfoList; } }
        public Global.MonsterInfo GetPlayerMonsterInfoByName(string name) { return playerMonsterinfoList[name]; }
        private void AddToPlayerMonsterInfoList(Global.MonsterInfo mi) { playerMonsterinfoList.Add(mi.UniqueID, mi); }
        private void RemoveFromPlayerMonsterInfoList(Global.MonsterInfo mi)
        {
            if (playerMonsterinfoList.ContainsKey(mi.UniqueID))
                playerMonsterinfoList.Remove(mi.UniqueID);
        }

        Dictionary<string, GameObject> enemyCharacters;
        public Dictionary<string, GameObject> GetEnemyCharacterList { get { return enemyCharacters; } }
        public GameObject GetEnemyCharacterByID(string id)
        {
            return enemyCharacters[id];
        }
        private void AddToEnemyCharacterList(GameObject character)
        {
            enemyCharacters.Add(character.GetComponent<Monster>().GetUniqueID, character);
        }
        private void RemoveFromEnemyCharacterList(GameObject character)
        {
            enemyCharacters.Remove(character.GetComponent<Monster>().GetUniqueID);//.name);
        }

        // Monster Setup Objects
        [SerializeField]
        private GameObject FriendlyTeamGO;
        [SerializeField]
        private GameObject EnemyTeamGO;

        // Battle Objects - unused
        Queue<GameObject> actionQueue = new Queue<GameObject>();

        void OnEnable()
        {
            if (Instance != null)
                Debug.LogError("There should never be more than one CombatManager.");
            Instance = this;
        }

        void Awake()
        {
            Assert.IsNotNull(FriendlyTeamGO);
            Assert.IsNotNull(EnemyTeamGO);
        }

        // Use this for initialization
        void Start()
        {
            // Ensure the Combat manager knows what loot is at stake in this battle.
            battleLoot = GameManager.Instance.GetCacheBattleBounty;

            // Ititial setup of battlefield
            battlefieldController.Setup();
            // Initial setup of UI controller
            battleUIController.Setup();
            // Initial Setup of MonsterSpawner
            monsterSpawner.Setup();

            playerCharacters = new Dictionary<string, GameObject>();
            playerMonsterinfoList = new Dictionary<string, Global.MonsterInfo>();
            enemyCharacters = new Dictionary<string, GameObject>();
            // Add all monsters from player's party to 'PlayerCharacters'
            foreach (var kvp in GameManager.Instance.GetPlayerMonsterParty)
            {
                AddToPlayerMonsterInfoList(kvp.Value);
            }

            // Spawn Enemy monsters
            AddEnemyMonsters();

            // Spawn Players Monsters
            AddPlayerMonsters();

            // Register Listeners
            RegisterEventCallbacks();
        }

        void RegisterEventCallbacks()
        {
            DeathEventInfo.RegisterListener(OnUnitDied);
            HPChangedEventInfo.RegisterListener(OnHPChange);
            UnitSpawnEventInfo.RegisterListener(OnUnitSpawn);
        }
        
        void UnregisterEventCallbacks()
        {
            DeathEventInfo.UnregisterListener(OnUnitDied);
            HPChangedEventInfo.UnregisterListener(OnHPChange);
            UnitSpawnEventInfo.UnregisterListener(OnUnitSpawn);
        }

        void AddPlayerMonsters()
        {
            // get number of Monster slots available
            int availableSlotCount = battlefieldController.FindUnoccupiedFriendlySlotCount();

            // get number of monsters on this team
            // Check theres room for all of them
            // spawn each monster, unless there are more monsters than available slots.
            int numToSpawn = availableSlotCount;
            List<Global.MonsterInfo> activeDemons = new List<Global.MonsterInfo>();
            foreach (var kvp in playerMonsterinfoList)
            {
                if (kvp.Value.IsSummoned)
                    activeDemons.Add(kvp.Value);
            }

            if (activeDemons.Count <= availableSlotCount)
                numToSpawn = activeDemons.Count;

            // add hero
            GameObject heroGO = monsterSpawner.SpawnHero(TeamName.Friendly,
                FriendlyTeamGO,
                battlefieldController.GetFriendlySlot(1, 1));
            // add to PlayerCharacterList
            AddToPlayerCharacterList(heroGO);

            // for each active Monster (with a free slot available to be summoned
            for (int i = 0; i < numToSpawn; i++)
            {
                GameObject nextAvailableSlot = battlefieldController.FindNextUnoccupiedFriendlySlot();
                if (nextAvailableSlot != null)
                {
                    int randIndex = Random.Range(1, 4);
                    AddSummonedPlayerMonster(activeDemons[i], nextAvailableSlot);
                }
            }            
        }

        // TODO... This may longer be required - Index is no longer used for Player summon abilities
        public void AddSummonedPlayerMonster(int index, GameObject unitSlot)
        {
            // TODO... temp, remove this - may be better setting mi in 'spawnMonster' to a default argument
            Global.MonsterInfo mi = new Global.MonsterInfo();

            GameObject monsterGO = monsterSpawner.SpawnMonster(index,
                        mi,
                        TeamName.Friendly,
                        FriendlyTeamGO,
                        unitSlot);
            // add to PlayerCharacterList
            AddToPlayerCharacterList(monsterGO);
        }

        public void AddSummonedPlayerMonster(Global.MonsterInfo mi, GameObject unitSlot)
        {
            GameObject monsterGO = monsterSpawner.SpawnMonster(0,
                        mi,
                        TeamName.Friendly,
                        FriendlyTeamGO,
                        unitSlot);
            // add to PlayerCharacterList
            AddToPlayerCharacterList(monsterGO);
        }

        void AddEnemyMonsters()
        {
            // get number of Monster slots available
            int availableSlotCount = battlefieldController.FindUnoccupiedEnemySlotCount();

            List<Global.MonsterInfo> enemyMonsterParty = GameManager.Instance.GetEnemyMonsterParty;
            // spawn each monster, unless there are more monsters than available slots.
            int numToSpawn = availableSlotCount;
            if (enemyMonsterParty.Count <= availableSlotCount)
                numToSpawn = enemyMonsterParty.Count;

            //int numToSpawn = availableSlotCount;
            //if (EnemyMonstersNum <= availableSlotCount)
            //    numToSpawn = EnemyMonstersNum;

            for (int i = 0; i < numToSpawn; i++)
            {
                GameObject nextAvailableSlot = battlefieldController.FindNextUnoccupiedEnemySlot();
                if (nextAvailableSlot != null)
                {
                    //int randIndex = Random.Range(1, 4);
                    GameObject monsterGO = monsterSpawner.SpawnMonster(0,
                        enemyMonsterParty[i],
                        TeamName.Enemy,
                        EnemyTeamGO,
                        nextAvailableSlot);
                    // Add to EnemyCharacterList
                    AddToEnemyCharacterList(monsterGO);
                }
            }
        }

        public void AddToActionQueue(GameObject monster)
        {
            if (!actionQueue.Contains(monster))
            {
                actionQueue.Enqueue(monster);

                // Temp
                Debug.Log(monster.name + " joined the action queue");
            }
        }

        public GameObject TakeFromActionQueue()
        {
            return actionQueue.Dequeue();
        }

        public void ClearEventListentersOnSceneClosure()
        {
            UnregisterEventCallbacks();
            battlefieldController.UnregisterEventCallbacks();
            battleUIController.UnregisterEventCallbacks();
        }

        public void PassChangedStatsToGM()
        {
            // TODO... i dont like how this info is passed back - rework
        
            // Update Stats for persistance in future battles
            // Update Hero HP            
            int hp = playerCharacters[GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo.PlayerName].GetComponent<Hero>().GetHP;
            GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo.CurrentHP = hp;
            GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo.GoldOwned += battleLoot;

            // update Monster stats
            foreach (var kvp in playerMonsterinfoList)
            {
                GameManager.Instance.AddToPlayerMonsterParty(kvp.Value);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // loop through ALL characters
            int attackDelay = 1000;
            foreach (var character in playerCharacters)
            {
                //if (character.Value.at)
            }
            // find the character with the lowest Attack delay
            // This charcacter takes its turn
            // subtracte this characters attack delay from all other character's attack delay

        }

        // may move this
        #region EventCallbacks

        void OnUnitDied(DeathEventInfo deathEventInfo)
        {
            Debug.Log("CombatManager Alerted to Character Death: " + deathEventInfo.UnitGO.name);

            if (deathEventInfo.TeamName == TeamName.Friendly)
            {
                // Dead character is freindly
                // Update Dictionary of player charcters                
                RemoveFromPlayerCharacterList(deathEventInfo.UnitGO);
                // Update Characater status
                if (playerMonsterinfoList.ContainsKey(deathEventInfo.UnitGO.name))
                {
                    playerMonsterinfoList[deathEventInfo.UnitGO.name].IsSummoned = false;
                    playerMonsterinfoList[deathEventInfo.UnitGO.name].IsDead = true;
                }
            }
            else if (deathEventInfo.TeamName == TeamName.Enemy)
            {
                // Dead charcter in an enemy
                // Update Dictionary of enemy charcters
                RemoveFromEnemyCharacterList(deathEventInfo.UnitGO);
            }

            // Remove object
            Destroy(deathEventInfo.UnitGO);
        }

        // This updates the player's MonsterList info with the new HP for the monsters
        void OnHPChange(HPChangedEventInfo hpChangedEventInfo)
        {
            Debug.Log("CombatManager Alerted to Character HP Change: " + hpChangedEventInfo.UnitGO.name);

            // We only care about freindly monsters at this point
            if (hpChangedEventInfo.UnitGO.GetComponent<Character>().GetTeam == TeamName.Friendly)
            {
                string uID = hpChangedEventInfo.UnitGO.GetComponent<Character>().GetUniqueID;
                // Also, we only care about monsters in our playerMonsterInfoList (i.e NOT the hero) <- this is because the hero damage is tracked differently.
                if (playerMonsterinfoList.ContainsKey(uID))
                {
                    playerMonsterinfoList[hpChangedEventInfo.UnitGO.GetComponent<Character>().GetUniqueID].CurrentHP = hpChangedEventInfo.UnitGO.GetComponent<Character>().GetHP;

                    // Update Summon Menu
                    ActionMenu menu = monsterSpawner.PopulateHeroSummonMenu(GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo);
                    playerCharacters[GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo.PlayerName].GetComponent<Hero>().SetMenu(menu);
                }
            }
        }

        void OnUnitSpawn(UnitSpawnEventInfo unitSpawnEventInfo)
        {
            Debug.Log("CombatManager Alerted to unit Spawned: " + unitSpawnEventInfo.UnitGO.name);

            // We only care about freindly monsters at this point
            if (unitSpawnEventInfo.UnitGO.GetComponent<Character>().GetTeam == TeamName.Friendly)
            {
                // Also, we only care about monsters in out playerMonsterInfoList (i.e NOT the hero) <- this is because the hero can not be summoned
                if (playerMonsterinfoList.ContainsKey(unitSpawnEventInfo.UnitGO.GetComponent<Character>().GetUniqueID))
                {
                    playerMonsterinfoList[unitSpawnEventInfo.UnitGO.GetComponent<Character>().GetUniqueID].IsSummoned = true;

                    // Update Summon Menu
                    ActionMenu menu = monsterSpawner.PopulateHeroSummonMenu(GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo);
                    playerCharacters[GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo.PlayerName].GetComponent<Hero>().SetMenu(menu);
                }
            }
        }


        #endregion
    }
}
