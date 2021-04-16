using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogueOnStart : Actor, IBeginPlay
{
    public DialogueScript script;

    // Start is called before the first frame update
    public void BeginPlay()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("No game manager");
            return;
        }

        if (!GameManager.Instance.GetCurrentGameMode())
        {
            Debug.LogError("No game mode");
            return;
        }

        DialogueManager dialogueManager = GameManager.Instance.GetCurrentGameMode().GetGameModeUtil<DialogueManager>();

        if (dialogueManager)
        {
            dialogueManager.StartDialogue(script);
        }
    }
}
