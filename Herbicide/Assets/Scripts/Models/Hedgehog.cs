using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Hedgehog Defender.
/// </summary>
public class Hedgehog : Defender
{
    /// <summary>
    /// A Hedgehog's Defender Class.
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.WHISPERER;

    /// <summary>
    /// Starting attack range of a Hedgehog.
    /// </summary>
    public override float BASE_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Maximum attack range of a Hedgehog.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Hedgehog.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack cooldown of a Hedgehog.
    /// </summary>
    public override float BASE_ATTACK_COOLDOWN => 10f;

    /// <summary>
    /// Maximum attack cooldown of a Hedgehog.
    /// </summary>
    public override float MAX_ATTACK_COOLDOWN => float.MaxValue;

    /// <summary>
    /// Starting chase range of a Hedgehog.
    /// </summary>
    public override float BASE_CHASE_RANGE => BASE_ATTACK_RANGE;

    /// <summary>
    /// Maximum chase range of a Hedgehog.
    /// </summary>
    public override float MAX_CHASE_RANGE => MAX_ATTACK_RANGE;

    /// <summary>
    /// Minimum chase range of a Hedgehog.
    /// </summary>
    public override float MIN_CHASE_RANGE => MIN_ATTACK_RANGE;

    /// <summary>
    /// Starting movement speed of a Hedgehog.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a Hedgehog.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum movement speed of a Hedgehog.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Starting health of a Hedgehog.
    /// </summary>
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// Maximum health of a Hedgehog.
    /// </summary>
    public override int MAX_HEALTH => int.MaxValue;

    /// <summary>
    /// Minimum health of a Hedgehog.
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Name of a Hedgehog.
    /// </summary>
    public override string NAME => "Hedgehog";

    /// <summary>
    /// ModelType of a Hedgehog.
    /// </summary>
    public override ModelType TYPE => ModelType.HEDGEHOG;

    /// <summary>
    /// How many seconds a Hedgehog's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .25f;

    /// <summary>
    /// Returns an instantiated Hedgehog prefab.
    /// </summary>
    /// <returns>an instantiated Hedgehog prefab.</returns>
    public override GameObject Copy() { return HedgehogFactory.GetHedgehogPrefab(); }

    /// <summary>
    /// Returns the animation track that represents this Hedgehog when placing.
    /// </summary>
    /// <returns>the animation track that represents this Hedgehog when placing.</returns>
    public override Sprite[] GetPlacementTrack() { return HedgehogFactory.GetPlacementTrack(); }

    /// <summary>
    /// Sets this Hedgehog's 2D Collider properties.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
