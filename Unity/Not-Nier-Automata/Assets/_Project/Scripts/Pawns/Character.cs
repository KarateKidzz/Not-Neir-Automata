using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : Pawn, IPhysicsTick
{
    [Header("Character")]
    [Header("Movement")]
    public float turnSpeed = 10f;
    private Vector2 move;
    private bool isSprinting;
    private Vector3 targetDirection;
    private float velocity;
    private Quaternion freeRotation;

    [Header("Animation")]
    public Animator animator;

    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
    private static readonly int MovingHash = Animator.StringToHash("Moving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int ComboHash = Animator.StringToHash("Combo");
    private static readonly int FailedComboHash = Animator.StringToHash("FailedCombo");

    #region Input

    public override void SetupInput(UnityEngine.InputSystem.PlayerInput inputComponent)
    {
        inputComponent.actions["Move"].performed += Move;
        inputComponent.actions["Sprint"].performed += Sprint;
        inputComponent.actions["Attack1"].performed += Attack;
        inputComponent.actions["Attack2"].performed += CompanionAttack;
        inputComponent.actions["FireRate1"].performed += FireRate1;
        inputComponent.actions["FireRate2"].performed += FireRate2;
        inputComponent.actions["FireRate3"].performed += FireRate3;
        inputComponent.actions["Pause"].performed += Pause;
        inputComponent.actions["Interact"].performed += Interact;

        inputComponent.actions["Move"].canceled += Move;
        inputComponent.actions["Sprint"].canceled += Sprint;
        inputComponent.actions["Attack1"].canceled += Attack;
        inputComponent.actions["Attack2"].canceled += CompanionAttack;
        inputComponent.actions["FireRate1"].canceled += FireRate1;
        inputComponent.actions["FireRate2"].canceled += FireRate2;
        inputComponent.actions["FireRate3"].canceled += FireRate3;
        inputComponent.actions["Pause"].canceled += Pause;
        inputComponent.actions["Interact"].canceled += Interact;
    }

    public override void ClearInput(UnityEngine.InputSystem.PlayerInput inputComponent)
    {
        inputComponent.actions["Move"].performed -= Move;
        inputComponent.actions["Sprint"].performed -= Sprint;
        inputComponent.actions["Attack1"].performed -= Attack;
        inputComponent.actions["Attack2"].performed -= CompanionAttack;
        inputComponent.actions["FireRate1"].performed -= FireRate1;
        inputComponent.actions["FireRate2"].performed -= FireRate2;
        inputComponent.actions["FireRate3"].performed -= FireRate3;
        inputComponent.actions["Pause"].performed -= Pause;
        inputComponent.actions["Interact"].performed -= Interact;

        inputComponent.actions["Move"].canceled -= Move;
        inputComponent.actions["Sprint"].canceled -= Sprint;
        inputComponent.actions["Attack1"].canceled -= Attack;
        inputComponent.actions["Attack2"].canceled -= CompanionAttack;
        inputComponent.actions["FireRate1"].canceled -= FireRate1;
        inputComponent.actions["FireRate2"].canceled -= FireRate2;
        inputComponent.actions["FireRate3"].canceled -= FireRate3;
        inputComponent.actions["Pause"].canceled -= Pause;
        inputComponent.actions["Interact"].canceled -= Interact;
    }

    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (stopMovement)
        {
            return;
        }

        if (context.ReadValueAsButton())
        {
            if (weaponUser)
            {
                if (!weaponUser.isAttacking)
                {
                    animator.SetTrigger(AttackHash);
                    animator.SetBool(ComboHash, false);
                    animator.SetBool(FailedComboHash, false);
                }
                else
                {
                    
                    CombatManager combatManager = GameManager.Instance.GetCurrentGameMode().GetGameModeUtil<CombatManager>();

                    // Allow combos outside of combat scenes
                    if (!combatManager)
                    {
                        animator.SetBool(ComboHash, true);
                        return;
                    }

                    if (combatManager.WasInputInTimeWithMusic())
                    {
                        bool failed = animator.GetBool(FailedComboHash);

                        if (!failed)
                        {
                            animator.SetBool(ComboHash, true);
                        }
                    }
                    else
                    {
                        animator.SetBool(ComboHash, false);
                        animator.SetBool(FailedComboHash, true);
                    }
                }
            }
        }
    }

    public void CompanionAttack(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            foreach(Companion companion in Companions)
            {
                if (companion.WeaponUser)
                {
                    companion.WeaponUser.AttackAutomatic();
                }
            }
        }
        else
        {
            foreach (Companion companion in Companions)
            {
                if (companion.WeaponUser)
                {
                    companion.WeaponUser.FinishAttack();
                }
            }
        }
    }

    public void FireRate1(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            SetFireRate(0);
        }
    }

    public void FireRate2(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            SetFireRate(1);
        }
    }

    public void FireRate3(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            SetFireRate(2);
        }
    }

    void SetFireRate(int rate)
    {
        foreach (Companion companion in Companions)
        {
            if (companion.WeaponUser)
            {
                Debug.Log("Set rate");

                Weapon weapon = companion.WeaponUser.currentlyEquippedWeapon;

                BeatAttackWeapon beatAttackWeapon = weapon ? weapon as BeatAttackWeapon : null;

                if (beatAttackWeapon)
                {
                    Debug.Log("Weapon exists");
                    beatAttackWeapon.fireRate = (BeatFireRate)rate;
                }
            }
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            if  (GameManager.Instance.IsPaused())
            {
                GameManager.Instance.PauseGame(false);
            }
            else
            {
                GameManager.Instance.PauseGame(true);
            }
        }
    }

    PlayerInteract cachePlayerInteract;

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            if (cachePlayerInteract)
            {
                cachePlayerInteract.Interact();
            }
            else
            {
                cachePlayerInteract = GetComponent<PlayerInteract>();
                Debug.Assert(cachePlayerInteract);
                cachePlayerInteract.Interact();
            }
        }
    }

    #endregion

    #region Movement

    public void PhysicsTick(float DeltaTime)
    {
        bool isMoving = move.magnitude > 0.1f;
        float speed = isMoving ? isSprinting ? 1f : 0.5f : 0f;

        speed = Mathf.SmoothDamp(animator.GetFloat(VelocityHash), speed, ref velocity, 0.2f);
        animator.SetFloat(VelocityHash, speed);

        if (isMoving)
        {
            animator.SetBool(MovingHash, true);
        }
        else
        {
            animator.SetBool(MovingHash, false);
        }

        // Update target direction relative to the camera view (or not if the Keep Direction option is checked)
        UpdateTargetDirection();
        if (move != Vector2.zero && targetDirection.magnitude > 0.1f && !stopMovement)
        {
            Vector3 lookDirection = targetDirection.normalized;
            freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
            var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
            var eulerY = transform.eulerAngles.y;

            if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
            var euler = new Vector3(0, eulerY, 0);

            //if (move.y > 0.8f && move.x < 0.2f)
            //{
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * Time.deltaTime);
            //}
        }
    }

    void UpdateTargetDirection()
    {
        PlayerController playerController = owningController as PlayerController;
        Camera camera = playerController && playerController.CameraManager && playerController.CameraManager.managingCamera ? playerController.CameraManager.managingCamera : Camera.main;

        if (!camera)
        {
            return;
        }

        var forward = camera.transform.TransformDirection(Vector3.forward);
        forward.y = 0;

        //get the right-facing direction of the referenceTransform
        var right = camera.transform.TransformDirection(Vector3.right);

        // determine the direction the player will face based on input and the referenceTransform's right and forward directions
        targetDirection = move.x * right + move.y * forward;
    }

    #endregion
}
