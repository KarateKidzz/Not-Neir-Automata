using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUser : MonoBehaviour
{
    public Transform leftHandBone;
    public Transform rightHandBone;

    /// <summary>
    /// Is an attack currently happening? Ie, an animation is playing
    /// </summary>
    public bool isAttacking;

    /// <summary>
    /// Are we holding down the fire button and should do automatic fire?
    /// </summary>
    public bool IsFiring
    {
        get
        {
            if(currentlyEquippedWeapon)
            {
                return currentlyEquippedWeapon.AutoFire;
            }
            return false;
        }
    }

    /// <summary>
    /// Prefabs of weapons
    /// </summary>
    public GameObject[] inventoryWeapons;

    public Weapon currentlyEquippedWeapon;

    public bool useCameraAsDirection;

    public void Attack()
    {
        if (currentlyEquippedWeapon)
        {
            currentlyEquippedWeapon.StartAttack();
        }
    }

    /// <summary>
    /// Toggle auto fire. The weapon will keep attacking. Some weapons, like swords, only attack once per input. However, projectiles and "machine guns" and so on will do auto fire.
    /// </summary>
    public void AttackAutomatic()
    {
        if (currentlyEquippedWeapon)
        {
            currentlyEquippedWeapon.AutoAttack();
        }
    }

    public void FinishAttack()
    {
        if (currentlyEquippedWeapon)
        {
            currentlyEquippedWeapon.FinishAttack();
        }
    }

    private void Start()
    {
        if (currentlyEquippedWeapon)
        {
            currentlyEquippedWeapon.Equip(this, useCameraAsDirection);
        }
    }

    public void RegisterHit(Damageable hit)
    {
        hit.Damage(currentlyEquippedWeapon.damage);
    }
}
