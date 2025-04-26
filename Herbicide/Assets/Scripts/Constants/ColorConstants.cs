using UnityEngine;

/// <summary>
/// Constants for colors.
/// </summary>
public static class ColorConstants
{
    /// <summary>
    /// The alpha value for a ghost occupant.
    /// </summary>
    public const byte GhostOccupantAlpha = 200;

    /// <summary>
    /// The color of a range indicator.
    /// </summary>
    public static readonly Color32 SurfaceRangeIndicatorColor = new Color32(51, 255, 0, 128);

    /// <summary>
    /// The color of a victim when they are chilled.
    /// </summary>
    public static readonly Color32 ChilledModelColor = new Color32(100, 100, 255, 255);

    /// <summary>
    /// The base color of a model.
    /// </summary>
    public static readonly Color32 BaseModelColor = new Color32(255, 255, 255, 255);
}
