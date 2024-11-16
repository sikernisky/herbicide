using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Bear Defender.
/// </summary>
public class Bear : Defender
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// DefenderClass of a Bear.
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.MAULER;

    /// <summary>
    /// Damage a Bear deals each chomp.
    /// </summary>
    public float CHOMP_DAMAGE => 15;

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
    public override float BASE_MAIN_ACTION_RANGE => 2f;

    /// <summary>
    /// Maximum attack range of a Bear.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Bear.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Starting attack cooldown of a Bear.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => .8f;

    /// <summary>
    /// Maximum attack cooldown of a Bear.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting main action animation duration of a Bear.
    /// </summary>
    public override float BASE_MAIN_ACTION_ANIMATION_DURATION => .25f;

    /// <summary>
    /// Starting chase range of a Bear.
    /// </summary>
    public override float BASE_CHASE_RANGE => BASE_MAIN_ACTION_RANGE;

    /// <summary>
    /// Maximum chase range of a Bear.
    /// </summary>
    public override float MAX_CHASE_RANGE => MAX_MAIN_ACTION_RANGE;

    /// <summary>
    /// Minimum chase range of a Bear.
    /// </summary>
    public override float MIN_CHASE_RANGE => MIN_MAIN_ACTION_RANGE;

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
    /// How many seconds a Bear's idle animation lasts,
    /// from start to finish.
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// How currency is required to buy a Bear.
    /// </summary>
    public override int COST => 75;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the (X, Y) dimensions of the Bear's placement track.
    /// </summary>
    /// <returns>the (X, Y) dimensions of the Bear's placement track.</returns>
    public override Vector2Int GetPlacementTrackDimensions() => new Vector2Int(19, 26);

    #endregion
}
