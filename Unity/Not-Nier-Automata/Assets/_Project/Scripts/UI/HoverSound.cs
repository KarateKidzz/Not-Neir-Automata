using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverSound : MonoBehaviour, IPointerEnterHandler
{
    public UnityEvent onHover;

    public void OnMouseOver()
    {
        //onHover.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHover.Invoke();
    }
}
