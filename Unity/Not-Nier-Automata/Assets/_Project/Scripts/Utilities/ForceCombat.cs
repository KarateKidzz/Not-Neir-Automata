using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCombat : MonoBehaviour
{
    Pawn pawn;

    private void Start()
    {
        pawn = GetComponent<Pawn>();

        if (pawn)
        {
            GameMode currentGameMode = GameManager.Instance.GetCurrentGameMode();

            if (!currentGameMode)
            {
                GameMode test = GameManager.Instance.GetCurrentGameMode<GameMode>();

                if (test)
                {
                    Debug.LogError("ERROR");
                }
                

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
}
