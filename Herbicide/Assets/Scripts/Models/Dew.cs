using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Dew currency.
/// </summary>
public class Dew : Currency
{
    /// <summary>
    /// Starting value of a Dew.
    /// </summary>
    public override int BASE_VALUE => 0;

    /// <summary>
    /// Maximum value of a Dew.
    /// </summary>
    public override int MAX_VALUE => int.MaxValue;

    /// <summary>
    /// Mimimum value of a Dew.
    /// </summary>
    public override int MIN_VALUE => 0;

    /// <summary>
    /// Type of a Dew.
    /// </summary>
    public override ModelType TYPE => ModelType.DEW;


    /// <summary>
    /// Returns the GameObject that represents this Dew on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this Dew on the grid.
    /// </returns>
    public override GameObject Copy() { return DewFactory.GetDewPrefab(); }

    /// <summary>
    /// Returns a Sprite that represents this Dew when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Dew when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() { return DewFactory.GetPlacementTrack(); }
}
