using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateOnKeypress : MonoBehaviour
{
    public KeyCode ActivationKey = KeyCode.LeftControl;
    public int PriorityBoostAmount = 10;
    public GameObject Reticle;

    Cinemachine.CinemachineVirtualCameraBase vcam;
    bool boosted = false;

    void Start()
    {
        vcam = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
    }

    public void LeftControl(InputAction.CallbackContext context)
    {
        bool pressed = context.ReadValueAsButton();

        if (vcam != null)
        {
            if (pressed)
            {
                if (!boosted)
                {
                    vcam.Priority += PriorityBoostAmount;
                    boosted = true;
                }
            }
            else if (boosted)
            {
                vcam.Priority -= PriorityBoostAmount;
                boosted = false;
            }
        }
    }

    void Update()
    {
        if (Reticle != null)
            Reticle.SetActive(boosted);
    }
}
