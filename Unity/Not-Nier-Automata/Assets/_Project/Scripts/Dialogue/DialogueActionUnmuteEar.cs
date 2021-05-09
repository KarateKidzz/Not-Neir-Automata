using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Action Unmute", menuName = "Game/Create Dialogue Action Unmute", order = 5)]
public class DialogueActionUnmuteEar : DialogueAction
{
    public override void TriggerDialogueAction()
    {
        JammerUI jammerUI = GameManager.GetGameModeUtil<JammerUI>();
        if(jammerUI)
        {
            jammerUI.UnmuteRightEar();
        }
    }
}
