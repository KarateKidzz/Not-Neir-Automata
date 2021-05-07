using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public enum MusicState
{
    NotPlaying,
    Exploration,
    Combat
}

public class MusicManager : GameModeUtil, IBeginPlay
{
    [EventRef]
    public string[] combatMusicEvents;

    [EventRef]
    public string[] explorationMusicEvents;

    [SerializeField, ReadOnly]
    protected MusicState currentState;

    public bool playExploreMusic = true;

    EventInstance eventInstance;

    public EventInstance EventInstance => eventInstance;

    EventDescription nextEventDescription;

    public EventDescription GetDescriptionOfCurrentEvent => nextEventDescription;

    public int CurrentEventIndex;

    public List<string> AllEvents => GetAllEvents();

    List<string> GetAllEvents()
    {
        List<string> result = new List<string>();

        result.AddRange(combatMusicEvents);
        result.AddRange(explorationMusicEvents);

        return result;
    }

    public void BeginPlay()
    {
        StartCoroutine(StartExplorationAfterWait(2f));
    }

    public void StartCombat()
    {
        PlayEvent(combatMusicEvents[0]);
        CurrentEventIndex = 0;
    }

    public void EndCombat()
    {
        PlayEvent(explorationMusicEvents[0]);
        CurrentEventIndex = combatMusicEvents.Length;
    }

    public override void EndUtil()
    {
        base.EndUtil();

        StopInstance();
    }

    void StartInstance()
    {
        BeatCallbacks beatCallbacks = GetComponent<BeatCallbacks>();

        if (beatCallbacks)
        {
            beatCallbacks.SetupForEventInstance(eventInstance);
        }
        

        TempoSync tempoSync = GetComponent<TempoSync>();

        if (tempoSync)
        {
            tempoSync.StartTrackingBeats(eventInstance);
        } 

        eventInstance.start();
    }

    void StopInstance()
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
            eventInstance.clearHandle();
        }
    }

    IEnumerator StartExplorationAfterWait(float wait)
    {
        CurrentEventIndex = combatMusicEvents.Length;

        yield return new WaitForSeconds(wait);

        if (!eventInstance.isValid())
        {
            PlayEvent(explorationMusicEvents[0]);
        }
    }

    public void PlayEvent(string eventName, bool stopInstantly = false)
    {
        nextEventDescription.clearHandle();
        nextEventDescription = RuntimeManager.GetEventDescription(eventName);

        if (stopInstantly)
        {
            StopInstance();
        }

        StartCoroutine(WaitForInstanceToEnd(eventName));
    }

    IEnumerator WaitForInstanceToEnd(string nextEvent)
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            PLAYBACK_STATE playbackState;

            do
            {
                eventInstance.getPlaybackState(out playbackState);

                yield return null;
            }
            while (playbackState != PLAYBACK_STATE.STOPPED);

            eventInstance.release();
            eventInstance.clearHandle();
        }

        nextEventDescription.createInstance(out eventInstance);
        StartInstance();

        yield return null;
    }
}
