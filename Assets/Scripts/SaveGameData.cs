using System;
using System.Collections.Generic;
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
    public WeaponSaveData weapon = new WeaponSaveData();
    public int weakEnemiesStillToSpawn;
    public int strongEnemiesStillToSpawn;
    public List<EnemySaveData> aliveEnemies = new List<EnemySaveData>();
}

[Serializable]
public class WeaponSaveData
{
    public int equippedWeaponIndex = -1;
    public int loadedBullets;
    public int loadedAmmoIndex;
    public int selectedAmmoIndex;
    public int m16FireMode;
    public bool revolverHammerRacked;
}

public enum EnemyKind
{
    Weak,
    Strong
}

[Serializable]
public class EnemySaveData
{
    public EnemyKind enemyKind;
    public Vector3 position;
    public Quaternion rotation;
    public int health;
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

        string thumbnailPath = GetThumbnailPath(slotId);
        if (File.Exists(thumbnailPath))
        {
            File.Delete(thumbnailPath);
        }
    }

    public static void DeleteAll()
    {
        for (int slotId = 1; slotId <= slotCount; slotId++)
        {
            Delete(slotId);
        }
    }
    private static string GetSavePath(int slotId)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}.json");
    }

    public static string GetThumbnailPath(int slotId)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}.png");
    }

    public static void SaveThumbnail(int slotId, Texture2D screenshot)
    {
        File.WriteAllBytes(GetThumbnailPath(slotId), screenshot.EncodeToPNG());
    }
    public static bool SaveExists(int slotId)
    {
        return File.Exists(GetSavePath(slotId));
    }
}
