using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPlayerController : PlayerController
{
    public bool DebugEnabled { get; protected set; }

    protected Pawn mainPawn;

    protected Pawn spectatorPawn;

    public override void ActivateController()
    {
        base.ActivateController();

        Debug.LogError("ACTIVATE");
    }

    protected override void SetupInput(PlayerInput inputComponent)
    {
        base.SetupInput(inputComponent);

        Debug.Log("Setup Input");

        inputComponent.actions["Debug"].performed += ToggleDebug;
        inputComponent.actions["Debug"].canceled += ToggleDebug;
    }

    protected override void ClearInput(PlayerInput inputComponent)
    {
        base.ClearInput(inputComponent);

        Debug.Log("Clear Input");

        inputComponent.actions["Debug"].performed -= ToggleDebug;
        inputComponent.actions["Debug"].canceled -= ToggleDebug;
    }

    protected void ToggleDebug(InputAction.CallbackContext context)
    {
        Debug.Log("BUTTON");
        if (context.ReadValueAsButton())
        {
            Debug.Log("TOGGLE DEBUG");
            if (DebugEnabled)
            {
                if (mainPawn)
                {
                    Unpossess();
                    Possess(mainPawn);

                    DebugEnabled = false;
                }
                else
                {
                    Debug.LogError("[Debug Player Controller] Can't return to main pawn as it doesn't exist");
                }
            }
            else
            {
                if (spectatorPawn)
                {
                    Unpossess();
                    Possess(spectatorPawn);

                    DebugEnabled = true;
                }
                else
                {
                    if (GameManager.IsValid())
                    {
                        GameMode gameMode = GameManager.Instance.GetCurrentGameMode();

                        if (gameMode)
                        {
                            Debug.Log("spawning spectator");
                            GameObject prefab = GameManager.Instance.GetCurrentGameMode().defaultSpectatorPawnPrefab;

                            if (prefab)
                            {
                                Debug.Log("possessing spectator");
                                GameObject spawnedSpectator = Instantiate(prefab);
                                spectatorPawn = spawnedSpectator.GetComponent<Pawn>();
                                Debug.Assert(spectatorPawn);

                                mainPawn = PossessedPawn;

                                Unpossess();
                                Possess(spectatorPawn);

                                DebugEnabled = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
