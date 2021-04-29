using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField, ReadOnly]
    Interactable currentInteractable;

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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Interactable>())
        {
            currentInteractable = null; 
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
