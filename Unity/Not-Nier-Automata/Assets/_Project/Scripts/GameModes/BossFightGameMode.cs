using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightGameMode : GameMode, IBeginPlay
{
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
        Debug.LogWarning("end of level!!!");
        GameManager.Instance.LevelLoader.LoadScene("Menu");
    }
}
