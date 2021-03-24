using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCombat : MonoBehaviour
{
    Pawn pawn;

    bool quitting;

    private void OnApplicationQuit()
    {
        quitting = true;
    }

    private void OnEnable()
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

    private void OnDisable()
    {
        if (quitting)
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
