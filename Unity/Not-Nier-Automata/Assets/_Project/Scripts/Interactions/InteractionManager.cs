using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : GameModeUtil
{
    public GameObject canvas;

    public override void StartUtil(GameMode gameMode)
    {
        base.StartUtil(gameMode);

        HideInteract();
    }

    public void ShowInteract()
    {
        canvas.SetActive(true);
    }

    public void HideInteract()
    {
        canvas.SetActive(false);
    }
}
