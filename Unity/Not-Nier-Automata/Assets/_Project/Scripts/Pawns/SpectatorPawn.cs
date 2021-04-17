using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpectatorPawn : Pawn, ITick
{
    #region UI

    [Space]

    [SerializeField]
    [Tooltip("Camera rotation by mouse movement is active")]
    private bool _enableRotation = true;

    [SerializeField]
    [Tooltip("Sensitivity of mouse rotation")]
    private float _mouseSense = 1.8f;

    [Space]

    [SerializeField]
    [Tooltip("Camera zooming in/out by 'Mouse Scroll Wheel' is active")]
    private bool _enableTranslation = true;

    [SerializeField]
    [Tooltip("Velocity of camera zooming in/out")]
    private float _translationSpeed = 55f;

    [Space]

    [SerializeField]
    [Tooltip("Camera movement speed")]
    private float _movementSpeed = 10f;

    [SerializeField]
    [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
    private float _boostedSpeed = 50f;

    [Space]

    [SerializeField]
    [Tooltip("Acceleration at camera movement is active")]
    private bool _enableSpeedAcceleration = true;

    [SerializeField]
    [Tooltip("Rate which is applied during camera movement")]
    private float _speedAccelerationFactor = 1.5f;

    #endregion UI    

    private float _currentIncrease = 1;
    private float _currentIncreaseMem = 0;

    Vector2 move;
    Vector2 upDown;
    bool isSprinting;
    Vector2 scroll;
    Vector2 mouseMove;

    public override void SetupInput(PlayerInput inputComponent)
    {
        base.SetupInput(inputComponent);

        inputComponent.actions["Move"].performed += Move;
        inputComponent.actions["VerticalMove"].performed += UpDown;
        inputComponent.actions["Sprint"].performed += Sprint;
        inputComponent.actions["Scroll"].performed += Scroll;
        inputComponent.actions["MouseMove"].performed += MouseMove;
        inputComponent.actions["Screenshot"].performed += Screenshot;

        inputComponent.actions["Move"].canceled += Move;
        inputComponent.actions["VerticalMove"].canceled += UpDown;
        inputComponent.actions["Sprint"].canceled += Sprint;
        inputComponent.actions["Scroll"].canceled += Scroll;
        inputComponent.actions["MouseMove"].canceled += MouseMove;
        inputComponent.actions["Screenshot"].canceled += Screenshot;
    }

    public override void ClearInput(PlayerInput inputComponent)
    {
        base.ClearInput(inputComponent);

        inputComponent.actions["Move"].performed -= Move;
        inputComponent.actions["VerticalMove"].performed -= UpDown;
        inputComponent.actions["Sprint"].performed -= Sprint;
        inputComponent.actions["Scroll"].performed -= Scroll;
        inputComponent.actions["MouseMove"].performed -= MouseMove;
        inputComponent.actions["Screenshot"].performed -= Screenshot;

        inputComponent.actions["Move"].canceled -= Move;
        inputComponent.actions["VerticalMove"].canceled -= UpDown;
        inputComponent.actions["Sprint"].canceled -= Sprint;
        inputComponent.actions["Scroll"].canceled -= Scroll;
        inputComponent.actions["MouseMove"].canceled -= MouseMove;
        inputComponent.actions["Screenshot"].canceled -= Screenshot;
    }

    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void UpDown(InputAction.CallbackContext context)
    {
        upDown = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        scroll = context.ReadValue<Vector2>();
    }

    public void MouseMove(InputAction.CallbackContext context)
    {
        mouseMove = context.ReadValue<Vector2>();
    }

    public void Screenshot(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            string file = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + System.IO.Path.DirectorySeparatorChar + System.Guid.NewGuid().ToString();
            ScreenCapture.CaptureScreenshot(file);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_boostedSpeed < _movementSpeed)
            _boostedSpeed = _movementSpeed;
    }
#endif

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    public void Tick(float DeltaTime)
    {
        if (!IsPossessed())
            return;

        // Translation
        if (_enableTranslation)
        {
            AddMovement(transform.forward * scroll.y * DeltaTime * _translationSpeed);
        }

        // Movement
        Vector3 deltaPosition = Vector3.zero;
        float currentSpeed = isSprinting ? _boostedSpeed : _movementSpeed;
        deltaPosition = new Vector3(move.x, upDown.y, move.y);

        // Calc acceleration
        CalculateCurrentIncrease(deltaPosition != Vector3.zero);

        AddMovement(transform.rotation * deltaPosition * currentSpeed * _currentIncrease); 

        // Rotation
        if (_enableRotation)
        {
            // Pitch
            transform.rotation *= Quaternion.AngleAxis(
                -mouseMove.y * _mouseSense,
                Vector3.right
            );

            // Paw
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + mouseMove.x * _mouseSense,
                transform.eulerAngles.z
            );
        }
    }
}
