using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FMOD;
using FMOD.Studio;
using FMODUnity;

public class PauseMenu : MonoBehaviour
{
    private CursorLockMode previousLockMode = CursorLockMode.None;
    private bool previousCursorVisible = true;

    private void Awake()
    {
        UnPause();
    }

    public void Pause()
    {
        if (GameManager.Instance.PlayerController)
        {
            UnityEngine.InputSystem.PlayerInput playerInput = GameManager.Instance.PlayerController.GetPlayerInputComponent();

            playerInput.SwitchCurrentActionMap("UI");
        }

        gameObject.SetActive(true);
        Time.timeScale = 0;

        previousLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Bus pauseBus = RuntimeManager.GetBus("bus:/Pause");

        if (pauseBus.isValid())
        {
            pauseBus.lockChannelGroup();
            pauseBus.setPaused(true);
            pauseBus.unlockChannelGroup();
        }

    }

    public void UnPause()
    {
        if (GameManager.Instance.PlayerController)
        {
            UnityEngine.InputSystem.PlayerInput playerInput = GameManager.Instance.PlayerController.GetPlayerInputComponent();

            playerInput.SwitchCurrentActionMap("Player");
        }

        gameObject.SetActive(false);
        Time.timeScale = 1;

        Cursor.lockState = previousLockMode;
        Cursor.visible = previousCursorVisible;

        Bus pauseBus = RuntimeManager.GetBus("bus:/Pause");

        if (pauseBus.isValid())
        {
            pauseBus.lockChannelGroup();
            pauseBus.setPaused(false);
            pauseBus.unlockChannelGroup();
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
