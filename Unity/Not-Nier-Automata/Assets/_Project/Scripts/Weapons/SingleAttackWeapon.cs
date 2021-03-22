using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAttackWeapon : Weapon
{
    public override void AutoAttack(Vector3 direction = default)
    {
        base.AutoAttack(direction);

        autoFire = false;
    }
}
