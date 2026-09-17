using UnityEngine;

public class Glock_Behavior : Weapon_Behavior
{
    private bool shotSingleShot = false;
   private string selectedCaliberName = "9mm";
   int ammoIndex = 0;
    
public string SelectedCaliberName { get => selectedCaliberName; }
    protected override bool canFire()
    {
        if(!base.canFire() || shotSingleShot) return false;
        return true;
    }

    protected override void AfterFire()
    {
        shotSingleShot = true;
    }

    protected override void ReleaseTrigger()
    {
        shotSingleShot = false;
    }

    protected override void UniqueAction()
    {
        ammoIndex++;
        if (ammoIndex >= WeaponData.AmmoOptions.Count)
        {
            ammoIndex = 0;
        }
        
        selectedAmmo = WeaponData.AmmoOptions[ammoIndex];
        selectedCaliberName = selectedAmmo.CaliberName;
    }
}
