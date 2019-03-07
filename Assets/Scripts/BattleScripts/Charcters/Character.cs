using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Battle
{
    public class Character : MonoBehaviour
    {

        // Will be changed to be Ability specific
        protected float attackCD = 10;
        protected float attackTimer;
        protected bool isReady = false;
        public void SetIsReady(bool ready) { isReady = ready; }
        public bool GetIsReady { get { return isReady; } }

        [SerializeField]
        protected GameObject SpeedBarGO;

        protected int hP;
        public void SetHP(int hp) { hP = hp; }
        public int GetHP { get { return hP; } }
        protected int maxHP;
        public void SetMaxHP(int hp) { maxHP = hp; }
        public int GetMaxHP { get { return maxHP; } }

        protected Dictionary<string, Ability> Abilities;
        public Dictionary<string, Ability> GetAbilities { get { return Abilities; } }
        public Ability GetAbilityByName(string name)
        {
            return Abilities[name];
        }
        public void SetMonsterAbilities(List<Ability> abilities)
        {
            Abilities = new Dictionary<string, Ability>();
            for (int i = 0; i < abilities.Count; i++)
            {
                Abilities.Add(abilities[i].GetAbilityName, abilities[i]);
            }
        }

        protected TeamName team;
        public void SetTeam(TeamName teamName) { team = teamName; }
        public TeamName GetTeam { get { return team; } }
        protected GameObject myTeam;
        protected GameObject enemyTeam;

        protected Sprite monsterSprite;
        public void SetMonsterSprite(Sprite sprite) { monsterSprite = sprite; }
        public Sprite GetMonsterSprite() { return monsterSprite; }

        void Awake()
        {
            Assert.IsNotNull(SpeedBarGO);
        }

        protected virtual void Start()
        {
            Debug.Log("Character, start method for: " + team.ToString() + "_" + this.name);

            //hP = maxHP;
            attackTimer = attackCD;

            // Set this monster's Sprite
            AssignSprite();
            // Set monster's colider, (Make it clickable)
            CircleCollider2D col = this.gameObject.AddComponent<CircleCollider2D>();
            col.radius = 1.0f;
            // Set unclickable to start
            MakeUnclickable();

            // Discove whcih teams are friend or foe
            DiscoverTeams();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                TakeTurn();
            }
            else
                ScaleSpeedBar();
        }

        public void MakeClickable()
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            // Set Sprite Shader to Glow
            this.gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Custom/Sprite Outline"));
        }

        public void MakeUnclickable()
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            // Reset Sprite Shader to Dprite Default (not Glowing)
            this.gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        }

        protected void AssignSprite()
        {
            SpriteRenderer MonsterSprite = this.gameObject.AddComponent<SpriteRenderer>();
            // Set Correct Monster sprite
            MonsterSprite.sprite = monsterSprite;
            MonsterSprite.sortingLayerName = "Characters";
           // MonsterSprite.material = Resources.Load("BattleResources/Materials/SpriteHighlightMaterial") as Material;
        }

        protected void DiscoverTeams()
        {
            // TODO... refactor i don't like how this decides which team its on, and how it selects its target
            // should use setup from Combat manager instead

            // get team this monster is on
            myTeam = this.gameObject.transform.parent.gameObject;
            // get opposing team
            switch (team)
            {
                case TeamName.Friendly:
                    enemyTeam = GameObject.FindGameObjectWithTag("EnemyTeam");
                    break;
                case TeamName.Enemy:
                    enemyTeam = GameObject.FindGameObjectWithTag("FriendlyTeam");
                    break;
                default:
                    enemyTeam = myTeam;
                    Debug.LogError("Issue with team tags, this shouldn't happen!");
                    break;
            }
        }

        protected void ScaleSpeedBar()
        {
            float timePast = attackCD - attackTimer;
            float scale = (timePast / attackCD);

            SpeedBarGO.transform.localScale = new Vector3(1.1f, scale, 1.0f);

            float barLength = 2;
            float offset = (barLength / 2) - (scale);
            SpeedBarGO.transform.localPosition = new Vector3(0.0f, offset, 0.0f);
        }

        // TODO....
        // Split AI/Player attack code
        protected void TakeTurn()
        {
            if (team == TeamName.Enemy)
                Attack();
            else
            {
                isReady = true;
                // TODO... im not sure about how this works, in regards to updating the Click-a-bility in more than one place, 
                // should this be passed to UIController?
                // If so BattleUIController may not need to be public
                if (CombatManager.Instance.battleUIController.GetAbilityState == BattleUIController.AbilityState.CharcterSelect)
                    MakeClickable();

                CombatManager.Instance.AddToActionQueue(this.gameObject);
            }
        }

        protected void Attack()
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

                Abilities.ElementAt(abilityIndex).Value.Action(this.gameObject, mobList[targetIndex]);
                //UseAbilityOn(Abilities.ElementAt(abilityIndex).Value, mobList[targetIndex]);
            }
        }

        public void UpdateAttackTimer(float cd)
        {
            attackCD = cd;
            attackTimer = attackCD;
        }

        public void TakeDamage(int damage)
        {
            Debug.Log(this.name + " got hit for " + damage);
            hP -= damage;

            // Trigger Attacked Event callback
            EventCallbacks.TakeDamageEventInfo tdei = new EventCallbacks.TakeDamageEventInfo();
            tdei.EventDescription = "Unit " + gameObject.name + " Has taken " + damage + " damage";
            tdei.Damage = damage;
            tdei.UnitGO = gameObject;
            tdei.FireEvent();

            // check for death
            if (hP <= 0)
                CharacterDies();
        }

        protected virtual void CharacterDies()
        {
            Debug.Log(this.name + " has died!");

            // Trigger Death Event callback
            EventCallbacks.DeathEventInfo dei = new EventCallbacks.DeathEventInfo();
            dei.EventDescription = "Unit " + gameObject.name + " has died.";
            dei.UnitGO = gameObject;
            dei.TeamName = team;
            dei.FireEvent();
        }
    }
}