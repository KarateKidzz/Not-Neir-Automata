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
            int currentIndex = musicManager.CurrentEventIndex;
            List<string> allEvents = musicManager.AllEvents;
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = allEvents.Count - 1;
            }
            musicManager.PlayEvent(allEvents[currentIndex], true);
            musicManager.CurrentEventIndex = currentIndex;

            ShowMusicText();
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
            int currentIndex = musicManager.CurrentEventIndex;
            List<string> allEvents = musicManager.AllEvents;
            currentIndex++;
            if (currentIndex >= allEvents.Count)
            {
                currentIndex = 0;
            }
            musicManager.PlayEvent(allEvents[currentIndex], true);

            musicManager.CurrentEventIndex = currentIndex;

            ShowMusicText();
        }
    }

    public void ShowMusicText()
    {
        if (musicManager)
        {
            FMOD.Studio.EventDescription description = musicManager.GetDescriptionOfCurrentEvent;
            {
                if (description.getUserProperty("title", out FMOD.Studio.USER_PROPERTY property) == FMOD.RESULT.OK)
                {
                    titleText.SetText(property.stringValue());
                }
            }
        }
    }
}
