using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the top most game object that has extra methods for destroying, updating etc.
/// </summary>
public abstract class Actor : MonoBehaviour
{
    private void OnEnable()
    {
        ScriptExecution.Register(this);
    }

    private void OnDisable()
    {
        ScriptExecution.Unregister(this);
    }

    /// <summary>
    /// Destory this actor and its gameobject
    /// </summary>
    public void Destory()
    {
        if (this is IEndPlay EndPlay)
        {
            EndPlay.EndPlay(EndPlayModeReason.Destroyed);
        }

        Destroy(gameObject);
    }
}
