using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    private static bool quitting;

    public GameObject defaultPawnPrefab;

    public GameObject defaultPlayerControllerPrefab;

    /// <summary>
    /// Utilities attached to this game mode.
    /// </summary>
    public readonly Dictionary<Type, GameModeUtil> Utilities = new Dictionary<Type, GameModeUtil>();

    void OnApplicationQuit()
    {
        quitting = true;
    }

    void Awake()
    {
        LevelLoader.TryLoadOverworld();

        GameModeUtil[] utils = GetComponents<GameModeUtil>();

        for (int i = 0; i < utils.Length; i++)
        {
            Utilities.Add(utils[i].GetType(), utils[i]);
        }
    }

    protected virtual void Start()
    {
        GameManager.Instance.SetCurrentGameMode(this);
    }

    protected virtual void OnDestroy()
    {
        if (!quitting)
        {
            GameManager.Instance.RemoveCurrentGameMode();
        }
    }

    public virtual void StartGameMode()
    {
        foreach(GameModeUtil util in Utilities.Values)
        {
            util.StartUtil(this);
        }

        GameObject spawnedPawn;
        GameObject spawnedPlayer;
        

        if (!defaultPlayerControllerPrefab)
        {
            Debug.Log("[Game Mode] No player controller prefab. Spawning default");
            GameManager.Instance.TrySpawnPlayerController();
            return;
        }

        spawnedPlayer = Instantiate(defaultPlayerControllerPrefab);

        if (defaultPawnPrefab)
        {
            spawnedPawn = Instantiate(defaultPawnPrefab);
            Pawn pawn = spawnedPawn.GetComponent<Pawn>();
            Debug.Assert(pawn);
            PlayerController player = spawnedPlayer.GetComponent<PlayerController>();
            Debug.Assert(player);
            player.Possess(pawn);
            player.ActivateController();
        }
        else
        {
            Debug.LogWarning("No pawn for the player to possess");
        }
    }

    public virtual void UpdateGameMode()
    {
        foreach (GameModeUtil util in Utilities.Values)
        {
            util.UpdateUtil();
        }
    }

    public virtual void EndGameMode()
    {
        foreach (GameModeUtil util in Utilities.Values)
        {
            util.EndUtil();
        }
    }
}
