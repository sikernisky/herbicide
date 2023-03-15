using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a GameObject that can be placed on a Tile or Flooring
/// component.
/// </summary>
public interface IPlaceable : ISlottable
{
    //TODO: Implement in future sprint

    /// <summary>
    /// Returns and instantiates the prefab for this IPlaceable.
    /// </summary>
    /// <returns>the prefab for this IPlaceable.</returns>
    GameObject MakePlaceableObject();
}
