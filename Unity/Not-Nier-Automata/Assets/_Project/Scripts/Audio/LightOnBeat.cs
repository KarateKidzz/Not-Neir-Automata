using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightOnBeat : Actor, IBeginPlay, IEndPlay
{
    public Light controllingLight;

    public float turnOffTime = 0.2f;

    public float onIntensity = 1f;

    public float offIntensity = 0f;

    public void BeginPlay()
    {
        controllingLight.intensity = offIntensity;
        BeatCallbacks.OnBeatChange += OnBeat;
    }

    public void EndPlay(EndPlayModeReason Reason)
    {
        BeatCallbacks.OnBeatChange -= OnBeat;
    }

    void OnBeat(FMOD.Studio.EventInstance instance, FMOD.Studio.TIMELINE_BEAT_PROPERTIES beat)
    {
        controllingLight.intensity = onIntensity;

        DOTween.To(() => controllingLight.intensity, value => controllingLight.intensity = value, offIntensity, turnOffTime);
    }
}
