using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class DialogueStarter : EventHandler, IBeginPlay
{
    public EmitterGameEvent playEvent;

    public DialogueScript script;

    private void OnEnable()
    {
        ScriptExecution.Register(this);
    }

    private void OnDisable()
    {
        ScriptExecution.Unregister(this);
    }

    public void BeginPlay()
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
            PlayDialogue();
        }
    }

    public void PlayDialogue()
    {
        DialogueManager dialogueManager = GameManager.Instance.GetCurrentGameMode().GetGameModeUtil<DialogueManager>();
        if (dialogueManager != null)
        {
            if (!dialogueManager.IsPlayingDialogueScript(script))
            {
                dialogueManager.StartDialogue(script);
            }
        }
        else
        {
            Debug.LogError("Dialogue manager is null");
        }
    }
}
