using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Squirrel Defender. It Bites Enemies.
/// </summary>
public class Bear : Defender
{
    /// <summary>
    /// Bears are Maulers.
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.MAULER;

    /// <summary>
    /// Damage a Bear deals each chomp.
    /// </summary>
    public float CHOMP_DAMAGE => 10;

    /// <summary>
    /// Damage a Bear deals over a bleed effect. 
    /// </summary>
    public float BLEED_DAMAGE => 7;

    /// <summary>
    /// Damage a Bear deals over a bleed effect. 
    /// </summary>
    public float BLEED_DURATION => .75f;

    /// <summary>
    /// The number of ticks a Bear's bleed effect deals.
    /// </summary>
    public int BLEED_TICKS => 30;

    /// <summary>
    /// Starting attack range of a Bear.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 2f;

    /// <summary>
    /// Maximum attack range of a Bear.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Bear.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack cooldown of a Bear.
    /// </summary>
    public override float BASE_ATTACK_SPEED => .75f;

    /// <summary>
    /// Maximum attack cooldown of a Bear.
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Starting chase range of a Bear.
    /// </summary>
    public override float BASE_CHASE_RANGE => BASE_ATTACK_RANGE;

    /// <summary>
    /// Maximum chase range of a Bear.
    /// </summary>
    public override float MAX_CHASE_RANGE => MAX_ATTACK_RANGE;

    /// <summary>
    /// Minimum chase range of a Bear.
    /// </summary>
    public override float MIN_CHASE_RANGE => MIN_ATTACK_RANGE;

    /// <summary>
    /// Starting movement speed of a Bear.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a Bear.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum movement speed of a Bear.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Starting health of a Bear.
    /// </summary>
    public override float BASE_HEALTH => 300;

    /// <summary>
    /// Maximum health of a Bear.
    /// </summary>
    public override float MAX_HEALTH => int.MaxValue;

    /// <summary>
    /// Minimum health of a Bear.
    /// </summary>
    public override float MIN_HEALTH => 0;

    /// <summary>
    /// ModelType of a Bear.
    /// </summary>
    public override ModelType TYPE => ModelType.BEAR;

    /// <summary>
    /// How many seconds a Bear's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .25f;

    /// <summary>
    /// How many seconds a Bear's idle animation lasts,
    /// from start to finish.
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// How currency is required to buy a Bear.
    /// </summary>
    public override int COST => 75;


    /// <summary>
    /// Returns an instantiated GameObject with a Bear component attached.
    /// </summary>
    /// <returns>an instantiated GameObject with a Bear component
    ///  attached.</returns>
    public override GameObject Copy() { return DefenderFactory.GetDefenderPrefab(ModelType.BEAR); }

    /// <summary>
    /// Returns the animation track that represents this Bear when placing.
    /// </summary>
    /// <returns>the animation track that represents this Bear when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack()
    {
        return DefenderFactory.GetPlacementTrack(ModelType.BEAR, GetTier());
    }

    /// <summary>
    /// Returns the (X, Y) dimensions of the Bear's placement track.
    /// </summary>
    /// <returns>the (X, Y) dimensions of the Bear's placement track.</returns>
    public override Vector2Int GetPlacementTrackDimensions()
    {
        return new Vector2Int(19, 32);
    }

    /// <summary>
    /// Sets this Bear's 2D Collider properties.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
