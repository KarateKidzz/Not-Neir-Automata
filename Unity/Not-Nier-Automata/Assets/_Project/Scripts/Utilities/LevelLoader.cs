using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    /// <summary>
    /// Overworld level name
    /// </summary>
    private static readonly string OverworldSceneName = "Overworld";

    /// <summary>
    /// Name of the main menu scene name
    /// </summary>
    private static readonly string MainMenuSceneName = "Menu";

    /// <summary>
    /// Tries to load the overworld scene, if it's not already loaded.
    /// </summary>
    public static void TryLoadOverworld()
    {
        Scene OverworldScene = SceneManager.GetSceneByName(OverworldSceneName);

        if (!OverworldScene.isLoaded)
        {
            SceneManager.LoadScene(OverworldSceneName, LoadSceneMode.Additive);
        }
    }

    /// <summary>
    /// If starting a new game and the overworld is the only level, will load the main menu.
    /// </summary>
    public void LoadFirstLevel()
    {
        // If we are the only loaded scene, load the main menu
        // Don't worry about UI transition and the LoadMenu method
        // We just need to get some content on screen
        if (SceneManager.sceneCount == 1)
        {
            //SceneManager.LoadScene(MainMenuSceneName, LoadSceneMode.Additive);
            //StartCoroutine(SetActiveAfterLoaded(MainMenuSceneName));
            StartCoroutine(LoadSceneAndUnload(MainMenuSceneName));
        }
    }

    /// <summary>
    /// Loads the menu scene and unloads any scenes that should be removed.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Scene ActiveScene = SceneManager.GetActiveScene();

        // If already loaded
        if (ActiveScene.name == sceneName)
        {
            return;
        }

        bool inOverworld = ActiveScene.name == OverworldSceneName;

        StartCoroutine(LoadSceneAndUnload(sceneName, !inOverworld ? SceneManager.GetActiveScene().name : null));
    }

    /// <summary>
    /// Handles the complete unload and load process.
    /// </summary>
    /// <param name="LoadSceneName"></param>
    /// <param name="UnloadSceneName"></param>
    /// <returns></returns>
    IEnumerator LoadSceneAndUnload(string LoadSceneName, string UnloadSceneName = null)
    {
        if (UnloadSceneName != null)
        {
            AsyncOperation UnloadOperation = SceneManager.UnloadSceneAsync(UnloadSceneName);
            while (!UnloadOperation.isDone)
            {
                yield return null;
            }

            Debug.Log("[Scene Transition] Unloaded Scene (" + UnloadSceneName + ")");
        }

        AsyncOperation LoadOperation = SceneManager.LoadSceneAsync(LoadSceneName, LoadSceneMode.Additive);
        LoadOperation.allowSceneActivation = true;

        LoadOperation.completed += (AsyncOperation Op) =>
        {
            Debug.Log("Set Active");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(LoadSceneName));
            Debug.Log("Set Active After");
        };
    }

    IEnumerator SetActiveAfterLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        while (!scene.isLoaded)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(scene);
    }

}
