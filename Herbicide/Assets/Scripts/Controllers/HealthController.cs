using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls player health related events.
/// </summary>
public class HealthController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the HealthController singleton.
    /// </summary>
    private static HealthController instance;

    /// <summary>
    /// The most recent GameState.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// The text that displays the player's health.
    /// </summary>
    [SerializeField]
    private TMP_Text healthText;

    /// <summary>
    /// The number of lives the player starts with.
    /// </summary>
    private const int STARTING_LIVES = 3;

    /// <summary>
    /// The number of lives the player currently has.
    /// </summary>
    private int lives;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the HealthController singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        HealthController[] healthControllers = FindObjectsOfType<HealthController>();
        Assert.IsNotNull(healthControllers, "Array of HealthControllers is null.");
        Assert.AreEqual(1, healthControllers.Length);
        instance = healthControllers[0];
        instance.lives = STARTING_LIVES;
        instance.UpdateHealthText();
    }

    /// <summary>
    /// Main update loop for the HealthController.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public static void UpdateHealthController(GameState gameState)
    {
        instance.gameState = gameState;
    }

    /// <summary>
    /// Subscribes to the SaveLoadManager's OnLoadRequested and OnSaveRequested events.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SubscribeToSaveLoadEvents(LevelController levelController)
    {
        Assert.IsNotNull(levelController, "LevelController is null.");

        SaveLoadManager.SubscribeToToLoadEvent(instance.LoadHealthData);
        SaveLoadManager.SubscribeToToSaveEvent(instance.SaveHealthData);
    }

    /// <summary>
    /// Removes a life from the player.
    /// </summary>
    public static void LoseLife()
    {
        instance.lives = Mathf.Max(0, instance.lives - 1);
        instance.UpdateHealthText();
    }

    /// <summary>
    /// Returns the number of lives the player has remaining.
    /// </summary>
    /// <returns>the number of lives the player has remaining.</returns>
    public static int LivesRemaining() => instance.lives;

    /// <summary>
    /// Updates the health text to display the current number of lives.
    /// </summary>
    private void UpdateHealthText()  => healthText.text = lives.ToString();

    /// <summary>
    /// Loads the Health data from the SaveLoadManager.
    /// </summary>
    private void LoadHealthData() { }

    /// <summary>
    /// Saves the Health data to the SaveLoadManager.
    /// </summary>
    private void SaveHealthData() { }

    #endregion
}
