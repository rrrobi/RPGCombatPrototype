using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using EventCallbacks;

namespace Battle
{
    public class CombatManager : MonoBehaviour
    {


        public static CombatManager Instance { get; protected set; }
        MonsterSpawner monsterSpawner = new MonsterSpawner();
        public BattleUIController battleUIController = new BattleUIController();
        public BattlefieldController battlefieldController = new BattlefieldController();

        // Character lists, and function to maintain these lists
        Dictionary<string, GameObject> playerCharacters;
        Dictionary<string, GameObject> enemyCharacters;

        public Dictionary<string, GameObject> GetPlayerCharacterList { get { return playerCharacters; } }
        public Dictionary<string, GameObject> GetEnemyCharacterList { get { return enemyCharacters; } }
        public GameObject GetPlayerCharacterByName(string name)
        {
            return playerCharacters[name];
        }
        public GameObject GetEnemyCharacterByName(string name)
        {
            return enemyCharacters[name];
        }
        private void AddToPlayerCharacterList(GameObject character)
        {
            playerCharacters.Add(character.name, character);
        }
        private void AddToEnemyCharacterList(GameObject character)
        {
            enemyCharacters.Add(character.name, character);
        }
        private void RemoveFromPlayerCharacterList(GameObject character)
        {
            playerCharacters.Remove(character.name);
        }
        private void RemoveFromEnemyCharacterList(GameObject character)
        {
            enemyCharacters.Remove(character.name);
        }

        // Monster Setup Objects
        [SerializeField]
        private int FriendlyMonstersNum;
        [SerializeField]
        private GameObject FriendlyTeamGO;
        [SerializeField]
        private int EnemyMonstersNum;
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
            EnemyMonstersNum = GameManager.Instance.GetNumOfEnemies;
            FriendlyMonstersNum = GameManager.Instance.GetNumOfFriendlies;

            // Ititial setup of battlefield
            battlefieldController.Setup();
            // Initial setup of UI controller
            battleUIController.Setup();
            // Initial Setup of MonsterSpawner
            monsterSpawner.Setup();

            playerCharacters = new Dictionary<string, GameObject>();
            enemyCharacters = new Dictionary<string, GameObject>();

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
        }
        
        void UnregisterEventCallbacks()
        {
            DeathEventInfo.UnregisterListener(OnUnitDied);
        }

        void AddPlayerMonsters()
        {
            // get number of Monster slots available
            int availableSlotCount = battlefieldController.FindUnoccupiedFriendlySlotCount();

            // get number of monsters on this team
            // Check theres room for all of them
            // spawn each monster, unless there are more monsters than available slots.
            int numToSpawn = availableSlotCount;
            List<Global.MonsterInfo> activeDemons = GameManager.Instance.GetPlayerActiveMonsters;
            if (activeDemons.Count <= availableSlotCount) //(FriendlyMonstersNum <= availableSlotCount)
                numToSpawn = activeDemons.Count;// FriendlyMonstersNum;

            // add hero
            GameObject heroGO = monsterSpawner.SpawnHero(TeamName.Friendly,
                FriendlyTeamGO,
                battlefieldController.GetFriendlySlot(1, 1));
            // add to PlayerCharacterList
            playerCharacters.Add(heroGO.name, heroGO);

            // for each Monster 
            for (int i = 0; i < numToSpawn; i++)
            {
                GameObject nextAvailableSlot = battlefieldController.FindNextUnoccupiedFriendlySlot();
                if (nextAvailableSlot != null)
                {

                    int randIndex = Random.Range(1, 4);
                    GameObject monsterGO = monsterSpawner.SpawnMonster(0,
                        activeDemons[i],
                        TeamName.Friendly,
                        FriendlyTeamGO,
                        nextAvailableSlot);
                    // add to PlayerCharacterList
                    playerCharacters.Add(monsterGO.name, monsterGO);
                }
            }

        }

        public void AddSummonedPlayerMonster(int index, GameObject unitSlot)
        {
            // TODO... temp, remove this
            Global.MonsterInfo mi = new Global.MonsterInfo();

            GameObject monsterGO = monsterSpawner.SpawnMonster(index,
                        mi,
                        TeamName.Friendly,
                        FriendlyTeamGO,
                        unitSlot);
            // add to PlayerCharacterList
            playerCharacters.Add(monsterGO.name, monsterGO);
        }

        void AddEnemyMonsters()
        {
            // get number of Monster slots available
            int availableSlotCount = battlefieldController.FindUnoccupiedEnemySlotCount();

            // spawn each monster, unless there are more monsters than available slots.
            int numToSpawn = availableSlotCount;
            if (EnemyMonstersNum <= availableSlotCount)
                numToSpawn = EnemyMonstersNum;

            for (int i = 0; i < numToSpawn; i++)
            {
                GameObject nextAvailableSlot = battlefieldController.FindNextUnoccupiedEnemySlot();
                if (nextAvailableSlot != null)
                {
                    // TODO... Temp, remove this
                    Global.MonsterInfo mi = new Global.MonsterInfo();

                    int randIndex = Random.Range(1, 4);
                    GameObject monsterGO = monsterSpawner.SpawnMonster(randIndex,
                        mi,
                        TeamName.Enemy,
                        EnemyTeamGO,
                        nextAvailableSlot);
                    // Add to EnemyCharacterList
                    enemyCharacters.Add(monsterGO.name, monsterGO);
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
        }

        // Update is called once per frame
        void Update()
        {

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

        #endregion
    }
}
