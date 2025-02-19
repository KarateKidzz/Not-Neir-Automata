﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : Actor, ITick
{
    public Camera managingCamera;

    public GameObject listenerObject;

    public GameObject cameraBrain;

    public Transform target;

    public GameObject thirdPersonCamera;

    public GameObject firstPersonCamera;

    public Cinemachine.CinemachineBrain brain;

    /// <summary>
    /// Cameras that need a follow target
    /// </summary>
    public CinemachineVirtualCamera[] followCameras;

    public void SetFollowTarget(Transform followTarget)
    {
        target = followTarget; 

        for (int i = 0; i < followCameras.Length; i++)
        {
            CinemachineVirtualCamera camera = followCameras[i];
            if (!camera)
            {
                continue;
            }

            camera.Follow = followTarget;
        }
    }

    public Vector3 GetCameraPosition()
    {
        return cameraBrain.transform.position;
    }

    public Quaternion GetCameraRotation()
    {
        return cameraBrain.transform.rotation;
    }

    public void Tick(float DeltaTime)
    {
        if (target && listenerObject && cameraBrain)
        {
            listenerObject.transform.position = target.position;

            listenerObject.transform.rotation = cameraBrain.transform.rotation;
        }
    }

    public void SwitchToFirstPerson()
    {
        thirdPersonCamera.SetActive(false);
        firstPersonCamera.SetActive(true);
    }

    public void SwitchToThirdPerson()
    {
        thirdPersonCamera.SetActive(true);
        firstPersonCamera.SetActive(false);
    }
}
