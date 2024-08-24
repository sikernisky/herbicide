using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Quill projectile.
/// </summary>
public class Quill : Projectile
{
    /// <summary>
    /// ModelType of an Quill.
    /// </summary>
    public override ModelType TYPE => ModelType.QUILL;

    /// <summary>
    /// Starting speed of an Quill.
    /// </summary>
    public override float BASE_SPEED => 18f;

    /// <summary>
    /// Maximum speed of an Quill.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Quill.
    /// </summary>
    public override float MIN_SPEED => 0f;

    /// <summary>
    /// Starting damage of an Quill.
    /// </summary>
    public override int BASE_DAMAGE => 3; //default: 3

    /// <summary>
    /// Maximum damage of an Quill.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Quill.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of an Quill.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;

    /// <summary>
    /// How many seconds an Quill's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;


    /// <summary>
    /// Returns the GameObject that represents this Quill on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this Quill on the grid.
    /// </returns>
    public override GameObject Copy() { return ProjectileFactory.GetProjectilePrefab(TYPE); }

    /// <summary>
    /// Returns a Sprite that represents this Quill when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Quill when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() { return ProjectileFactory.GetPlacementTrack(TYPE); }
}
