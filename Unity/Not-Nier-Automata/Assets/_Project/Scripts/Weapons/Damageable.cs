using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : Actor, IInitialize
{
    public int startingHealth = 100;

    [SerializeField, ReadOnly]
    protected int health = 100;

    protected CombatLines combatLines;

    public UnityEvent onDamage;

    public void Initialize()
    {
        health = startingHealth;
        combatLines = GetComponent<CombatLines>();
    }

    public void Damage(int amount)
    {
        health -= amount;

        if (combatLines)
        {
            combatLines.PlayLine(combatLines.damageLines.GetRandomLine());
        }

        onDamage.Invoke();
        Debug.Log($"{gameObject.name} health is {health}");
        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        health += amount;
    }

    public void Restore()
    {
        health = startingHealth;
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} died");
        Destroy(gameObject);
    }
}
