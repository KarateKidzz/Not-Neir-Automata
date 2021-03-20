using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    protected int health = 100;

    protected int startingHealth;

    protected CombatLines combatLines;

    public UnityEvent onDamage;

    private void Start()
    {
        startingHealth = health;
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
