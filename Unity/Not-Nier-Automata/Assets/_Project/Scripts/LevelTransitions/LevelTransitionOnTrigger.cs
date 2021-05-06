using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionOnTrigger : Actor
{
    public string levelToLoad;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Player") || (other.attachedRigidbody && other.attachedRigidbody.CompareTag("Player")))
        {
            GameManager.Instance.LevelLoader.LoadScene(levelToLoad);
        }
    }
}
