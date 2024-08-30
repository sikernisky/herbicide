using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Model that can be placed on a Tree
/// to defend the player from enemies.
/// </summary>
public abstract class Defender : Mob
{
    #region Fields

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
    /// The position of the tree on which this Defender sits.
    /// </summary>
    private Vector3 treePos;

    /// <summary>
    /// Current tier of this Defender.
    /// </summary>
    private int tier = MIN_TIER;

    #endregion

    #region Stats

    /// <summary>
    /// Class of this Defender.
    /// </summary>
    public abstract DefenderClass CLASS { get; }

    /// <summary>
    /// Maximum tier of this Defender.
    /// </summary>
    public static readonly int MAX_TIER = 3;

    /// <summary>
    /// Lowest/starting tier of this Defender.
    /// </summary>
    public static readonly int MIN_TIER = 1;

    #endregion

    #region Methods

    /// <summary>
    /// Gets the position of the tree on which this Defender sits.
    /// </summary>
    /// <param name="treePos">the position of the tree on which this Defender sits.
    /// </param>
    public void SetTreePosition(Vector3 treePos) => this.treePos = treePos;

    /// <summary>
    /// Returns the position of the tree on which this Defender sits.
    /// </summary>
    /// <returns>the position of the tree on which this Defender sits.</returns>
    public Vector3 GetTreePosition() => treePos;

    /// <summary>
    /// Sets this Defender's tier to 1.
    /// </summary>
    public void ResetTier() => tier = MIN_TIER;

    /// <summary>
    /// Upgrades this Defender's tier by one if it is less than 3. Adds
    /// the upgrades the Defender gets at its new tier.
    /// </summary>
    public virtual void Upgrade()
    {
        Assert.IsFalse(tier == MAX_TIER, "Cannot upgrade a Defender that is already at max tier.");
        Assert.IsTrue(tier >= MIN_TIER, "Cannot upgrade a Defender that is at a tier less than 1.");

        if (tier < MAX_TIER) tier++;
        RestartAttackCooldown();
    }

    /// <summary>
    /// Returns this Defender's current tier.
    /// </summary>
    /// <returns>this Defender's current tier. </returns>
    public int GetTier() => tier;

    /// <summary>
    /// Resets this Defender's model.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetTier();
    }

    /// <summary>
    /// Returns a fresh copy of this Defender from the object pool.
    /// </summary>
    /// <returns>a fresh copy of this Defender from the object pool.</returns>
    public override GameObject CreateNew() => DefenderFactory.GetDefenderPrefab(TYPE);

    /// <summary>
    /// Returns the animation track that represents this Defender when placing.
    /// </summary>
    /// <returns>the animation track that represents this Defender when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack() => DefenderFactory.GetPlacementTrack(TYPE, GetTier());

    #endregion
}
