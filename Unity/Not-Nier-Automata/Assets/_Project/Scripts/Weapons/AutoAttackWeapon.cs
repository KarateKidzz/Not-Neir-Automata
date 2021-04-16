using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of weapon that keeps firing when AutoAttack is called
/// </summary>
public class AutoAttackWeapon : Weapon, ITick
{
    float timeOfLastAttack;

    public float minWait = 0.1f;

    public override void StartAttack(Vector3 direction = default)
    {
        base.StartAttack(direction);

        timeOfLastAttack = Time.time;
    }

    protected override bool CanSingleFire()
    {
        return Time.time >= timeOfLastAttack + minWait;
    }

    protected virtual bool ShouldDoNextAutoAttack()
    {
        return Time.time >= timeOfLastAttack + minWait;
    }

    public void Tick(float DeltaTime)
    {
        if (autoFire)
        {
            if (ShouldDoNextAutoAttack())
            {
                StartAttack();
            }
        }
    }
}
