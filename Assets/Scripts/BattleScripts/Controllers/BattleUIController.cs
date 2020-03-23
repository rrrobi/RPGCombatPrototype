using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using UnityEngine.UI;
using EventCallbacks;

namespace Battle
{
    public class BattleUIController
    {
        public enum AbilityState
        {
            CharcterSelect,
            ActionSelect,
            TargetSelect
        };
        AbilityState abilityState;
        public AbilityState GetAbilityState { get { return abilityState; } }

        GameObject FriendlyPanel;
        GameObject EnemyPanel;
        GameObject ActionPanel;
        GameObject MenuPanel;
        GameObject GameOverPanel;
        GameObject VictoryPanel;

        // for the GoBack button - not implemented yet
        GameObject ActivePanel;
        GameObject PreviousPanel;

        // UI templates
        GameObject monsterPanelTemplate;
        GameObject actionButtonTemplate;
        GameObject targetButtonTemplate;

        // Selected action - may move this to Combatmanager
        GameObject SelectedCharacter;
        //Attack SelectedAttack;
        Ability SelectedAbility;
        List<GameObject> SelectedTargets;

        // Use this for initialization
        public void Setup()
        {
         // Read in GO templates from Resources
            monsterPanelTemplate = Resources.Load("BattleResources/Prefabs/UI/MonsterPanel") as GameObject;
            actionButtonTemplate = Resources.Load("BattleResources/Prefabs/UI/ActionButton") as GameObject;
            targetButtonTemplate = Resources.Load("BattleResources/Prefabs/UI/TargetButton") as GameObject;

            // Get UI Panel Gameobjects to use later
            // TODO... ReThink
            // I HATE this
            FriendlyPanel = GameObject.Find("FriendlyPanel");
            EnemyPanel = GameObject.Find("EnemyPanel");
            ActionPanel = GameObject.Find("ActionPanel");
            MenuPanel = GameObject.Find("MenuPanel");
            GameOverPanel = GameObject.Find("GameOverPanel");
            VictoryPanel = GameObject.Find("VictoryPanel");
            ActionPanel.SetActive(false);
            MenuPanel.SetActive(false);
            GameOverPanel.SetActive(false);
            VictoryPanel.SetActive(false);

            RegisterEventCallbacks();
            // Starts battle in the character select state
            //abilityState = AbilityState.CharcterSelect;
            TransferAbilityState(AbilityState.CharcterSelect, AbilityState.CharcterSelect);
        }

        #region HP panels
        private void AddToHPPanel(GameObject character)
        {
            if (FriendlyPanel != null)
            {

                GameObject panelGO;
                // Instantiate panel
                if (character.GetComponent<Character>().GetTeam == TeamName.Friendly)
                    panelGO = GameObject.Instantiate(monsterPanelTemplate, Vector3.zero, Quaternion.identity, FriendlyPanel.transform) as GameObject;
                else
                    panelGO = GameObject.Instantiate(monsterPanelTemplate, Vector3.zero, Quaternion.identity, EnemyPanel.transform) as GameObject;
                // Set buttons varables
                panelGO.name = character.GetComponent<Character>().GetTeam.ToString() + "_" + character.GetComponent<Character>().GetUniqueID + "_Panel";
                Text[] panelTexts = panelGO.GetComponentsInChildren<Text>();
                foreach (var text in panelTexts)
                {
                    if (text.name == "NameText")
                    {
                        text.text = character.name;
                    }
                    if (text.name == "HPText")
                    {
                        text.text = "HP : " + character.GetComponent<Character>().GetHP + "/" + character.GetComponent<Character>().GetMaxHP;
                    }

                }

            }
            else
            {

            }
        }

        private void UpdateHPPanel(GameObject character)
        {
            // TODO.. Is there a better way of doing this, i dont like it
            GameObject panelGO = GameObject.Find(character.GetComponent<Character>().GetTeam.ToString() + "_" + character.GetComponent<Character>().GetUniqueID + "_Panel");
            Text[] texts = panelGO.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                if (text.name == "HPText")
                {
                    text.text = "HP : " + character.GetComponent<Character>().GetHP + "/" + character.GetComponent<Character>().GetMaxHP;
                }
            }
        }
        #endregion

        #region Target highlight Code

        void TargetHighlight()
        {
            // get ability type
            switch (SelectedAbility.GetTargetType())
            {
                // if attack - targets will be enemies
                case TargetType.Enemy:
                    // Get list of all Enemies currently in the battle
                    List<GameObject> enemyList = CombatManager.Instance.battlefieldController.FindAllCurrentEnemies();
                    SelectedTargets = enemyList;
                    // Loop through targets, adding them to the pannel
                    foreach (var enemy in enemyList)
                    {
                        enemy.GetComponent<Character>().MakeClickable();
                    }
                    break;
                // if support - targets will be Friendlies
                case TargetType.Friendly:
                    // Get list of all Friendlies currently in the battle
                    List<GameObject> friendlyList = CombatManager.Instance.battlefieldController.FindAllCurrentFriendlies();
                    SelectedTargets = friendlyList;
                    // Loop through targets, adding them to the pannel
                    foreach (var friend in friendlyList)
                    {
                        friend.GetComponent<Character>().MakeClickable();
                    }
                    break;
                // if summon - Targets will be unOccupied character slots on your side
                case TargetType.SummonSlot:
                    // Get List of Unoccupied frindly unitslots
                    List<GameObject> unitSlotList = CombatManager.Instance.battlefieldController.FindAllCurrentUnoccupiedFriendlySlots();
                    SelectedTargets = unitSlotList;
                    // Loop through targets, adding them to the pannel
                    foreach (var unitSlot in unitSlotList)
                    {
                        unitSlot.GetComponent<UnitSlot>().MakeClickable();
                    }
                    break;
                case TargetType.None:
                    // TODO... look into more robust error handling
                    Debug.Log("Not exactly sure what should happen here, this is probably an error!");
                    break;
                default:
                    // TODO... look into more robust error handling
                    Debug.Log("Not exactly sure what should happen here, this is probably an error!");
                    break;
            }
        }

        void RemoveTargetHighlight()
        {
            // Once ability has been done, remove highlight from targets 
            // and make them No longer clickable
            // Get ability type
            switch (SelectedAbility.GetTargetType())
            {
                // if attack - targets will be enemies
                case TargetType.Enemy:
                    // Loop through targets, adding them to the pannel
                    foreach (var enemy in SelectedTargets)
                    {
                        enemy.GetComponent<Character>().MakeUnclickable();
                    }
                    break;
                // if support - targets will be Friendlies
                case TargetType.Friendly:
                    // Loop through targets, adding them to the pannel
                    foreach (var friend in SelectedTargets)
                    {
                        friend.GetComponent<Character>().MakeUnclickable();
                    }
                    break;
                // if summon - Targets will be unOccupied character slots on your side
                case TargetType.SummonSlot:
                    // Loop through targets, adding them to the pannel
                    foreach (var unitSlot in SelectedTargets)
                    {
                        unitSlot.GetComponent<UnitSlot>().MakeUnclickable();
                    }
                    break;
                case TargetType.None:
                    // TODO... look into more robust error handling
                    Debug.Log("Not exactly sure what should happen here, this is probably an error!");
                    break;
                default:
                    // TODO... look into more robust error handling
                    Debug.Log("Not exactly sure what should happen here, this is probably an error!");
                    break;
            }
        }

        void CharacterSelectHighlight()
        {
            List<GameObject> friendlyList = CombatManager.Instance.battlefieldController.FindAllCurrentFriendlies();
            foreach (var friendly in friendlyList)
            {
                if (friendly.GetComponent<Character>().GetIsReady)
                    friendly.GetComponent<Character>().MakeClickable();
            }
        }

        void RemoveCharacterSelectHighlight()
        {
            List<GameObject> friendlyList = CombatManager.Instance.battlefieldController.FindAllCurrentFriendlies();
            foreach (var friendly in friendlyList)
            {
                friendly.GetComponent<Character>().MakeUnclickable();
            }
        }

        void TransferAbilityState(AbilityState from, AbilityState to)
        {
            // Handle 'From' Logic
            // Rules, for changes depending on what state the battle is moving FROM
            // e.g. if moving 'from' CharacterSelect, we stop highlighting the 'Ready' characters
            switch(from)
            {
                case AbilityState.CharcterSelect:
                    // remove highlight from friendly charcters
                    //RemoveCharacterSelectHighlight(); <- Not required to highlight anymore, might rename this state 
                    break;
                case AbilityState.ActionSelect:
                    break;
                case AbilityState.TargetSelect:
                    // remove highlight from targets
                    RemoveTargetHighlight();
                    break;
            }

            // Handle 'To' logic
            // Rules, for changes depending on what state the battle is moving TO
            // e.g. if moving 'to' CharacterSelect, we begin to highlight the 'Ready' characters
            switch (to)
            {
                case AbilityState.CharcterSelect:
                    // highlight all 'ready' friendly characters
                    //CharacterSelectHighlight(); <- Not required to highlight anymore, might rename this state 
                    abilityState = AbilityState.CharcterSelect;
                    break;
                case AbilityState.ActionSelect:
                    abilityState = AbilityState.ActionSelect;
                    break;
                case AbilityState.TargetSelect:
                    // highlight relevant Targets
                    TargetHighlight();
                    abilityState = AbilityState.TargetSelect;
                    break;
            }
        }

        #endregion

        #region Action Panel Code

        void ActionPanelSetup(GameObject character)
        {
            // Clear existing panel
            ClearActionPanel();

            // Get that characters abilities
            Dictionary<string, Ability> abilities = character.GetComponent<Character>().GetAbilities;

            // Add panel for each ability
            foreach (var kvp in abilities)
            {
                AddToActionPanel(kvp.Value);//.GetAbilityName);
            }

            // Get that characters Menus
            Dictionary<string, ActionMenu> menus = character.GetComponent<Character>().GetMenus;
            foreach (var kvp in menus)
            {
                AddMenuToActionPanel(kvp.Value);
            }
        }

        void AddToActionPanel(Ability ability)//string buttonName)
        {
            // Instantiate button
            GameObject buttonGO = GameObject.Instantiate(actionButtonTemplate, Vector3.zero, Quaternion.identity, ActionPanel.transform) as GameObject;
            // Set buttons varables
            buttonGO.name = ability.GetAbilityName + "_Button";//buttonName + "_Button";
            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            buttonText.text = ability.GetAbilityName;// buttonName;

            // Provide instructions for what each button does
            buttonGO.GetComponent<Button>().onClick.AddListener(ActionSelectbuttonPressed);
            
        }

        void AddMenuToActionPanel(ActionMenu menu)
        {
            // Instantiate button
            GameObject buttonGO = GameObject.Instantiate(actionButtonTemplate, Vector3.zero, Quaternion.identity, ActionPanel.transform) as GameObject;
            // Set buttons varables
            buttonGO.name = menu.GetMenuName + "_Button";//buttonName + "_Button";
            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            buttonText.text = menu.GetMenuName;

            buttonGO.GetComponent<Button>().onClick.AddListener(MenuSelectButtonPressed);
        }

        void ClearActionPanel()
        {
            foreach (Transform child in ActionPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        #endregion

        #region Menu Panel Code

        void MenuPanelSetup(Character character, ActionMenu menu)
        {
            // Clear existing MenuPanel
            ClearMenuPanel();

            // Add button for each Ability
            foreach (var ability in menu.GetActionList)
            {
                AddToMenuPanel(ability);
            }
        }

        void AddToMenuPanel(Ability ability)
        {
            // Instantiate button
            GameObject buttonGO = GameObject.Instantiate(actionButtonTemplate, Vector3.zero, Quaternion.identity, MenuPanel.transform) as GameObject;
            // Set buttons varables
            buttonGO.name = ability.GetAbilityName + "_Button";//buttonName + "_Button";
            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            buttonText.text = ability.GetAbilityName;// buttonName;

            // Provide instructions for what each button does
            buttonGO.GetComponent<Button>().onClick.AddListener(ActionSelectbuttonPressed);
        }

        void ClearMenuPanel()
        {
            foreach (Transform child in MenuPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        #endregion

        #region Button pressed Reactions

        public void ActionSelectbuttonPressed()
        {
            string buttonClicked = EventSystem.current.currentSelectedGameObject.name;
            Debug.Log(buttonClicked);
            buttonClicked = buttonClicked.Replace("_Button", "");

            // record which action has been chosen
            SelectedAbility = SelectedCharacter.GetComponent<Character>().SearchAllAbilitiesByName(buttonClicked);

            // deactivate action panel - And Menu panel (we might be using action from menu panel)
            ActionPanel.SetActive(false);
            MenuPanel.SetActive(false);

            TransferAbilityState(abilityState, AbilityState.TargetSelect);
        }

        public void MenuSelectButtonPressed()
        {
            string buttonClicked = EventSystem.current.currentSelectedGameObject.name;
            Debug.Log(buttonClicked);
            buttonClicked = buttonClicked.Replace("_Button", "");

            // record which action has been chosen
            //SelectedAbility = SelectedCharacter.GetComponent<Character>().GetAbilityByName(buttonClicked);

            // deactivate action panel
            ActionPanel.SetActive(false);
            MenuPanel.SetActive(true);

            // Find whcih menu button was pressed
            ActionMenu selectedMenu = SelectedCharacter.GetComponent<Character>().GetMenuByName(buttonClicked);

            // Populate Menu
            MenuPanelSetup(SelectedCharacter.GetComponent<Character>(), selectedMenu);
        }

        public void GameOverButtonPressed()
        {
            Debug.Log("Game over clicked!");
            CombatManager.Instance.ClearEventListentersOnSceneClosure();
            GameManager.Instance.GameOver();            
        }

        public void VictoryButtonPressed()
        {
            Debug.Log("Victory clicked!");
            CombatManager.Instance.ClearEventListentersOnSceneClosure();
            CombatManager.Instance.PassChangedStatsToGM();
            GameManager.Instance.ReturnToDungeon();
        }

        #endregion

        private void ToggleButtonInteractable(GameObject button, bool setTo)
        {
            button.GetComponent<Button>().interactable = setTo;
        }



        #region EventCallbacks

        void RegisterEventCallbacks()
        {
            SelectedObjectEventInfo.RegisterListener(OnHighlightSelected);
            CharacterReadyEventInfo.RegisterListener(OnCharacterReady);

            BattleWonEventInfo.RegisterListener(OnBattleWon);
            HPChangedEventInfo.RegisterListener(OnHPChange);
            HeroDeathEventInfo.RegisterListener(OnHeroDeath);
            DeathEventInfo.RegisterListener(OnUnitDied);
            UnitSpawnEventInfo.RegisterListener(OnUnitSpawn);
        }

        public void UnregisterEventCallbacks()
        {
            SelectedObjectEventInfo.UnregisterListener(OnHighlightSelected);
            CharacterReadyEventInfo.UnregisterListener(OnCharacterReady);

            BattleWonEventInfo.UnregisterListener(OnBattleWon);
            HPChangedEventInfo.UnregisterListener(OnHPChange);
            HeroDeathEventInfo.UnregisterListener(OnHeroDeath);
            DeathEventInfo.UnregisterListener(OnUnitDied);
            UnitSpawnEventInfo.UnregisterListener(OnUnitSpawn);
        }

        void OnCharacterReady(CharacterReadyEventInfo characterReadyEventInfo)
        {
            // record which freindly character has been selected
            SelectedCharacter = characterReadyEventInfo.UnitGO;

            // Create action panel for this character
            TransferAbilityState(abilityState, AbilityState.ActionSelect);
            ActionPanel.SetActive(true);
            ActionPanelSetup(SelectedCharacter);
        }

        void OnHighlightSelected(SelectedObjectEventInfo selectedEventInfo)
        {
            //if (abilityState == AbilityState.CharcterSelect)
            //{
            //    // record which freindly character has been selected
            //    SelectedCharacter = selectedEventInfo.UnitGO;

            //    // Create action panel for this character
            //    //abilityState = AbilityState.ActionSelect;
            //    TransferAbilityState(abilityState, AbilityState.ActionSelect);
            //    ActionPanel.SetActive(true);
            //    ActionPanelSetup(SelectedCharacter);
            //}

            if (abilityState == AbilityState.TargetSelect)
            {
                SelectedAbility.NewAction(SelectedCharacter,
                        selectedEventInfo.UnitGO);

                TransferAbilityState(abilityState, AbilityState.CharcterSelect);
            }
        }

        void OnBattleWon(BattleWonEventInfo battleWonEventInfo)
        {
            Debug.Log("BattleUIController Alerted to Hero Death!");

            // Close all existing windows
            ActionPanel.SetActive(false);
            MenuPanel.SetActive(false);
            // Open Victory Window
            VictoryPanel.SetActive(true);

            VictoryPanel.GetComponentInChildren<Button>().onClick.AddListener(VictoryButtonPressed);
        }        

        void OnHPChange(HPChangedEventInfo hpChangedEventInfo)
        {
            Debug.Log("BattleUIController Alerted to Character HP Change: " + hpChangedEventInfo.UnitGO.name);

            UpdateHPPanel(hpChangedEventInfo.UnitGO);
        }

        void OnUnitDied(DeathEventInfo deathEventInfo)
        {
            Debug.Log("BattleUIController Alerted to Character Death: " + deathEventInfo.UnitGO.name);
            // check if chacter that died is the currently selected character, 
            // if so, the 'abilityState' must return to character select
            if (deathEventInfo.UnitGO == SelectedCharacter)
                TransferAbilityState(abilityState, AbilityState.CharcterSelect);

            // Update UI
            // TODO... ReThink
            // I HATE this
            GameObject.Destroy(GameObject.Find(deathEventInfo.TeamName + "_" + deathEventInfo.UnitGO.GetComponent<Character>().GetUniqueID + "_Panel"));
        }

        void OnHeroDeath(HeroDeathEventInfo heroDeathEventInfo)
        {
            Debug.Log("BattleUIController Alerted to Hero Death!");

            // Close all existing windows
            ActionPanel.SetActive(false);
            MenuPanel.SetActive(false);
            // Open GameOver Window
            GameOverPanel.SetActive(true);

            GameOverPanel.GetComponentInChildren<Button>().onClick.AddListener(GameOverButtonPressed);
        }

        void OnUnitSpawn(UnitSpawnEventInfo unitSpawnEventInfo)
        {
            Debug.Log("BattleUIController Alerted to unit Spawned: " + unitSpawnEventInfo.UnitGO.name);
            AddToHPPanel(unitSpawnEventInfo.UnitGO);
        }

        #endregion
    }
}