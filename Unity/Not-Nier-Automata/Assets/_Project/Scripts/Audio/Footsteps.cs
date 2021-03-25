using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FMODUnity;

public class Footsteps : MonoBehaviour
{
    [EventRef]
    public string footstepEvent;
    public void PlayFootstep()
    {
        RuntimeManager.PlayOneShotAttached(footstepEvent, gameObject);
    }
}
