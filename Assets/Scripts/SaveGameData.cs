using System;
using System.IO;
using UnityEngine;
[Serializable]
public class SaveGameData
{
    public int saveVersion = 1;
    public int slotId;
    public string savedAtTime;
    public string sceneName;
    public int waveIndex;
    public int playerHealth;
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public Vector3 playerScale;
    public string equippedWeaponId;
}

public static class SaveSystem
{
    public const int slotCount = 5;
    public const int currentSaveVersion = 1;
    public static void Save(SaveGameData data, int slotId)
    {
        data.slotId = slotId;
        data.saveVersion = currentSaveVersion;
        data.savedAtTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string path = GetSavePath(slotId);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"Saved to: {path}");
    }
    public static SaveGameData Load(int slotId)
    {
        string path = GetSavePath(slotId);
        if (!File.Exists(path))
        {
            Debug.Log($"No save file found at: {path}");
            return null;
        }
        string json = File.ReadAllText(path);
        SaveGameData data =  JsonUtility.FromJson<SaveGameData>(json);
        
        Debug.Log($"Loaded slot {slotId} from: {path}");
        return data;
    }

    public static void Delete(int slotId)
    {
        string path = GetSavePath(slotId);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    public static int SaveToNextSlot(SaveGameData data, int totalSlots)
    {
        int chosenSlot = -1;
        DateTime oldestSaveTime = DateTime.MaxValue;
        for (int slot = 1; slot <= totalSlots; slot++)
        {
            SaveGameData existingSave = Load(slot);

            if (existingSave == null)
            {
                chosenSlot = slot;
                break;
            }
            if (DateTime.TryParse(existingSave.savedAtTime, out DateTime saveTime)
                && saveTime < oldestSaveTime)
            {
                oldestSaveTime = saveTime;
                chosenSlot = slot;
            }
        }
        if (chosenSlot == -1)
        {
            Debug.LogError("Could not choose a save slot.");
            return -1;
        }
        data.slotId = chosenSlot;
        Save(data , data.slotId);
        return chosenSlot;
    }
    private static string GetSavePath(int slotId)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}.json");
    }
    public static bool SaveExists(int slotId)
    {
        return File.Exists(GetSavePath(slotId));
    }
}