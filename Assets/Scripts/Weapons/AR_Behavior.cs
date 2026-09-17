using UnityEngine;

public class AR_Behavior : Weapon_Behavior
{
    private e_FireMode fireMode = e_FireMode.SemiAutomatic;
    private bool shotSingleBullet;
    [SerializeField] private float secondsBetweenShots = 0.1f;
    private float nextShotTime;
    protected override void UniqueAction()
    {
        SwitchFireMode();
    }

    private void SwitchFireMode()
    {
        switch (fireMode)
        {
            case e_FireMode.Automatic:
                fireMode = e_FireMode.SemiAutomatic;
                break;
            case e_FireMode.SemiAutomatic:
                fireMode = e_FireMode.Automatic;
                break;
        }
    }

    protected override bool canFire()
    {
        if(!base.canFire() || Time.time < nextShotTime) return false;
        if(fireMode == e_FireMode.SemiAutomatic && shotSingleBullet)  return false;
            return true;
    }

   

    override protected void AfterFire()
    {
        shotSingleBullet = true;
        nextShotTime = Time.time + secondsBetweenShots;
    }

    protected override void ReleaseTrigger()
    {
        shotSingleBullet = false;
    }

   
    public e_FireMode FireMode => fireMode;
    
}

public enum e_FireMode
{
    SemiAutomatic,
    Automatic
}
