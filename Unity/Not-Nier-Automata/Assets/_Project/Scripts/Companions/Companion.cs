using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Follows a pawn
/// </summary>
public class Companion : Pawn
{
    /// <summary>
    /// Party leader / pawn to follow
    /// </summary>
    protected Pawn leader;

    /// <summary>
    /// Camera manager of the pawn. Only applies if the leader is the player
    /// </summary>
    protected CameraManager cameraManager;

    protected MovementNoise noise;

    public float turnSpeed = 1;

    public Vector3 followOffset;

    protected override void Start()
    {
        base.Start();
        noise = GetComponent<MovementNoise>();
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

    protected override void Update()
    {
        if (leader)
        {
            if (cameraManager)
            {
                Vector3 currentEuler = transform.rotation.eulerAngles;
                Vector3 desiredEuler = cameraManager.cameraBrain.transform.rotation.eulerAngles;
                float currentYaw = currentEuler.y;
                float desiredYaw = desiredEuler.y;
                Vector3 finalEuler = new Vector3(currentEuler.x, Mathf.LerpAngle(currentYaw, desiredYaw, turnSpeed * Time.deltaTime), currentEuler.z);

                Quaternion quaternion = transform.rotation;
                quaternion.eulerAngles = finalEuler;
                transform.rotation = quaternion;

                Vector3 finalPosition = leader.transform.position + quaternion * followOffset;

                if (noise)
                {
                    finalPosition += noise.GetMovementNoise();
                }

                Vector3 updatedPosition = Vector3.Lerp(transform.position, finalPosition, turnSpeed * Time.deltaTime);

                AddMovement(updatedPosition - transform.position);
                
            }
            else
            {
                Vector3 updatedPosition = leader.transform.position + followOffset;

                AddMovement(updatedPosition - transform.position);
            }
        }
    }
}
