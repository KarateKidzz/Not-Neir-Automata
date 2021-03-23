using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Repeatidly attacks after a set delay
/// </summary>
public class DelayedAutoWeapon : AutoAttackWeapon
{
    public float attackDelay = 1f;

    protected float nextAttackTime;

    public override void StartAttack(Vector3 direction = default)
    {
        base.StartAttack(direction);

        nextAttackTime = Time.time + attackDelay;
    }

    protected override bool ShouldDoNextAutoAttack()
    {
        return Time.time >= nextAttackTime;
    }
}
