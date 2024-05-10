using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Stone Wall. These are the most basic type of Wall:
/// they have infinite health, are immovable, and are impassable.
/// </summary>
public class StoneWall : Wall
{
    /// <summary>
    /// Name of this Stone Wall.
    /// </summary>
    public override string NAME => "Stone Wall";

    /// <summary>
    /// Type of a Stone Wall.
    /// </summary>
    public override ModelType TYPE => ModelType.STONE_WALL;

    /// <summary>
    /// Returns the GameObject that represents this StoneWall on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this StoneWall on the grid</returns>
    public override GameObject Copy() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns true if this StoneWall is dead, false otherwise.
    /// </summary>
    /// <returns>true if this StoneWall is dead, false otherwise.</returns>
    public override bool Dead() { return GetHealth() <= 0; }

    public override Sprite[] GetPlacementTrack()
    {
        throw new System.NotImplementedException();
    }

    public override void SetColliderProperties()
    {
        throw new System.NotImplementedException();
    }
}
