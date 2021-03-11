using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUser : MonoBehaviour
{
    public Transform leftHandBone;
    public Transform rightHandBone;

    public bool isAttacking;

    /// <summary>
    /// Prefabs of weapons
    /// </summary>
    public GameObject[] inventoryWeapons;

    public Weapon currentlyEquippedWeapon;

    public void Attack()
    {
        if (currentlyEquippedWeapon)
        {
            currentlyEquippedWeapon.StartAttack();
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
            currentlyEquippedWeapon.Equip(this);
        }
    }

    public void RegisterHit(Damageable hit)
    {
        hit.Damage(currentlyEquippedWeapon.damage);
    }
}
