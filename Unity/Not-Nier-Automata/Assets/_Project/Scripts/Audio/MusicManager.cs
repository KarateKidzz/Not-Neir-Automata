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
    public string combatMusicEvent;

    [EventRef]
    public string explorationMusicEvent;

    [SerializeField, ReadOnly]
    protected MusicState currentState;

    public bool playExploreMusic = true;

    EventInstance eventInstance;

    public void BeginPlay()
    {
        StartCoroutine(StartExplorationAfterWait(2f));
    }

    public void StartCombat()
    {
        StartCoroutine(WaitForInstanceToEnd(MusicState.Combat));
    }

    public void EndCombat()
    {
        StartCoroutine(WaitForInstanceToEnd(MusicState.Exploration));
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
        yield return new WaitForSeconds(wait);

        if (!eventInstance.isValid())
        {
            yield return WaitForInstanceToEnd(MusicState.Exploration);
        }
    }

    IEnumerator WaitForInstanceToEnd(MusicState nextState)
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

        if (nextState == MusicState.Exploration && playExploreMusic)
        {
            eventInstance = RuntimeManager.CreateInstance(explorationMusicEvent);
            StartInstance();
        }
        else if (nextState == MusicState.Combat)
        {
            eventInstance = RuntimeManager.CreateInstance(combatMusicEvent);
            StartInstance();
        }

        yield return null;
    }
}
