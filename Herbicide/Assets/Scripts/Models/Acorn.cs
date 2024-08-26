using UnityEngine;

/// <summary>
/// Represents an Acorn projectile.
/// </summary>
public class Acorn : Projectile
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an Acorn.
    /// </summary>
    public override ModelType TYPE => ModelType.ACORN;

    /// <summary>
    /// Starting speed of an Acorn.
    /// </summary>
    public override float BASE_SPEED => 9f;

    /// <summary>
    /// Maximum speed of an Acorn.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Acorn.
    /// </summary>
    public override float MIN_SPEED => 0f;

    /// <summary>
    /// Starting damage of an Acorn.
    /// </summary>
    public override int BASE_DAMAGE => 100; //default: 5

    /// <summary>
    /// Maximum damage of an Acorn.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Acorn.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of an Acorn.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;

    /// <summary>
    /// How many seconds an Acorn's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the GameObject that represents this Acorn on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this Acorn on the grid.
    /// </returns>
    public override GameObject Copy() { return ProjectileFactory.GetProjectilePrefab(TYPE); }

    /// <summary>
    /// Returns a Sprite that represents this Acorn when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Acorn when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() { return ProjectileFactory.GetPlacementTrack(TYPE); }

    #endregion
}
