using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

/// <summary>
/// Controls scene transitions and scene-related events.
/// </summary>
public class SceneController : MonoBehaviour
{
    #region Fields

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

    #endregion

    #region Methods

    /// <summary>
    /// Main update loop for the SceneController.
    /// </summary>
    public static void UpdateScene()
    {
        timeElapsed += Time.deltaTime;
        if (InputManager.DidKeycodeDown(KeyCode.N)) LoadNextLevelWithFadeDelay();
    }

    /// <summary>
    /// Returns the amount of time elapsed since this scene began.
    /// </summary>
    /// <returns>the amount of time elapsed since this scene began.</returns>
    public static float GetTimeElapsed() => timeElapsed;

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
        SetUnityProperties();
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
        SetUnityProperties();
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
        SetUnityProperties();
        timeElapsed = 0;
    }

    /// <summary>
    /// Sets properties of the engine.
    /// </summary>
    private static void SetUnityProperties()
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
    private void LoadScene(string sceneName)
    {
        if (loadingScene) return;
        loadingScene = true;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// [!!BUTTON EVENT!!]
    /// 
    /// Loads the next level with a fade delay. If already loading, does nothing.
    /// </summary>
    public static void LoadNextLevelWithFadeDelay()
    {
        if (instance.loadingScene) return;
        int currentLevel = SaveLoadManager.GetLoadedGameLevel();
        int maxLevel = JSONController.GetMaxLevelIndex();
        if (currentLevel >= maxLevel) instance.LoadScene("MainMenu");
        else
        {
            SaveLoadManager.SaveGameLevel(currentLevel + 1);
            SaveLoadManager.Save();
            instance.StartCoroutine(instance.ReloadSceneAfterFadeOut());
        }
    }

    /// <summary>
    /// Loads a scene with a fade delay. If already loading, does nothing.
    /// </summary>
    /// <param name="sceneName">the name of the scene to load</param>
    public static void LoadSceneWithFadeDelay(string sceneName)
    {
        if (instance.loadingScene) return;
        instance.StartCoroutine(instance.LoadSceneAfterFadeOut(sceneName));
    }

    /// <summary>
    /// Reloads the current scene with a fade delay. If already loading, does nothing.
    /// </summary>
    public static void LoadCurrentSceneWithFadeDelay()
    {
        if (instance.loadingScene) return;
        instance.StartCoroutine(instance.ReloadSceneAfterFadeOut());
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    private void ReloadScene() => LoadScene(SceneManager.GetActiveScene().name);

    /// <summary>
    /// Loads a scene asynchonously.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <returns>A reference to the coroutine.</returns>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Assert.IsNotNull(sceneName, "Name of scene to load is null.");
        Assert.IsTrue(sceneName.Length > 0, "Name of scene to load is empty.");

        if (SceneUtility.GetBuildIndexByScenePath(sceneName) == -1)
        {
            Debug.LogWarning($"Scene '{sceneName}' not found. Loading 'mainmenu' instead.");
            sceneName = "MainMenu";
        }

        // Start loading the scene asynchronously.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            // Update the loading progress.
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            //Debug.Log($"Loading progress: {progress * 100}%");
            yield return null;
        }
        loadingScene = false;
    }

    /// <summary>
    /// Reloads the current scene after playing the Canvas fader
    /// out.
    /// </summary>
    /// <returns>a reference to the coroutine.</returns>
    private IEnumerator ReloadSceneAfterFadeOut()
    {
        CanvasController.PlayFaderIn();
        yield return new WaitForSeconds(CanvasController.FADE_TIME);
        ReloadScene();
    }

    /// <summary>
    /// Loads a scene after playing the Canvas fader out.
    /// </summary>
    /// <param name="sceneName">the name of the scene to load</param>
    /// <returns>a reference to the coroutine</returns>
    private IEnumerator LoadSceneAfterFadeOut(string sceneName)
    {
        CanvasController.PlayFaderIn();
        yield return new WaitForSeconds(CanvasController.FADE_TIME);
        LoadScene(sceneName);
    }

    #endregion
}
