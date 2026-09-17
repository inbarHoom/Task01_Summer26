using UnityEngine;
using UnityEngine.InputSystem;

public class Combat : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    private Weapon_Behavior currentWeapon;
    
    void setWeapon()
    {
        currentWeapon = inventory.equipedWeapon;
    }

    void Update()
    {
        setWeapon();
        if(currentWeapon == null) return;
        if(Keyboard.current.rKey.wasPressedThisFrame)
            currentWeapon.TryReload();
        else if(Mouse.current.leftButton.isPressed)
            currentWeapon.TryShoot();
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
            currentWeapon.TryReleaseTrigger();
        else if(Keyboard.current.fKey.wasPressedThisFrame)
            currentWeapon.TryUniqueAction();
    }
}
