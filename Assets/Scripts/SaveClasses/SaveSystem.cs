using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
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

    static string savePath = Application.persistentDataPath + $"/Saves/";

    public static void FullSave(HeroInfo hi, Dictionary<int, DungeonFloorData> dungeonData)
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        BinaryFormatter formatter = GetBinaryFormatter();
        string path = savePath + $"{activeSaveSlot.ToString()}Save.slot";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(hi, dungeonData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadFull(ActiveSaveSlot saveSlot = ActiveSaveSlot.Default)
    {
        if (saveSlot == ActiveSaveSlot.Default)
            saveSlot = activeSaveSlot;

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        string path = savePath + $"{saveSlot.ToString()}Save.slot";

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found: {path}");
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();
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

    private static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        SurrogateSelector selector = new SurrogateSelector();

        Vector2ISerializationSurrogate vector2ISurrogate = new Vector2ISerializationSurrogate();

        selector.AddSurrogate(typeof(Vector2Int), new StreamingContext(StreamingContextStates.All), vector2ISurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
