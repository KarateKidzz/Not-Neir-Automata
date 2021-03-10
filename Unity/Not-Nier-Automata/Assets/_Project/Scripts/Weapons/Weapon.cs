using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    WeaponUser weaponUser;

    public string weaponName = "Weapon";

    public int damage = 10;

    public void Equip(WeaponUser user)
    {
        if (weaponUser)
        {
            Debug.LogWarning("There is already a weapon user for this weapon");
            return;
        }

        Debug.Log($"{user.gameObject.name} equipped {weaponName}");
        weaponUser = user;
    }

    public virtual void StartAttack()
    {

    }

    public virtual void FinishAttack()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (weaponUser)
        {
            if (weaponUser.gameObject != collision.gameObject)
            {
                Damageable hit = collision.gameObject.GetComponent<Damageable>();

                if (hit)
                {
                    Debug.Log("Weapon hit something: " + collision.gameObject.transform.root.gameObject.name);
                    weaponUser.RegisterHit(hit);
                }
            }                       
        }        
    }
}
