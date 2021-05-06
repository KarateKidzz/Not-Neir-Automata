using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DialogueManager : GameModeUtil
{
    class OngoingDialogue
    {
        public DialogueScript script;
        public int currentLine = -1;
        public GameObject currentSpeaker = null;
        public FMODUnity.StudioVoiceEmitter currentVoice = null;
    }

    [EventRef]
    public string dialogueSnapshot;

    FMOD.Studio.EventInstance dialogueSnapshotInstance;

    int dialoguesUsingSnapshot;

    [SerializeField, ReadOnly]
    List<OngoingDialogue> scripts = new List<OngoingDialogue>();

    public override void StartUtil(GameMode gameMode)
    {
        base.StartUtil(gameMode);

        if (!string.IsNullOrEmpty(dialogueSnapshot))
        {
            dialogueSnapshotInstance = RuntimeManager.CreateInstance(dialogueSnapshot);
        }
    }

    public override void EndUtil()
    {
        base.EndUtil();

        if (dialogueSnapshotInstance.isValid())
        {
            dialogueSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            dialogueSnapshotInstance.release();
        }
    }

    public void StartDialogue(DialogueScript script)
    {
        scripts.Add(new OngoingDialogue() { script = script });
        if (script.useDialogueSnapshot)
        {
            dialoguesUsingSnapshot++;
            if (dialogueSnapshotInstance.isValid())
            {
                FMOD.Studio.PLAYBACK_STATE state;
                dialogueSnapshotInstance.getPlaybackState(out state);
                if (dialoguesUsingSnapshot > 0 && state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                {
                    dialogueSnapshotInstance.start();
                }
            }
        }
        Debug.Log($"[Dialogue Manager] Starting script {script}");
    }

    public void SkipAllDialogue()
    {
        Debug.Log($"[Dialogue Manager] Skipping all dialogue");

        toRemove.AddRange(scripts);
    }

    List<OngoingDialogue> toRemove = new List<OngoingDialogue>();

    public override void UpdateUtil()
    {
        foreach (OngoingDialogue dialogue in scripts)
        {
            // wait while someone is speaking
            if (dialogue.currentSpeaker && dialogue.currentVoice && dialogue.currentVoice.IsPlaying())
            {
                continue;
            }

            dialogue.currentSpeaker = null;
            dialogue.currentVoice = null;

            dialogue.currentLine++;

            if (dialogue.currentLine < dialogue.script.lines.Length)
            {
                DialogueLine line = dialogue.script.lines[dialogue.currentLine];

                UniqueAsset uniqueAsset = line.staticSpeaker;
                UniqueAsset runtimeSpeaker = null;
                GameObject speaker;

                if (uniqueAsset != null)
                {
                    runtimeSpeaker = AssetIDs.Instance.GetFirstInstanceOfID(uniqueAsset.ID);
                    speaker = runtimeSpeaker ? runtimeSpeaker.gameObject : null;
                }
                else
                {
                    speaker = line.runtimeSpeaker.gameObject;
                }

                if (speaker == null)
                {
                    Debug.LogError($"[Dialogue Manager] Could not find a speaker for this line. Static Speaker: {uniqueAsset}. Runtime Speaker: {runtimeSpeaker}. Line: {line}");
                    continue;
                }

                dialogue.currentSpeaker = speaker;

                FMODUnity.StudioVoiceEmitter voice = speaker.GetComponent<FMODUnity.StudioVoiceEmitter>();

                dialogue.currentVoice = voice;

                if (voice)
                {
                    Debug.Log($"[Dialogue] {speaker.gameObject.name} is saying \"{line.subtitle}\"");
                    voice.PlayProgrammerSound(line.audioLine);
                }
                else
                {
                    Debug.LogError($"[Dialogue Manager] Can't play line because {speaker.gameObject.name} doesn't have a voice!");
                }
            }
            else
            {
                toRemove.Add(dialogue);
            }
        }

        foreach(OngoingDialogue dialogue in toRemove)
        {
            if (dialogue.script.useDialogueSnapshot)
            {
                dialoguesUsingSnapshot--;
                if (dialoguesUsingSnapshot < 0)
                {
                    dialoguesUsingSnapshot = 0;
                }
                if (dialoguesUsingSnapshot == 0)
                {
                    FMOD.Studio.PLAYBACK_STATE state;
                    dialogueSnapshotInstance.getPlaybackState(out state);
                    if (dialogueSnapshotInstance.isValid() && state != FMOD.Studio.PLAYBACK_STATE.STOPPED)
                    {
                        dialogueSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    }
                }
            }
            scripts.Remove(dialogue);
        }

        toRemove.Clear();
    }
}
