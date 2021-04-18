using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class Level01GameMode : GameMode
{
    public PlayableDirector introDirector;

    public GameObject[] introObjects;

    public InputAction skipAction;

    public override void StartGameMode()
    {
        base.StartGameMode();

        introDirector.played += StartPlaying;
        introDirector.stopped += FinishedPlaying;

        introDirector.Play();

        skipAction.Enable();

        skipAction.performed += OnSkipButton;
        skipAction.canceled += OnSkipButton;
    }

    public void OnSkipButton(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            if (introDirector.state == PlayState.Playing)
            {
                introDirector.Stop();
            }
        }    
    }

    void StartPlaying(PlayableDirector director)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void FinishedPlaying(PlayableDirector director)
    {
        if (!quitting)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ScriptExecution.BeginPlay();

            for (int i = 0; i < introObjects.Length; i++)
            {
                Destroy(introObjects[i]);
            }

            skipAction.performed -= OnSkipButton;
            skipAction.canceled -= OnSkipButton;
        }
    }
}
