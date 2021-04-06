using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : GameModeUtil
{
    class OngoingDialogue
    {
        public DialogueScript script;
        public int currentLine = -1;
        public GameObject currentSpeaker = null;
        public FMODUnity.StudioVoiceEmitter currentVoice = null;
    }

    [SerializeField, ReadOnly]
    List<OngoingDialogue> scripts = new List<OngoingDialogue>();

    public void StartDialogue(DialogueScript script)
    {
        scripts.Add(new OngoingDialogue() { script = script });
        Debug.Log($"[Dialogue Manager] Starting script {script}");
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
                UniqueAsset runtimeSpeaker = AssetIDs.Instance.GetFirstInstanceOfID(uniqueAsset.ID);
                GameObject speaker = null;
                if (uniqueAsset == null)
                {
                    speaker = line.runtimeSpeaker.gameObject;
                }
                else
                {
                    speaker = runtimeSpeaker.gameObject;
                }

                if (speaker == null)
                {
                    continue;
                }

                dialogue.currentSpeaker = speaker;

                FMODUnity.StudioVoiceEmitter voice = speaker.GetComponent<FMODUnity.StudioVoiceEmitter>();

                dialogue.currentVoice = voice;

                if (voice)
                {
                    Debug.Log($"[Dialogue] \"{line.subtitle}\"");
                    voice.PlayProgrammerSound(line.audioLine);
                }
                
            }
            else
            {
                toRemove.Add(dialogue);
            }
        }

        foreach(OngoingDialogue dialogue in toRemove)
        {
            scripts.Remove(dialogue);
        }

        toRemove.Clear();
    }
}
