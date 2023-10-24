using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

/// <summary>
/// Responsible for switching between scenes, maintaining
/// information about the current scene, and loading
/// scenes.
/// </summary>
public class SceneController : MonoBehaviour
{
    /// <summary>
    /// Maximum frame rate.
    /// </summary>
    private const int MAX_FRAME_RATE = 60;

    /// <summary>
    /// Reference to the SceneController singleton.
    /// </summary>
    private static SceneController instance;

    /// <summary>
    /// How much time has elapsed since this scene was loaded.
    /// </summary>
    private static float timeElapsed;

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
    /// Finds and sets the SceneController singleton in a level.
    /// </summary>
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
    /// Sets properties of the engine.
    /// </summary>
    public static void SetUnityProperties()
    {
        Application.targetFrameRate = MAX_FRAME_RATE;
        Screen.SetResolution(1920, 1080, false);
    }

    /// <summary>
    /// Returns the current FPS.
    /// </summary>
    /// <returns>current game FPS.</returns>
    public static int GetFPS()
    {
        float localTime = timeElapsed;
        localTime += (Time.unscaledDeltaTime - localTime) * 0.1f;
        float fps = 1.0f / localTime;
        int roundedFps = Mathf.RoundToInt(fps);
        return roundedFps;
    }
}
