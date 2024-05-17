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
    /// Current tier of this Defender.
    /// </summary>
    private int tier;

    /// <summary>
    /// Gets the position of the tree on which this Defender sits.
    /// </summary>
    /// <param name="treePos">the position of the tree on which this Defender sits.
    /// /// </param>
    public void SetTreePosition(Vector3 treePos) { this.treePos = treePos; }

    /// <summary>
    /// Returns the position of the tree on which this Defender sits.
    /// </summary>
    /// <returns>the position of the tree on which this Defender sits.</returns>
    public Vector3 GetTreePosition() { return treePos; }

    /// <summary>
    /// Sets this Defender's tier to 1.
    /// </summary>
    public void ResetTier() { tier = 1; }

    /// <summary>
    /// Upgrades this Defender's tier by one if it is less than 3.
    /// </summary>
    public void UpgradeTier() { if (tier < 3) tier++;} 

    /// <summary>
    /// Returns this Defender's current tier.
    /// </summary>
    /// <returns>this Defender's current tier. </returns>
    public int GetTier() { return tier; }

    /// <summary>
    /// Resets this Defender's model.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetTier();
    }
}
