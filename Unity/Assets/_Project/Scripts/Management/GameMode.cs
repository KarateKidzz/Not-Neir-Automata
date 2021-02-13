using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    private static bool quitting;

    public GameObject defaultPawnPrefab;

    public GameObject defaultPlayerControllerPrefab;

    void OnApplicationQuit()
    {
        quitting = true;
    }

    void Awake()
    {
        LevelLoader.TryLoadOverworld();
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
            player.ActivateController();
            player.Possess(pawn);
        }
        else
        {
            Debug.LogWarning("No pawn for the player to possess");
        }
    }

    public virtual void UpdateGameMode()
    {

    }

    public virtual void EndGameMode()
    {

    }
}
