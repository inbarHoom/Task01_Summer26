using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HUD_Manager : MonoBehaviour
{
    [Header("Ammo Counter")]
    [SerializeField] private GameObject ammoCounter;
    [FormerlySerializedAs("ammoCounter")] [SerializeField] private TextMeshProUGUI counter;
    
    [Header("Firing Mode")]
    [SerializeField] protected TextMeshProUGUI fireMode;
    
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryGO;
    [SerializeField] private Inventory inventory;
    private WeaponSO currentWeapon;
    private int hoveredSlotIndex = -1;
    
    [Header("instructions")]
    [SerializeField] private TextMeshProUGUI instructions;
    string startInstructions = "Hold 'i' to see the instructions";
    string instructionsMenu = "WASD - Movement \nTab - Hold to open invetory, Hover with mouse and release tab to choose weapon \n *Instructions change with each weapon*";
    private string arInstructions = "R - Reload \nLeft Mouse - Shoot \nF - Change firing mode ";
    private string revolverInstructions = "R - Reload \nLeft Mouse - Shoot \nF - Rack the hammer";
    private string glockInstructions = "R - Reload \nLeft Mouse - Shoot \nF - Change ammo type";
    
    [Header("Taking Damage")]
    [SerializeField] private Image hurtImage;
    [SerializeField] private TextMeshProUGUI deathScreen;
    [SerializeField]private float hurtDuration = 0.5f;
    
    [Header("Waves")]
    [SerializeField] private List<Sprite> waves = new();
   [SerializeField] private Image waveIndicator;
   [SerializeField] private TextMeshProUGUI finishText;

    

    void Start()
    {
        ammoCounter.SetActive(false);
        instructions.text = startInstructions;
        inventoryGO.SetActive(false); 
        hurtImage.enabled = false;
        deathScreen.enabled = false;
        finishText.enabled = false;
    }

    private void OnEnable()
    {
        Weapon_Behavior.ShowBullets += ShowAmmoCounter;
        Inventory.OnEquipped += setCurrentWeapon;
        Combat.OnPlayerTakeDamage +=  HurtPlayer;
        Combat.OnDeath += ShowDeathScreen;
        Wave_Manager.OnWaveUpdate += ShowWaveNumber;
        Wave_Manager.OnFinishGame += FinishGame;
    }

    private void OnDisable()
    {
        Weapon_Behavior.ShowBullets -= ShowAmmoCounter;
        Inventory.OnEquipped -= setCurrentWeapon;
        Combat.OnPlayerTakeDamage -=  HurtPlayer; 
        Combat.OnDeath -= ShowDeathScreen;
        Wave_Manager.OnWaveUpdate -= ShowWaveNumber;
        Wave_Manager.OnFinishGame -= FinishGame;
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
            hoveredSlotIndex = -1;
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

    private void HurtPlayer()
    {
        StopAllCoroutines();
        StartCoroutine(HurtFlash());
    }
    private IEnumerator HurtFlash()
    {
        hurtImage.enabled = true;

        yield return new WaitForSeconds(hurtDuration);

        hurtImage.enabled = false;
    }
    void ShowDeathScreen()
    {
        deathScreen.enabled = true;
        hurtImage.enabled = true;
    }

    void ShowWaveNumber(int waveNumber)
    {
        waveIndicator.sprite = waves[waveNumber];
    }

    void FinishGame()
    {
        finishText.enabled = true;
    }
}
