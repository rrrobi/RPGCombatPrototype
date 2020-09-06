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
            TargetSelect,
            ConfirmSelect
        };
        AbilityState abilityState;
        public AbilityState GetAbilityState { get { return abilityState; } }

        GameObject FriendlyPanel;
        GameObject EnemyPanel;
        GameObject ActionPanel;
        GameObject MenuPanel;
        GameObject BackPanel;
        GameObject GameOverPanel;
        GameObject VictoryPanel;
        const int MAX_CHAR_PANALS = 12;
        float[] CharPanel_XPosList = new float[MAX_CHAR_PANALS];

        // for the GoBack button - not implemented yet
        GameObject ActivePanel;
        GameObject PreviousPanel;

        // UI templates
        GameObject monsterPanelTemplate;
        GameObject actionButtonTemplate;
        GameObject targetButtonTemplate;

        // Selected action - may move this to Combatmanager
        GameObject SelectedCharacter;
        Ability SelectedAbility;
        // possible options for an ability to target
        List<GameObject> PossibleTargets;
        // Actual target selected for an ability
        GameObject SelectedTarget;

        // Turn tracking variables
        bool isMenuSetup = false;

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
            BackPanel = GameObject.Find("BackPanel");
            GameOverPanel = GameObject.Find("GameOverPanel");
            VictoryPanel = GameObject.Find("VictoryPanel");
            ActionPanel.SetActive(false);
            MenuPanel.SetActive(false);
            BackPanel.SetActive(false);
            GameOverPanel.SetActive(false);
            VictoryPanel.SetActive(false);

            BackPanelSetup();
            SetupPanelPositions();

            RegisterEventCallbacks();
            // Starts battle in the character select state
            //abilityState = AbilityState.CharcterSelect;
            TransferAbilityState(AbilityState.CharcterSelect, AbilityState.CharcterSelect);
        }


        private void SetupPanelPositions()
        {            
            float panelWidth = FriendlyPanel.GetComponent<RectTransform>().rect.width;
            float endPadding = 50;
            float interPanalGap = 75;

            for (int i = 0; i < CharPanel_XPosList.Length; i++)
            {

                CharPanel_XPosList[i] = -(panelWidth/2) + endPadding + (i * interPanalGap);

                //if (CharPanel_XPosList[i] < +(panelWidth / 2) - endPadding)
                //    Debug.LogError("Charcater panel spaceing has run too far to the right, and has gone off the page... look into this!");
            }
        }

        #region Character panels
        private void AddToCharacterPanel(GameObject character)
        {
            if (FriendlyPanel != null)
            {               
                GameObject panelGO;
                // Instantiate panel
                //if (character.GetComponent<Character>().GetTeam == TeamName.Friendly)
                //    panelGO = GameObject.Instantiate(monsterPanelTemplate, Vector3.zero, Quaternion.identity, FriendlyPanel.transform) as GameObject;
                //else
                //    panelGO = GameObject.Instantiate(monsterPanelTemplate, Vector3.zero, Quaternion.identity, EnemyPanel.transform) as GameObject;
                if (character.GetComponent<Character>().GetTeam == TeamName.Friendly)
                    panelGO = GameObject.Instantiate(monsterPanelTemplate, Vector3.zero, Quaternion.identity, FriendlyPanel.transform) as GameObject;
                else
                    panelGO = GameObject.Instantiate(monsterPanelTemplate, Vector3.zero, Quaternion.identity, FriendlyPanel.transform) as GameObject;
                // Set buttons varables
                panelGO.name = character.GetComponent<Character>().GetTeam.ToString() + "_" + character.GetComponent<Character>().GetUniqueID + "_Panel";
                Text[] panelTexts = panelGO.GetComponentsInChildren<Text>();
                foreach (var text in panelTexts)
                {
                    if (text.name == "NameText")
                    {
                        text.text = $"{character.name} : {character.GetComponent<Character>().GetAbilityDelay()}";
                    }
                    if (text.name == "HPText")
                    {
                        text.text = "HP : " + character.GetComponent<Character>().GetHP + "/" + character.GetComponent<Character>().GetMaxHP;
                    }
                }

                Slider[] sliders = panelGO.GetComponentsInChildren<Slider>();
                foreach (var slider in sliders)
                {
                    if (slider.gameObject.name == "HealthBar")
                    {
                        slider.maxValue = character.GetComponent<Character>().GetMaxHP;
                        slider.value = character.GetComponent<Character>().GetHP;
                    }
                    else if (slider.gameObject.name == "ManaBar")
                    {
                        slider.maxValue = character.GetComponent<Character>().GetMaxMP;
                        slider.value = character.GetComponent<Character>().GetMP;
                    }
                }

                //Slider healthSlider = panelGO.GetComponentInChildren<Slider>();
                //healthSlider.maxValue = character.GetComponent<Character>().GetMaxHP;
                //healthSlider.value = character.GetComponent<Character>().GetHP;

                //Slider manaSlider = panelGO.GetComponentInChildren<Slider>();
                //manaSlider.maxValue = character.GetComponent<Character>().GetMaxMP;
                //manaSlider.value = character.GetComponent<Character>().GetMP;

                // TODO... Find a better way
                // We have to get All image components in children becasue otherwise the first image it finds is the actual panel image.
                // So it set the logo to replace the panel itself.
                Image[] images = panelGO.GetComponentsInChildren<Image>();
                foreach (var characterLogo in images)
                {
                    if (characterLogo.name == "CharacterLogo")
                        characterLogo.sprite = character.GetComponent<Character>().GetMonsterSprite();
                }
            }
            else
            {

            }
        }

        private void UpdateCharacterPanelHP(GameObject character)
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

            // Still playing around with this
            //float missingHP = character.GetComponent<Character>().GetMaxHP - character.GetComponent<Character>().GetHP;
            //float scale = (missingHP / FullHP);

            //SpeedBarGO.transform.localScale = new Vector3(1.1f, scale, 1.0f);

            //float barLength = 2;
            //float offset = (barLength / 2) - (scale);
            //SpeedBarGO.transform.localPosition = new Vector3(0.0f, offset, 0.0f);
            Slider[] sliders = panelGO.GetComponentsInChildren<Slider>();
            foreach (var slider in sliders)
            {
                if (slider.gameObject.name == "HealthBar")
                {
                    slider.value = character.GetComponent<Character>().GetHP;
                }
            }

            //Slider healthSlider = panelGO.GetComponentInChildren<Slider>();
            //healthSlider.value = character.GetComponent<Character>().GetHP;
        }

        private void UpdateCharacterPanelMP(GameObject character)
        {
            // TODO.. Is there a better way of doing this, i dont like it
            GameObject panelGO = GameObject.Find(character.GetComponent<Character>().GetTeam.ToString() + "_" + character.GetComponent<Character>().GetUniqueID + "_Panel");
            
            Slider[] sliders = panelGO.GetComponentsInChildren<Slider>();
            foreach (var slider in sliders)
            {
                if (slider.gameObject.name == "ManaBar")
                {
                    slider.value = character.GetComponent<Character>().GetMP;
                }
            }

            //Slider healthSlider = panelGO.GetComponentInChildren<Slider>();
            //healthSlider.value = character.GetComponent<Character>().GetHP;
        }

        public void OrderCharacterPanels(bool initialSetup)
        {
            // foreach character in CombatManager's OrderList
            int i = 0;
            foreach (var character in CombatManager.Instance.GetBattleOrderList())
            {
                // Find accociated Charcater panel
                // TODO.. Is there a better way of doing this, i dont like it
                GameObject panelGO = GameObject.Find(character.Value.GetComponent<Character>().GetTeam.ToString() + "_" + character.Value.GetComponent<Character>().GetUniqueID + "_Panel");

                // Move to the location dication by that charcaters place in the OrderList
                if (initialSetup)
                    panelGO.transform.localPosition = new Vector3(CharPanel_XPosList[i], -0.0f, 0.0f);
                else
                    panelGO.GetComponent<UICharacterPanel>().MoveTo(new Vector3(CharPanel_XPosList[i], -0.0f, 0.0f));

                i++;
            }
        }
        
        public bool CheckCharacterPanelsInPlace()
        {
            bool allInPosition = true;
            int i = 0;
            foreach (var character in CombatManager.Instance.GetBattleOrderList())
            {
                // Find accociated Charcater panel
                // TODO.. Is there a better way of doing this, i dont like it
                GameObject panelGO = GameObject.Find(character.Value.GetComponent<Character>().GetTeam.ToString() + "_" + character.Value.GetComponent<Character>().GetUniqueID + "_Panel");

                if (Vector3.Distance(panelGO.GetComponent<UICharacterPanel>().transform.localPosition, new Vector3(CharPanel_XPosList[i], -0.0f, 0.0f)) > 0.1f)
                    allInPosition = false;

                i++;
            }

            return allInPosition;
        }

        #endregion

        #region Target/Confirm highlight Code

        void TargetHighlight()
        {
            // get ability type
            switch (SelectedAbility.GetTargetType())
            {
                // if attack - targets will be enemies
                case TargetType.Enemy:
                    // Get list of all Enemies currently in the battle
                    List<GameObject> enemyList = CombatManager.Instance.battlefieldController.FindAllCurrentEnemies();
                    PossibleTargets = enemyList;
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
                    PossibleTargets = friendlyList;
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
                    PossibleTargets = unitSlotList;
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
                    foreach (var enemy in PossibleTargets)
                    {
                        enemy.GetComponent<Character>().MakeUnclickable();
                    }
                    break;
                // if support - targets will be Friendlies
                case TargetType.Friendly:
                    // Loop through targets, adding them to the pannel
                    foreach (var friend in PossibleTargets)
                    {
                        friend.GetComponent<Character>().MakeUnclickable();
                    }
                    break;
                // if summon - Targets will be unOccupied character slots on your side
                case TargetType.SummonSlot:
                    // Loop through targets, adding them to the pannel
                    foreach (var unitSlot in PossibleTargets)
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

        void ConfirmHighlight()
        {
            List<GameObject> affectedTargets = SelectedAbility.GetAbilityTargetList(SelectedTarget);

            foreach (var item in affectedTargets)
            {
                if (SelectedAbility.GetTargetType() == TargetType.Friendly
                    || SelectedAbility.GetTargetType() == TargetType.Enemy)
                    item.GetComponent<Character>().MakeGlowingClickable();
                else if (SelectedAbility.GetTargetType() == TargetType.SummonSlot)
                    item.GetComponent<UnitSlot>().MakeGlowingClickable();
            }
        }

        void RemoveConfirmHighlight()
        {
            List<GameObject> affectedTargets = SelectedAbility.GetAbilityTargetList(SelectedTarget);

            foreach (var item in affectedTargets)
            {
                if (SelectedAbility.GetTargetType() == TargetType.Friendly
                    || SelectedAbility.GetTargetType() == TargetType.Enemy)
                    item.GetComponent<Character>().MakeUnclickable();
                else if (SelectedAbility.GetTargetType() == TargetType.SummonSlot)
                    item.GetComponent<UnitSlot>().MakeUnclickable();
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
                case AbilityState.ConfirmSelect:
                    // TODO... Remove confirmation flash
                    RemoveConfirmHighlight();
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
                    // Previous turn is over - Reset tracking variables
                    isMenuSetup = false;
                    break;
                case AbilityState.ActionSelect:
                    abilityState = AbilityState.ActionSelect;
                    break;
                case AbilityState.TargetSelect:
                    // highlight relevant Targets
                    TargetHighlight();
                    abilityState = AbilityState.TargetSelect;
                    break;
                case AbilityState.ConfirmSelect:
                    // TODO... Activate Confirm flash
                    ConfirmHighlight();
                    abilityState = AbilityState.ConfirmSelect;
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
                // Abilites where Charges are not relevant have 'Charges' set to -1 by default
                // if 'charges' == 0, then they are both required and 'out of stock'
                if (kvp.Value.GetAbilityCharges != 0)
                    AddToActionPanel(kvp.Value);//.GetAbilityName);
            }

            // Get that characters Menus
            Dictionary<string, ActionMenu> menus = character.GetComponent<Character>().GetMenus;
            foreach (var kvp in menus)
            {
                AddMenuToActionPanel(kvp.Value);
            }
        }

        void AddToActionPanel(Ability ability)
        {
            // Instantiate button
            GameObject buttonGO = GameObject.Instantiate(actionButtonTemplate, Vector3.zero, Quaternion.identity, ActionPanel.transform) as GameObject;
            // Set buttons varables
            buttonGO.name = ability.GetAbilityName + "_Button";
            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            buttonText.text = ability.GetAbilityName;
            // Inactive is mana cost not available
            if (SelectedCharacter.GetComponent<Character>().GetMP < ability.GetAbilityManaCost)
                buttonGO.GetComponent<Button>().interactable = false;

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
                // Abilites where Charges are not relevant have 'Charges' set to -1 by default
                // if 'charges' == 0, then they are both required and 'out of stock'
                if (ability.GetAbilityCharges != 0)
                    AddToMenuPanel(ability);
            }
        }

        void AddToMenuPanel(Ability ability)
        {
            // Instantiate button
            GameObject buttonGO = GameObject.Instantiate(actionButtonTemplate, Vector3.zero, Quaternion.identity, MenuPanel.transform) as GameObject;
            // Set buttons varables
            buttonGO.name = ability.GetAbilityName + "_Button";
            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            buttonText.text = ability.GetAbilityName;
            // Inactive is mana cost not available
            if (SelectedCharacter.GetComponent<Character>().GetMP < ability.GetAbilityManaCost)
                buttonGO.GetComponent<Button>().interactable = false;

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

        #region Back Panel

        void BackPanelSetup()
        {
            // TODO... Check if this being called twice causes a problem with multiple calls to to 'BackButtonPressed'
            BackPanel.GetComponentInChildren<Button>().onClick.AddListener(BackButtonPressed);
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

            // Activate BackPanel - We might want to go back and change our mind
            BackPanel.SetActive(true);
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

            // Activate BackPanel - We might want to go back and change our mind
            BackPanel.SetActive(true);
            // deactivate action panel
            ActionPanel.SetActive(false);
            MenuPanel.SetActive(true);

            // Find which menu button was pressed
            ActionMenu selectedMenu = SelectedCharacter.GetComponent<Character>().GetMenuByName(buttonClicked);

            // Populate Menu
            MenuPanelSetup(SelectedCharacter.GetComponent<Character>(), selectedMenu);
            isMenuSetup = true;
        }

        public void BackButtonPressed()
        {
            switch (abilityState)
            {
                case AbilityState.ActionSelect:
                    // if Action select stage (Only occus when in Menu)
                    // - Close Menu Panel
                    MenuPanel.SetActive(false);
                    // - Re-Activate Action panel
                    ActionPanel.SetActive(true);
                    // - Close Back Panel
                    BackPanel.SetActive(false);
                    isMenuSetup = false;
                    break;
                case AbilityState.TargetSelect:
                    // If Target Stage
                    // - If Action used was in Menu
                    if (isMenuSetup)
                    {
                        //   - Re-Activate Menu panel
                        MenuPanel.SetActive(true);
                        //   - Change Stage back to Ability select stage
                        TransferAbilityState(abilityState, AbilityState.ActionSelect);
                    }
                    // - If Action used from Action Panel
                    else
                    {
                        //   - Re-Activate Action panel
                        ActionPanel.SetActive(true);
                        //   - Change Stage back to Ability select stage
                        TransferAbilityState(abilityState, AbilityState.ActionSelect);
                        //   - Close Back Panel
                        BackPanel.SetActive(false);
                    }
                    break;
                case AbilityState.ConfirmSelect:
                    // If Confirm Stage
                    // - Return to Target Stage
                    TransferAbilityState(abilityState, AbilityState.TargetSelect);                    
                    break;
            }          

            
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
            MPChangedEventInfo.RegisterListener(OnMPChange);
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
            MPChangedEventInfo.UnregisterListener(OnMPChange);
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
                SelectedTarget = selectedEventInfo.UnitGO;
                TransferAbilityState(abilityState, AbilityState.ConfirmSelect);
            }
            else if (abilityState == AbilityState.ConfirmSelect)
            {
                TransferAbilityState(abilityState, AbilityState.CharcterSelect);

                SelectedAbility.NewAction(SelectedCharacter,
                        selectedEventInfo.UnitGO);
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

            UpdateCharacterPanelHP(hpChangedEventInfo.UnitGO);
        }

        void OnMPChange(MPChangedEventInfo mpChangedEventInfo)
        {
            Debug.Log("BattleUIController Alerted to Character MP Change: " + mpChangedEventInfo.UnitGO.name);

            UpdateCharacterPanelMP(mpChangedEventInfo.UnitGO);
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
            AddToCharacterPanel(unitSpawnEventInfo.UnitGO);
        }

        #endregion
    }
}