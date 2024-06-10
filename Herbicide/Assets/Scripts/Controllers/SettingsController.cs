using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Controls the settings of the game. This class
/// should eventually be reworked because right now it
/// is just a database for preliminary playtesting.
/// </summary>
public class SettingsController : MonoBehaviour
{
    /// <summary>
    /// Reference to the SettingsController singleton.
    /// </summary>
    private static SettingsController instance;

    // -----------------BEGIN PLAYTESTING----------------- //

    /// <summary>
    /// true if health bars should be shown; otherwise, false.
    /// </summary>
    public static bool SHOW_HEALTH_BARS { get; private set; } = true;


    // -----------------END PLAYTESTING----------------- //


    /// <summary>
    /// Finds and sets the SettingsController singleton for a level.
    /// </summary>
    /// <param name="levelController">The SettingsController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        SettingsController[] settingsControllers = FindObjectsOfType<SettingsController>();
        Assert.IsNotNull(settingsControllers, "Array of SettingsControllers is null.");
        Assert.AreEqual(1, settingsControllers.Length);
        instance = settingsControllers[0];
    }

    /// <summary>
    /// ### BUTTON EVENT ###
    /// 
    /// Toggles the health bars on and off.
    /// </summary>
    /// <param name="playtestButtonPrefab">The prefab of the playtest button.</param>
    public void ToggleHealthBars(GameObject playtestButtonPrefab)
    {
        ToggleCheckmark(playtestButtonPrefab);
        SHOW_HEALTH_BARS = !SHOW_HEALTH_BARS;
    }

    /// <summary>
    /// Toggles the checkmark on the playtest button.
    /// </summary>
    /// <param name="playtestButtonPrefab">The playtest button prefab.</param>
    private void ToggleCheckmark(GameObject playtestButtonPrefab)
    {
        if (playtestButtonPrefab == null) return;
        GameObject checkmark = playtestButtonPrefab.transform.GetChild(0).gameObject;
        if (checkmark == null) return;
        Image image = checkmark.GetComponent<Image>();
        if (image == null) return;

        image.enabled = !image.enabled;
    }


}
