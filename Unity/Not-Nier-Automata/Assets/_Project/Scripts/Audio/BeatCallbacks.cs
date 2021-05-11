using System;
using System.Runtime.InteropServices;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

/// <summary>
/// Handles setting up and receiving beat callbacks from an FMOD Studio Event Emitter.
/// </summary>
public class BeatCallbacks : Actor, IInitialize
{
    EVENT_CALLBACK beatCallback;

    /// <summary>
    /// Callback when the beat or bar changes.
    /// </summary>
    public static Action<EventInstance,TIMELINE_BEAT_PROPERTIES> OnBeatChange;

    /// <summary>
    /// Callback when the marker changes.
    /// </summary>
    public static Action<EventInstance, TIMELINE_MARKER_PROPERTIES> OnMarkerChange;

    public void Initialize()
    { 
        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new EVENT_CALLBACK(BeatEventCallback);

        StudioEventEmitter eventEmitter = GetComponent<StudioEventEmitter>();

        if (!eventEmitter)
        {
            return;
        }

        SetupForEventInstance(eventEmitter.EventInstance);
    }

    /// <summary>
    /// Monitor this event instance for callbacks
    /// </summary>
    /// <param name="eventInstance"></param>
    public void SetupForEventInstance(EventInstance eventInstance)
    {
        eventInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    /// <summary>
    /// Callback from FMOD
    /// </summary>
    /// <param name="type"></param>
    /// <param name="instance"></param>
    /// <param name="parameterPtr"></param>
    /// <returns></returns>
    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instance, IntPtr parameterPtr)
    {
        if (!GameManager.IsValid() || GameManager.Instance.IsPaused())
        {
            return FMOD.RESULT.OK;
        }

        EventInstance eventInstance = new EventInstance(instance);

        switch (type)
        {
            case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));

                    OnBeatChange?.Invoke(eventInstance,parameter);
                }
                break;
            case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                {
                    var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));

                    OnMarkerChange?.Invoke(eventInstance, parameter);
                }
                break;
        }
        
        return FMOD.RESULT.OK;
    }
}
