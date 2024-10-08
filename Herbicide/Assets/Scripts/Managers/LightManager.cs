using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Manages 2D Lighting in the scene.
/// </summary>
public class LightManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the LightManager singleton.
    /// </summary>
    private static LightManager instance;

    /// <summary>
    /// The global light in the scene.
    /// </summary>
    [SerializeField]
    private Light2D globalLight;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the LightManager singleton.
    /// </summary>
    /// <param name="levelController">the LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        LightManager[] lightManagers = FindObjectsOfType<LightManager>();
        Assert.IsNotNull(lightManagers, "Array of LightManagers is null.");
        Assert.AreEqual(1, lightManagers.Length);
        instance = lightManagers[0];
    }

    #endregion
}
