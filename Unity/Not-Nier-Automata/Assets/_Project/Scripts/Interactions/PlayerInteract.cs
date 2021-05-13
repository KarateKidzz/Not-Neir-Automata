using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : Actor, IInitialize
{
    [SerializeField, ReadOnly]
    Interactable currentInteractable;

    InteractionManager interactionManager;

    public void Initialize()
    {
        interactionManager = GameManager.GetGameModeUtil<InteractionManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        Interactable interactable = go.GetComponentInParentThenChildren<Interactable>();
        if (currentInteractable)
        {
            if (interactable)
            {
                currentInteractable = interactable;
            }
        }
        else
        {
            currentInteractable = interactable;
        }

        if (currentInteractable && interactionManager)
        {
            interactionManager.ShowInteract();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParentThenChildren<Interactable>())
        {
            currentInteractable = null;

            if (interactionManager)
            {
                interactionManager.HideInteract();
            }
        }
    }

    public void Interact()
    {
        if (currentInteractable)
        {
            currentInteractable.Interact();
        }
    }
}
