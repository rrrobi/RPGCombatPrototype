using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActiveSaveSlot
{
    Slot1,
    Slot2,
    Slot3
}

public class LaunchMenuManager : MonoBehaviour
{
    bool toggleNewGame = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGamePressed()
    {
        toggleNewGame = true;
        GameObject.Find("SaveSlot1Button").GetComponent<Button>().interactable = true;
        GameObject.Find("SaveSlot2Button").GetComponent<Button>().interactable = true;
        GameObject.Find("SaveSlot3Button").GetComponent<Button>().interactable = true;   
        
        // TODO... Give player instructions

    }

    public void SaveSlot1Pressed()
    {
        GameManager.Instance.SetActiveSaveSlot(ActiveSaveSlot.Slot1);
        if (toggleNewGame)
        {
            LaunchGame();
        }
        // TODO... Deal with load
    }
    public void SaveSlot2Pressed()
    {
        GameManager.Instance.SetActiveSaveSlot(ActiveSaveSlot.Slot2);
        if (toggleNewGame)
        {
            LaunchGame();
        }
        // TODO... Deal with load
    }
    public void SaveSlot3Pressed()
    {
        GameManager.Instance.SetActiveSaveSlot(ActiveSaveSlot.Slot3);
        if (toggleNewGame)
        {
            LaunchGame();
        }
        // TODO... Deal with load
    }

    void LaunchGame()
    {
        GameManager.Instance.InitialiseGame();
    }
}
