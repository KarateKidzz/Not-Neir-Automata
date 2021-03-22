using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : GameModeUtil
{
    [EventRef]
    public string combatMusicEvent;

    EventInstance eventInstance;

    public void StartCombat()
    {
        StopInstance();

        eventInstance = RuntimeManager.CreateInstance(combatMusicEvent);
        StartInstance();
    }

    public void EndCombat()
    {
        StopInstance();
    }

    void StartInstance()
    {
        BeatCallbacks beatCallbacks = GetComponent<BeatCallbacks>();
        beatCallbacks.SetupForEventInstance(eventInstance);

        TempoSync tempoSync = GetComponent<TempoSync>();
        tempoSync.StartTrackingBeats(eventInstance);

        eventInstance.start();
    }

    void StopInstance()
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventInstance.release();
            eventInstance.clearHandle();
        }
    }
}
