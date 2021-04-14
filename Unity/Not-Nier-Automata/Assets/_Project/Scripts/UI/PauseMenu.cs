using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD;
using FMOD.Studio;
using FMODUnity;

public class PauseMenu : MonoBehaviour
{
    private CursorLockMode previousLockMode = CursorLockMode.None;
    private bool previousCursorVisible = true;

    public GameObject mainPauseButtons;
    public GameObject optionsButtons;

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

        mainPauseButtons.SetActive(true);
        optionsButtons.SetActive(false);

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
        GameManager.Instance.LevelLoader.LoadScene("Menu");

        if (GameManager.Instance.PlayerController)
        {
            UnityEngine.InputSystem.PlayerInput playerInput = GameManager.Instance.PlayerController.GetPlayerInputComponent();

            playerInput.SwitchCurrentActionMap("UI");
        }

        gameObject.SetActive(false);
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Bus pauseBus = RuntimeManager.GetBus("bus:/Pause");

        if (pauseBus.isValid())
        {
            pauseBus.lockChannelGroup();
            pauseBus.setPaused(false);
            pauseBus.unlockChannelGroup();
        }
    }

    public void ShowOptions()
    {
        mainPauseButtons.SetActive(false);
        optionsButtons.SetActive(true);
    }

    public void HideOptions()
    {
        mainPauseButtons.SetActive(true);
        optionsButtons.SetActive(false);
    }

    public void SetMusicVolume(float value)
    {
        VCA musicVCA = RuntimeManager.GetVCA("vca:/MX");

        if (musicVCA.isValid())
        {
            musicVCA.setVolume(value);
        }
    }

    public void SetSoundEffectsVolume(float value)
    {
        VCA soundVCA = RuntimeManager.GetVCA("vca:/SFX");

        if (soundVCA.isValid())
        {
            soundVCA.setVolume(value);
        }
    }

    public void SetDialogueVolume(float value)
    {
        VCA dialogueVCA = RuntimeManager.GetVCA("vca:/DX");

        if (dialogueVCA.isValid())
        {
            dialogueVCA.setVolume(value);
        }
    }

    public void SetMasterVolume(float value)
    {
        Bus masterBus = RuntimeManager.GetBus("bus:/");

        if (masterBus.isValid())
        {
            masterBus.lockChannelGroup();
            masterBus.setVolume(value);
            masterBus.unlockChannelGroup();
        }
    }
}
