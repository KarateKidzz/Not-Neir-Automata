using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    /// <summary>
    /// Set if only one gameobject should speak this line. This is for "only the player says this"
    /// </summary>
    public UniqueAsset staticSpeaker;

    /// <summary>
    /// Set if an object in the world should say this line. This is for "this specific NPC in the world says this"
    /// </summary>
    public GuidReference runtimeSpeaker;

    [TextArea]
    public string subtitle;

    public string audioLine;

    public DialogueAction finishedDialogueAction;
}

[CreateAssetMenu(fileName = "Script", menuName = "Game/Create Dialogue Script", order = 1)]
public class DialogueScript : ScriptableObject
{
    public DialogueLine[] lines;

    /// <summary>
    /// Use the dialogue snapshot to duck some sounds and make speech easier to hear
    /// </summary>
    public bool useDialogueSnapshot;
}
