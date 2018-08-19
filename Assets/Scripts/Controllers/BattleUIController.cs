using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using UnityEngine.UI;
using EventCallbacks;

public class BattleUIController 
{
    GameObject FriendlyPanel;
    GameObject ActionPanel;
    GameObject TargetPanel;
    GameObject GameOverPanel;
    GameObject VictoryPanel;

    // for the GoBack button - not implemented yet
    GameObject ActivePanel;
    GameObject PreviousPanel;

    // Look into better way of doing this, 
    // I dont like having to have all these buttons added in this way
    GameObject FriendlyButtonTemplate;
    GameObject ActionButtonTemplate;
    GameObject TargetButtonTemplate;

    // TODO... rethink this
    // Holds whcih character button was pressed - I dont like this
    // May change later
    GameObject SelectedCharacterButton;

    // Selected action - may move this to Combatmanager
    GameObject SelectedCharacter;
    Attack SelectedAttack;
    Ability SelectedAbility;

    // Use this for initialization
    public void Setup() {//Start () {
        // Read in GO templates from Resources
        FriendlyButtonTemplate = Resources.Load("Prefabs/UI/MonsterPanel") as GameObject;
        ActionButtonTemplate = Resources.Load("Prefabs/UI/ActionButton") as GameObject;
        TargetButtonTemplate = Resources.Load("Prefabs/UI/TargetButton") as GameObject;

        // Get UI Panel Gameobjects to use later
        // TODO... ReThink
        // I HATE this
        FriendlyPanel = GameObject.Find("FriendlyPanel");        
        ActionPanel = GameObject.Find("ActionPanel");
        TargetPanel = GameObject.Find("TargetPanel");
        GameOverPanel = GameObject.Find("GameOverPanel");
        VictoryPanel = GameObject.Find("VictoryPanel");
        ActionPanel.SetActive(false);
        TargetPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        VictoryPanel.SetActive(false);

        RegisterEventCallbacks();
    }

    #region Friendly Panel Code

    void AddToFriendlyPanel(GameObject character)
    {
        // Instantiate button
        GameObject buttonGO = GameObject.Instantiate(FriendlyButtonTemplate, Vector3.zero, Quaternion.identity, FriendlyPanel.transform) as GameObject;
        // Set buttons varables
        buttonGO.name = character.GetComponent<Character>().GetTeam.ToString() + "_" + character.name + "_Button";
        Text[] buttonTexts = buttonGO.GetComponentsInChildren<Text>();

        buttonGO.GetComponent<Button>().interactable = false;
        buttonGO.GetComponent<Button>().onClick.AddListener(CharcterSelectbuttonPressed);
        foreach (var text in buttonTexts)
        {
            switch (text.name)
            {
                case "NameText":
                    text.text = character.name.Replace("_", " ");
                    break;
                case "HPText":
                    text.text = "HP : " + character.GetComponent<Character>().GetHP + "/" + character.GetComponent<Character>().GetMaxHP;
                    break;
            }
        }
    }

    void RemoveFromFriendlyPanel(GameObject charcter)
    {
        // TODO... For when a charcter is killed and needs to be removed from a panel
        // May be reusable for the other panels
        GameObject buttonGO = FriendlyPanel.transform.Find(charcter.name + "_Button").gameObject;

        if (buttonGO != null)
        {
            GameObject.Destroy(buttonGO);
        }
    }

    private void UpdateFriendlyPanel(GameObject character)
    {
        // TODO.. Is there a better way of doing this, i dont like it
        GameObject buttonGO = GameObject.Find(character.GetComponent<Character>().GetTeam.ToString() + "_" + character.name + "_Button");
        Text[] texts = buttonGO.GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            if (text.name == "HPText")
            {
                text.text = "HP : " + character.GetComponent<Character>().GetHP + "/" + character.GetComponent<Character>().GetMaxHP;
            }

        }

    }

    #endregion

    #region Target Panel Code

    void TargetPanelSetup()
    {
        // Clear existing panel
        ClearTargetPanel();

        // get ability type
        switch (SelectedAbility.GetAbilityType())
        {
            // if attack - targets will be enemies
            case AbilityType.Attack:
                // Get list of all Enemies currently in the battle
                List<GameObject> enemyList = CombatManager.Instance.battlefieldController.FindAllCurrentEnemies();
                // Loop through targets, adding them to the pannel
                foreach (var enemy in enemyList)
                {
                    AddToTargetPanel(enemy);
                }
                break;
            // if support - targets will be Friendlies
            case AbilityType.Support:
                break;
            // if summon - Targets will be unOccupied character slots on your side
            case AbilityType.Summon:
                break;
            case AbilityType.None:
                // TODO... look into more robust error handling
                Debug.Log("Not exactly sure what shout happen here, this is probably an error!");
                break;
            default:
                // TODO... look into more robust error handling
                Debug.Log("Not exactly sure what shout happen here, this is probably an error!");
                break;
        }        

        
    }

    void AddToTargetPanel(GameObject target)
    {
        // Instantiate button
        GameObject buttonGO = GameObject.Instantiate(TargetButtonTemplate, Vector3.zero, Quaternion.identity, TargetPanel.transform) as GameObject;
        // Set buttons varables
        buttonGO.name = target.GetComponent<Character>().GetTeam.ToString() + "_" + target.name + "_Button";
        Text buttonText = buttonGO.GetComponentInChildren<Text>();
        buttonText.text = target.name;

        // Provide instructions for what each button does
        buttonGO.GetComponent<Button>().onClick.AddListener(EnemySelectbuttonPressed);
    }

    void RemoveFromTargetPanel(GameObject charcter)
    {
        // TODO... For when a charcter is killed and needs to be removed from a panel
        // May be reusable for the other panels
        GameObject buttonGO = TargetPanel.transform.Find(charcter.name + "_Button").gameObject;

        if (buttonGO != null)
        {
            GameObject.Destroy(buttonGO);
        }
    }

    void ClearTargetPanel()
    {
        foreach (Transform child in TargetPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
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
            AddToActionPanel(kvp.Value.GetAbilityName);
        }
    }

    void AddToActionPanel(string buttonName)
    {
        // Instantiate button
        GameObject buttonGO = GameObject.Instantiate(ActionButtonTemplate, Vector3.zero, Quaternion.identity, ActionPanel.transform) as GameObject;
        // Set buttons varables
        buttonGO.name = buttonName + "_Button";
        Text buttonText = buttonGO.GetComponentInChildren<Text>();
        buttonText.text = buttonName;

        // Provide instructions for what each button does
        buttonGO.GetComponent<Button>().onClick.AddListener(ActionSelectbuttonPressed);
    }

    void ClearActionPanel()
    {
        foreach (Transform child in ActionPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    #endregion

    #region Button pressed Reactions

    public void CharcterSelectbuttonPressed()
    {
        string buttonClicked = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(buttonClicked);
        string[] buttonNameParts = buttonClicked.Split('_');
        buttonClicked = buttonClicked.Replace("_Button", "");
        Debug.Log(buttonNameParts[1]);

        // record which freindly character has been selected
        SelectedCharacter = CombatManager.Instance.GetPlayerCharacterByName(buttonNameParts[1]);//buttonClicked);
        SelectedCharacterButton = EventSystem.current.currentSelectedGameObject;

        // Create action panel for this character
        ActionPanel.SetActive(true);
        ActionPanelSetup(SelectedCharacter);
    }

    public void ActionSelectbuttonPressed()
    {
        string buttonClicked = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(buttonClicked);
        buttonClicked = buttonClicked.Replace("_Button", "");

        // record which action has been chosen
        SelectedAbility = SelectedCharacter.GetComponent<Character>().GetAbilityByName(buttonClicked);

        // deactivate action panel
        ActionPanel.SetActive(false);
        // Activate Enemy panel
        TargetPanel.SetActive(true);
        TargetPanelSetup();

    }

    public void EnemySelectbuttonPressed()
    {
        string buttonClicked = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(buttonClicked);
        buttonClicked = buttonClicked.Replace("_Button", "");
        string[] buttonNameParts = buttonClicked.Split('_');
        Debug.Log(buttonNameParts[1]);

        // Currently this is only used to selct target for selected action
        // Carry out selected action upon this target
        SelectedCharacter.GetComponent<Character>().UseAbilityOn(
            SelectedAbility,
            CombatManager.Instance.GetEnemyCharacterByName(buttonNameParts[1]));// buttonClicked));
        // Toggle the charcter button to non-interactable
        ToggleButtonInteractable(SelectedCharacterButton, false);
         
        // Deactivate Enemy panel
        TargetPanel.SetActive(false);
    }

    public void GameOverButtonPressed()
    {
        Debug.Log("Game over/victory clicked!");
        // Temp - Quit game from editor - this will NOT be in the game at all later
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void VictoryButtonPressed()
    {
        Debug.Log("Victory clicked!");
        // Temp - Quit game from editor - this will NOT be in the game at all later
        UnityEditor.EditorApplication.isPlaying = false;
    }

    #endregion

    private void ToggleButtonInteractable(GameObject button, bool setTo)
    {
        button.GetComponent<Button>().interactable = setTo;
    }

    

    #region EventCallbacks

    void RegisterEventCallbacks()
    {
        BattleWonEventInfo.RegisterListener(OnBattleWon);
        TakeDamageEventInfo.RegisterListener(OnDamageTaken);
        HeroDeathEventInfo.RegisterListener(OnHeroDeath);
        DeathEventInfo.RegisterListener(OnUnitDied);
        UnitSpawnEventInfo.RegisterListener(OnUnitSpawn);
    }

    void OnBattleWon(BattleWonEventInfo battleWonEventInfo)
    {
        Debug.Log("BattleUIController Alerted to Hero Death!");

        // Close all existing windows
        FriendlyPanel.SetActive(false);
        ActionPanel.SetActive(false);
        TargetPanel.SetActive(false);
        // Open Victory Window
        VictoryPanel.SetActive(true);

        VictoryPanel.GetComponentInChildren<Button>().onClick.AddListener(GameOverButtonPressed);
    }

    void OnDamageTaken(TakeDamageEventInfo takeDamageEventInfo)
    {
        Debug.Log("BattleUIController Alerted to Character taken damge: " + takeDamageEventInfo.UnitGO.name);

        UpdateFriendlyPanel(takeDamageEventInfo.UnitGO);
    }

    void OnUnitDied(DeathEventInfo deathEventInfo)
    {
        Debug.Log("BattleUIController Alerted to Character Death: " + deathEventInfo.UnitGO.name);
        // Update UI
        // TODO... ReThink
        // I HATE this
        if (deathEventInfo.TeamName == TeamName.Friendly) // Enemies do not maintain a panel of buttons anymore
            GameObject.Destroy(GameObject.Find(deathEventInfo.TeamName + "_" + deathEventInfo.UnitGO.name + "_Button"));
    }

    void OnHeroDeath(HeroDeathEventInfo heroDeathEventInfo)
    {
        Debug.Log("BattleUIController Alerted to Hero Death!");

        // Close all existing windows
        FriendlyPanel.SetActive(false);
        ActionPanel.SetActive(false);
        TargetPanel.SetActive(false);
        // Open GameOver Window
        GameOverPanel.SetActive(true);

        GameOverPanel.GetComponentInChildren<Button>().onClick.AddListener(GameOverButtonPressed);
    }

    void OnUnitSpawn(UnitSpawnEventInfo unitSpawnEventInfo)
    {
        Debug.Log("BattleUIController Alerted to unit Spawned: " + unitSpawnEventInfo.UnitGO.name);
        if (unitSpawnEventInfo.UnitGO.GetComponent<Character>().GetTeam == TeamName.Friendly)
            AddToFriendlyPanel(unitSpawnEventInfo.UnitGO);
        else if (unitSpawnEventInfo.UnitGO.GetComponent<Character>().GetTeam == TeamName.Enemy)
        {
            //AddToTargetPanel(unitSpawnEventInfo.UnitGO); // <-- Enemies do not maintain a panel of buttons anymore
        }
        else
            // TODO.. improve error handling
            Debug.Log("Something has gone very wrong here");
    }
    #endregion
 }
