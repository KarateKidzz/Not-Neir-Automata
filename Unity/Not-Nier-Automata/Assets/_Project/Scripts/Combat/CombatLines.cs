using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[Serializable]
public struct VoiceLines
{
    public string[] lines;

    public float waitBetweenLines;

    float lastLineTime;

    /// <summary>
    /// Get a random line and assume it's being spoken straight away. Will sometimes return empty strings if too many lines have been said and there need to be wait
    /// </summary>
    /// <returns></returns>
    public string GetRandomLine()
    {
        float currentTime = Time.time;

        if (lastLineTime + waitBetweenLines > currentTime)
        {
            return string.Empty;
        }

        int randomIndex = UnityEngine.Random.Range(0, lines.Length);
        lastLineTime = Time.time;
        return lines[randomIndex];
    }
}

public class CombatLines : MonoBehaviour
{
    /// <summary>
    /// Emitter to play the lines on
    /// </summary>
    public StudioVoiceEmitter voiceEmitter;

    /// <summary>
    /// Lines when starting a fight like "you're going donw!"
    /// </summary>
    public VoiceLines enterCombatLines;

    /// <summary>
    /// Combat lines like "reloading", "I can do this all day!"
    /// </summary>
    public VoiceLines combatLines;

    /// <summary>
    /// Leave combat lines like "that was easy", "anyone else?!"
    /// </summary>
    public VoiceLines leaveCombatLines;

    /// <summary>
    /// Death lines
    /// </summary>
    public VoiceLines deathLines;

    /// <summary>
    /// Damaged lines like "ugh", "ow"
    /// </summary>
    public VoiceLines damageLines;

    /// <summary>
    /// Grunt lines when attacking "he-ah!"
    /// </summary>
    public VoiceLines gruntLines;

    /// <summary>
    /// Play a voice line
    /// </summary>
    /// <param name="line"></param>
    public void PlayLine(string line)
    {
        Debug.Assert(voiceEmitter);

        if (string.IsNullOrEmpty(line))
        {
            return;
        }
        voiceEmitter.PlayProgrammerSound(line);
    }
}
