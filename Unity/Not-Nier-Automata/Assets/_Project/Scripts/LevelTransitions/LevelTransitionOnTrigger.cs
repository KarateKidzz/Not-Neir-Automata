using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionOnTrigger : Actor
{
    public string levelToLoad;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ON TRIGGER");
        Debug.Log(other.tag);
        if (other.CompareTag("Player") || (other.attachedRigidbody && other.attachedRigidbody.CompareTag("Player")))
        {
            Debug.Log("IS PLAYER");
            GameManager.Instance.LevelLoader.LoadScene(levelToLoad);
        }
    }
}
