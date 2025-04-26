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
        instance.SetGlobalLightIntensity(RenderingConstants.DefaultGlobalLightIntensity);
    }

    /// <summary>
    /// Updates the LightManager.
    /// </summary>
    public static void UpdateLightManager() { }

    /// <summary>
    /// Sets the global light intensity.
    /// </summary>
    /// <param name="intensity">The intensity of the global light.</param>
    private void SetGlobalLightIntensity(float intensity)
    {
        if (Mathf.Approximately(intensity, globalLight.intensity)) return;
        Assert.IsTrue(intensity >= 0 && intensity <= 1, "Intensity must be between 0 and 1.");
        globalLight.intensity = intensity;
    }


    #endregion
}
