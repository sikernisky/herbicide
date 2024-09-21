using UnityEngine;

/// <summary>
/// Represents a Squirrel Defender.
/// </summary>
public class Squirrel : Defender
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Starting health of a Squirrel
    /// </summary>
    public override float BASE_HEALTH => 200;

    /// <summary>
    /// Maximum health of a Squirrel
    /// </summary>
    public override float MAX_HEALTH => 200;

    /// <summary>
    /// Minimum health of a Squirrel
    /// </summary>
    public override float MIN_HEALTH => 0;

    /// <summary>
    /// Starting attack range of a Squirrel
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 4f;

    /// <summary>
    /// Maximum attack range of a Squirrel
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Squirrel
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Number of attacks per second a Squirrel starts with.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => .5f;

    /// <summary>
    /// Most amount of attack cooldown this Squirrel can have.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting chase range of a Squirrel.
    /// </summary>
    public override float BASE_CHASE_RANGE => BASE_MAIN_ACTION_RANGE;

    /// <summary>
    /// Maximum chase range of a Squirrel.
    /// </summary>
    public override float MAX_CHASE_RANGE => MAX_MAIN_ACTION_RANGE;

    /// <summary>
    /// Minimum chase range of a Squirrel.
    /// </summary>
    public override float MIN_CHASE_RANGE => MAX_CHASE_RANGE;

    /// <summary>
    /// Starting movement speed of a Squirrel.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a Squirrel.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minumum movement speed of a Squirrel.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// How many seconds a Squirrel's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .25f;

    /// <summary>
    /// How many seconds a Squirrel's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => Mathf.Clamp(GetMainActionCooldown(), 0.0001f, float.MaxValue);

    /// <summary>
    /// How much currency it takes to place a Squirrel
    /// </summary>
    public override int COST => 50;

    /// <summary>
    /// Type of a Squirrel
    /// </summary>
    public override ModelType TYPE => ModelType.SQUIRREL;

    /// <summary>
    /// Class of a Squirrel
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.TREBUCHET;

    #endregion

    #region Methods

    /// <summary>
    /// Returns this Squirrel's current chase range, which is always its
    /// current attack range (since Squirrels do not chase & are stationary.)
    /// </summary>
    /// <returns>this Squirrel's current chase & attack range.</returns>
    public override float GetChaseRange() => GetMainActionRange();

    /// <summary>
    /// Returns the (X, Y) dimensions of the Bear's placement track.
    /// </summary>
    /// <returns>the (X, Y) dimensions of the Bear's placement track.</returns>
    public override Vector2Int GetPlacementTrackDimensions() => new Vector2Int(16, 20);

    #endregion
}

