using System;
using System.Runtime.InteropServices;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

/// <summary>
/// Handles setting up and receiving beat callbacks from an FMOD Studio Event Emitter.
/// </summary>
public class BeatCallbacks : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    EVENT_CALLBACK beatCallback;

    public StudioEventEmitter eventEmitter;

    /// <summary>
    /// Callback when the beat or timeline marker changes.
    /// </summary>
    public static Action<EventInstance,TIMELINE_BEAT_PROPERTIES> OnBeatChange;

    private void Start()
    {
        EventInstance eventInstance = eventEmitter.EventInstance;


        timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new EVENT_CALLBACK(BeatEventCallback);

        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        // Pass the object through the userdata of the instance
        eventInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        eventInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2, 10, 400, 100));
            GUILayout.Box(string.Format("Current Bar = {0}, Last Marker = {1}", timelineInfo.currentMusicBar, (string)timelineInfo.lastMarker));
        GUILayout.EndArea();
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instance, IntPtr parameterPtr)
    {
        EventInstance eventInstance = new EventInstance(instance);
        
        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = eventInstance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                        
                        if (timelineInfo.currentMusicBar != parameter.beat)
                        {
                            OnBeatChange?.Invoke(eventInstance,parameter);
                        }

                        timelineInfo.currentMusicBar = parameter.bar;
                    }
                    break;
                case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
}
