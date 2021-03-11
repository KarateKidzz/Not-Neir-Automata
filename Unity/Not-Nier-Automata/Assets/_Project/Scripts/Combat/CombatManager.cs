using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : GameModeUtil
{
    /// <summary>
    /// How many people are attacking the player?
    /// </summary>
    public int NumberOfAttackers => attackers.Count;

    /// <summary>
    /// Returns true if the player is in combat
    /// </summary>
    public bool IsInCombat => NumberOfAttackers > 0;

    List<Pawn> attackers = new List<Pawn>();

    TempoSync tempoSync;

    public override void StartUtil(GameMode gameMode)
    {
        base.StartUtil(gameMode);

        tempoSync = GetComponent<TempoSync>();
    }

    /// <summary>
    /// Checks if the player input was in time with the music. Returns true if not in combat.
    /// </summary>
    /// <returns></returns>
    public bool WasInputInTimeWithMusic()
    {
        if (!IsInCombat)
        {
            return true;
        }

        if (!tempoSync)
        {
            return true;
        }

        return tempoSync.IsInputInTime();
    }

    public void AddAttacker(Pawn attackingPawn)
    {
        attackers.Add(attackingPawn);
        if (NumberOfAttackers > 0)
        {
            StartCombat();
        }
    }

    public void RemoveAttacker(Pawn attackingPawn)
    {
        attackers.Remove(attackingPawn);
    }

    void StartCombat()
    {
        GameMode currentGameMode = GameManager.Instance.GetCurrentGameMode<GameMode>();

        if (currentGameMode)
        {
            if (currentGameMode.Utilities.ContainsKey(typeof(MusicManager)))
            {
                MusicManager musicManager = currentGameMode.Utilities[typeof(MusicManager)] as MusicManager;

                musicManager.StartCombat();
            }
        }
    }
}
