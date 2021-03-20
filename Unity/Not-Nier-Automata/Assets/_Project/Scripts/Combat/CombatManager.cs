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

    [Range(0f,100f)]
    public float combatLineChance;

    List<Pawn> attackers = new List<Pawn>();

    TempoSync tempoSync;

    public TempoSync TempoSync => tempoSync;

    public override void StartUtil(GameMode gameMode)
    {
        base.StartUtil(gameMode);

        tempoSync = GetComponent<TempoSync>();

        BeatCallbacks.OnBeatChange += OnBeat;
    }

    public override void EndUtil()
    {
        base.EndUtil();

        BeatCallbacks.OnBeatChange -= OnBeat;
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
        int previousAttackers = NumberOfAttackers;
        attackers.Add(attackingPawn);
        if (NumberOfAttackers > 0 && previousAttackers == 0)
        {
            StartCombat();
        }

        CombatLines combatLines = attackingPawn.GetComponent<CombatLines>();
        if (combatLines)
        {
            combatLines.PlayLine(combatLines.enterCombatLines.GetRandomLine());
        }
    }

    public void RemoveAttacker(Pawn attackingPawn)
    {
        attackers.Remove(attackingPawn);
        CombatLines combatLines = attackingPawn.GetComponent<CombatLines>();
        if (combatLines)
        {
            combatLines.PlayLine(combatLines.leaveCombatLines.GetRandomLine());
        }
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

    void SayEnemyCombatLine()
    {
        if (attackers.Count == 0)
        {
            return;
        }

        float random = Random.Range(0f, 100f);

        if (random < combatLineChance)
        {
            int randomEnemy = Random.Range(0, attackers.Count);

            Pawn randomPawn = attackers[randomEnemy];

            if (!randomPawn)
            {
                return;
            }

            CombatLines combatLines = randomPawn.GetComponent<CombatLines>();

            if (!combatLines)
            {
                return;
            }

            combatLines.PlayLine(combatLines.combatLines.GetRandomLine());
        }
    }

    void OnBeat(FMOD.Studio.EventInstance instance, FMOD.Studio.TIMELINE_BEAT_PROPERTIES beat)
    {
        SayEnemyCombatLine();
    }
}
