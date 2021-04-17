using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Level01GameMode : GameMode
{
    public PlayableDirector introDirector;

    public override void StartGameMode()
    {
        base.StartGameMode();

        introDirector.stopped += FinishedPlaying;

        introDirector.Play();
    }

    void FinishedPlaying(PlayableDirector director)
    {
        ScriptExecution.BeginPlay();   
    }
}
