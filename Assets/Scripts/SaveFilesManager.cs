using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFilesManager : MonoBehaviour
{
    [Header("Player attributes")]
    public Transform player;
    public int playerHealth = 100;

    [Header("Gameplay attributes")]
    [SerializeField] private int maxWaveIndex = 3;
    
    [Header("Game components")]
   [SerializeField] private Canvas pauseCanvas;
   [SerializeField] private Canvas saveFilesCanvas;
   [SerializeField] private List<Button> saveSlots = new List<Button>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        saveFilesCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveButtonPressed()
    {
        SaveGameData data = new SaveGameData();

        data.waveIndex = 1;
        data.playerHealth = playerHealth;
        data.playerPosition = player.position;
        data.playerRotation = player.rotation;
        data.playerScale = player.localScale;

        int savedSlot = SaveSystem.SaveToNextSlot(data, saveSlots.Capacity);

        if (savedSlot != -1)
            Debug.Log($"Saved to slot {savedSlot}");
    }
    
    public void LoadButtonPressed()
    {
        pauseCanvas.enabled = false;
        saveFilesCanvas.enabled = true;
    }
}
