using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using DG.Tweening;

public class JammerUI : GameModeUtil, IInitialize
{
    [ParamRef]
    public string jammerParameter;

    FMOD.Studio.PARAMETER_DESCRIPTION jammerParameterDescription;

    FMOD.DSP channelMixDSP;

    int jammerNumber;

    string currentInput;

    Jammer currentJammer;

    public GameObject uiParent;

    private CursorLockMode previousLockMode = CursorLockMode.None;
    private bool previousCursorVisible = true;

    public override void StartUtil(GameMode gameMode)
    {
        base.StartUtil(gameMode);

        Close();
    }

    public void Initialize()
    {
        RuntimeManager.StudioSystem.getParameterDescriptionByName(jammerParameter, out jammerParameterDescription);

        channelMixDSP.clearHandle();
        if (RuntimeManager.StudioSystem.getBus("bus:/", out FMOD.Studio.Bus masterBus) == FMOD.RESULT.OK)
        {
            masterBus.lockChannelGroup();
            FMOD.RESULT result = masterBus.getChannelGroup(out FMOD.ChannelGroup channelGroup);
            if (result == FMOD.RESULT.OK)
            {
                int numberOfDsp = -1;
                channelGroup.getNumDSPs(out numberOfDsp);

                for (int i = 0; i < numberOfDsp; i++)
                {
                    channelGroup.getDSP(i, out FMOD.DSP dsp);
                    dsp.getType(out FMOD.DSP_TYPE type);
                    if (type == FMOD.DSP_TYPE.CHANNELMIX)
                    {
                        channelMixDSP = dsp;
                        break;
                    }
                }
                if (!channelMixDSP.hasHandle())
                {
                    Debug.LogError("No dsp");
                }
            }
            else
            {
                Debug.LogError("No channel group");
                Debug.LogError(result);
            }
            masterBus.unlockChannelGroup();
        }
        else
        {
            Debug.LogError("No master");
        }
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

    float GetRightChannelVolume()
    {
        float result = 0f;
        if (channelMixDSP.hasHandle())
        {
            channelMixDSP.getParameterFloat((int)FMOD.DSP_CHANNELMIX.GAIN_CH1, out result);
        }
        return result;
    }

    void SetRightChannelVolume(float volume)
    {
        if (channelMixDSP.hasHandle())
        {
            channelMixDSP.setParameterFloat((int)FMOD.DSP_CHANNELMIX.GAIN_CH1, volume);
        }
    }

    public void MuteRightEar()
    {
        if (channelMixDSP.hasHandle())
        {
            DOTween.To(()=> GetRightChannelVolume(), value => SetRightChannelVolume(value), -80f, 2f);
        }
        else
        {
            Debug.LogError("DID NOT GET DSP");
        }
    }

    public void UnmuteRightEar()
    {
        if (channelMixDSP.hasHandle())
        {
            DOTween.To(() => GetRightChannelVolume(), value => SetRightChannelVolume(value), 0f, 2f);
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
