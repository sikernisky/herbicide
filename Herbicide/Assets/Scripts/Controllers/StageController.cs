using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

/// <summary>
/// Controls the flow and display of stages throughout the game.
/// </summary>
public class StageController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The different stages of the day.
    /// </summary>
    public enum StageOfDay
    {
        MORNING = 0,
        NOON = 1,
        EVENING = 2,
        NIGHT = 3
    }


    /// <summary>
    /// Reference to the StageController singleton.
    /// </summary> 
    private static StageController instance;

    /// <summary>
    /// The most recent GameState.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// The stage data: the stage and the latest enemy that
    /// spawns at that stage.
    /// <summary> 
    private Dictionary<StageOfDay, float> stageData;

    /// <summary>
    /// The number of seconds between stages.
    /// </summary>
    private float TIME_BETWEEN_STAGES => 5f;

    /// <summary>
    /// Stores the number of seconds that have elapsed since the last stage began.
    /// </summary>
    private float timeSinceLastStage;

    /// <summary>
    /// Stores the number of seconds that have elapsed since the last intermission began.
    /// </summary>
    private float intermissionTimer;

    /// <summary>
    /// true if there is an ongoing intermission between stages; otherwise, false. 
    /// </summary>
    private bool isActiveIntermission;

    /// <summary>
    /// The current stage the player is on. Starts at 1.
    /// </summary>
    private StageOfDay currentStage;

    /// <summary>
    /// The text component that displays the current stage.
    /// </summary>
    [SerializeField]
    private TMP_Text stageText;

    /// <summary>
    /// The Image component for the intermission timer bar.
    /// </summary>
    [SerializeField]
    private Image intermissionTimerBarFill;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the StageController singleton for a level.
    /// </summary>
    /// <param name="levelController">The LevelController singleton. </param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        StageController[] stageControllers = FindObjectsOfType<StageController>();
        Assert.IsNotNull(stageControllers, "Array of StageControllers is null.");
        Assert.AreEqual(1, stageControllers.Length);
        instance = stageControllers[0];
        instance.stageText.text = "Stage " + instance.currentStage;
        LightManager.AdjustLightingForStageOfDay(instance.currentStage, 0.0f);
    }

    /// <summary>
    /// Main update loop for the StageController.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public static void UpdateStageController(GameState gameState)
    {
        if (instance == null) return;
        instance.gameState = gameState;
        if (gameState != GameState.ONGOING) return;
        if(instance.stageData == null) return;

        Assert.IsTrue(instance.stageData.ContainsKey(StageOfDay.MORNING), "You need to spawn an enemy " +
            "during the MORNING STAGE in Tiled.");

        float latestSpawnThisStage = instance.stageData[instance.currentStage];
        int numActiveEnemies = ControllerManager.NumActiveEnemies();

        if (instance.timeSinceLastStage > latestSpawnThisStage && numActiveEnemies <= 0)
        {
            // Start intermission
            instance.isActiveIntermission = true;
        }

        if (instance.isActiveIntermission)
        {
            instance.intermissionTimer += Time.deltaTime;

            float timePercentage = instance.intermissionTimer / instance.TIME_BETWEEN_STAGES;
            instance.intermissionTimerBarFill.transform.localScale = new Vector3(1 - timePercentage, 2, 1);

            if (instance.intermissionTimer >= instance.TIME_BETWEEN_STAGES)
            {
                instance.isActiveIntermission = false;
                instance.currentStage++;
                instance.stageText.text = "Stage " + instance.currentStage;
                instance.intermissionTimer = 0;
                instance.timeSinceLastStage = 0f;
                LightManager.AdjustLightingForStageOfDay(instance.currentStage);
                ControllerManager.ResetNexiiToSpawnPositions(instance);
            }
        }
        else instance.timeSinceLastStage += Time.deltaTime;

    }

    /// <summary>
    /// Sets the stage data for this level.
    /// </summary>
    /// <param name="stageData">The stage data for this level: <br></br>
    /// 
    /// Key: the stage number. <br></br>
    /// Value: the latest enemy that spawns at that stage. <br></br> 
    /// </param>
    public static void SetStageData(Dictionary<StageOfDay, float> stageData)
    {
        instance.stageData = new Dictionary<StageOfDay, float>(stageData);
    }

    /// <summary>
    /// Returns the current stage the player is on.
    /// </summary>
    /// <returns>the current stage. </returns>
    public static StageOfDay GetCurrentStage() => instance.currentStage;

    /// <summary>
    /// Returns the final stage of the level.
    /// </summary>
    /// <returns>the final stage of the level</returns>
    public static StageOfDay GetFinalStage() => StageOfDay.NIGHT;

    /// <summary>
    /// Returns the number of seconds that have elapsed
    /// since the last stage began.
    /// </summary>
    /// <returns>the number of seconds that have elapsed since the
    /// last stage began. </returns> 
    public static float GetTimeSinceLastStageBegan() => instance.timeSinceLastStage;



    #endregion
}
