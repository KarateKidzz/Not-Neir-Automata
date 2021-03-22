using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotProjectile : SingleAttackWeapon
{
    public GameObject projectilePrefab;

    public override void StartAttack(Vector3 direction = default)
    {
        base.StartAttack(direction);

        SpawnProjectile(projectilePrefab);
    }
}
