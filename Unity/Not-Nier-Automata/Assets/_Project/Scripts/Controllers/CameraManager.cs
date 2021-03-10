using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public Camera managingCamager;

    public GameObject listenerObject;

    public GameObject cameraBrain;

    public Transform target; 

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

    private void Update()
    {
        if (target && listenerObject && cameraBrain)
        {
            listenerObject.transform.position = target.position;

            listenerObject.transform.rotation = cameraBrain.transform.rotation;
        }

        



    }

}
