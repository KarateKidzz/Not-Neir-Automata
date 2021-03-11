using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : GameModeUtil
{
    /// <summary>
    /// How many people are attacking the player?
    /// </summary>
    public int NumberOfAttackers { get; private set; }

    /// <summary>
    /// Checks if the player input was in time with the music. Returns true if not in combat.
    /// </summary>
    /// <returns></returns>
    public bool WasInputInTimeWithMusic()
    {
        return false;
    }
}
