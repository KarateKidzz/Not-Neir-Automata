using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodPlaylist : Actor, IInitialize
{
    MusicManager musicManager;

    public void Initialize()
    {
        musicManager = GameManager.GetGameModeUtil<MusicManager>();
        if (!musicManager)
        {
            Debug.LogWarning("No music manager");
        }
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
}
