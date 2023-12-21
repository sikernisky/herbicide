using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an Acorn projectile thrown by a Squirrel.
/// </summary>
public class Acorn : Projectile
{
    /// <summary>
    /// ProjectileType of an Acorn.
    /// </summary>
    public override ProjectileType TYPE => ProjectileType.ACORN;

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
    public override int BASE_DAMAGE => 5;

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
    /// Name of an Acorn.
    /// </summary>
    public override string NAME => "Acorn";

    /// <summary>
    /// How many seconds an Acorn's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;
}
