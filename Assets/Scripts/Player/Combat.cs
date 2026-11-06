using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class Combat : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    private Weapon_Behavior currentWeapon;
  [SerializeField]  private int health = 100;
  private bool mobileFireHeld;
  
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

    public void StartFireFromUI()
    {
        mobileFireHeld = true;
    }

    public void StopFireFromUI()
    {
        mobileFireHeld = false;
        if(currentWeapon != null) currentWeapon.TryReleaseTrigger();
    }

    void setWeapon()
    {
        currentWeapon = inventory.equipedWeapon;
    }

    void Update()
    {
        setWeapon();
        if(currentWeapon == null) return;
        if (PlatformUI.UseMobileControls)
        {
            if(mobileFireHeld) currentWeapon.TryShoot();
            return;
        }
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

    public int CurrentHealth => health;

    public void RestoreHealth(int savedHealth)
    {
        health = Mathf.Max(1, savedHealth);
    }

    public void ReloadFromUI()
    {
        if(currentWeapon == null) return;
        currentWeapon.TryReload();
    }

    public void FireFromUI()
    {
        if(currentWeapon == null) return;
        currentWeapon.TryShoot();
        
    }

    public void UniqueActionFromUI()
    {
        if(currentWeapon == null) return;
        currentWeapon.TryUniqueAction();

    }
}
