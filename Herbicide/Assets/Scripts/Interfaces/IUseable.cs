using UnityEngine;

/// <summary>
/// Contract for an object that can be placed onto
/// the world.
/// </summary>
public interface IUseable
{
    /// <summary>
    /// Returns the sprite animation track to be used during
    /// placement.
    /// </summary>
    /// <returns>the sprite animation track to be used during
    /// placement.</returns>
    Sprite[] GetPlacementTrack();

    /// <summary>
    /// Returns the X, Y size of the IUseable when it is being
    /// placed under the Canvas.
    /// </summary>
    /// <returns>the X, Y size of the IUseable when it is being
    /// placed under the Canvas.</returns>
    Vector2Int GetPlacementSize();

    /// <summary>
    /// Event that occurs when the IUseable is successfully placed.
    /// </summary>
    /// <param name="worldPosition">the world position where the IUseable was placed.</param>
    void OnPlace(Vector3 worldPosition);
}
