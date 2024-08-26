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
    public static void MoveCamera(Vector2 pos)
    {
        instance.GetCamera().transform.position = pos;
    }

    /// <summary>
    /// Converts a position from screen coordinates to world coordinates.
    /// </summary>
    /// <param name="pos">The position in screen coordinates to convert.</param>
    /// <returns>The corresponding position in world coordinates.</returns>
    public static Vector2 ScreenToWorldPoint(Vector2 pos)
    {
        return instance.GetCamera().ScreenToWorldPoint(pos);
    }

    /// <summary>
    /// Converts a position from world coordinates to screen coordinates.
    /// </summary>
    /// <param name="pos">The position in world coordinates to convert.</param>
    /// <returns>The corresponding position in screen coordinates.</returns>
    public static Vector3 WorldToScreenPoint(Vector2 pos)
    {
        return instance.GetCamera().WorldToScreenPoint(pos);
    }

    /// <summary>
    /// Returns the Camera controlled by the CameraController singleton.
    /// </summary>
    /// <returns>the Camera controlled by the CameraController 
    /// singleton.</returns>
    private Camera GetCamera()
    {
        return cam;
    }

    #endregion
}
