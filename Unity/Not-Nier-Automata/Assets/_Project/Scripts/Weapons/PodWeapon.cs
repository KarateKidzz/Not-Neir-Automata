using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodWeapon : BeatAttackWeapon
{
    public GameObject projectilePrefab;

    public override void StartAttack()
    {
        base.StartAttack();

        SpawnProjectile(projectilePrefab);
    }
}
