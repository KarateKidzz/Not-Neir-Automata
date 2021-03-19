using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAttackWeapon : Weapon
{
    public override void AutoAttack()
    {
        base.AutoAttack();

        autoFire = false;
    }
}
