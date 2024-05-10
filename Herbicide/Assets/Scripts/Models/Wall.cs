using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Wall. 
/// </summary>
public abstract class Wall : PlaceableObject
{
    /// <summary>
    /// Starting health of a Wall
    /// </summary>
    public override int BASE_HEALTH => int.MaxValue;

    /// <summary>
    /// Maximum health of a Wall
    /// </summary> 
    public override int MAX_HEALTH => int.MaxValue;
    /// <summary>
    /// Minimum health of a Wall
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// true if this Wall is an occupier, false otherwise.
    /// </summary>
    public override bool OCCUPIER => true;
}
