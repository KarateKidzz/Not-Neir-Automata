using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightGameMode : GameMode, IBeginPlay
{
    public DialogueScript endGameScript;

    public void BeginPlay()
    {
        CombatManager combatManager = GetGameModeUtil<CombatManager>();

        if (combatManager)
        {
            combatManager.onEndCombat += OnEndCombat;
        }
    }

    public void OnEndCombat()
    {
        DialogueManager dialogueManager = GetGameModeUtil<DialogueManager>();

        if (dialogueManager)
        {
            dialogueManager.StartDialogue(endGameScript);
        }
    }
}
