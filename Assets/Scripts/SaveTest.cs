using UnityEngine;

public class SaveTest : MonoBehaviour
{
    public Transform player;
    public int playerHealth = 100;

    private void Start()
    {
        // Create some test data and save it.
        SaveGameData data = new SaveGameData();
        data.slotId = 1;
        data.waveIndex = 3;

        data.playerHealth = playerHealth;
        data.playerPosition = player.position;
        data.playerRotation = player.rotation;
        data.playerScale = player.localScale;

        SaveSystem.Save(data);

        // Load the save and apply it to the player.
        SaveGameData loadedData = SaveSystem.Load(1);

        if (loadedData != null)
        {
            playerHealth = loadedData.playerHealth;
            player.SetPositionAndRotation(loadedData.playerPosition, loadedData.playerRotation);
            player.localScale = loadedData.playerScale;
            
            Debug.Log($"Loaded health: {playerHealth}");
        }
    }
}
