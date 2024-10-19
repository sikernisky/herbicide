using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Owl Defender.
/// </summary>
public class Owl : Defender
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// DefenderClass of a Owl.
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.TREBUCHET;

    /// <summary>
    /// Starting attack range of a Owl.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 4f;

    /// <summary>
    /// Maximum attack range of a Owl.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Owl.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Starting attack speed of a Owl.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => 0.4f;

    /// <summary>
    /// Maximum attack speed of a Owl.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting chase range of a Owl.
    /// </summary>
    public override float BASE_CHASE_RANGE => BASE_MAIN_ACTION_RANGE;

    /// <summary>
    /// Maximum chase range of a Owl.
    /// </summary>
    public override float MAX_CHASE_RANGE => MAX_MAIN_ACTION_RANGE;

    /// <summary>
    /// Minimum chase range of a Owl.
    /// </summary>
    public override float MIN_CHASE_RANGE => MIN_MAIN_ACTION_RANGE;

    /// <summary>
    /// Starting movement speed of a Owl. 
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a Owl.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minimum movement speed of a Owl.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Starting health value of a Owl.
    /// </summary>
    public override float BASE_HEALTH => 100f;

    /// <summary>
    /// Largest health value a Owl can have.
    /// </summary>
    public override float MAX_HEALTH => 100f;

    /// <summary>
    /// Smallest health value a Owl can have.
    /// </summary>
    public override float MIN_HEALTH => 0f;

    /// <summary>
    /// How many seconds a Owl's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .2f;

    /// <summary>
    /// How many seconds a Owl's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => Mathf.Clamp(GetMainActionCooldownRemaining(), 0.0001f, float.MaxValue);

    /// <summary>
    /// Type of a Owl.
    /// </summary>
    public override ModelType TYPE => ModelType.OWL;

    /// <summary>
    /// Cost of a Owl.
    /// </summary>
    public override int COST => 75;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the (X, Y) dimensions of the Owl's placement track.
    /// </summary>
    /// <returns>the (X, Y) dimensions of the Owl's placement track.</returns>
    public override Vector2Int GetPlacementTrackDimensions() => new Vector2Int(32, 32);

    #endregion
}
