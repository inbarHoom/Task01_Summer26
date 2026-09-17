using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class Weapon_Behavior : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponData;
    [SerializeField] private GameObject muzzlePoint;
    protected AmmoSO selectedAmmo;
    private  AmmoSO loadedAmmo;
    public static event Action<int> ShowBullets;
    

    private int loadedBullets = 0;

    private bool isReloading;

    protected virtual void Awake()
    {
        selectedAmmo = weaponData.AmmoOptions[0];
        loadedAmmo = selectedAmmo;
    }

    public void TryShoot()
    {
        Shoot();
    }

    protected virtual void Shoot()
    {
       if (!canFire()) return; 
       GameObject bulletObject = Instantiate(loadedAmmo.BulletPrefab, muzzlePoint.transform.position, muzzlePoint.transform.rotation);
       Bullet_Controller bullet = bulletObject.GetComponent<Bullet_Controller>();
       bullet.Initialize(loadedAmmo);
       loadedBullets--;
       ShowBullets.Invoke(loadedBullets);
       AfterFire();
        
    }
    protected virtual void AfterFire()
    {
        
    }

    public void TryReleaseTrigger()
    {
        ReleaseTrigger();
    }
    protected virtual void ReleaseTrigger()
    {
        
    }

    protected virtual bool canFire()
    {
       return loadedBullets > 0;
    }

    public void TryReload()
    {
        Reload();
        isReloading = false;
    }

    protected virtual void Reload()
    {
        if (!isReloading)
        {
            isReloading = true;
            loadedAmmo = selectedAmmo;
            LoadedBullets = weaponData.Capacity;
            ShowBullets.Invoke(loadedBullets);
            AfterReload();
        }
    }

    protected virtual void AfterReload()
    {
        
    }

    public void TryUniqueAction()
    {
        UniqueAction();
    }

    protected virtual void UniqueAction()
    {
        
    }

    public int LoadedBullets
    {
        get { return loadedBullets; }
        set { loadedBullets = value; }
    }

    public WeaponSO WeaponData => weaponData;
}
