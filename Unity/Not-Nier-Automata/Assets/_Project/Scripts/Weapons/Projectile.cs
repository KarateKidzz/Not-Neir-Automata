using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;

    public float maxLifetime = 10f;

    public Vector3 direction;

    protected Weapon firer;

    public void Fire(Weapon weapon, Vector3 fireDirection)
    {
        transform.position = weapon.transform.position;
        firer = weapon;
        direction = fireDirection.normalized;
        StartCoroutine(WaitToDie());
    }

    private void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition += direction * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    IEnumerator WaitToDie()
    {
        if (maxLifetime < 0)
        {
            maxLifetime = 1;
        }

        yield return new WaitForSeconds(maxLifetime);

        Destroy(transform.root.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (firer)
        {
            WeaponUser weaponUser = firer.WeaponUser;

            if (weaponUser)
            {
                if (weaponUser.gameObject != other.gameObject)
                {

                    Damageable hit = other.gameObject.GetComponentInParentThenChildren<Damageable>();

                    if (hit)
                    {
                        Debug.Log("Weapon hit something: " + other.gameObject.transform.root.gameObject.name);
                        weaponUser.RegisterHit(hit);
                    }
                }
            }
        }
    }
}
