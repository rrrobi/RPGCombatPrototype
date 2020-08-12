using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Global;

public class LaunchMenuManager : MonoBehaviour
{
    bool toggleNewGame = false;
    const int NUMBER_OF_SAVESLOTS = 3;

    HeroDataReader heroData = new HeroDataReader();

    // Start is called before the first frame update
    void Start()
    {
        heroData.Setup();
        heroData.ReadData();

        // Set up save buttons
        for (int i = 0; i < NUMBER_OF_SAVESLOTS; i++)
        {
            GameObject slotButton = GameObject.Find($"SaveSlot{i + 1}Button");

            ActiveSaveSlot slot = (ActiveSaveSlot)i;
            SaveData data = SaveSystem.LoadFull(slot);
            if (data == null) // if null then the save file does not exist for this slot
            {
                slotButton.GetComponentInChildren<Text>().text = "Empty";
                slotButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                slotButton.GetComponentInChildren<Text>().text = $"{data.HeroInfoData.PlayerName} - {data.Date}";
                slotButton.GetComponent<Button>().interactable = true;
            }                      
        }

        //GameObject slot1Button = GameObject.Find("SaveSlot1Button");
        //GameObject slot2Button = GameObject.Find("SaveSlot2Button");
        //GameObject slot3Button = GameObject.Find("SaveSlot3Button");
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
        SaveSystem.SetActiveSaveSlot(ActiveSaveSlot.Slot1);
        if (toggleNewGame)
        {
            CreateNewSave();            
        }
        LaunchGame();
        // TODO... Deal with load
    }
    public void SaveSlot2Pressed()
    {
        SaveSystem.SetActiveSaveSlot(ActiveSaveSlot.Slot2);
        if (toggleNewGame)
        {
            CreateNewSave();            
        }
        LaunchGame();
        // TODO... Deal with load
    }
    public void SaveSlot3Pressed()
    {
        SaveSystem.SetActiveSaveSlot(ActiveSaveSlot.Slot3);
        if (toggleNewGame)
        {
            CreateNewSave();
        }
        LaunchGame();
        // TODO... Deal with load
    }

    void CreateNewSave()
    {
        Dictionary<int, DungeonFloorData> dungeonData = new Dictionary<int, DungeonFloorData>();
        SaveSystem.FullSave(heroData.heroWrapper.HeroData.HeroInfo, dungeonData); 
    }

    void LaunchGame()
    {
        SceneManager.LoadScene("Dungeon");
    }
}
