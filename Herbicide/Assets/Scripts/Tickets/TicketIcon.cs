using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Component for a ticket that represents a ModelType in its
/// sequence of ModelTypes. Lights up when the ModelType the icon
/// represents has been obtained.
/// </summary>
public class TicketIcon : UIModel
{
    #region Fields

    /// <summary>
    /// Image component that displays the icon's spash.
    /// </summary>
    [SerializeField]
    private Image splash;

    /// <summary>
    /// Image component that displays the icon's shadow.
    /// </summary>
    [SerializeField]
    private Image splashShadow;

    /// <summary>
    /// The original color of the icon.
    /// </summary>
    private Color originalColor;

    #endregion

    #region Methods

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake() => originalColor = splash.color;

    /// <summary>
    /// Lights up the icon.
    /// </summary>
    public void LightUp() => splash.color = originalColor;

    /// <summary>
    /// Darkens the icon.
    /// </summary>
    public void Darken()
    {
        Color adjustedColor = originalColor * .5f; // Scale RGB values
        adjustedColor.a = originalColor.a; // Keep alpha unchanged
        splash.color = adjustedColor;
    }

    /// <summary>
    /// Hides the icon.
    /// </summary>
    public void Hide()
    {
        splash.enabled = false;
        splashShadow.enabled = false;
    }

    /// <summary>
    /// Shows the icon.
    /// </summary>
    public void Show()
    {
        splash.enabled = true;
        splashShadow.enabled = true;
    }

    /// <summary>
    /// Sets the icon to the given sprite.
    /// </summary>
    /// <param name="sprite">the given sprite. </param>
    public void SetSplash(Sprite sprite)
    {
        splash.sprite = sprite;
        splashShadow.sprite = sprite;
    }

    /// <summary>
    /// Returns the ModelType of this UIModel.
    /// </summary>
    /// <returns>the ModelType of this UIModel.</returns>
    public override ModelType GetModelType() => ModelType.TICKET_ICON;

    #endregion
}
