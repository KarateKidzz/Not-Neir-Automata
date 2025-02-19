﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Defines an object that can be interacted with by the player
/// </summary>
[DisallowMultipleComponent]
public class Interactable : Actor, IInitialize
{
    protected Pawn owningPawn;

    public Pawn OwningPawn => owningPawn;

    public UnityEvent onInteract;

    public UnityEvent onFailedInteraction;

    public virtual void Initialize()
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
        else
        {
            onFailedInteraction.Invoke();
        }
    }
}
