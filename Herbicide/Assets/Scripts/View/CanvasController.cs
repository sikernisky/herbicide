using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Controls general UI events, such as positioning elements,
/// appearing and dissapearing elements, and more. 
/// </summary>
public class CanvasController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the CanvasController singleton.
    /// </summary>
    private static CanvasController instance;

    /// <summary>
    /// Number of seconds it takes for the Fader
    /// to fade in and out.
    /// </summary>
    private const float DEFAULT_FADE_TIME = .5f;

    /// <summary>
    /// Number of seconds to wait before fading in the Fader
    /// component after the player wins, loses, or ties a
    /// level.
    /// </summary>
    private const float FADE_DELAY = 1.5f;

    /// <summary>
    /// The maximum alpha value (0-1) that the Fader component
    /// fades to.
    /// </summary>
    private const float FADE_DARKNESS = .7f;

    /// <summary>
    /// The main canvas in the scene.
    /// </summary>
    [SerializeField]
    private Canvas gameCanvas;

    /// <summary>
    /// The Image component that fades in and out for transitional
    /// purposes between game states and modes.
    /// </summary>
    [SerializeField]
    private Image fader;

    /// <summary>
    /// true if the Fader is fading in or fading out.
    /// </summary>
    private bool fading;

    /// <summary>
    /// true if the Fader component has faded in after the GameState shifted
    /// from ONGOING to WIN, LOSE, or TIE.
    /// </summary>
    private bool fadedAfterLevelEnd;

    /// <summary>
    /// The alpha value that the Fader value is fading towards, from
    /// 0.0 - 1.0.
    /// </summary>
    private float faderTargetAlpha;

    /// <summary>
    /// The current GameState.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// Number of seconds we have been waiting since a level has ended.
    /// Used to queue in the Fader when this value hits a certain number
    /// of seconds.
    /// </summary>
    private float fadeDelayTimer;

    /// <summary>
    /// true if debug mode is on.
    /// </summary>
    private bool isDebug;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the CanvasController singleton for a level.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        CanvasController[] canvasControllers = FindObjectsOfType<CanvasController>();
        Assert.IsNotNull(canvasControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, canvasControllers.Length);
        instance = canvasControllers[0];
        Assert.IsNotNull(instance.fader);
        Assert.IsNotNull(instance.gameCanvas);

        instance.fader.gameObject.SetActive(true);
        instance.fader.transform.SetParent(instance.gameCanvas.transform);
        RectTransform faderTransform = instance.fader.GetComponent<RectTransform>();
        faderTransform.anchorMin = new Vector2(0, 0);
        faderTransform.anchorMax = new Vector2(1, 1);
        faderTransform.offsetMin = new Vector2(0, 0);
        faderTransform.offsetMax = new Vector2(0, 0);
        PlayFaderOut();
    }

    /// <summary>
    /// Finds and sets the CanvasController singleton for the MainMenu.
    /// </summary>
    /// <param name="mainMenuController">The MainMenuController singleton.</param>
    public static void SetSingleton(MainMenuController mainMenuController)
    {
        if (mainMenuController == null) return;
        if (instance != null) return;

        CanvasController[] canvasControllers = FindObjectsOfType<CanvasController>();
        Assert.IsNotNull(canvasControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, canvasControllers.Length);
        instance = canvasControllers[0];
        Assert.IsNotNull(instance.fader);
        Assert.IsNotNull(instance.gameCanvas);

        instance.fader.gameObject.SetActive(true);
        instance.fader.transform.SetParent(instance.gameCanvas.transform);
        RectTransform faderTransform = instance.fader.GetComponent<RectTransform>();
        faderTransform.localScale = new Vector3(1, 1, 1);
        faderTransform.anchorMin = new Vector2(0, 0);
        faderTransform.anchorMax = new Vector2(1, 1);
        faderTransform.offsetMin = new Vector2(0, 0);
        faderTransform.offsetMax = new Vector2(0, 0);
        PlayFaderOut();
    }


    /// <summary>
    /// Finds and sets the CanvasController singleton for the SkillMenu.
    /// </summary>
    /// <param name="skillMenuController">The SkillMenuController singleton.</param>
    public static void SetSingleton(CollectionMenuController skillMenuController)
    {
        if (skillMenuController == null) return;
        if (instance != null) return;

        CanvasController[] canvasControllers = FindObjectsOfType<CanvasController>();
        Assert.IsNotNull(canvasControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, canvasControllers.Length);
        instance = canvasControllers[0];
        Assert.IsNotNull(instance.fader);
        Assert.IsNotNull(instance.gameCanvas);

        instance.fader.gameObject.SetActive(true);
        instance.fader.transform.SetParent(instance.gameCanvas.transform);
        RectTransform faderTransform = instance.fader.GetComponent<RectTransform>();
        faderTransform.localScale = new Vector3(1, 1, 1);
        faderTransform.anchorMin = new Vector2(0, 0);
        faderTransform.anchorMax = new Vector2(1, 1);
        faderTransform.offsetMin = new Vector2(0, 0);
        faderTransform.offsetMax = new Vector2(0, 0);
        PlayFaderOut();
    }

    /// <summary>
    /// Main update loop for the CanvasController.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>  
    public static void UpdateCanvas(GameState gameState)
    {
        instance.gameState = gameState;
        instance.UpdateFader();

        //Check to fade in after the player wins, loses, or ties
        GameState gs = instance.gameState;
        if (gs != GameState.ONGOING && gs != GameState.MENU)
        {
            instance.fadeDelayTimer = Mathf.Clamp(
                instance.fadeDelayTimer + Time.deltaTime,
                0f,
                FADE_DELAY
            );

            if (!instance.fadedAfterLevelEnd && instance.fadeDelayTimer >= FADE_DELAY)
            {
                PlayFaderIn();
                instance.fadedAfterLevelEnd = true;
            }
        }
    }

    /// <summary>
    /// Updates the Fader component to fade in or out.
    /// </summary>
    private void UpdateFader()
    {
        if (!fading) return;
        Color faderColor = fader.color;
        float newAlpha = Mathf.MoveTowards(
            faderColor.a,
            faderTargetAlpha,
            Time.deltaTime * DEFAULT_FADE_TIME
        );
        Color newFaderColor = new Color(faderColor.r, faderColor.g, faderColor.b, newAlpha);
        fader.color = newFaderColor;
        if (Mathf.Approximately(newAlpha, faderTargetAlpha)) fading = false;
    }

    /// <summary>
    /// Tells the Fader to fade in.
    /// </summary>
    public static void PlayFaderIn()
    {
        instance.faderTargetAlpha = FADE_DARKNESS;
        instance.fading = true;
    }

    /// <summary>
    /// Tells the Fader to fade out.
    /// </summary>
    public static void PlayFaderOut()
    {
        instance.faderTargetAlpha = 0.0f;
        instance.fading = true;
    }

    /// <summary>
    /// Sets the debug mode to be off or on.
    /// </summary>
    /// <param name="levelController">The LevelController instance.</param>
    /// <param name="isOn">true if the debug mode should be on; false if not.</param>
    public static void SetDebug(LevelController levelController, bool isOn)
    {
        Assert.IsNotNull(levelController);

        instance.isDebug = isOn;
    }

    /// <summary>
    /// Returns true if debug mode is on.
    /// </summary>
    /// <returns>true if debug mode is on; otherwise, false.</returns>
    private bool IsDebugging() => isDebug;

    #endregion
}
