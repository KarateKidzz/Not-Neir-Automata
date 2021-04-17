using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using FMODUnity;
using FMOD.Studio;

public class Level01GameMode : GameMode
{
    public PlayableDirector introDirector;

    public GameObject[] introObjects;

    [EventRef]
    public string musicEvent;

    [EventRef]
    public string snapshotEvent;

    EventInstance musicInstance;

    EventInstance snapshotInstance;

    public override void StartGameMode()
    {
        base.StartGameMode();

        introDirector.played += StartPlaying;
        introDirector.stopped += FinishedPlaying;

        introDirector.Play();
    }

    void StartPlaying(PlayableDirector director)
    {
        //musicInstance = RuntimeManager.CreateInstance(musicEvent);
        //musicInstance.start();

        //snapshotInstance = RuntimeManager.CreateInstance(snapshotEvent);
        //snapshotInstance.start();
    }

    void FinishedPlaying(PlayableDirector director)
    {
        //musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //musicInstance.release();

        //snapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //snapshotInstance.release();

        ScriptExecution.BeginPlay();

        for (int i = 0; i < introObjects.Length; i++)
        {
            Destroy(introObjects[i]);
        }
    }
}
