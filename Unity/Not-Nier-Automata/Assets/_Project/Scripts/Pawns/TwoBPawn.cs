using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class TwoBPawn : Pawn
{
    public bool lockToCameraForward = false;
    public float turnSpeed = 10f;

    private float turnSpeedMultiplier;
    private bool isSprinting = false;
    public Animator anim;
    private Vector3 targetDirection;
    private Vector2 move;
    private Quaternion freeRotation;
    private float velocityY;
    private float currentSprintSpeed;
    private float sprintVel;

    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    void FixedUpdate()
    {
        bool isMoving = move.magnitude > 0.1f;
        float speed = isMoving ? isSprinting ? 1f : 0.5f : 0f;

        speed = Mathf.SmoothDamp(anim.GetFloat("Velocity Z"), speed, ref velocityY, 0.2f);
        anim.SetFloat("Velocity Z", speed);

        if (isMoving)
        {
            anim.SetBool("Moving", true);
        }
        else
        {
            anim.SetBool("Moving", false);
        }

        //currentSprintSpeed = Mathf.SmoothDamp(anim.GetFloat("AnimationSpeed"), isSprinting ? 2f : 1f, ref sprintVel, 0.2f);
        //anim.SetFloat("AnimationSpeed", currentSprintSpeed);

        // Update target direction relative to the camera view (or not if the Keep Direction option is checked)
        UpdateTargetDirection();
        if (move != Vector2.zero && targetDirection.magnitude > 0.1f)
        {
            Vector3 lookDirection = targetDirection.normalized;
            freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
            var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
            var eulerY = transform.eulerAngles.y;

            if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
            var euler = new Vector3(0, eulerY, 0);

            //if (move.y > 0.8f && move.x < 0.2f)
            //{
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * turnSpeedMultiplier * Time.deltaTime);
            //}
        }
    }

    public virtual void UpdateTargetDirection()
    {
        PlayerController playerController = owningController as PlayerController;
        Camera camera = playerController && playerController.CameraManager && playerController.CameraManager.managingCamager ? playerController.CameraManager.managingCamager : Camera.main;

        turnSpeedMultiplier = 0.8f;
        var forward = camera.transform.TransformDirection(Vector3.forward);
        forward.y = 0;

        //get the right-facing direction of the referenceTransform
        var right = camera.transform.TransformDirection(Vector3.right);

        // determine the direction the player will face based on input and the referenceTransform's right and forward directions
        targetDirection = move.x * right + move.y * forward;
    }
}
