using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using UnityEngine.UI;

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
        buttonGO.name = character.name + "_Button";
        Text[] buttonTexts = buttonGO.GetComponentsInChildren<Text>();

        buttonGO.GetComponent<Button>().onClick.AddListener(CharcterSelectbuttonPressed);
        foreach (var text in buttonTexts)
        {
            switch (text.name)
            {
                case "NameText":
                    text.text = character.name.Replace("_", " ");
                    break;
                case "HPText":
                    text.text = "HP: 9999/10000";
                    break;
            }
        }
    }

    void RemoveFromFriendlyPanel(GameObject charcter)
    {
        // TODO... For when a charcter is killed and needs to be removed from a panel
        // May be reusable for the other panels
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
        buttonGO.name = character.name + "_Button";
        Text buttonText = buttonGO.GetComponentInChildren<Text>();
        buttonText.text = character.name;

        // Provide instructions for what each button does
        buttonGO.GetComponent<Button>().onClick.AddListener(EnemySelectbuttonPressed);
    }

    void ActionPanelSetup(GameObject character)
    {
        // TODO...
        // to be dynamic for each characters abilities

        // Clear existing panel
        ClearActionPanel();

        // Get that characters abilities
        Dictionary<string, Attack> abilities = character.GetComponent<Monster>().GetAbilities;

        // Add panel for each ability
        foreach (var kvp in abilities)
        {
            AddToActionPanel(kvp.Value.GetAttackName);
        }
        //for (int i = 0; i < abilities.Length; i++)
        //{
        //    AddToActionPanel(abilities[i].GetAttackName);
        //}

        //AddToActionPanel("Attack");
        //AddToActionPanel("Defend");
        //AddToActionPanel("Heal");
        //AddToActionPanel("Skip");

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
        buttonClicked = buttonClicked.Replace("_Button", "");
        Debug.Log(buttonClicked);

        // record which freindly character has been selected
        SelectedCharacter = CombatManager.Instance.GetPlayerCharacterByName(buttonClicked);
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
        Debug.Log(buttonClicked);

        // Currently this is only used to selct target for selected action
        // Carry out selected action upon this target
        SelectedCharacter.GetComponent<Monster>().UseAbilityOn(
            SelectedAttack, 
            CombatManager.Instance.GetEnemyCharacterByName(buttonClicked));
        // Toggle the charcter button to non-interactable
        ToggleButtonInteractable(SelectedCharacterButton, false);
         
        // Deactivate Enemy panel
        EnemyPanel.SetActive(false);
    }

    private void ToggleButtonInteractable(GameObject button, bool setTo)
    {
        button.GetComponent<Button>().interactable = setTo;
    }
}
