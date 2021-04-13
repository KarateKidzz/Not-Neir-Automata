using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    Interactable currentInteractable;

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        currentInteractable = go.GetComponent<Interactable>();

        if (currentInteractable)
        {
            Debug.Log("Found interactable");
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
            Debug.Log("Actually Interact");
            currentInteractable.Interact();
        }
    }
}
