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
        // For use in onther areas to ensure all completed before trying to use the character
        private bool IsSetupComplete = false;
        public bool GetIsSetupComplete { get { return IsSetupComplete; } }


        string uniqueID;
        public string GetUniqueID { get { return uniqueID; } }
        public void SetUniqueID(string ID) { uniqueID = ID; }

        protected float abilityDelay;
        public float GetAbilityDelay() { return abilityDelay; }
        public void SetAbilityDelay(float AD) { abilityDelay = AD; }

        protected bool isReady = false;
        public void SetIsReady(bool ready) { isReady = ready; }
        public bool GetIsReady { get { return isReady; } }

        [SerializeField]
        protected GameObject SpeedBarGO;

        #region Character Stats variables
        protected int hP;
        public void SetHP(int hp) { hP = hp; }
        public int GetHP { get { return hP; } }
        protected int maxHP;
        public void SetMaxHP(int hp) { maxHP = hp; }
        public int GetMaxHP { get { return maxHP; } }

        protected int mP;
        public void SetMP(int mp) { mP = mp; }
        public int GetMP { get { return mP; } }
        protected int maxMP;
        public void SetMaxMP(int mp) { maxMP = mp; }
        public int GetMaxMP { get { return maxMP; } }

        protected int strengthModifer;
        public void SetStrengthModifer(int str) { strengthModifer = str; }
        public int GetStrengthModifer { get { return strengthModifer; } }
        protected int willModifer;
        public void SetWillModifer(int will) { willModifer = will; }
        public int GetWillModifer { get { return willModifer; } }
        protected int agilityModifer;
        public void SetAgilityModifer(int agi) { agilityModifer = agi; }
        public int GetAgilityModifer { get { return agilityModifer; } }
        #endregion

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
        protected Dictionary<string, ActionMenu> Menus = new Dictionary<string, ActionMenu>();
        public Dictionary<string, ActionMenu> GetMenus { get { return Menus; } }
        public ActionMenu GetMenuByName(string name) { return Menus[name]; }
        public void SetMenu(ActionMenu menu)
        {
            if (Menus.ContainsKey(menu.GetMenuName))
            {
                Menus[menu.GetMenuName] = menu;
            }
            else
                Menus.Add(menu.GetMenuName, menu);
        }
        public void SetMenus(List<ActionMenu> menus)
        {            
            for (int i = 0; i < menus.Count; i++)
            {
                // If Menu already exists - replace with new
                if (Menus.ContainsKey(menus[i].GetMenuName))
                    Menus[menus[i].GetMenuName] = menus[i];
                // Else add new menu to list
                else
                    Menus.Add(menus[i].GetMenuName, menus[i]);
            }
        }
        /// <summary>
        /// seach ability dictionary and list of menus for ability by name
        /// </summary>
        public Ability SearchAllAbilitiesByName(string name)
        {
            if (Abilities.ContainsKey(name))
                return Abilities[name];
            
            foreach (var menu in Menus)
            {
                foreach (var ability in menu.Value.GetActionList)
                {
                    if (ability.GetAbilityName == name)
                        return ability;
                }
            }

            Debug.LogError("Ability name not found in Ability dictionary  OR ANY of the menus: " + name);
            return null;
        }

        protected TeamName team;
        public void SetTeam(TeamName teamName) { team = teamName; }
        public TeamName GetTeam { get { return team; } }
        protected GameObject myTeam;
        protected GameObject enemyTeam;

        protected GameObject UnitSlot;
        public void SetUnitSlot(GameObject unitSlot) { UnitSlot = unitSlot; }
        public GameObject GetUnitSlot() { return UnitSlot; }

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

            abilityDelay = Random.Range(3, 10);  // TODO... - will be set from monster Info

            // Set this monster's Sprite
            AssignSprite();
            // Set monster's colider, (Make it clickable)
            CircleCollider2D col = this.gameObject.AddComponent<CircleCollider2D>();
            col.radius = 1.0f;
            // Set unclickable to start
            MakeUnclickable();

            // Discove whcih teams are friend or foe
            DiscoverTeams();

            // Confirm Setup is complete
            IsSetupComplete = true;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            //attackTimer -= Time.deltaTime;
            //if (attackTimer <= 0)
            //{
            //    TakeTurn();
            //}
            //else
            //    ScaleSpeedBar();
        }

        public void MakeGlowingClickable()
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            // Set Sprite Shader to Glow
            this.gameObject.GetComponentInChildren<SpriteRenderer>().material = new Material(Shader.Find("Custom/Sprite Glow"));
        }

        public void MakeClickable()
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            // Set Sprite Shader to Highlight
            this.gameObject.GetComponentInChildren<SpriteRenderer>().material = new Material(Shader.Find("Custom/Sprite Outline"));
        }

        public void MakeUnclickable()
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            // Reset Sprite Shader to Dprite Default (not Glowing)
            this.gameObject.GetComponentInChildren<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        }

        protected void AssignSprite()
        {
            SpriteRenderer MonsterSprite = this.gameObject.GetComponentInChildren<SpriteRenderer>(); //this.gameObject.AddComponent<SpriteRenderer>();
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

        // TODO....
        // Split AI/Player attack code
        public void TakeTurn()
        {
            if (team == TeamName.Enemy)
                Attack();
            else
            {
                // Trigger Attacked Event callback
                EventCallbacks.CharacterReadyEventInfo crei = new EventCallbacks.CharacterReadyEventInfo();
                crei.EventDescription = $"Unit {gameObject.name} Is ready to take it's turn.";
                crei.UnitGO = gameObject;
                crei.FireEvent();


                //isReady = true;
                //// TODO... im not sure about how this works, in regards to updating the Click-a-bility in more than one place, 
                //// should this be passed to UIController?
                //// If so BattleUIController may not need to be public
                //if (CombatManager.Instance.battleUIController.GetAbilityState == BattleUIController.AbilityState.CharcterSelect)
                //    MakeClickable();

                //CombatManager.Instance.AddToActionQueue(this.gameObject);
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

                Abilities.ElementAt(abilityIndex).Value.NewAction(this.gameObject, mobList[targetIndex]);
                //UseAbilityOn(Abilities.ElementAt(abilityIndex).Value, mobList[targetIndex]);
            }
        }

        public void UpdateAttackTimer(float cd)
        {
            abilityDelay = cd;
            //attackCD = cd;
            //attackTimer = attackCD;
        }

        public void TakeDamage(int damage)
        {
            Debug.Log(this.name + " got hit for " + damage);
            hP -= damage;

            // Trigger Attacked Event callback
            EventCallbacks.HPChangedEventInfo hpcei = new EventCallbacks.HPChangedEventInfo();
            hpcei.EventDescription = "Unit " + gameObject.name + " Has taken " + damage + " damage";
            hpcei.UnitGO = gameObject;
            hpcei.FireEvent();

            // check for death
            if (hP <= 0)
                CharacterDies();
        }

        public void RecoverHitPoints(int hitPoints)
        {
            Debug.Log(this.name + " got healed for " + hitPoints);

            hP = Mathf.Clamp(hP += hitPoints, 0, maxHP);

            // Trigger Attacked Event callback - 
            // TODO... Alter these events, split 'TakeDamage' into separate 'GetAttacked' and 'HealthChanged' events (currently only 'HealthChanged' would be used)
            EventCallbacks.HPChangedEventInfo hpcei = new EventCallbacks.HPChangedEventInfo();
            hpcei.EventDescription = "Unit " + gameObject.name + " Has recovered " + hP + " health";
            hpcei.UnitGO = gameObject;
            hpcei.FireEvent();

            // check for death
            if (hP <= 0)
                CharacterDies();
        }

        public void RemoveMana(int manaCost)
        {
            Debug.Log(this.name + " has used " + manaCost + " mana");
            mP -= manaCost;
            // Mana can't drop below 0
            mP = Mathf.Max(mP, 0);

            // Trigger Mana change Event callback
            EventCallbacks.MPChangedEventInfo mpcei = new EventCallbacks.MPChangedEventInfo();
            mpcei.EventDescription = $"Unit {gameObject.name} has had the mana cost({manaCost}) deducted from its mana pool.";
            mpcei.UnitGO = gameObject;
            mpcei.FireEvent();
        }

        public void RecoverMana(int mana)
        {
            Debug.Log(this.name + " has regained " + mana + " mana");
            mP += mana;
            // Mana can't go above the max
            mP = Mathf.Clamp(mP += mana, 0, maxMP);

            // Trigger Mana change Event callback
            EventCallbacks.MPChangedEventInfo mpcei = new EventCallbacks.MPChangedEventInfo();
            mpcei.EventDescription = $"Unit {gameObject.name} has regained {mana} point to it's mana pool.";
            mpcei.UnitGO = gameObject;
            mpcei.FireEvent();
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