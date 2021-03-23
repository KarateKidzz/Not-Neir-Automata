using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WeaponSounds : MonoBehaviour
{
    [EventRef]
    public string swingSoundEvent;

    public void Swing()
    {
        if (!string.IsNullOrEmpty(swingSoundEvent))
        {
            RuntimeManager.PlayOneShotAttached(swingSoundEvent, gameObject);
        }
    }
}
