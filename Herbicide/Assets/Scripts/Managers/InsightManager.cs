using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls insights; tooltips, ability icons and descriptions,
/// etc.
/// </summary>
public class InsightManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the InsightManager singleton.
    /// </summary>
    private static InsightManager instance;

    /// <summary>
    /// The current game state.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// The mob whose ability is currently being displayed.
    /// </summary>
    private Mob currentMob;

    /// <summary>
    /// The image component that displays the ability icon.
    /// </summary>
    [SerializeField]
    private Image abilityImage;

    /// <summary>
    /// The image component that displays the tooltip.
    /// </summary>
    [SerializeField]
    private Image tooltipImage;

    /// <summary>
    /// The text component that displays the tooltip text.
    /// </summary>
    [SerializeField]
    private TMP_Text tooltipText;

    /// <summary>
    /// The offset of the ability image from the mob's position.
    /// </summary>
    private Vector3 iconOffsetFromMob => new Vector3(0, 120, 1);

    /// <summary>
    /// The offset of the tooltip image from the ability icon.
    /// </summary>
    private Vector3 tooltipOffsetFromIcon => new Vector3(0, 96, 1);

    #endregion

    #region Methods

    /// <summary>
    /// Sets the InsightManager singleton.
    /// </summary>
    /// <param name="levelController">the LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        InsightManager[] insightManagers = FindObjectsOfType<InsightManager>();
        Assert.IsNotNull(insightManagers, "Array of LightManagers is null.");
        Assert.AreEqual(1, insightManagers.Length);
        instance = insightManagers[0];
        instance.abilityImage.enabled = false;
    }

    /// <summary>
    /// Main update loop for the InsightManager.
    /// </summary>
    /// <param name="gameState">The current game state</param>
    public static void UpdateInsightManager(GameState gameState)
    {
        instance.gameState = gameState;
    }

    /// <summary>
    /// Checks for input events.
    /// </summary>
    public static void CheckInputEvents()
    {
        if (InputController.DidPrimaryUp()) StopDisplayingAbility();
    }

    /// <summary>
    /// Displays the ability icon and description of the selected mob
    /// over its GameObject.
    /// </summary>
    /// <param name="mobToDisplayOver">The mob to display the ability icon and description over.</param>
    public static void DisplayAbilityOfMob(Mob mobToDisplayOver)
    {
        Assert.IsNotNull(mobToDisplayOver, "Mob is null.");
        if(mobToDisplayOver == instance.currentMob)
        {
            StopDisplayingAbility();
            return;
        }

        Ability mobAbility = mobToDisplayOver.GetAbility();
        Assert.IsNotNull(mobAbility, "Mob ability is null.");

        instance.abilityImage.enabled = true;
        instance.abilityImage.sprite = mobAbility.GetAbilityIcon();
        Vector3 abilityWorldPosition = mobToDisplayOver.GetPosition();
        Vector3 abilityScreenPosition = CameraController.WorldToScreenPoint(abilityWorldPosition);
        abilityScreenPosition += instance.iconOffsetFromMob;
        instance.abilityImage.transform.position = abilityScreenPosition;
        instance.currentMob = mobToDisplayOver;
    }

    /// <summary>
    /// Stops displaying the ability icon and description of the selected mob.
    /// </summary>
    public static void StopDisplayingAbility()
    {
        instance.currentMob = null;
        instance.abilityImage.sprite = null;
        instance.abilityImage.enabled = false;
        instance.tooltipImage.sprite = null;
        instance.tooltipImage.enabled = false;
        instance.tooltipText.text = "";
        instance.tooltipText.enabled = false;
    }

    /// <summary>
    /// Stops displaying the tooltip.
    /// </summary>
    public static void StopDisplayingTooltip()
    {
        instance.tooltipImage.sprite = null;
        instance.tooltipImage.enabled = false;
        instance.tooltipText.text = "";
        instance.tooltipText.enabled = false;
        instance.abilityImage.color = Color.white;
    }

    /// <summary>
    /// Returns true if an ability is currently being displayed.
    /// </summary>
    /// <returns>true if an ability is currently being displayed; otherwise, false.</returns>
    public static bool IsDisplayingAbility() => instance.abilityImage.enabled;

    #endregion

    #region Button Events

    /// <summary>
    /// Triggered when the ability icon is clicked. Displays
    /// a tooltip with the ability description.
    /// </summary>
    public void OnPointerEnterOrExitAbilityIcon()
    {
        Assert.IsNotNull(currentMob, "Current ability is null.");

        if(tooltipText.enabled) StopDisplayingTooltip();
        else
        {
            instance.tooltipImage.transform.position = instance.abilityImage.transform.position + instance.tooltipOffsetFromIcon;
            instance.tooltipImage.enabled = true;
            instance.tooltipText.enabled = true;
            instance.tooltipText.text = currentMob.GetAbility().GetAbilityDescription();
            instance.abilityImage.color = Color.yellow;
        }
    } 

    #endregion
}
