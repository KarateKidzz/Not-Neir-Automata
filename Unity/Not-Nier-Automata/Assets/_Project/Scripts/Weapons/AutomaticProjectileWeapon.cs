﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticProjectileWeapon : DelayedAutoWeapon
{
    public GameObject projectile;

    public override void StartAttack(Vector3 direction = default)
    {
        base.StartAttack(direction);

        SpawnProjectile(projectile);
    }
}
