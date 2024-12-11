using UnityEngine;

/// <summary>
/// Represents a Squirrel Defender.
/// </summary>
public class Squirrel : Defender
{
    #region Fields

    /// <summary>
    /// The amount of damage a Squirrel's acorn does in tier one.
    /// </summary>
    public int ACORN_DAMAGE_TIER_ONE => 5;

    /// <summary>
    /// The amount of damage a Squirrel's acorn does in tier two.
    /// </summary>
    public int ACORN_DAMAGE_TIER_TWO => 5;

    /// <summary>
    /// The amount of damage a Squirrel's acorn does in tier three.
    /// </summary>
    public int ACORN_DAMAGE_TIER_THREE => 5;

    /// <summary>
    /// The Ability of a Squirrel in tier two.
    /// </summary>
    [SerializeField]
    private Ability tierTwoAbility;

    /// <summary>
    /// The Ability of a Squirrel in tier three.
    /// </summary>
    [SerializeField]
    private Ability tierThreeAbility;

    #endregion

    #region Stats

    /// <summary>
    /// DefenderClass of a Squirrel.
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.TREBUCHET;

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
    public override float BASE_MAIN_ACTION_RANGE => 3f;

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
    public override float BASE_MAIN_ACTION_SPEED => 1.25f;

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
    public override float BASE_MAIN_ACTION_ANIMATION_DURATION => .25f;

    /// <summary>
    /// How many seconds a Squirrel's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => Mathf.Clamp(GetMainActionCooldownRemaining(), 0.0001f, float.MaxValue);

    /// <summary>
    /// How much currency it takes to place a Squirrel
    /// </summary>
    public override int COST => 50;

    /// <summary>
    /// Type of a Squirrel
    /// </summary>
    public override ModelType TYPE => ModelType.SQUIRREL;

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
    public override Vector2Int GetPlacementTrackDimensions() => new Vector2Int(32, 32);

    /// <summary>
    /// Returns the Squirrel's ability. This depends on the Squirrel's tier.
    /// </summary>
    /// <returns>the Squirrel's ability.</returns>
    public override Ability GetAbility()
    {
        if (GetTier() == 1) return base.GetAbility();
        else if (GetTier() == 2) return tierTwoAbility;
        else return tierThreeAbility;
    }

    #endregion
}

