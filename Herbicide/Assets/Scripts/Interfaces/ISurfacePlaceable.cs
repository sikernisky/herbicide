using UnityEngine;

/// <summary>
/// Contract for an object that can be placed onto the TileGrid.
/// </summary>
public interface ISurfacePlaceable : IUseable
{
    /// <summary>
    /// The Object of the ISurfacePlaceable.
    /// </summary>
    PlaceableObject Object { get; }

    /// <summary>
    /// true if this ISurfacePlaceable prevents traversal of the surface
    /// it is placed on; otherwise, false.
    /// </summary>
    bool IsTraversable { get; }

    /// <summary>
    /// true if this ISurfacePlaceable has been placed; otherwise, false.
    /// </summary>
    bool PlacedOnSurface { get; }
}
