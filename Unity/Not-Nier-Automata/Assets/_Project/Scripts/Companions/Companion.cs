using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Follows a pawn
/// </summary>
[RequireComponent(typeof(Pawn))]
[DisallowMultipleComponent]
public class Companion : Actor, ITick, IInitialize, IEndPlay
{
    /// <summary>
    /// Party leader / pawn to follow
    /// </summary>
    protected Pawn leader;

    /// <summary>
    /// The pawn attached to this companion
    /// </summary>
    protected Pawn owner;

    public Pawn Pawn => owner;

    /// <summary>
    /// Camera manager of the pawn. Only applies if the leader is the player
    /// </summary>
    protected CameraManager cameraManager;

    protected MovementNoise noise;

    public float turnSpeed = 1;

    public Vector3 followOffset;

    /// <summary>
    /// A piece of UI that can be spawned to show more information about the companion
    /// </summary>
    public GameObject companionUIPrefab;

    /// <summary>
    /// The spawned UI, if it exists
    /// </summary>
    protected GameObject spawnedCompanionUI;

    public void Initialize()
    {
        noise = GetComponent<MovementNoise>();
        owner = GetComponent<Pawn>();
    }

    public void EndPlay(EndPlayModeReason Reason)
    {
        if (spawnedCompanionUI)
        {
            Destroy(spawnedCompanionUI);
        }
    }

    /// <summary>
    /// Tells this companion to start following the leader
    /// </summary>
    /// <param name="leader"></param>
    public virtual void Follow(Pawn leader)
    {
        this.leader = leader;

        Controller controller = leader.GetController();
        PlayerController playerController = controller ? controller as PlayerController : null;
        cameraManager = playerController ? playerController.CameraManager : null;

        transform.position = leader.transform.position + followOffset;
    }

    public virtual void StopFollowing()
    {
        leader = null;
    }

    public void Tick(float DeltaTime)
    {
        if (leader)
        {
            if (cameraManager)
            {
                bool leaderIsPossessedByPlayer = leader.IsPossessed() && leader.GetController() is PlayerController;
                Vector3 currentEuler = transform.rotation.eulerAngles;
                Vector3 desiredEuler = leaderIsPossessedByPlayer ? cameraManager.cameraBrain.transform.rotation.eulerAngles : currentEuler;
                float currentYaw = currentEuler.y;
                float desiredYaw = desiredEuler.y;
                Vector3 finalEuler = new Vector3(currentEuler.x, Mathf.LerpAngle(currentYaw, desiredYaw, turnSpeed * DeltaTime), currentEuler.z);

                Quaternion quaternion = transform.rotation;
                quaternion.eulerAngles = finalEuler;
                transform.rotation = quaternion;

                Vector3 finalPosition = leader.transform.position + quaternion * followOffset;

                if (noise)
                {
                    finalPosition += noise.GetMovementNoise();
                }

                Vector3 updatedPosition = Vector3.Lerp(transform.position, finalPosition, turnSpeed * DeltaTime);

                owner.AddMovement(updatedPosition - transform.position);
                
            }
            else
            {
                Vector3 updatedPosition = leader.transform.position + followOffset;

                owner.AddMovement(updatedPosition - transform.position);
            }
        }
    }

    private CursorLockMode previousLockMode = CursorLockMode.None;
    private bool previousCursorVisible = true;

    public void ShowCompanionUI()
    {
        if (!spawnedCompanionUI)
        {
            spawnedCompanionUI = Instantiate(companionUIPrefab);
            Debug.Assert(spawnedCompanionUI);
        }

        spawnedCompanionUI.SetActive(true);

        if (GameManager.IsValid() && GameManager.Instance.PlayerController && GameManager.Instance.PlayerController.CameraManager)
        {
            GameManager.Instance.PlayerController.CameraManager.brain.enabled = false;
        }

        if (GameManager.Instance.PlayerController)
        {
            UnityEngine.InputSystem.PlayerInput playerInput = GameManager.Instance.PlayerController.GetPlayerInputComponent();

            playerInput.SwitchCurrentActionMap("UI");
        }

        previousLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideCompanionUI()
    {
        if (spawnedCompanionUI)
        {
            spawnedCompanionUI.SetActive(false);

            if (GameManager.IsValid() && GameManager.Instance.PlayerController && GameManager.Instance.PlayerController.CameraManager)
            {
                GameManager.Instance.PlayerController.CameraManager.brain.enabled = true;
            }

            if (GameManager.Instance.PlayerController)
            {
                UnityEngine.InputSystem.PlayerInput playerInput = GameManager.Instance.PlayerController.GetPlayerInputComponent();

                playerInput.SwitchCurrentActionMap("Player");
            }

            Cursor.lockState = previousLockMode;
            Cursor.visible = previousCursorVisible;
        }
    }

    public bool ShowingCompanionUI()
    {
        return spawnedCompanionUI && spawnedCompanionUI.activeSelf;
    }
}
