using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class JammerUI : GameModeUtil
{
    [ParamRef]
    public string jammerParameter;

    FMOD.Studio.PARAMETER_DESCRIPTION jammerParameterDescription;

    int jammerNumber;

    string currentInput;

    Jammer currentJammer;

    public GameObject uiParent;

    private CursorLockMode previousLockMode = CursorLockMode.None;
    private bool previousCursorVisible = true;

    public override void StartUtil(GameMode gameMode)
    {
        base.StartUtil(gameMode);

        RuntimeManager.StudioSystem.getParameterDescriptionByName(jammerParameter, out jammerParameterDescription);

        Close();
    }

    public void IncrementJammerNumber()
    {
        jammerNumber++;
        SetJammerParameter();
    }

    public void DecrementJammerNumber()
    {
        jammerNumber--;
        if (jammerNumber < 0)
        {
            jammerNumber = 0;
        }
        SetJammerParameter();
    }

    void SetJammerParameter()
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByID(jammerParameterDescription.id, jammerNumber);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError(string.Format(("[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}"), jammerParameterDescription, result));
        }
    }

    public void StartJammerInteract(Jammer jammer)
    {
        currentJammer = jammer;
        Open();
    }

    public void PressButton(int number)
    {
        if (!currentJammer)
        {
            return;
        }

        currentInput += number.ToString();

        if (currentInput == currentJammer.unlockPhrase)
        {
            Debug.Log("[Jammer UI] Input Accepted! Unlocking Jammer");
            UnlockJammer();
        }
    }

    public void PressResetButton()
    {
        currentInput = "";
    }

    public void PressCloseButton()
    {
        Close();
    }

    public void UnlockJammer()
    {
        if (currentJammer)
        {
            currentJammer.DisableJammer();
        }
        
        Close();
    }

    public void Open()
    {
        if (GameManager.IsValid() && GameManager.Instance.PlayerController && GameManager.Instance.PlayerController.CameraManager)
        {
            GameManager.Instance.PlayerController.CameraManager.brain.enabled = false;
        }

        uiParent.SetActive(true);
        PressResetButton();

        if (GameManager.Instance.PlayerController)
        {
            UnityEngine.InputSystem.PlayerInput playerInput = GameManager.Instance.PlayerController.GetPlayerInputComponent();

            playerInput.SwitchCurrentActionMap("UI");
        }

        previousLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        if (GameManager.IsValid() && GameManager.Instance.PlayerController && GameManager.Instance.PlayerController.CameraManager)
        {
            GameManager.Instance.PlayerController.CameraManager.brain.enabled = true;
        }

        PressResetButton();
        uiParent.SetActive(false);
        currentJammer = null;

        if (GameManager.Instance.PlayerController)
        {
            UnityEngine.InputSystem.PlayerInput playerInput = GameManager.Instance.PlayerController.GetPlayerInputComponent();

            playerInput.SwitchCurrentActionMap("Player");
        }

        Cursor.lockState = previousLockMode;
        Cursor.visible = previousCursorVisible;
    }
}
