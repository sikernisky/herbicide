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
    /// The upgrades this Defender has.
    /// </summary>
    protected HashSet<UpgradeType> activeUpgrades;

    /// <summary>
    /// Current tier of this Defender.
    /// </summary>
    private int tier = MIN_TIER;

    /// <summary>
    /// Maximum tier of this Defender.
    /// </summary>
    public static readonly int MAX_TIER = 3;

    /// <summary>
    /// Lowest/starting tier of this Defender.
    /// </summary>
    public static readonly int MIN_TIER = 1;

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
    /// </param>
    public void SetTreePosition(Vector3 treePos) { this.treePos = treePos; }

    /// <summary>
    /// Returns the position of the tree on which this Defender sits.
    /// </summary>
    /// <returns>the position of the tree on which this Defender sits.</returns>
    public Vector3 GetTreePosition() { return treePos; }

    /// <summary>
    /// Sets this Defender's tier to 1.
    /// </summary>
    public void ResetTier() { tier = MIN_TIER; }

    /// <summary>
    /// Resets this Defender's upgrades.
    /// </summary>
    public void ResetUpgrades() 
    {
        if(activeUpgrades == null) activeUpgrades = new HashSet<UpgradeType>();
        else activeUpgrades.Clear();
    }

    /// <summary>
    /// Upgrades this Defender's tier by one if it is less than 3. Adds
    /// the upgrades the Defender gets at its new tier.
    /// </summary>
    public void UpgradeTier() 
    {
        if (tier < 3)
        {
            tier++;
            if(tier == 2) SetColor(Color.yellow);
            if(tier == 3) SetColor(Color.red);
            HashSet<UpgradeType> upgrades = GetUpgradesByTier(tier);
            foreach (UpgradeType upgrade in upgrades)
            {
                activeUpgrades.Add(upgrade);
            }
        }
    }

    /// <summary>
    /// Returns a HashSet of the upgrades that should be applied
    /// to this Defender at its current tier.
    /// </summary>
    /// <param name="tier">the tier of which to get upgrades.</param>
    /// <returns>a HashSet of the upgrades that should be applied
    /// to this Defender at its current tier.</returns>
    protected abstract HashSet<UpgradeType> GetUpgradesByTier(int tier);

    /// <summary>
    /// Returns true if this Defender has a certain upgrade; otherwise, false.
    /// </summary>
    /// <returns>true if this Defender has a certain upgrade; otherwise, false.</returns>
    public bool HasUpgrade(UpgradeType upgradeType) { return activeUpgrades.Contains(upgradeType); }   

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
        ResetUpgrades();
    }
}
