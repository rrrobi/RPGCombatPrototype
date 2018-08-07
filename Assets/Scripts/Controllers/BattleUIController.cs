using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using UnityEngine.UI;
using EventCallbacks;

public class BattleUIController : MonoBehaviour {

    [SerializeField]
    GameObject FriendlyPanel;
    [SerializeField]
    GameObject ActionPanel;
    [SerializeField]
    GameObject EnemyPanel;

    // for the GoBack button - not implemented yet
    [SerializeField]
    GameObject ActivePanel;
    [SerializeField]
    GameObject PreviousPanel;

    // Look into better way of doing this, 
    // I dont like having to have all these buttons added in this way
    [SerializeField]
    GameObject FriendlyButtonTemplate;
    [SerializeField]
    GameObject ActionButtonTemplate;
    [SerializeField]
    GameObject EnemyButtonTemplate;

    // TODO... rethink this
    // Holds whcih character button was pressed - I dont like this
    // May change later
    GameObject SelectedCharacterButton;

    // Selected action - may move this to Combatmanager
    GameObject SelectedCharacter;
    Attack SelectedAttack;

    void Awake()
    {
        Assert.IsNotNull(FriendlyPanel);
        Assert.IsNotNull(ActionPanel);
        Assert.IsNotNull(EnemyPanel);
        Assert.IsNotNull(FriendlyButtonTemplate);
        Assert.IsNotNull(ActionButtonTemplate);
        Assert.IsNotNull(EnemyButtonTemplate);
    }

    // Use this for initialization
    void Start () {
        InitialFriendlyPanelSetup();

        InitialEnemyPanelSetup();

        RegisterEventCallbacks();
    }

    // Update is called once per frame
    void Update () {
	}

    void InitialFriendlyPanelSetup()
    {
        // foreach Character on the friendly side
        foreach (var characterKVP in CombatManager.Instance.GetPlayerCharacterList)
        {
            AddToFriendlyPanel(characterKVP.Value);
        }
    }    

    void AddToFriendlyPanel(GameObject character)
    {
        // Instantiate button
        GameObject buttonGO = Instantiate(FriendlyButtonTemplate, Vector3.zero, Quaternion.identity, FriendlyPanel.transform) as GameObject;
        // Set buttons varables
        buttonGO.name = character.GetComponent<Monster>().GetTeam.ToString() + "_" + character.name + "_Button";
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
                    text.text = "HP : " + character.GetComponent<Monster>().GetHP + "/" + character.GetComponent<Monster>().GetMaxHP;
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
            Destroy(buttonGO);
        }
    }

    void InitialEnemyPanelSetup()
    {
        // foreach Character on the Enemy side
        foreach (var characterKVP in CombatManager.Instance.GetEnemyCharacterList)
        {
            AddToEnemyPanel(characterKVP.Value);
        }
    }

    void AddToEnemyPanel(GameObject character)
    {
        // Instantiate button
        GameObject buttonGO = Instantiate(EnemyButtonTemplate, Vector3.zero, Quaternion.identity, EnemyPanel.transform) as GameObject;
        // Set buttons varables
        buttonGO.name = character.GetComponent<Monster>().GetTeam.ToString() + "_" + character.name + "_Button";
        Text buttonText = buttonGO.GetComponentInChildren<Text>();
        buttonText.text = character.name;

        // Provide instructions for what each button does
        buttonGO.GetComponent<Button>().onClick.AddListener(EnemySelectbuttonPressed);
    }

    void RemoveFromEnemyPanel(GameObject charcter)
    {
        // TODO... For when a charcter is killed and needs to be removed from a panel
        // May be reusable for the other panels
        GameObject buttonGO = EnemyPanel.transform.Find(charcter.name + "_Button").gameObject;

        if (buttonGO != null)
        {
            Destroy(buttonGO);
        }
    }

    void ActionPanelSetup(GameObject character)
    {
        // Clear existing panel
        ClearActionPanel();

        // Get that characters abilities
        Dictionary<string, Attack> abilities = character.GetComponent<Monster>().GetAbilities;

        // Add panel for each ability
        foreach (var kvp in abilities)
        {
            AddToActionPanel(kvp.Value.GetAttackName);
        }
    }

    void AddToActionPanel(string buttonName)
    {
        // Instantiate button
        GameObject buttonGO = Instantiate(ActionButtonTemplate, Vector3.zero, Quaternion.identity, ActionPanel.transform) as GameObject;
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
            Destroy(child.gameObject);
        }
    }

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
        SelectedAttack = SelectedCharacter.GetComponent<Monster>().GetAbilityByName(buttonClicked);

        // deactivate action panel
        ActionPanel.SetActive(false);
        // Activate Enemy panel
        EnemyPanel.SetActive(true);
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
        SelectedCharacter.GetComponent<Monster>().UseAbilityOn(
            SelectedAttack,
            CombatManager.Instance.GetEnemyCharacterByName(buttonNameParts[1]));// buttonClicked));
        // Toggle the charcter button to non-interactable
        ToggleButtonInteractable(SelectedCharacterButton, false);
         
        // Deactivate Enemy panel
        EnemyPanel.SetActive(false);
    }

    private void ToggleButtonInteractable(GameObject button, bool setTo)
    {
        button.GetComponent<Button>().interactable = setTo;
    }

    private void UpdateCharacterPanel(GameObject character)
    {
        // TODO.. Is there a better way of doing this, i dont like it
        GameObject buttonGO = GameObject.Find(character.GetComponent<Monster>().GetTeam.ToString() + "_" + character.name + "_Button");
        Text[] texts  = buttonGO.GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            if (text.name == "HPText")
            {
                text.text = "HP : " + character.GetComponent<Monster>().GetHP + "/" + character.GetComponent<Monster>().GetMaxHP;
            }

        }
        
    }

    #region EventCallbacks

    void RegisterEventCallbacks()
    {
        TakeDamageEventInfo.RegisterListener(OnDamageTaken);
        DeathEventInfo.RegisterListener(OnUnitDied);
        UnitSpawnEventInfo.RegisterListener(OnUnitSpawn);
    }

    void OnDamageTaken(TakeDamageEventInfo takeDamageEventInfo)
    {
        Debug.Log("BattleUIController Alerted to Character taken damge: " + takeDamageEventInfo.UnitGO.name);

        UpdateCharacterPanel(takeDamageEventInfo.UnitGO);
    }

    void OnUnitDied(DeathEventInfo deathEventInfo)
    {
        Debug.Log("BattleUIController Alerted to Character Death: " + deathEventInfo.UnitGO.name);
        // Update UI
        // TODO... ReThink
        // I HATE this
        Destroy(GameObject.Find(deathEventInfo.TeamName + "_" + deathEventInfo.UnitGO.name + "_Button"));
    }

    void OnUnitSpawn(UnitSpawnEventInfo unitSpawnEventInfo)
    {
        Debug.Log("BattleUIController Alerted to unit Spawned: " + unitSpawnEventInfo.UnitGO.name);
    }
    #endregion
 }
