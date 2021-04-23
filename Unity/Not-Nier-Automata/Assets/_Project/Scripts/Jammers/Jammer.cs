using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jammer : Interactable, IBeginPlay, IEndPlay
{
    public string unlockPhrase = "1234";

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
        Destory();
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
        
        if (jammerUI)
        {
            jammerUI.DecrementJammerNumber();
        }
        
        onInteract.RemoveListener(OpenJammmerUI);
    }

    void OpenJammmerUI()
    {
        if (jammerUI)
        {
            jammerUI.StartJammerInteract(this);
        }
    }
}
