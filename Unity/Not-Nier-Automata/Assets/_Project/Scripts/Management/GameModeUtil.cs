using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic class that can be attached to game modes to provide additional behaviour like music or combat.
/// </summary>
public class GameModeUtil : Actor
{
    protected GameMode owningGameMode;

    public virtual void StartUtil(GameMode gameMode)
    {
        Debug.Log($"[Game Mode Util : {GetType().FullName}] Started");
        owningGameMode = gameMode;
    }

    public virtual void EndUtil()
    {

    }

    public virtual void UpdateUtil()
    {

    }
}
