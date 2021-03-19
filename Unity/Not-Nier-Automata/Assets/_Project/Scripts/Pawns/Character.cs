using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : Pawn
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

    #region Input

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
                    Debug.Log("Doing first combo");
                    animator.SetTrigger(AttackHash);
                }
                else
                {
                    Debug.Log("Checking timing for next combo");
                    GameModeUtil gameModeUtil;
                    if (GameManager.Instance.GetCurrentGameMode<GameMode>().Utilities.TryGetValue(typeof(CombatManager), out gameModeUtil))
                    {
                        if ((gameModeUtil as CombatManager).WasInputInTimeWithMusic())
                        {
                            Debug.Log("Doing next attack");
                            animator.SetTrigger(AttackHash);
                        }
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

    #endregion

    #region Movement

    void FixedUpdate()
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
        Camera camera = playerController && playerController.CameraManager && playerController.CameraManager.managingCamager ? playerController.CameraManager.managingCamager : Camera.main;

        var forward = camera.transform.TransformDirection(Vector3.forward);
        forward.y = 0;

        //get the right-facing direction of the referenceTransform
        var right = camera.transform.TransformDirection(Vector3.right);

        // determine the direction the player will face based on input and the referenceTransform's right and forward directions
        targetDirection = move.x * right + move.y * forward;
    }

    #endregion
}
