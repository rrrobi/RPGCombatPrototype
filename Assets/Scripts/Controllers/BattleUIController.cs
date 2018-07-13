﻿using System.Collections;
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

    // Look into better way of doing this, 
    // I dont like having to have all these buttons added in this way
    [SerializeField]
    GameObject FriendlyButtonTemplate;
    [SerializeField]
    GameObject ActionButtonTemplate;
    [SerializeField]
    GameObject EnemyButtonTemplate;

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

        InitialActionPanelSetup();

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



    void InitialActionPanelSetup()
    {
        // TODO...
        // to be dynamic for each characters abilities

        AddToActionPanel("Attack");
        AddToActionPanel("Defend");
        AddToActionPanel("Heal");
        AddToActionPanel("Skip");

    }

    void AddToActionPanel(string buttonName)
    {
        // Instantiate button
        GameObject buttonGO = Instantiate(ActionButtonTemplate, Vector3.zero, Quaternion.identity, ActionPanel.transform) as GameObject;
        // Set buttons varables
        buttonGO.name = buttonName + " Button";
        Text buttonText = buttonGO.GetComponentInChildren<Text>();
        buttonText.text = buttonName;

        // TODO...
        // Provide instructions for what each button does
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
        buttonGO.name = character.name + " Button";
        Text buttonText = buttonGO.GetComponentInChildren<Text>();
        buttonText.text = character.name;

        // TODO...
        // Provide instructions for what each button does
    }



    public void CharcterSelectbuttonPressed()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    public void ActionSelectbuttonPressed()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    public void EnemySelectbuttonPressed()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }
}
