using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interactable object that can only be interacted with when the player mutes the mutes
/// </summary>
public class InteractableNoMusic : Interactable
{
    [Range(0f,1f)]
    public float volumeThreshold = 0.1f;

    public override bool CanInteract()
    {
        FMOD.Studio.VCA musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/MX");

        if (musicVCA.isValid())
        {
            musicVCA.getVolume(out float volume);

            if (volume <= volumeThreshold)
            {
                return true;
            }
        }

        MusicManager musicManager = GameManager.GetGameModeUtil<MusicManager>();

        if (musicManager)
        {
            musicManager.EventInstance.getPaused(out bool paused);

            return paused;
        }

        return false;

    }
}
