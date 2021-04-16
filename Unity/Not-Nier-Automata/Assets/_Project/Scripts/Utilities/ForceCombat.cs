using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCombat : Actor, IBeginPlay, IEndPlay
{
    Pawn pawn;

    public void BeginPlay()
    {
        pawn = GetComponent<Pawn>();

        if (pawn)
        {
            GameMode currentGameMode = GameManager.Instance.GetCurrentGameMode();

            if (!currentGameMode)
            {
                Debug.Log("No game mode. Can't start combat");
                return;
            }

            CombatManager combatManager = currentGameMode.GetGameModeUtil<CombatManager>();

            if (combatManager)
            {
                combatManager.AddAttacker(pawn);
            }
        }
    }

    public void EndPlay(EndPlayModeReason Reason)
    {
        if (Reason == EndPlayModeReason.ApplicationQuit)
        {
            return;
        }

        if (pawn)
        {
            GameMode currentGameMode = GameManager.Instance.GetCurrentGameMode();

            if (currentGameMode)
            {
                CombatManager combatManager = currentGameMode.GetGameModeUtil<CombatManager>();

                if (combatManager)
                {
                    combatManager.RemoveAttacker(pawn);
                }
            }
        }
        else
        {
            Debug.Log("No PAWN");
        }
    }
}
