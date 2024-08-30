using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Disable warning for using SetActiveRecursively.
#pragma warning disable CS0618

/// <summary>
/// Controls the settings menu and stores settings data.
/// </summary>
public class SettingsController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the SettingsController singleton.
    /// </summary>
    private static SettingsController instance;

    /// <summary>
    /// Reference to the settings menu GameObject.
    /// </summary>
    [SerializeField]
    private GameObject settingsMenu;

    /// <summary>
    /// Reference to the music volume slider.
    /// </summary>
    [SerializeField]
    private Slider musicVolumeSlider;

    /// <summary>
    /// Reference to the soundfx volume slider.
    /// </summary>
    [SerializeField]
    private Slider soundFXVolumeSlider;

    /// <summary>
    /// true if health bars should be shown; otherwise, false.
    /// </summary>
    public static bool SHOW_HEALTH_BARS { get; private set; } = true;

    #endregion

    #region Methods

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
        instance.CloseSettingsMenu();
    }

    /// <summary>
    /// Main update loop for the SettingsController.
    /// </summary>
    public static void UpdateSettingsMenu()
    {
        if (InputController.DidEscapeDown()) OnEscape();
        if (SettingsMenuOpen()) instance.UpdateSliders();
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
    /// ### BUTTON EVENT ###
    /// 
    /// Opens/closes the settings menu.
    /// </summary>
    public void ClickSettingsButton()
    {
        if (SettingsMenuOpen()) CloseSettingsMenu();
        else OpenSettingsMenu();
    }

    /// <summary>
    /// Performs logic when the escape button is pressed.
    /// </summary>
    public static void OnEscape()
    {
        if (SettingsMenuOpen()) instance.CloseSettingsMenu();
        else instance.OpenSettingsMenu();
    }

    /// <summary>
    /// Opens the settings menu.
    /// </summary>
    private void OpenSettingsMenu() => settingsMenu.SetActiveRecursively(true);

    /// <summary>
    /// Closes the settings menu.
    /// </summary>
    private void CloseSettingsMenu() => settingsMenu.SetActiveRecursively(false);

    /// <summary>
    /// Returns true if the settings menu is open; false otherwise.
    /// </summary>
    /// <returns>true if the settings menu is open; false otherwise.</returns>
    public static bool SettingsMenuOpen() => instance.settingsMenu.activeSelf;

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

    /// <summary>
    /// Updates the sliders in the settings menu.
    /// </summary>
    private void UpdateSliders()
    {
        SoundController.SetMusicVolume(musicVolumeSlider.value);
        SoundController.SetSoundFXVolume(soundFXVolumeSlider.value);
    }

    #endregion
}
