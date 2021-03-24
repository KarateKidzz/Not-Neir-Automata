using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls level loading, level transitions, game mode management and more.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    #region Fields

    /// <summary>
    /// The currently running game mode, if any
    /// </summary>
    [SerializeField, ReadOnly]
    private GameMode currentGameMode;

    /// <summary>
    /// Script to handle level loading
    /// </summary>
    [SerializeField]
    private LevelLoader levelLoader;

    /// <summary>
    /// Prefab player controller to spawn when needed
    /// </summary>
    [SerializeField]
    private GameObject playerControllerPrefab;

    [SerializeField]
    private PauseMenu pauseMenu;

    /// <summary>
    /// Spawned player controller
    /// </summary>
    private PlayerController playerController;

    private List<Pawn> allPawns = new List<Pawn>();

    private List<Controller> allControllers = new List<Controller>();

    #endregion

    #region Properties

    public LevelLoader LevelLoader => levelLoader;

    public PlayerController PlayerController => playerController;

    public List<Pawn> AllPawns => allPawns;

    public List<Controller> AllControllers => allControllers;

    #endregion

    #region Unity Functions

    protected override void Awake()
    {
        base.Awake();
        levelLoader.LoadFirstLevel();
    }

    void Update()
    {
        if (currentGameMode)
        {
            currentGameMode.UpdateGameMode();
        }
    }

    #endregion

    #region Methods

    public GameMode GetCurrentGameMode()
    {
        return currentGameMode;
    }

    /// <summary>
    /// Get the currently running game mode. Can't always promise to be valid.
    /// </summary>
    /// <returns></returns>
    public T GetCurrentGameMode<T>() where T : GameMode
    {
        return currentGameMode as T;
    }

    /// <summary>
    /// Set the current game mode.
    /// </summary>
    /// <param name="gameMode"></param>
    public void SetCurrentGameMode(GameMode gameMode)
    {
        RemoveCurrentGameMode();   
        currentGameMode = gameMode;
        currentGameMode.StartGameMode();
    }

    /// <summary>
    /// Remove the current game mode, ready for a new one to take over.
    /// </summary>
    public void RemoveCurrentGameMode()
    {
        if (currentGameMode)
        {
            currentGameMode.EndGameMode();
            currentGameMode = null;
        }
    }

    public void TrySpawnPlayerController()
    {
        if (!playerController)
        {
            if (!playerControllerPrefab)
            {
                Debug.LogError("No player controller prefab");
                return;
            }
            Instantiate(playerControllerPrefab);
        }
    }

    public void SetPlayerController(PlayerController player)
    {
        playerController = player;
    }

    public void ClearPlayerController()
    {
        playerController = null;
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            pauseMenu.Pause();
        }
        else
        {
            pauseMenu.UnPause();
        }
    }

    public bool IsPaused()
    {
        return pauseMenu.gameObject.activeSelf;
    }

    #endregion
}


