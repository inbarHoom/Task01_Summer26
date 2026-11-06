using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveFilesManager : MonoBehaviour
{
    [Header("Player attributes")]
    public Transform player;
    [Header("Game components")]
   [SerializeField] private Canvas pauseCanvas;
   [SerializeField] private Canvas saveFilesCanvas;
   [SerializeField] private List<Button> saveSlots = new List<Button>();

    private Combat combat;
    private Inventory inventory;
    private Wave_Manager waveManager;
    private bool openedFromPauseMenu;

    private void Awake()
    {
        combat = player.GetComponent<Combat>();
        inventory = player.GetComponent<Inventory>();
        waveManager = FindFirstObjectByType<Wave_Manager>();
    }

    private IEnumerator Start()
    {
        ConfigureSaveFilesMenu();
        SaveFilesMenuView.Hide(saveFilesCanvas);

        if (SaveSession.HasPendingLoad)
        {
            // HUD_Manager.Start resets its UI. Wait until it has completed before
            // equipping the saved weapon and publishing its ammo value.
            yield return null;
            RestorePlayer(SaveSession.PendingLoadData);
        }
    }

    public void SaveButtonPressed()
    {
        openedFromPauseMenu = true;
        pauseCanvas.enabled = false;
        OpenSaveFilesMenu(true);
    }

    public void LoadButtonPressed()
    {
        openedFromPauseMenu = true;
        pauseCanvas.enabled = false;
        OpenSaveFilesMenu(false);
    }

    private void OpenSaveFilesMenu(bool showSaveButton)
    {
        SaveSession.ClearSelection();
        ConfigureSaveFilesMenu(showSaveButton);
        SaveFilesMenuView.Show(saveFilesCanvas);
    }

    private void ConfigureSaveFilesMenu(bool showSaveButton = true)
    {
        SaveFilesMenuView.Configure(
            saveFilesCanvas,
            SaveSession.SelectSlot,
            LoadSelectedSlot,
            SaveSelectedSlot,
            DeleteSelectedSlot,
            CloseSaveFilesMenu,
            showSaveButton);
    }

    private void SaveSelectedSlot()
    {
        StartCoroutine(SaveSelectedSlotCoroutine());
    }

    private IEnumerator SaveSelectedSlotCoroutine()
    {
        if (SaveSession.SelectedSlot < 1)
        {
            Debug.Log("Choose a save slot first.");
            yield break;
        }

        SaveGameData data = new SaveGameData();
        data.sceneName = SceneManager.GetActiveScene().name;
        data.playerHealth = combat.CurrentHealth;
        data.playerPosition = player.position;
        data.playerRotation = player.rotation;
        data.weapon = BuildWeaponSaveData();
        waveManager.WriteSaveData(data);

        SaveSystem.Save(data, SaveSession.SelectedSlot);
        SaveFilesMenuView.Hide(saveFilesCanvas);

        // Capture after the save menu has been hidden, so the thumbnail shows
        // the paused game rather than the buttons of the save menu.
        yield return new WaitForEndOfFrame();
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        SaveSystem.SaveThumbnail(SaveSession.SelectedSlot, screenshot);
        Destroy(screenshot);

        SaveFilesMenuView.Show(saveFilesCanvas);
    }

    private void LoadSelectedSlot()
    {
        if (SaveSession.SelectedSlot < 1)
        {
            return;
        }

        SaveGameData data = SaveSystem.Load(SaveSession.SelectedSlot);
        if (data == null)
        {
            return;
        }

        SaveSession.PrepareLoad(data);
        Time.timeScale = 1;
        SceneManager.LoadScene(string.IsNullOrWhiteSpace(data.sceneName)
            ? SceneManager.GetActiveScene().name
            : data.sceneName);
    }

    private void DeleteSelectedSlot()
    {
        if (SaveSession.SelectedSlot < 1)
        {
            return;
        }

        SaveSystem.Delete(SaveSession.SelectedSlot);
        SaveSession.ClearSelection();
        SaveFilesMenuView.RefreshSlots(saveFilesCanvas);
    }

    private void CloseSaveFilesMenu()
    {
        SaveFilesMenuView.Hide(saveFilesCanvas);
        if (openedFromPauseMenu)
        {
            pauseCanvas.enabled = true;
        }
    }

    private WeaponSaveData BuildWeaponSaveData()
    {
        WeaponSaveData data = new WeaponSaveData
        {
            equippedWeaponIndex = inventory.EquippedWeaponIndex
        };

        if (inventory.equipedWeapon == null)
        {
            return data;
        }

        Weapon_Behavior weapon = inventory.equipedWeapon;
        data.loadedBullets = weapon.LoadedBullets;
        data.loadedAmmoIndex = weapon.LoadedAmmoIndex;
        data.selectedAmmoIndex = weapon.SelectedAmmoIndex;

        if (weapon is AR_Behavior ar)
        {
            data.m16FireMode = (int)ar.FireMode;
        }
        else if (weapon is Revolver_Behavior revolver)
        {
            data.revolverHammerRacked = revolver.IsHammerRacked;
        }

        return data;
    }

    private void RestorePlayer(SaveGameData data)
    {
        player.SetPositionAndRotation(data.playerPosition, data.playerRotation);
        combat.RestoreHealth(data.playerHealth);

        if (data.weapon == null || data.weapon.equippedWeaponIndex < 0)
        {
            return;
        }

        inventory.EquipWeapon(data.weapon.equippedWeaponIndex);
        Weapon_Behavior weapon = inventory.equipedWeapon;
        weapon.RestoreAmmoState(
            data.weapon.loadedBullets,
            data.weapon.loadedAmmoIndex,
            data.weapon.selectedAmmoIndex);

        if (weapon is AR_Behavior ar)
        {
            ar.RestoreFireMode(data.weapon.m16FireMode);
        }
        else if (weapon is Revolver_Behavior revolver)
        {
            revolver.RestoreHammerState(data.weapon.revolverHammerRacked);
        }
    }
}
