using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Defines an object that can be interacted with by the player
/// </summary>
[RequireComponent(typeof(Pawn))]
public class Interactable : MonoBehaviour
{
    protected Pawn owningPawn;

    public Pawn OwningPawn => owningPawn;

    public UnityEvent onInteract;

    private void Awake()
    {
        owningPawn = GetComponent<Pawn>();
    }

    public virtual bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (CanInteract())
        {
            onInteract.Invoke();
        }
    }
}
