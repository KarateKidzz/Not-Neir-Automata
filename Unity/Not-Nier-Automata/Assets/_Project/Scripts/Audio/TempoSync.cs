using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class TempoSync : MonoBehaviour
{
    /// <summary>
    /// Seconds of leniency between each beat that will register as an "in sync" action
    /// </summary>
    public float leniency = 0.2f;

    EventInstance trackedEventInstance;

    float eventBPM;
    float secondsBetweenBeats;
    int currentBeat;
    int currentBar;

    float currentBeatTime;

    public void StartTrackingBeats(EventInstance eventInstance)
    {
        trackedEventInstance = eventInstance;
        BeatCallbacks.OnBeatChange += OnBeatChange;
    } 

    void OnDestroy()
    {
        BeatCallbacks.OnBeatChange -= OnBeatChange;
    }

    void OnBeatChange(EventInstance eventInstance, TIMELINE_BEAT_PROPERTIES timelineProperties)
    {
        if (eventInstance.handle == trackedEventInstance.handle)
        {
            // Update BPM
            if (timelineProperties.tempo != eventBPM)
            {
                SetBPM(timelineProperties.tempo);
            }

            // Fire "NewBeat"
            if (timelineProperties.beat != currentBeat)
            {
                currentBeat = timelineProperties.beat;
                currentBeatTime = Time.realtimeSinceStartup;
            }
        }
    }

    void SetBPM(float newBPM)
    {
        secondsBetweenBeats = 60 / newBPM;
        eventBPM = newBPM;
    }

    /// <summary>
    /// Checks if the current game time is in time with the music
    /// </summary>
    /// <returns></returns>
    public bool IsInputInTime()
    {
        float currentInputTime = Time.realtimeSinceStartup;

        if (currentInputTime >= currentBeatTime && currentInputTime <= currentBeatTime + leniency)
        {
            return true;
        }
        if (currentInputTime >= currentBeatTime + secondsBetweenBeats - leniency && currentInputTime <= currentBeatTime + secondsBetweenBeats)
        {
            return true;
        }
       
        return false;
    }
}
