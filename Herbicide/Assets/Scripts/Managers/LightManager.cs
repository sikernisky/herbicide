using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using static StageController;

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

    /// <summary>
    /// true if a transition is in progress; otherwise, false.
    /// </summary>
    private bool transitionInProgress;

    /// <summary>
    /// Duration of the transition from the current light value to the desired light value.
    /// </summary>
    private float transitionDuration;

    /// <summary>
    /// The target intensity of the global light during the transition.
    /// </summary>
    private float targetIntensity;

    /// <summary>
    /// The starting intensity of the global light during the transition.
    /// </summary>
    private float startIntensity;

    /// <summary>
    /// The counter for the transition.
    /// </summary>
    private float transitionCounter;

    /// <summary>
    /// The number of seconds it takes to transition between stage lighting.
    /// </summary>
    private const float SECONDS_TO_TRANSITION_BETWEEN_STAGE_LIGHTING = 1.0f;

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

    /// <summary>
    /// Updates the LightManager.
    /// </summary>
    public static void UpdateLightManager()
    {
        if (instance == null) return;

        if (instance.transitionInProgress)
        {
            instance.transitionCounter += Time.deltaTime;
            float t = instance.transitionCounter / instance.transitionDuration;
            instance.globalLight.intensity = Mathf.Lerp(instance.startIntensity, instance.targetIntensity, t);
            if (t >= 1f)
            {
                instance.globalLight.intensity = instance.targetIntensity; // Ensure it hits the target exactly
                instance.transitionInProgress = false; // Transition complete
            }
        }
    }

    /// <summary>
    /// Sets the global light intensity.
    /// </summary>
    /// <param name="intensity">The intensity of the global light.</param>
    /// <param name="duration">The duration of the transition from
    /// the current light value to the desired light value. </param>
    private void SetGlobalLightIntensity(float intensity, float duration)
    {
        if (Mathf.Approximately(intensity, globalLight.intensity)) return;
        Assert.IsTrue(duration >= 0, "Transition duration must be greater than or equal to 0.");
        Assert.IsTrue(intensity >= 0 && intensity <= 1, "Intensity must be between 0 and 1.");

        transitionInProgress = true;
        transitionDuration = duration;
        transitionCounter = 0;
        startIntensity = globalLight.intensity;
        targetIntensity = intensity;
    }

    /// <summary>
    /// Sets lighting values based on the given stage.
    /// </summary>
    /// <param name="stage">The stage of the day.</param>
    /// <param name="duration">The duration of the transition from the current 
    /// light value to the desired light value.</param>
    public static void AdjustLightingForStageOfDay(StageController.StageOfDay stage, 
        float duration = SECONDS_TO_TRANSITION_BETWEEN_STAGE_LIGHTING)
    {
        float newIntensity;
        switch (stage)
        {
            case StageOfDay.MORNING:
                newIntensity = 0.8f;
                break;
            case StageOfDay.NOON:
                newIntensity = 1.0f;
                break;
            case StageOfDay.EVENING:
                newIntensity = 0.8f;
                break;
            case StageOfDay.NIGHT:
                newIntensity = 0.6f;
                break;
            default:
                newIntensity = instance.globalLight.intensity;
                break;
        }
        instance.SetGlobalLightIntensity(newIntensity, duration);
    }

    #endregion
}
