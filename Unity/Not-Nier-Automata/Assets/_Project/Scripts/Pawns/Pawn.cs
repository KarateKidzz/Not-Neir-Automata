using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pawn : Actor, IInitialize, IBeginPlay, IEndPlay, ILateTick
{ 
    protected Controller owningController;

    /// <summary>
    /// If true, stops the pawn from accepting input from controllers
    /// </summary>
    [Header("Pawn")]
    public bool stopMovement;

    [SerializeField]
    protected Faction faction;

    public Faction Faction => faction;

    [SerializeField]
    protected bool autoPossessPlayer;

    public bool AutoPossessPlayer => autoPossessPlayer;

    [SerializeField]
    protected bool autoPossessAI;

    [SerializeField]
    protected GameObject aiControllerPrefab;

    [SerializeField]
    protected GameObject cameraFollowTarget;

    public GameObject CameraFollowTarget => cameraFollowTarget;

    [SerializeField]
    protected GameObject cameraLookAtTarget;

    public GameObject CameraLookAtTarget => cameraLookAtTarget;

    [SerializeField]
    protected bool autoHideCursorOnPossess;

    [SerializeField, ReadOnly]
    protected List<Companion> companions = new List<Companion>();

    public List<Companion> Companions => companions;

    protected WeaponUser weaponUser;

    public WeaponUser WeaponUser => weaponUser;

    public event Action<Controller> OnPossess;

    public event Action<Controller> OnUnpossess;

    /// <summary>
    /// This update's movement
    /// </summary>
    protected Vector3 movementVector;

    #region Unity Methods

    public virtual void Initialize()
    {
        GameManager.Instance.AllPawns.Add(this);

        weaponUser = GetComponent<WeaponUser>();
    }

    public virtual void EndPlay(EndPlayModeReason Reason)
    {
        if (Reason != EndPlayModeReason.ApplicationQuit)
        {
            GameManager.Instance.AllPawns.Remove(this);
        }
    }

    public virtual void BeginPlay()
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

    public virtual void LateTick(float DeltaTime)
    {
        if (movementVector != Vector3.zero)
        {
            transform.position += ConsumeMovement();
        }
    }

    #endregion

    #region Movement

    public void AddMovement(Vector3 movement)
    {
        movementVector += movement;
    }

    public Vector3 ConsumeMovement()
    {
        Vector3 result = movementVector;
        movementVector = Vector3.zero;
        return result;
    }

    #endregion

    #region Pawn

    public Controller GetController()
    {
        return owningController;
    }

    public virtual void OnPossessed(Controller possessingController)
    {
        if (owningController)
        {
            Debug.LogWarning("Pawn seems to still have a controller controlling it. Should unpossess the pawn before possessing");
        }

        owningController = possessingController;

        OnPossess?.Invoke(possessingController);

        if (possessingController is PlayerController && autoHideCursorOnPossess)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public virtual void OnUnpossessed()
    {
        if (owningController is PlayerController && autoHideCursorOnPossess)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        OnUnpossess?.Invoke(owningController);

        owningController = null;
    }

    #endregion

    #region Companions

    public virtual void AddCompanion(Companion companion)
    {
        Debug.Log($"{gameObject.name} added {companion.gameObject.name} as a companion");
        Companions.Add(companion);
        companion.Follow(this);
        if (companion.WeaponUser)
        {
            companion.WeaponUser.useCameraAsDirection = true;
        }
    }

    public virtual void RemoveCompanion(Companion companion)
    {
        Debug.Log($"{gameObject.name} removed {companion.gameObject.name} as a companion");
        companion.StopFollowing();
        if (companion.WeaponUser)
        {
            companion.WeaponUser.useCameraAsDirection = false;
        }
        Companions.RemoveAll(c => c == companion);
    }

    #endregion

    #region Input

    /// <summary>
    /// Called by player controllers to allow a pawn to take action on player input
    /// </summary>
    /// <param name="inputComponent"></param>
    public virtual void SetupInput(UnityEngine.InputSystem.PlayerInput inputComponent)
    {

    }

    /// <summary>
    /// Gives the pawn chance to clear any inputs they added during <see cref="SetupInput(UnityEngine.InputSystem.PlayerInput)"/>
    /// </summary>
    /// <param name="inputComponent"></param>
    public virtual void ClearInput(UnityEngine.InputSystem.PlayerInput inputComponent)
    {

    }

    #endregion
}
