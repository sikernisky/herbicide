using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Bunny Defender.
/// </summary>
public class Bunny : Defender
{
    #region Fields

    /// <summary>
    /// Bunnies do not draw a range indicator.
    /// </summary>
    public override bool DRAWS_RANGE_INDICATOR => false;

    /// <summary>
    /// How much a Dew produced by a Bunny is worth at tier 1.
    /// </summary>
    public int DEW_VALUE_TIER_ONE => 15;

    /// <summary>
    /// How much a Dew produced by a Bunny is worth at tier 2.
    /// </summary>
    public int DEW_VALUE_TIER_TWO => 30;

    /// <summary>
    /// How much a Dew produced by a Bunny is worth at tier 3.
    /// </summary>
    public int DEW_VALUE_TIER_THREE => 60;

    #endregion

    #region Stats

    /// <summary>
    /// DefenderClass of a Bunny.
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.WHISPERER;

    /// <summary>
    /// Starting generation range of a Bunny. This is how far
    /// generated items can travel from the Bunny.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 2f;

    /// <summary>
    /// Maximum generation range of a Bunny.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum generation range of a Bunny.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Starting generation speed of a Bunny.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => .045f;

    /// <summary>
    /// Maximum generation speed of a Bunny.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting chase range of a Bunny.
    /// </summary>
    public override float BASE_CHASE_RANGE => BASE_MAIN_ACTION_RANGE;

    /// <summary>
    /// Maximum chase range of a Bunny.
    /// </summary>
    public override float MAX_CHASE_RANGE => MAX_MAIN_ACTION_RANGE;

    /// <summary>
    /// Minimum chase range of a Bunny.
    /// </summary>
    public override float MIN_CHASE_RANGE => MIN_MAIN_ACTION_RANGE;

    /// <summary>
    /// Starting movement speed of a Bunny. 
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a Bunny.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minimum movement speed of a Bunny.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Starting health value of a Bunny.
    /// </summary>
    public override float BASE_HEALTH => 100f;

    /// <summary>
    /// Largest health value a Bunny can have.
    /// </summary>
    public override float MAX_HEALTH => 100f;

    /// <summary>
    /// Smallest health value a Bunny can have.
    /// </summary>
    public override float MIN_HEALTH => 0f;

    /// <summary>
    /// How many seconds a Bunny's generation animation lasts,
    /// from start to finish. 
    /// </summary>
    public float GENERATION_ANIMATION_DURATION => .2f;

    /// <summary>
    /// How many seconds a Bunny's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => Mathf.Clamp(GetMainActionCooldownRemaining(), 0.0001f, float.MaxValue);

    /// <summary>
    /// Type of a Bunny.
    /// </summary>
    public override ModelType TYPE => ModelType.BUNNY;

    /// <summary>
    /// How much a Bunny costs to place.
    /// </summary>
    public override int COST => 25;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the (X, Y) dimensions of the Bunny's placement track.
    /// </summary>
    /// <returns>the (X, Y) dimensions of the Bunny's placement track.</returns>
    public override Vector2Int GetPlacementTrackDimensions() => new Vector2Int(25, 32);

    /// <summary>
    /// Called when this Bunny activates in the scene.
    /// </summary>
    public override void OnSpawn()
    {
        base.OnSpawn();
        RestartMainActionCooldown(); // Need to wait for the first generation.
    }

    #endregion
}
