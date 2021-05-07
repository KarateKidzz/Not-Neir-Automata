using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodPlaylist : Actor, IInitialize, IBeginPlay
{
    MusicManager musicManager;

    public TMPro.TextMeshProUGUI titleText;


    public void Initialize()
    {
        musicManager = GameManager.GetGameModeUtil<MusicManager>();
        if (!musicManager)
        {
            Debug.LogWarning("No music manager");
        }
    }

    public void BeginPlay()
    {
        ShowMusicText();
    }

    public void PressPrevious()
    {
        if (musicManager)
        {
            
        }
    }

    public void PressPause()
    {
        if (musicManager)
        {
            musicManager.EventInstance.setPaused(true);
        }
    }

    public void PressPlay()
    {
        if (musicManager)
        {
            musicManager.EventInstance.setPaused(false);
        }
    }

    public void PressNext()
    {
        if (musicManager)
        {

        }
    }

    public void ShowMusicText()
    {
        if (musicManager)
        {
            if (musicManager.EventInstance.getDescription(out FMOD.Studio.EventDescription description) == FMOD.RESULT.OK)
            {
                if (description.getUserProperty("title", out FMOD.Studio.USER_PROPERTY property) == FMOD.RESULT.OK)
                {
                    titleText.SetText(property.stringValue());
                }
            }
        }
    }
}
