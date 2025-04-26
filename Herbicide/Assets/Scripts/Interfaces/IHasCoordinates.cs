using UnityEngine;

/// <summary>
/// Contract for objects that have coordinates.
/// </summary>
public interface IHasCoordinates
{
    /// <summary>
    /// The (X, Y) coordinates of this IHasCoordinates.
    /// </summary>
    Vector2Int Coordinates { get; }

    /// <summary>
    /// Defines this IHasCoordinates with its coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of this IHasCoordinates.</param>
    /// <param name="y">The Y-coordinate of this IHasCoordinates.</param>
    void DefineWithCoordinates(int x, int y);
}