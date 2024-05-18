using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using System;

/// <summary>
/// Responsible for switching between scenes, maintaining
/// information about the current scene, and loading
/// scenes.
/// </summary>
public class SceneController : MonoBehaviour
{
    /// <summary>
    /// Reference to the SceneController singleton.
    /// </summary>
    private static SceneController instance;

    /// <summary>
    /// How much time has elapsed since this scene was loaded.
    /// </summary>
    private static float timeElapsed;

    /// <summary>
    /// true if we're currently loading a scene
    /// </summary>
    private bool loadingScene;


    /// <summary>
    /// Main update loop for the SceneController.
    /// </summary>
    public static void UpdateScene()
    {
        timeElapsed += Time.deltaTime;
    }

    /// <summary>
    /// Returns the amount of time elapsed since this scene began.
    /// </summary>
    /// <returns>the amount of time elapsed since this scene began.</returns>
    public static float GetTimeElapsed()
    {
        return timeElapsed;
    }

    /// <summary>
    /// Finds and sets the SceneController singleton in the Main Menu.
    /// </summary>
    /// <param name="mainMenuController">The MainMenuController reference.</param>
    public static void SetSingleton(MainMenuController mainMenuController)
    {
        Assert.IsNotNull(mainMenuController, "MainMenuController is null.");

        SceneController[] sceneControllers = FindObjectsOfType<SceneController>();
        Assert.IsNotNull(sceneControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, sceneControllers.Length);
        instance = sceneControllers[0];
        timeElapsed = 0;
    }

    /// <summary>
    /// Finds and sets the SceneController singleton in the Main Menu.
    /// </summary>
    /// <param name="levelController">The LevelController reference.</param>
    public static void SetSingleton(LevelController levelController)
    {
        Assert.IsNotNull(levelController, "LevelController is null.");

        SceneController[] sceneControllers = FindObjectsOfType<SceneController>();
        Assert.IsNotNull(sceneControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, sceneControllers.Length);
        instance = sceneControllers[0];
        timeElapsed = 0;
    }

    /// <summary>
    /// Finds and sets the SceneController singleton in the Main Menu.
    /// </summary>
    /// <param name="levelController">The LevelController reference.</param>
    public static void SetSingleton(CollectionMenuController levelController)
    {
        Assert.IsNotNull(levelController, "LevelController is null.");

        SceneController[] sceneControllers = FindObjectsOfType<SceneController>();
        Assert.IsNotNull(sceneControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, sceneControllers.Length);
        instance = sceneControllers[0];
        timeElapsed = 0;
    }

    /// <summary>
    /// Sets properties of the engine.
    /// </summary>
    public static void SetUnityProperties()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, false);
    }

    /// <summary>
    /// [!!BUTTON EVENT!!]
    /// 
    /// Loads a scene. If already loading, does nothing.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        if (loadingScene) return;
        loadingScene = true;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Loads a scene asynchonously.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <returns>A reference to the coroutine.</returns>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Assert.IsNotNull(sceneName, "Name of scene to load is null.");
        Assert.IsTrue(sceneName.Length > 0, "Name is scene to load is empty.");


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");
            yield return null;
        }
        loadingScene = false;
    }

    /// <summary>
    /// Runs a Coroutine reference. This is implemented for classes that
    /// need to run Coroutines but do not inherit from Monobehaviour. 
    /// </summary>
    /// <param name="reference">The Coroutine reference</param>
    public static void BeginCoroutine(IEnumerator reference)
    {
        Assert.IsNotNull(reference, "Reference to Coroutine is null.");
        instance.StartCoroutine(reference);
    }

    /// <summary>
    /// Stops a Coroutine reference. This is implemented for classes that
    /// need to run Coroutines but do not inherit from Monobehaviour. 
    /// </summary>
    /// <param name="reference">The Coroutine reference</param>
    public static void EndCoroutine(IEnumerator reference)
    {
        Assert.IsNotNull(reference, "Reference to Coroutine is null.");
        instance.StopCoroutine(reference);
    }
}
