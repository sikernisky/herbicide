using UnityEngine;

/// <summary>
/// Represents a Porcupine Defender.
/// </summary>
public class Porcupine : Defender
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// DefenderClass of a Porcupine.
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.TREBUCHET;

    /// <summary>
    /// Starting attack range of a Porcupine.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 5f;

    /// <summary>
    /// Maximum attack range of a Porcupine.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Porcupine.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack speed of a Porcupine.
    /// </summary>
    public override float BASE_ATTACK_SPEED => .6f;

    /// <summary>
    /// Maximum attack speed of a Porcupine.
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Starting chase range of a Porcupine.
    /// </summary>
    public override float BASE_CHASE_RANGE => BASE_ATTACK_RANGE;

    /// <summary>
    /// Maximum chase range of a Porcupine.
    /// </summary>
    public override float MAX_CHASE_RANGE => MAX_ATTACK_RANGE;

    /// <summary>
    /// Minimum chase range of a Porcupine.
    /// </summary>
    public override float MIN_CHASE_RANGE => MIN_ATTACK_RANGE;

    /// <summary>
    /// Starting movement speed of a Porcupine. 
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a Porcupine.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minimum movement speed of a Porcupine.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Starting health value of a Porcupine.
    /// </summary>
    public override float BASE_HEALTH => 100f;

    /// <summary>
    /// Largest health value a Porcupine can have.
    /// </summary>
    public override float MAX_HEALTH => 100f;

    /// <summary>
    /// Smallest health value a Porcupine can have.
    /// </summary>
    public override float MIN_HEALTH => 0f;

    /// <summary>
    /// How many seconds a Porcupine's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .2f;

    /// <summary>
    /// How many seconds a Porcupine's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => Mathf.Clamp(GetAttackCooldown(), 0.0001f, float.MaxValue);

    /// <summary>
    /// Type of a Porcupine.
    /// </summary>
    public override ModelType TYPE => ModelType.PORCUPINE;

    #endregion

    #region Methods

    /// <summary>
    /// Returns a new copy of a Porcupine prefab. 
    /// </summary>
    /// <returns>a new copy of a Porcupine prefab. </returns>
    public override GameObject Copy() {
        return DefenderFactory.GetDefenderPrefab(TYPE); 
    }

    /// <summary>
    /// Returns the Porcupine's animation track when placing.
    /// </summary>
    /// <returns>the Porcupine's animation track when placing.</returns>
    public override Sprite[] GetPlacementTrack()
    {
        return DefenderFactory.GetPlacementTrack(TYPE, GetTier());
    }

    /// <summary>
    /// Returns the (X, Y) dimensions of the Porcupine's placement track.
    /// </summary>
    /// <returns>the (X, Y) dimensions of the Porcupine's placement track.</returns>
    public override Vector2Int GetPlacementTrackDimensions()
    {
        return new Vector2Int(32, 32);
    }

    /// <summary>
    /// Sets the Porcupine's BoxCollider properties.
    /// </summary>
    public override void SetColliderProperties()
    {
        return; // no box collider
    }

    #endregion
}
