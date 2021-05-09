using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCorrectMusic : Interactable
{
    public string wantedEventName = "Exploration";

    public override bool CanInteract()
    {
        MusicManager musicManager = GameManager.GetGameModeUtil<MusicManager>();
        if (musicManager)
        {
            musicManager.GetDescriptionOfCurrentEvent.getUserProperty("title", out FMOD.Studio.USER_PROPERTY property);
            if (property.stringValue() == wantedEventName)
            {
                return true;
            }
            Debug.LogWarning("Wrong event playing: " + property.stringValue());
        }
        return false;
    }
}
