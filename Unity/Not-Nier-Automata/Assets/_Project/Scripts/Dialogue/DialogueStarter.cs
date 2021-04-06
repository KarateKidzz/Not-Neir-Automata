using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class DialogueStarter : EventHandler
{
    public EmitterGameEvent playEvent;

    public DialogueScript script;

    private void Start()
    {
        HandleGameEvent(EmitterGameEvent.ObjectStart);
    }

    private void OnDestroy()
    {
        HandleGameEvent(EmitterGameEvent.ObjectDestroy);
    }

    protected override void HandleGameEvent(EmitterGameEvent gameEvent)
    {
        if (playEvent == gameEvent)
        {
            DialogueManager dialogueManager = GameManager.Instance.GetCurrentGameMode().GetGameModeUtil<DialogueManager>();
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(script);
            }
            else
            {
                Debug.Log("Dialogue is null");
            }
        }
    }
}
