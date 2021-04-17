using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    private static bool quitting;

    /// <summary>
    /// If set, will spawn this pawn and make the player posses it. If not set, the player controller will find a pawn marked "auto possess player" in the world
    /// </summary>
    public GameObject defaultPawnPrefab;

    public GameObject defaultSpectatorPawnPrefab;

    /// <summary>
    /// If set, spawns this player controller. Otherwise, spawns the default player controller
    /// </summary>
    public GameObject defaultPlayerControllerPrefab;

    /// <summary>
    /// Utilities attached to this game mode.
    /// </summary>
    public readonly Dictionary<Type, GameModeUtil> Utilities = new Dictionary<Type, GameModeUtil>();

    void OnApplicationQuit()
    {
        quitting = true;
    }

    protected virtual void Awake()
    {
        Debug.Log("[Game Mode] Awake");

        LevelLoader.TryLoadOverworld();

        GameModeUtil[] utils = GetComponents<GameModeUtil>();

        for (int i = 0; i < utils.Length; i++)
        {
            Utilities.Add(utils[i].GetType(), utils[i]);
        }
    }

    protected virtual void Start()
    {
        Debug.Log("[Game Mode] Start");
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
        PlayerController player;

        // If no default player controller, spawn the default one and use that
        if (!defaultPlayerControllerPrefab)
        {
            Debug.Log("[Game Mode] No player controller prefab. Spawning default");
            GameManager.Instance.TrySpawnPlayerController();
            player = GameManager.Instance.PlayerController;
        }
        // Else spawn the player controller
        else
        {
            spawnedPlayer = Instantiate(defaultPlayerControllerPrefab);
            Debug.Assert(spawnedPlayer);
            player = spawnedPlayer.GetComponent<PlayerController>();
            Debug.Assert(player);
        }

        // If there's a default pawn defined, spawn it and possess it
        if (defaultPawnPrefab)
        {
            Debug.Log("[Game Mode] Spawning default pawn");
            spawnedPawn = Instantiate(defaultPawnPrefab);
            Debug.Assert(spawnedPawn);
            Pawn pawn = spawnedPawn.GetComponent<Pawn>();
            Debug.Assert(pawn);
            player.Possess(pawn);
        }
        // Else, assume there is a pawn in the world marked "auto possess player"
        else
        {
            Debug.Log("[Game Mode] Assuming pawn in the world will be possessed. If the player controller doesn't find a pawn, the player will not be able to move");
        }

        ScriptExecution.BeginPlay();
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

    public T GetGameModeUtil<T>() where T : GameModeUtil
    {
        GameModeUtil gameModeUtil;
        if (Utilities.TryGetValue(typeof(T), out gameModeUtil))
        {
            return gameModeUtil as T;
        }
        return null;
    }
}
