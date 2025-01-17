﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
public class PlayerController : Controller
{
    public GameObject cameraManagerPrefab;

    [SerializeField]
    protected UnityEngine.InputSystem.PlayerInput playerInput;

    protected CameraManager cameraManager;

    public CameraManager CameraManager => cameraManager;

    public override void ActivateController()
    {
        base.ActivateController();

        GameManager.Instance.SetPlayerController(this);

        if (cameraManagerPrefab && !cameraManager)
        {
            GameObject spawnedCameraManager = Instantiate(cameraManagerPrefab);
            Debug.Assert(spawnedCameraManager);
            cameraManager = spawnedCameraManager.GetComponent<CameraManager>();
            Debug.Assert(cameraManager);
        }

        if (!possessedPawn)
        {
            // Try and find a pawn to possess
            Pawn[] allPawns = FindObjectsOfType<Pawn>();

            for (int i = 0; i < allPawns.Length; i++)
            {
                if (allPawns[i].AutoPossessPlayer)
                {
                    Possess(allPawns[i]);
                    SetupInput(GetPlayerInputComponent());
                    return;
                }
            }
            Debug.Log("Player controller couldn't find pawn to possses");
        }
        else if (cameraManager)
        {
            Transform lookAt = possessedPawn.CameraFollowTarget ? possessedPawn.CameraFollowTarget.transform : possessedPawn.transform;
            cameraManager.SetFollowTarget(lookAt);
        }

        SetupInput(GetPlayerInputComponent());
    }

    public override void DisableController()
    {
        base.DisableController();

        ClearInput(GetPlayerInputComponent());
        GameManager.Instance.ClearPlayerController();
    }

    public override void Possess(Pawn pawnToPossess)
    {
        base.Possess(pawnToPossess);

        if (cameraManager)
        {
            Transform lookAt = pawnToPossess.CameraFollowTarget ? pawnToPossess.CameraFollowTarget.transform : pawnToPossess.transform;
            cameraManager.SetFollowTarget(lookAt);
        }

        pawnToPossess.SetupInput(GetPlayerInputComponent());
    }

    public override void Unpossess()
    {
        if (PossessedPawn)
        {
            PossessedPawn.ClearInput(GetPlayerInputComponent());
        }

        base.Unpossess();
    }

    protected virtual void SetupInput(UnityEngine.InputSystem.PlayerInput inputComponent)
    {

    }

    protected virtual void ClearInput(UnityEngine.InputSystem.PlayerInput inputComponent)
    {

    }

    public UnityEngine.InputSystem.PlayerInput GetPlayerInputComponent()
    {
        return playerInput;
    }
}
