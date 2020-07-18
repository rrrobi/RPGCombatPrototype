using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void FullSave()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/{GameManager.Instance.GetActiveSaveSlot().ToString()}Save.slot";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(GameManager.Instance.GetHeroData.heroWrapper.HeroData.HeroInfo, "Dungeon placeholder");

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadFull()
    {
        string path = Application.persistentDataPath + $"/{GameManager.Instance.GetActiveSaveSlot().ToString()}Save.slot";
        if (File.Exists(path))
        {

        }
        else
        {
            Debug.LogError($"Save file not found: {path}");
            return null;
        }

    }
}
