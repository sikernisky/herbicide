using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls the level's singular, orthographic Camera component.
/// </summary>
public class CameraController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The one Camera in the scene
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Reference to the CameraController singleton.
    /// </summary>
    private static CameraController instance;

    /// <summary>
    /// The current level.
    /// </summary>
    private int level;

    /// <summary>
    /// The number of pixels per unit in the game.
    /// </summary>
    public static int PIXELS_PER_UNIT => 16;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the CameraController singleton for a level. Also finds and sets
    /// the CameraController's camera.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        CameraController[] cameraControllers = FindObjectsOfType<CameraController>();
        Assert.IsNotNull(cameraControllers, "Array of CameraControllers is null.");
        Assert.AreEqual(1, cameraControllers.Length);
        instance = cameraControllers[0];
        instance.cam = Camera.main;
    }

    /// <summary>
    /// Subscribes to the SaveLoadManager's OnLoadRequested and OnSaveRequested events.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SubscribeToSaveLoadEvents(LevelController levelController)
    {
        Assert.IsNotNull(levelController, "LevelController is null.");

        SaveLoadManager.SubscribeToToLoadEvent(instance.LoadCameraData);
        SaveLoadManager.SubscribeToToLoadEvent(instance.SaveCameraData);
    }

    /// <summary>
    /// Finds and sets the CameraController singleton for the main menu. Also finds and sets
    /// the CameraController's camera.
    /// </summary>
    /// <param name="mainMenuController">The LevelController singleton.</param>
    public static void SetSingleton(MainMenuController mainMenuController)
    {
        CameraController[] cameraControllers = FindObjectsOfType<CameraController>();
        Assert.IsNotNull(cameraControllers, "Array of CameraControllers is null.");
        Assert.AreEqual(1, cameraControllers.Length);
        instance = cameraControllers[0];
        instance.cam = Camera.main;
    }

    /// <summary>
    /// Finds and sets the CameraController singleton for the skill menu. Also finds and sets
    /// the CameraController's camera.
    /// </summary>
    /// <param name="skillMenuController">The LevelController singleton.</param>
    public static void SetSingleton(CollectionMenuController skillMenuController)
    {
        CameraController[] cameraControllers = FindObjectsOfType<CameraController>();
        Assert.IsNotNull(cameraControllers, "Array of CameraControllers is null.");
        Assert.AreEqual(1, cameraControllers.Length);
        instance = cameraControllers[0];
        instance.cam = Camera.main;
    }

    /// <summary>
    /// Sets the camera's position.
    /// </summary>
    public static void MoveCamera(Vector3 pos) => instance.GetCamera().transform.position = pos;

    /// <summary>
    /// Converts a position from screen coordinates to world coordinates.
    /// </summary>
    /// <param name="pos">The position in screen coordinates to convert.</param>
    /// <returns>The corresponding position in world coordinates.</returns>
    public static Vector2 ScreenToWorldPoint(Vector2 pos) => instance.GetCamera().ScreenToWorldPoint(pos);

    /// <summary>
    /// Converts a position from world coordinates to screen coordinates.
    /// </summary>
    /// <param name="pos">The position in world coordinates to convert.</param>
    /// <returns>The corresponding position in screen coordinates.</returns>
    public static Vector3 WorldToScreenPoint(Vector2 pos) => instance.GetCamera().WorldToScreenPoint(pos);

    /// <summary>
    /// Returns the Camera controlled by the CameraController singleton.
    /// </summary>
    /// <returns>the Camera controlled by the CameraController 
    /// singleton.</returns>
    private Camera GetCamera() => cam;

    /// <summary>
    /// Sets the correct Camera scale based on the current
    /// level. 
    /// </summary>
    private void SetCameraScaleBasedOnLevel()
    {
        var camera = GetCamera();
        if(camera == null) return;
    }

    /// <summary>
    /// Saves the Camera's data to the current PlayerData.
    /// </summary>
    private void SaveCameraData() { }

    /// <summary>
    /// Loads the Camera's data from the current PlayerData.
    /// </summary>
    private void LoadCameraData()
    {
        level = SaveLoadManager.GetLoadedGameLevel();
        SetCameraScaleBasedOnLevel();
    }

    #endregion
}
