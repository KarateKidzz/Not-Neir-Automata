using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    Interactable currentInteractable;

    public float interactDistance = 15f;

    public Vector3 raycastOffset;

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position + raycastOffset, transform.forward * interactDistance, Color.blue, 0.2f);

        if (Physics.Raycast(transform.position + raycastOffset, transform.forward, out RaycastHit info, interactDistance))
        {
            GameObject go = info.collider.gameObject;
            currentInteractable = go.GetComponent<Interactable>();
        }
    }

    public void Interact()
    {
        Debug.Log("Interact");
        if (currentInteractable)
        {
            Debug.Log("Actually Interact");
            currentInteractable.Interact();
        }
    }
}
