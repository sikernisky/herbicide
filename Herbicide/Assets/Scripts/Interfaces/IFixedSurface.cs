using UnityEngine;

/// <summary>
/// Contract for a model that represents a surface on the game
/// that never moves
/// </summary>
public interface IFixedSurface : ISurface, IHasCoordinates
{
    /// <summary>
    /// The eight ISurfaces that are adjacent to this IFixedSurface.
    /// </summary>
    ISurface[] Neighbors { get; }

    /// <summary>
    /// true if this IFixedSurface has been defined with fixed coordinates;
    /// otherwise, false. IFixedSurfaces cannot be moved once defined.
    /// </summary>
    bool Defined { get; }

    /// <summary>
    /// Returns true if a IFixedSurface can be placed on this ISurface.
    /// If it can, places the IFixedSurface.
    /// </summary>
    /// <param name="candidate">The IFixedSurface to place.</param>
    /// <returns>true if a IFixedSurface can be placed on this ISurface;
    /// otherwise, false.</returns>
    bool Place(IFixedSurface candidate);

    /// <summary>
    /// Sets the four IFixedSurface that are adjacent to this IFixedSurface.
    /// </summary>
    /// <param name="neighbors">the four IFixedSurface that are adjacent to this IFixedSurface.</param>
    void SetNeighbors(ISurface[] neighbors);

    /// <summary>
    /// Updates the appearance of this IFixedSurface based on its neighbors.
    /// </summary>
    void UpdateAppearanceBasedOnNeighbors();
}