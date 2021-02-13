using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public Camera managingCamager;

    /// <summary>
    /// Cameras that need a follow target
    /// </summary>
    public CinemachineVirtualCamera[] followCameras;

    public void SetFollowTarget(Transform followTarget)
    {
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
}
