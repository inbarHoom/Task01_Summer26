using UnityEngine;

public class Revolver_Behavior : Weapon_Behavior
{
    bool isHammerRacked = false;
    [SerializeField] private Animator revolverAnim;
    protected override bool canFire()
    {
        return base.canFire() && isHammerRacked;
    }

    protected override void AfterFire()
    {
        isHammerRacked = false;
        revolverAnim.Play("ShootRev");
    }

    protected override void AfterReload()
    {
        RackHammer();
    }

    protected override void UniqueAction()
    {
        RackHammer();
    }

    private void RackHammer()
    {
        isHammerRacked = true;
        revolverAnim.Play("RackHammer");
    }
    
}
