using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jammer : Interactable, IBeginPlay, IEndPlay
{
    public string unlockPhrase = "1234";

    public override bool CanInteract()
    {
        return true;
    }

    public void DisableJammer()
    {
        Destroy(gameObject);
    }

    public void BeginPlay()
    {
        onInteract.AddListener(OpenJammmerUI);
    }

    public void EndPlay(EndPlayModeReason reason)
    {
        onInteract.RemoveListener(OpenJammmerUI);
    }

    void OpenJammmerUI()
    {
        GameMode gameMode;
        gameMode = GameManager.Instance.GetCurrentGameMode();

        if (!gameMode)
        {
            return;
        }

        JammerUI jammerUI;
        jammerUI = gameMode.GetGameModeUtil<JammerUI>();

        if (!jammerUI)
        {
            return;
        }

        jammerUI.StartJammerInteract(this);
    }
}
