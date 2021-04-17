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

    public GameObject LoadingScreenGameObject;

    public bool WaitForExtraLoad;

    private void Awake()
    {
        LoadingScreenGameObject.SetActive(false);
    }

    /// <summary>
    /// Tries to load the overworld scene, if it's not already loaded.
    /// </summary>
    public static void TryLoadOverworld()
    {
        Scene OverworldScene = SceneManager.GetSceneByName(OverworldSceneName);

        if (!OverworldScene.IsValid())
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

        StartCoroutine(LoadSceneAndUnload(sceneName, !inOverworld ? SceneManager.GetActiveScene().name : null, true));
    }

    /// <summary>
    /// Handles the complete unload and load process.
    /// </summary>
    /// <param name="LoadSceneName"></param>
    /// <param name="UnloadSceneName"></param>
    /// <returns></returns>
    IEnumerator LoadSceneAndUnload(string LoadSceneName, string UnloadSceneName = null, bool unloadAllExtraScenes = false)
    {
        LoadingScreenGameObject.SetActive(true);

        ScriptExecution.StartLevelTransition();

        if (UnloadSceneName != null)
        {
            AsyncOperation UnloadOperation = SceneManager.UnloadSceneAsync(UnloadSceneName);
            while (!UnloadOperation.isDone)
            {
                yield return null;
            }

            Debug.Log("[Scene Transition] Unloaded Scene (" + UnloadSceneName + ")");
        }

        if (unloadAllExtraScenes)
        {
            List<Scene> scenesToUnload = new List<Scene>();

            int loadedScenes = SceneManager.sceneCount;

            // If this was equal to 1, we could assume only the overworld was loaded
            if (loadedScenes > 1)
            {
                for (int i = 0; i < loadedScenes; i++)
                {
                    Scene currentScene = SceneManager.GetSceneAt(i);

                    if (currentScene.name != OverworldSceneName)
                    {
                        scenesToUnload.Add(currentScene);
                    }
                }
            }

            Debug.Log($"[Scene Transition] Unloading {scenesToUnload.Count} extra levels");

            foreach (Scene scene in scenesToUnload)
            {
                SceneManager.UnloadSceneAsync(scene);
            }

            while (SceneManager.sceneCount > 1)
            {
                yield return null;
            }
        }

        AsyncOperation LoadOperation = SceneManager.LoadSceneAsync(LoadSceneName, LoadSceneMode.Additive);
        LoadOperation.allowSceneActivation = true;

        LoadOperation.completed += (AsyncOperation Op) =>
        {
            Debug.Log($"[Scene Transition] Loading complete. Setting {LoadSceneName} scene to active");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(LoadSceneName));
            Debug.Log("[Scene Transition] Scene set active");

            StartCoroutine(WaitForExtraLoads());
        };
    }

    IEnumerator WaitForExtraLoads()
    {
        Debug.Log("[Scene Transition] Waiting for extra loading");

        while (WaitForExtraLoad)
        {
            yield return null;
        }

        Debug.Log("[Scene Transition] Finished all loading. Removing loading screen");

        LoadingScreenGameObject.SetActive(false);
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
