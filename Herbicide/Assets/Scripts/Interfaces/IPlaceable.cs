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

    /// <summary>
    /// Returns a GameObject that holds a SpriteRenderer component with
    /// this IPlaceable's placed Sprite. No other components are
    /// copied. 
    /// </summary>
    /// <returns>A GameObject with a SpriteRenderer component. </returns>
    GameObject MakeHollowObject();


}
