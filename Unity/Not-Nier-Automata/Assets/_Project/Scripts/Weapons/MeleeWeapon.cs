using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A melee weapon uses colliders to check if it hit an object.
/// </summary>
public class MeleeWeapon : Weapon
{
    private readonly List<Collider> colliders = new List<Collider>();

    private int collidersEnabled;

    private void Awake()
    {
        Collider[] rootColliders = GetComponents<Collider>();
        Collider[] childColliders = GetComponentsInChildren<Collider>();

        for (int i = 0; i < rootColliders.Length; i++)
        {
            colliders.Add(rootColliders[i]);
        }

        for (int i = 0; i < childColliders.Length; i++)
        {
            colliders.Add(childColliders[i]);
        }

        DisableColliders();
    }

    public void EnableColliders()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = true;
        }

        collidersEnabled++;
    }

    public void DisableColliders()
    {
        collidersEnabled--;

        if (collidersEnabled <= 0)
        {
            collidersEnabled = 0;

            for (int i = 0; i < colliders.Count; i++)
            {
                colliders[i].enabled = false;
            }
        }
    }

    public override void StartAttack()
    {
        base.StartAttack();

        EnableColliders();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();

        DisableColliders();
    }
}
