using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Jammer : Interactable, IBeginPlay, IEndPlay
{
    public string unlockPhrase = "1234";

    public UnityEvent onUnlock;

    JammerUI jammerUI;

    public override void Initialize()
    {
        base.Initialize();

        jammerUI = GameManager.GetGameModeUtil<JammerUI>();
    }

    public override bool CanInteract()
    {
        return true;
    }

    public void DisableJammer()
    {
        onUnlock?.Invoke();

        if (jammerUI)
        {
            jammerUI.DecrementJammerNumber();
        }

        onInteract.RemoveListener(OpenJammmerUI);
    }

    public void BeginPlay()
    {
        if (jammerUI)
        {
            jammerUI.IncrementJammerNumber();
        }

        onInteract.AddListener(OpenJammmerUI);
    }

    public void EndPlay(EndPlayModeReason reason)
    {
        if (reason == EndPlayModeReason.ApplicationQuit)
        {
            return;
        }


    }

    void OpenJammmerUI()
    {
        if (jammerUI)
        {
            jammerUI.StartJammerInteract(this);
        }
    }
}
