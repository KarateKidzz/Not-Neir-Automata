using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPlayerController : PlayerController
{
    public bool DebugEnabled { get; protected set; }

    protected Pawn mainPawn;

    protected Pawn spectatorPawn;

    protected override void SetupInput(PlayerInput inputComponent)
    {
        base.SetupInput(inputComponent);

        inputComponent.actions["Debug"].performed += ToggleDebug;
        inputComponent.actions["OpenCompanionUI"].performed += OpenCompanionUI;

        inputComponent.actions["Debug"].canceled += ToggleDebug;
        inputComponent.actions["OpenCompanionUI"].canceled += OpenCompanionUI;

        inputComponent.actions.FindActionMap("UI").FindAction("OpenCompanionUI").performed += OpenCompanionUI;
        inputComponent.actions.FindActionMap("UI").FindAction("OpenCompanionUI").canceled += OpenCompanionUI;
    }

    protected override void ClearInput(PlayerInput inputComponent)
    {
        base.ClearInput(inputComponent);

        inputComponent.actions["Debug"].performed -= ToggleDebug;
        inputComponent.actions["OpenCompanionUI"].performed -= OpenCompanionUI;

        inputComponent.actions["Debug"].canceled -= ToggleDebug;
        inputComponent.actions["OpenCompanionUI"].canceled -= OpenCompanionUI;

        inputComponent.actions.FindActionMap("UI").FindAction("OpenCompanionUI").performed -= OpenCompanionUI;
        inputComponent.actions.FindActionMap("UI").FindAction("OpenCompanionUI").canceled -= OpenCompanionUI;
    }

    protected void ToggleDebug(InputAction.CallbackContext context)
    { 
        if (context.ReadValueAsButton())
        {
            if (DebugEnabled)
            {
                if (mainPawn)
                {
                    Unpossess();
                    Possess(mainPawn);

                    cameraManager.SwitchToThirdPerson();

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

                    cameraManager.SwitchToFirstPerson();

                    DebugEnabled = true;
                }
                else
                {
                    if (GameManager.IsValid())
                    {
                        GameMode gameMode = GameManager.Instance.GetCurrentGameMode();

                        if (gameMode)
                        {
                            GameObject prefab = GameManager.Instance.GetCurrentGameMode().defaultSpectatorPawnPrefab;

                            if (prefab)
                            {
                                GameObject spawnedSpectator = Instantiate(prefab);
                                spectatorPawn = spawnedSpectator.GetComponent<Pawn>();
                                Debug.Assert(spectatorPawn);

                                mainPawn = PossessedPawn;

                                spawnedSpectator.transform.position = cameraManager.GetCameraPosition();
                                spawnedSpectator.transform.rotation = cameraManager.GetCameraRotation();

                                Unpossess();
                                Possess(spectatorPawn);

                                cameraManager.SwitchToFirstPerson();

                                DebugEnabled = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void OpenCompanionUI(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            if (PossessedPawn && PossessedPawn.Companions.Count > 0)
            {
                Companion companion = PossessedPawn.Companions[0];

                if (companion)
                {
                    if (companion.ShowingCompanionUI())
                    {
                        companion.HideCompanionUI();
                    }
                    else
                    {
                        companion.ShowCompanionUI();
                    }
                }
            }
        }
    }
}
