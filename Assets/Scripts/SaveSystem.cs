using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Global;


public enum ActiveSaveSlot
{
    Slot1,
    Slot2,
    Slot3,
    Default
}
public static class SaveSystem
{
    static ActiveSaveSlot activeSaveSlot;
    public static ActiveSaveSlot GetActiveSaveSlot() { return activeSaveSlot; }
    public static void SetActiveSaveSlot(ActiveSaveSlot slot) { activeSaveSlot = slot; }

    public static void FullSave(HeroInfo hi)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/{activeSaveSlot.ToString()}Save.slot";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(hi, "Dungeon placeholder");

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadFull(ActiveSaveSlot saveSlot = ActiveSaveSlot.Default)
    {
        if (saveSlot == ActiveSaveSlot.Default)
            saveSlot = activeSaveSlot;

        string path = Application.persistentDataPath + $"/{saveSlot.ToString()}Save.slot";

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found: {path}");
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        try
        {          
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        catch
        {
            stream.Close();
            File.Delete(path);
            Debug.LogWarning($"Loading file at: {path} failed, file has been deleted.");
            return null;
        }

    }
}
