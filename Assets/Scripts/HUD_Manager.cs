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
   
    [Header("Mobile")]
    [SerializeField] private Image mobileInventory;

    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private List<GameObject> weaponsPrefabsInv = new List<GameObject>(3);
    [SerializeField] private Button uniqueActionButton;
    [SerializeField] private Sprite m16UniqueAction;
    [SerializeField] private Sprite glockUniqueAction;
    [SerializeField] private Sprite revolverUniqueAction;
    private int previewWeaponIndex = 0;

    void Start()
    {
        ammoCounter.SetActive(false);
        instructions.text = startInstructions;
        inventoryGO.SetActive(false); 
        hurtImage.enabled = false;
        deathScreen.enabled = false;
        finishText.enabled = false;
        mobileInventory.gameObject.SetActive(false);
        foreach (GameObject weapon in weaponsPrefabsInv)
        {
            weapon.SetActive(false);
        }
        
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
        ShowUniqueActionButton();
    }

    private void LateUpdate()
    {
        ShowFireMode();
    }

    void ShowInstructions()
    {
        if(PlatformUI.UseMobileControls)
        {
            instructions.enabled = false;
            return;
        }
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
        /*
        if (PlatformUI.UseMobileControls)
        {
           // Time.timeScale = 0;
           // mobileInventoryGO.SetActive(true);
           return;
        }
        */
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            hoveredSlotIndex = -1;
            inventoryGO.SetActive(true);
          if (!PlatformUI.UseMobileControls)
          {
              Cursor.lockState = CursorLockMode.Locked;
              Cursor.visible = false;
          }
        }

        if(Keyboard.current.tabKey.wasReleasedThisFrame)
        {
            if (hoveredSlotIndex >= 0)
            {
                inventory.EquipWeapon(hoveredSlotIndex);
                ammoCounter.SetActive(true);
                
            }
            inventoryGO.SetActive(false);
         if (!PlatformUI.UseMobileControls)
         {
             Cursor.lockState = CursorLockMode.Locked;
             Cursor.visible = false;
         }
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

    public void ShowUniqueActionButton()
    {
        if(!PlatformUI.UseMobileControls) return;
        if(currentWeapon ==  null)
        {
            uniqueActionButton.gameObject.SetActive(false);
            return;
        }
        uniqueActionButton.gameObject.SetActive(true);
        switch (currentWeapon.WeaponName)
        {
            case e_WeaponName.M16:
                uniqueActionButton.image.sprite = m16UniqueAction;
                break;
            case e_WeaponName.Glock:
                uniqueActionButton.image.sprite =  glockUniqueAction;
                break;
            case e_WeaponName.Revolver:
                uniqueActionButton.image.sprite = revolverUniqueAction;
                break;
        }
    }

    public void ShowMobileInv()
    {
        mobileInventory.gameObject.SetActive(true);
        weaponName.text = inventory.Weapons[previewWeaponIndex].WeaponName.ToString();
        weaponsPrefabsInv[previewWeaponIndex].SetActive(true);
        Time.timeScale = 0;
    }

    public void SwitchPreview(int direction)
    {
        if(weaponsPrefabsInv.Count ==0) return;
        weaponsPrefabsInv[previewWeaponIndex].SetActive(false);
        previewWeaponIndex += direction;
        if (previewWeaponIndex == weaponsPrefabsInv.Count) previewWeaponIndex = 0;
        if(previewWeaponIndex < 0) previewWeaponIndex =  weaponsPrefabsInv.Count - 1;
        ShowMobileInv();
    }

    public void ChoosePreviewWeapon()
    {
        inventory.EquipWeapon(previewWeaponIndex);
        ammoCounter.SetActive(true);
        weaponsPrefabsInv[previewWeaponIndex].SetActive(false);
        mobileInventory.gameObject.SetActive(false);
        Time.timeScale = 1;
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
