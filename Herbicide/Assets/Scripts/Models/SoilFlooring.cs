using UnityEngine;

/// <summary>
/// Represents a Soil Flooring. 
/// </summary>
public class SoilFlooring : Flooring
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Type of a SoilFlooring.
    /// </summary>
    public override ModelType TYPE => ModelType.SOIL_FLOORING;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the index representing the correct Sprite in this 
    /// SoilFlooring's tile set. The correct sprite is determined by whether its
    /// neighbors are null or valid. <br></br>
    /// </summary>
    /// <param name="neighbors">this SoilFlooring's neighbors</param>
    /// <returns>the index representing the correct Sprite in this 
    /// SoilFlooring's tile set.</returns>
    protected override int GetTilingIndex(ISurface[] neighbors)
    {
        bool hasNorth = neighbors[0].HasModel<SoilFlooring>();
        bool hasEast = neighbors[1].HasModel<SoilFlooring>();
        bool hasSouth = neighbors[2].HasModel<SoilFlooring>();
        bool hasWest = neighbors[3].HasModel<SoilFlooring>();

        if (hasEast && !hasWest && !hasSouth && !hasNorth) return 0;
        if (hasEast && hasWest && !hasSouth && !hasNorth) return 1;
        if (!hasEast && hasWest && !hasSouth && !hasNorth) return 2;
        if (!hasEast && !hasWest && hasSouth && !hasNorth) return 3;
        if (hasEast && !hasWest && hasSouth && !hasNorth) return 4;
        if (hasEast && hasWest && hasSouth && !hasNorth) return 5;
        if (!hasEast && hasWest && hasSouth && !hasNorth) return 6;
        if (!hasEast && !hasWest && hasSouth && hasNorth) return 7;
        if (hasEast && !hasWest && hasSouth && hasNorth) return 8;
        if (hasEast && hasWest && hasSouth && hasNorth) return 9;
        if (!hasEast && hasWest && hasSouth && hasNorth) return 10;
        if (!hasEast && !hasWest && !hasSouth && hasNorth) return 11;
        if (hasEast && !hasWest && !hasSouth && hasNorth) return 12;
        if (hasEast && hasWest && !hasSouth && hasNorth) return 13;
        if (!hasEast && hasWest && !hasSouth && hasNorth) return 14;
        return 15; // has no neighbors
    }

    /// <summary>
    /// Returns the GameObject that represents this SoilFlooring on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this SoilFlooring on the grid.
    /// </returns>
    public override GameObject CreateNew() => FlooringFactory.GetFlooringPrefab(ModelType.SOIL_FLOORING);

    /// <summary>
    /// R
    /// </summary>
    /// <returns></returns>
    public override bool Dead() => false;

    #endregion
}
