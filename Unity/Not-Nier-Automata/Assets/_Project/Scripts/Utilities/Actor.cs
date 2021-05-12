using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the top most game object that has extra methods for destroying, updating etc.
/// </summary>
public abstract class Actor : MonoBehaviour
{
    static protected bool isQuitting { get; private set; }

    private void OnEnable()
    {
        ScriptExecution.Register(this);
    }

    private void OnDisable()
    {
        ScriptExecution.Unregister(this);
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (!isQuitting)
        {
            if (this is IEndPlay EndPlay)
            {
                EndPlay.EndPlay(EndPlayModeReason.Destroyed);
            }
        }
    }

    /// <summary>
    /// Destory this actor and its gameobject
    /// </summary>
    public void Destory()
    {
        Destroy(gameObject);
    }
}
