using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents something that is placed on the TileGrid
/// and helps the player win.
/// </summary>
public abstract class Defender : Mob
{
    /// <summary>
    /// Class of this Defender.
    /// </summary>
    public abstract DefenderClass CLASS { get; }

    /// <summary>
    /// The position of the tree on which this Defender sits.
    /// </summary>
    private Vector3 treePos;

    /// <summary>
    /// Class of this Defender.
    /// </summary>
    public enum DefenderClass
    {
        TREBUCHET,
        MAULER,
        COG,
        ANIMYST,
        WHISPERER
    }

    /// <summary>
    /// Gets the position of the tree on which this Defender sits.
    /// </summary>
    /// <param name="treePos">the position of the tree on which this Defender sits.
    /// /// </param>
    public void PassTreePosition(Vector3 treePos) { this.treePos = treePos; }

    /// <summary>
    /// Returns the position of the tree on which this Defender sits.
    /// </summary>
    /// <returns>the position of the tree on which this Defender sits.</returns>
    protected Vector3 GetTreePosition() { return treePos; }

    /// <summary>
    /// Returns the Euclidian distance from this Defender to a Model
    /// when this Defender is 1x1 and sits on a Tree.
    /// </summary>
    /// <param name="target">The Model from which to calculate distance.</param>
    public float DistanceToTargetFromTree(Model target)
    {
        Assert.IsNotNull(target);
        Assert.IsTrue(SIZE == new Vector2Int(1, 1));

        float distance = Vector3.Distance(GetTreePosition(), target.GetPosition());

        return distance;
    }
}
