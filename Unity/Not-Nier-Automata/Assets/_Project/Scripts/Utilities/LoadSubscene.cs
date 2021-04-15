using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSubscene : MonoBehaviour
{
    public string[] Scenes;

    private void Awake()
    {
        if (Scenes.Length == 0)
        {
            Debug.LogWarning("Empty scene loader");
            return;
        }

        Debug.Log($"[Load Subscene] Loading {Scenes.Length} scenes");

        for (int i = 0; i < Scenes.Length; i++)
        {
            string currentScene = Scenes[i];
            if (!SceneManager.GetSceneByName(currentScene).IsValid())
            {
                SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
                if (!SceneManager.GetSceneByName(currentScene).IsValid())
                {
                    Debug.LogError($"[Load Subscene] Failed to load scene {currentScene}");
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (enabled)
        {
            for (int i = 0; i < Scenes.Length; i++)
            {
                Debug.Log($"[Load Subscene] Unloading {Scenes[i]}");
                SceneManager.UnloadSceneAsync(Scenes[i]);
            }
        }
    }
}
