using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class Combat : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    private Weapon_Behavior currentWeapon;
  [SerializeField]  private int health = 100;
    
    public static event Action OnPlayerTakeDamage;
    public static event Action OnDeath;

    private void OnEnable()
    {
        Enemy_Combat.OnHit += PlayerTakeDamage;
    }

    private void OnDisable()
    {
        Enemy_Combat.OnHit -= PlayerTakeDamage;
    }

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
    public void PlayerTakeDamage(int amount)
    {
        health -= amount;
       if(health <= 0)
       {
           Time.timeScale = 0;
           OnDeath.Invoke();
       }
        OnPlayerTakeDamage.Invoke();
    }
}
