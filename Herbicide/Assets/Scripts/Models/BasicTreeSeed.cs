using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a BasicTreeSeed projectile.
/// </summary>
public class BasicTreeSeed : Projectile
{
    /// <summary>
    /// ModelType of a BasicTreeSeed.
    /// </summary>
    public override ModelType TYPE => ModelType.BASIC_TREE_SEED;

    /// <summary>
    /// Starting speed of a BasicTreeSeed.
    /// </summary>
    public override float BASE_SPEED => 3f;

    /// <summary>
    /// Maximum speed of a BasicTreeSeed.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of a BasicTreeSeed.
    /// </summary>
    public override float MIN_SPEED => 0f;

    /// <summary>
    /// Starting damage of a BasicTreeSeed.
    /// </summary>
    public override int BASE_DAMAGE => 5; //default: 5

    /// <summary>
    /// Maximum damage of a BasicTreeSeed.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of a BasicTreeSeed.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of a BasicTreeSeed.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;


    /// <summary>
    /// How many seconds an BasicTreeSeed's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;


    /// <summary>
    /// Returns the GameObject that represents this BasicTreeSeed on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this BasicTreeSeed on the grid.
    /// </returns>
    public override GameObject Copy() { return BasicTreeSeedFactory.GetBasicTreeSeedPrefab(); }

    /// <summary>
    /// Returns a Sprite that represents this BasicTreeSeed when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this BasicTreeSeed when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() { return BasicTreeSeedFactory.GetPlacementTrack(); }
}
