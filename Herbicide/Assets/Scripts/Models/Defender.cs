using System.Collections.Generic;
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

    /// <summary>
    /// true if this Defender draws a range indicator; false otherwise.
    /// </summary>
    public virtual bool DRAWS_RANGE_INDICATOR => true;

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

    /// <summary>
    /// Starting movement animation duration of a Defender.
    /// </summary>
    public override float BASE_MOVEMENT_ANIMATION_DURATION => 0;

    /// <summary>
    /// Returns the base attack speed of this Defender. Depends on the Defender's tier.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => CalculateBaseMainActionSpeed();

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
    /// Upgrades this Defender's tier to the given tier.
    /// </summary>
    /// <param name="newTier">the new tier of this Defender.</param>
    public virtual void Upgrade(int newTier)
    {
        Assert.IsFalse(newTier > MAX_TIER, "Cannot upgrade a Defender that is already at max tier.");
        Assert.IsTrue(newTier >= MIN_TIER, "Cannot upgrade a Defender that is at a tier less than 1.");

        tier = newTier;
        ResetBaseTint();
        RestartMainActionCooldown();
    }

    /// <summary>
    /// Returns the Defender's base main action speed. Depends on the Defender's tier.
    /// </summary>
    /// <returns>the Defender's base main action speed.</returns>
    protected abstract float CalculateBaseMainActionSpeed();

    /// <summary>
    /// Returns this Defender's current tier.
    /// </summary>
    /// <returns>this Defender's current tier. </returns>
    public int GetTier() => tier;

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

    /// <summary>
    /// Returns this Defender's starting base tint. Depends on the Defender's tier.
    /// </summary>
    /// <returns>this Defender's starting base tint.</returns>
    protected override Color32 CalculateStartingBaseTint()
    {
        if(GetTier() == 1) return Color.white;
        else if(GetTier() == 2) return Color.cyan;
        else return Color.magenta;
    }

    #endregion
}
