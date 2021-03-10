using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    public GameObject cameraManagerPrefab;

    protected CameraManager cameraManager;

    public CameraManager CameraManager => cameraManager;

    public override void ActivateController()
    {
        GameManager.Instance.SetPlayerController(this);

        if (cameraManagerPrefab)
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
    }

    public override void DisableController()
    {
        base.DisableController();

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
    }
}
