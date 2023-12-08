using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Bomb lobbed by a Butterfly.
/// </summary>
public class Bomb : Projectile
{
    /// <summary>
    /// ProjectileType of a Bomb.
    /// </summary>
    public override ProjectileType TYPE => ProjectileType.BOMB;

    /// <summary>
    /// Starting speed of a Bomb.
    /// </summary>
    public override float BASE_SPEED => 4.5f;

    /// <summary>
    /// Maximumum speed of a Bomb.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of a Bomb.
    /// </summary>
    public override float MIN_SPEED => 0;

    /// <summary>
    /// Starting damage of a Bomb.
    /// </summary>
    public override int BASE_DAMAGE => 20;
    /// <summary>
    /// Maximum damage of a Bomb.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of a Bomb.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of a Bomb.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;

    /// <summary>
    /// Name of a Bomb.
    /// </summary>
    public override string NAME => "Bomb";

    /// <summary>
    /// How many seconds a Bomb's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => .5f;

    /// <summary>
    /// The splatter GameObject.
    /// </summary>
    [SerializeField]
    private GameObject splatter;


    /// <summary>
    /// Returns the Bomb's splatter GameObject.
    /// </summary>
    /// <returns>the Bomb's splatter GameObject.</returns>
    public GameObject GetSplatter() { return splatter; }
}
