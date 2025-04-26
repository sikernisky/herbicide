using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// The list of allowed equipment types for this Defender.
    /// </summary>
    protected override HashSet<ModelType> AllowedEquipmentTypes => base.AllowedEquipmentTypes.Concat(ModelTypeConstants.AllowedEquipmentForDefenders).ToHashSet();

    #endregion

    #region Stats

    /// <summary>
    /// How much currency is required to buy this Model.
    /// </summary>
    public virtual int COST => 100;

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
    public override float BaseMovementAnimationDuration => 0;

    #endregion

    #region Methods

    /// <summary>
    /// Gets the position of the tree on which this Defender sits.
    /// </summary>
    /// <param name="treePos">the position of the tree on which this Defender sits.
    /// </param>
    public void ProvideTreePosition(Vector3 treePos) => this.treePos = treePos;

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
    public virtual void SetTier(int newTier)
    {
        Assert.IsFalse(newTier > MAX_TIER, "Cannot upgrade a Defender that is already at max tier.");
        Assert.IsTrue(newTier >= MIN_TIER, "Cannot upgrade a Defender that is at a tier less than 1.");

        tier = newTier;
        RestartMainActionCooldown();
    }

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
    /// Called when this Defender is placed or moved on the TileGrid.
    /// </summary>
    /// <param name="worldPosition">the world position where this Defender was
    /// placed.</param>
    public override void OnPlace(Vector3 worldPosition)
    {
        base.OnPlace(worldPosition);
        int coordX = TileGrid.PositionToCoordinate(GetTreePosition().x);
        int coordY = TileGrid.PositionToCoordinate(worldPosition.y);
        DefineWithCoordinates(coordX, coordY);
    }

    #endregion
}
