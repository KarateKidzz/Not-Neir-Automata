using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class DialogueStarterIfRightEar : DialogueStarter
{
    public DialogueScript scriptIfRightEarUnMuted;

    protected override void HandleGameEvent(EmitterGameEvent gameEvent)
    {
        if (playEvent == gameEvent)
        {
            PlayDialogue();

            DialogueManager dialogueManager = GameManager.Instance.GetCurrentGameMode().GetGameModeUtil<DialogueManager>();
            JammerUI jammerUI = GameManager.GetGameModeUtil<JammerUI>();
            if (dialogueManager != null && jammerUI != null)
            {
                if (jammerUI.IsRightEarMuted())
                {
                    PlayDialogue();
                }
                else
                {
                    if (!dialogueManager.IsPlayingDialogueScript(scriptIfRightEarUnMuted))
                    {
                        dialogueManager.StartDialogue(scriptIfRightEarUnMuted);
                    }
                }
            }
            else
            {
                Debug.LogError("Dialogue manager is null");
            }
        }
    }
}
