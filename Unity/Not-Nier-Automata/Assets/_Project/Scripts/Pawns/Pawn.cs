using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{ 
    protected Controller owningController;

    /// <summary>
    /// If true, stops the pawn from accepting input from controllers
    /// </summary>
    [Header("Pawn")]
    public bool stopMovement;

    [SerializeField]
    protected bool autoPossessPlayer;

    [SerializeField]
    protected bool autoPossessAI;

    [SerializeField]
    protected GameObject aiControllerPrefab;

    [SerializeField]
    protected GameObject cameraFollowTarget;

    [SerializeField]
    protected GameObject cameraLookAtTarget;

    [SerializeField]
    protected bool autoHideCursorOnPossess;

    public bool AutoPossessPlayer => autoPossessPlayer;

    public GameObject CameraFollowTarget => cameraFollowTarget;

    public GameObject CameraLookAtTarget => cameraLookAtTarget;

    private void Start()
    {
        if (!autoPossessPlayer && autoPossessAI)
        {
            if (!aiControllerPrefab)
            {
                Debug.LogError("Pawn is set to auto possess AI but not valid prefab to spawn");
                return;
            }

            GameObject spawnedController = Instantiate(aiControllerPrefab);
            Controller controller = spawnedController.GetComponent<Controller>();

            if (controller)
            {
                controller.Possess(this);
            }
        }
    }

    public void OnPossessed(Controller possessingController)
    {
        if (owningController)
        {
            Debug.LogWarning("Pawn seems to still have a controller controlling it. Should unpossess the pawn before possessing");
        }

        owningController = possessingController;

        if (possessingController is PlayerController && autoHideCursorOnPossess)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnUnpossessed()
    {
        if (owningController is PlayerController && autoHideCursorOnPossess)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        owningController = null;
    }
}
