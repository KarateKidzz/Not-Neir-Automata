using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Action Menu", menuName = "Game/Create Dialogue Action Menu")]
public class DialogueActionGoToMenu : DialogueAction
{
    public override void TriggerDialogueAction()
    {
        GameManager.Instance.LevelLoader.LoadScene("Menu");
    }
}
