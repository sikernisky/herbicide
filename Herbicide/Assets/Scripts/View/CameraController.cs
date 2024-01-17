using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


/// <summary>
/// Controls the level's singular, orthographic Camera component.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// The one Camera in the scene
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Reference to the CameraController singleton.
    /// </summary>
    private static CameraController instance;

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
    /// Returns the world position of some other position.
    /// </summary>
    /// <param name="pos">the position to convert.</param>
    /// <returns>the converted world position.</returns>
    public static Vector2 ScreenToWorldPoint(Vector2 pos)
    {
        return instance.GetCamera().ScreenToWorldPoint(pos);
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

    /// <summary>
    /// Returns the (Width, Height) of this CameraController's Camera
    /// component as a Vector2.
    /// </summary>
    /// <returns>A Vector2 containing the (Width, Height) of this CameraController's
    /// Camera component.</returns>
    public static Vector2 GetDimensions()
    {
        Camera cam = instance.GetCamera();
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        return new Vector2(width, height);
    }
}
