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
            GameMode currentGameMode = GameManager.Instance.GetCurrentGameMode<GameMode>();

            if (currentGameMode)
            {
                if (currentGameMode.Utilities.ContainsKey(typeof(CombatManager)))
                {
                    (currentGameMode.Utilities[typeof(CombatManager)] as CombatManager).AddAttacker(pawn);
                }
            }
        }
    }
}
