using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class HUD_Manager : MonoBehaviour
{
    [SerializeField] private GameObject ammoCounter;
    [FormerlySerializedAs("ammoCounter")] [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] protected TextMeshProUGUI fireMode;
    [SerializeField] private GameObject inventoryGO;
    [SerializeField] private TextMeshProUGUI instructions;

    private WeaponSO currentWeapon;
   [SerializeField] private Inventory inventory;
    string startInstructions = "Hold 'i' to see the instructions";
    string instructionsMenu = "WASD - Movement \nTab - Hold to open invetory, Hover with mouse and release tab to choose weapon \n *Instructions change with each weapon*";
    private string arInstructions = "R - Reload \nSpace - Shoot \nF - Change firing mode ";
    private string revolverInstructions = "\nR - Reload \nSpace - Shoot \nF - Rack the hammer";
    private string glockInstructions = "\"R - Reload \\nSpace - Shoot \\nF - Change ammo type";

    private int hoveredSlotIndex = -1;

    void Start()
    {
        ammoCounter.SetActive(false);
        instructions.text = startInstructions;
        inventoryGO.SetActive(false);
    }

    private void OnEnable()
    {
        Weapon_Behavior.ShowBullets += ShowAmmoCounter;
        Inventory.OnEquipped += setCurrentWeapon;
    }

    private void OnDisable()
    {
        Weapon_Behavior.ShowBullets -= ShowAmmoCounter;
        Inventory.OnEquipped -= setCurrentWeapon;
    }

    void Update()
    {
        ShowInstructions();
        ShowInventory();
    }

    private void LateUpdate()
    {
        ShowFireMode();
    }

    void ShowInstructions()
    {
        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (currentWeapon == null)
            {
                instructions.text = instructionsMenu;
                return;
            }
            switch (currentWeapon.WeaponName)
            {
                case e_WeaponName.Revolver:
                    instructions.text = revolverInstructions;
                    break;
                case e_WeaponName.M16:
                    instructions.text = arInstructions;
                    break;
                case e_WeaponName.Glock:
                    instructions.text = glockInstructions;
                    break;
            }
        }
        if(Keyboard.current.iKey.wasReleasedThisFrame)
            instructions.text = startInstructions;
    }

    void ShowAmmoCounter(int ammo)
    {
        counter.text = ammo.ToString();
    }

    void ShowInventory()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            inventoryGO.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if(Keyboard.current.tabKey.wasReleasedThisFrame)
        {
            if (hoveredSlotIndex >= 0)
            {
                inventory.EquipWeapon(hoveredSlotIndex);
                ammoCounter.SetActive(true);
                
            }
            inventoryGO.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void SetHoveredSlot(int slotIndex)
    {
        hoveredSlotIndex = slotIndex;
    }

    public void ShowFireMode()
    {
        if (currentWeapon == null) return;
        switch (currentWeapon.WeaponName)
        {
            default:
                fireMode.text = "";
                break;
            case e_WeaponName.M16:
                switch (inventory.equipedWeapon.GetComponent<AR_Behavior>().FireMode)
                {
                    case e_FireMode.Automatic:
                        fireMode.text = "Auto";
                        break;
                    case e_FireMode.SemiAutomatic:
                        fireMode.text = "Semi";
                        break;
                }
                break;
            case e_WeaponName.Glock:
               Glock_Behavior glock = inventory.equipedWeapon.GetComponent<Glock_Behavior>();
               fireMode.text = $"Next mag: {glock.SelectedCaliberName}";
                break;
        }
    }

    private void setCurrentWeapon(Weapon_Behavior current)
    {
        currentWeapon = current.WeaponData;
        ShowAmmoCounter(current.LoadedBullets);
    }
}
