using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public enum MultiSoundPlayOrder
    {
        Sequential,
        Random,
        First
    }

    [AddComponentMenu("FMOD Studio/FMOD Studio Voice Emitter")]
    public class StudioVoiceEmitter : EventHandler
    {
        #region Default Emitter

        [EventRef]
        public string Event = "";
        public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
        public EmitterGameEvent StopEvent = EmitterGameEvent.None;
        public bool AllowFadeout = true;
        public bool TriggerOnce = false;
        public bool Preload = false;

        /// <summary>
        /// If true and an event is playing, new sounds will stop the current one
        /// </summary>
        public bool OverridePlayingEvents;

        /// <summary>
        /// If true, new sounds won't play if an event is already playing
        /// </summary>
        public bool IgnoreNewPlays;

        protected FMOD.Studio.EventDescription eventDescription;
        public FMOD.Studio.EventDescription EventDescription { get { return eventDescription; } }

        protected FMOD.Studio.EventInstance instance;
        public FMOD.Studio.EventInstance EventInstance { get { return instance; } }

        private bool hasTriggered = false;
        private bool isQuitting = false;
        private bool isOneshot = false;
        private List<ParamRef> cachedParams = new List<ParamRef>();

        private const string SnapshotString = "snapshot";

        public bool IsActive { get; private set; } 

        void Start()
        {
            RuntimeUtils.EnforceLibraryOrder();
            if (Preload)
            {
                Lookup();
                eventDescription.loadSampleData();
                RuntimeManager.StudioSystem.update();
                FMOD.Studio.LOADING_STATE loadingState;
                eventDescription.getSampleLoadingState(out loadingState);
                while (loadingState == FMOD.Studio.LOADING_STATE.LOADING)
                {
                    #if WINDOWS_UWP
                    System.Threading.Tasks.Task.Delay(1).Wait();
                    #else
                    System.Threading.Thread.Sleep(1);
                    #endif
                    eventDescription.getSampleLoadingState(out loadingState);
                }
            }
            HandleGameEvent(EmitterGameEvent.ObjectStart);
        }

        void OnApplicationQuit()
        {
            isQuitting = true;
        }

        void OnDestroy()
        {
            if (!isQuitting)
            {
                HandleGameEvent(EmitterGameEvent.ObjectDestroy);

                if (instance.isValid())
                { 
                    RuntimeManager.DetachInstanceFromGameObject(instance);
                    if (eventDescription.isValid() && isOneshot)
                    {
                        instance.release();
                        instance.clearHandle();
                    }
                } 

                if (Preload)
                {
                    eventDescription.unloadSampleData();
                }
            }
        }

        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (PlayEvent == gameEvent)
            {
                PlayProgrammerInstrument();
            }
            if (StopEvent == gameEvent)
            {
                Stop();
            }
        }

        void Lookup()
        {
            eventDescription = RuntimeManager.GetEventDescription(Event);
        }

        void Play()
        {
            if (TriggerOnce && hasTriggered)
            {
                return;
            }

            if (string.IsNullOrEmpty(Event))
            {
                return;
            }

            cachedParams.Clear();

            if (!eventDescription.isValid())
            {
                Lookup();
            }

            if (!Event.StartsWith(SnapshotString, StringComparison.CurrentCultureIgnoreCase))
            {
                eventDescription.isOneshot(out isOneshot);
            }

            bool is3D;
            eventDescription.is3D(out is3D);

            IsActive = true;

            if (!instance.isValid())
            {
                instance.clearHandle();
            }

            // Let previous oneshot instances play out
            if (isOneshot && instance.isValid())
            {
                instance.release();
                instance.clearHandle();
            }

            if (!instance.isValid())
            {
                eventDescription.createInstance(out instance);

                SetupCallback();

                // Only want to update if we need to set 3D attributes
                if (is3D)
                {
                    var rigidBody = GetComponent<Rigidbody>();
                    var rigidBody2D = GetComponent<Rigidbody2D>();
                    var transform = GetComponent<Transform>();
                    if (rigidBody)
                    {
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody);
                    }
                    else
                    {
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
                    }
                }
            }

            foreach (var cachedParam in cachedParams)
            {
                instance.setParameterByID(cachedParam.ID, cachedParam.Value);
            }

            instance.start();

            hasTriggered = true;
        }

        public void Stop()
        {
            IsActive = false;
            cachedParams.Clear();
            if (instance.isValid())
            {
                instance.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.release();
                instance.clearHandle();
            }
        }

        public void SetParameter(string name, float value, bool ignoreseekspeed = false)
        {
            if (Settings.Instance.StopEventsOutsideMaxDistance && IsActive)
            {
                ParamRef cachedParam = cachedParams.Find(x => x.Name == name);

                if (cachedParam == null)
                {
                    eventDescription.getParameterDescriptionByName(name, out FMOD.Studio.PARAMETER_DESCRIPTION paramDesc);

                    cachedParam = new ParamRef
                    {
                        ID = paramDesc.id,
                        Name = paramDesc.name
                    };
                    cachedParams.Add(cachedParam);
                }

                cachedParam.Value = value;
            }

            if (instance.isValid())
            {
                instance.setParameterByName(name, value, ignoreseekspeed);
            }
        }

        public void SetParameter(FMOD.Studio.PARAMETER_ID id, float value, bool ignoreseekspeed = false)
        {
            if (Settings.Instance.StopEventsOutsideMaxDistance && IsActive)
            {
                ParamRef cachedParam = cachedParams.Find(x => x.ID.Equals(id));

                if (cachedParam == null)
                {
                    eventDescription.getParameterDescriptionByID(id, out FMOD.Studio.PARAMETER_DESCRIPTION paramDesc);

                    cachedParam = new ParamRef
                    {
                        ID = paramDesc.id,
                        Name = paramDesc.name
                    };
                    cachedParams.Add(cachedParam);
                }

                cachedParam.Value = value;
            }

            if (instance.isValid())
            {
                instance.setParameterByID(id, value, ignoreseekspeed);
            }
        }

        public bool IsPlaying()
        {
            if (instance.isValid())
            {
                instance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE playbackState);
                return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
            }
            return false;
        }

        #endregion

        #region Programmer Sound Data

        public string[] programmerSoundKeys;

        public MultiSoundPlayOrder playOrder = MultiSoundPlayOrder.Sequential;

        int currentSoundIndex;

        FMOD.Studio.EVENT_CALLBACK emitterCallback;

        void Awake()
        {
            emitterCallback = new FMOD.Studio.EVENT_CALLBACK(EventCallback);
        }

        void SetupCallback()
        { 
            instance.setCallback(emitterCallback);
        }

        /// <summary>
        /// Play the event and allow a key to be picked from the <see cref="programmerSoundKeys"/> array.
        /// </summary>
        public void PlayProgrammerInstrument()
        {
            if (programmerSoundKeys.Length == 0)
            {
                Debug.LogError("Programmer sound keys is empty. Add at least one value!");
                return;
            }

            string newKey = "";

            switch (playOrder)
            {
                case MultiSoundPlayOrder.Sequential:
                    if (currentSoundIndex >= programmerSoundKeys.Length)
                    {
                        currentSoundIndex = 0;
                    }
                    newKey = programmerSoundKeys[currentSoundIndex];
                    currentSoundIndex++;
                    break;
                case MultiSoundPlayOrder.Random:
                    int randomIndex = UnityEngine.Random.Range(0, programmerSoundKeys.Length);
                    newKey = programmerSoundKeys[randomIndex];
                    break;
                case MultiSoundPlayOrder.First:
                    newKey = programmerSoundKeys[0];
                    break;
            }

            PlayProgrammerSound(newKey);
        }

        /// <summary>
        /// Play ANY key/audio file on this event.
        /// </summary>
        /// <param name="key"></param>
        public void PlayProgrammerSound(string key)
        {
            string newKey = key;

            if (IsPlaying())
            {
                if (OverridePlayingEvents)
                {
                    // If overriding, stop current event
                    Stop();
                }
                else if (IgnoreNewPlays)
                {
                    // Don't play at all if something is playing and we should ignore
                    return;
                }
            }

            if (instance.isValid())
            {
                instance.release();
                instance.clearHandle();
            }

            Play();

            // Pin the string
            GCHandle stringHandle = GCHandle.Alloc(newKey, GCHandleType.Pinned);

            // Set the new user data
            instance.setUserData(GCHandle.ToIntPtr(stringHandle));
        }

        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        static FMOD.RESULT EventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            // Retrieve the user data
            instance.getUserData(out IntPtr stringPtr);

            // Get the string object
            GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
            string key = stringHandle.Target as string;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                    {
                        FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                        var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));

                        if (key.Contains("."))
                        {
                            FMOD.Sound dialogueSound;
                            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
                            if (soundResult == FMOD.RESULT.OK)
                            {
                                parameter.sound = dialogueSound.handle;
                                parameter.subsoundIndex = -1;
                                Marshal.StructureToPtr(parameter, parameterPtr, false);
                            }
                        }
                        else
                        {
                            FMOD.Studio.SOUND_INFO dialogueSoundInfo;
                            var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                            if (keyResult != FMOD.RESULT.OK)
                            {
                                Debug.LogError(keyResult);
                                break;
                            }
                            FMOD.Sound dialogueSound;
                            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                            if (soundResult == FMOD.RESULT.OK)
                            {
                                parameter.sound = dialogueSound.handle;
                                parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                                Marshal.StructureToPtr(parameter, parameterPtr, false);
                            }
                            else
                            {
                                Debug.LogError(soundResult);
                            }
                        }
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                    {
                        var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                        var sound = new FMOD.Sound(parameter.sound);
                        sound.release();

                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                        stringHandle.Free();

                        break;
                    }
            }
            return FMOD.RESULT.OK;
        }

        #endregion
    }
}