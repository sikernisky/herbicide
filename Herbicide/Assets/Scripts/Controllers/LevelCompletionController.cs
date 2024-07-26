using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// Controls events that occur when a level is completed.
/// </summary>
public class LevelCompletionController : MonoBehaviour
{
    /// <summary>
    /// Reference to the LevelCompletionController singleton.
    /// </summary>
    private static LevelCompletionController instance;

    /// <summary>
    /// The most recent game state.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// Reference to the level completion panel GameObject.
    /// </summary>
    [SerializeField]
    private GameObject levelCompletionPanel;

    /// <summary>
    /// Text that displays whether the player won, lost, or tied.
    /// </summary>
    [SerializeField]
    private TMP_Text resultText;

    /// <summary>
    /// Finds and sets the LevelCompletionController singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        LevelCompletionController[] levelCompletionControllers = FindObjectsOfType<LevelCompletionController>();
        Assert.IsNotNull(levelCompletionControllers, "Array of LevelCompletionControllers is null.");
        Assert.AreEqual(1, levelCompletionControllers.Length);
        instance = levelCompletionControllers[0];
        instance.resultText.text = "";
        instance.levelCompletionPanel.SetActive(false);
    }

    /// <summary>
    /// Main loop for the LevelCompletionController.
    /// </summary>
    /// <param name="gameState">The most recent game state.</param>
    public static void UpdateLevelCompletionController(GameState gameState)
    {
        if (instance == null) return;
        instance.gameState = gameState;


        bool gameOver = gameState == GameState.LOSE ||
                        gameState == GameState.WIN ||
                        gameState == GameState.TIE;

        if (gameOver) instance.OpenLevelCompletePanel();
    }

    /// <summary>
    /// Returns true if the Level Completion panel is open.
    /// </summary>
    /// <returns>true if the Level Completion panel is open;
    /// otherwise, false. </returns>
    private bool LevelCompletePanelOpen() { return levelCompletionPanel.activeSelf; }

    /// <summary>
    /// Opens the Level Completion panel and populates it
    /// depending on the most recent game state.
    /// </summary>
    private void OpenLevelCompletePanel()
    {
        if (LevelCompletePanelOpen()) return;

        levelCompletionPanel.SetActive(true);

        if (gameState == GameState.WIN)
        {
            resultText.text = "You Win!";
        }
        else if (gameState == GameState.LOSE)
        {
            resultText.text = "You Lose!";
        }
        else if (gameState == GameState.TIE)
        {
            resultText.text = "You Tied!";
        }
    }
}
