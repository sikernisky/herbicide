using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Kudzu enemy.
/// </summary>
public class Kudzu : Enemy
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// The base hop cooldown of a Kudzu. This scales
    /// proportionally to the Kudzu's modified movement speed.
    /// </summary>
    private float BASE_HOP_COOLDOWN => 0.75f;

    /// <summary>
    /// Type of a Kudzu.
    /// </summary>
    public override ModelType TYPE => ModelType.KUDZU;

    /// <summary>
    /// Base health of a Kudzu.
    /// </summary>
    public override float BaseHealth => 25;

    /// <summary>
    /// Upper bound of a Kudzu's health. 
    /// </summary>
    public override float MaxHealth => BaseHealth;

    /// <summary>
    /// Minimum health of a Kudzu
    /// </summary>
    public override float MinHealth => 0;

    /// <summary>
    /// Amount of attack cooldown this Kudzu starts with.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => 1f;

    /// <summary>
    /// Most amount of attack cooldown this Kudzu can have.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting main action animation duration of a Kudzu.
    /// </summary>
    public override float BaseMainActionAnimationDuration => .25f;

    /// <summary>
    /// Damage a Kudzu does each attack.
    /// </summary>
    public int BONK_DAMAGE => 50;

    /// <summary>
    /// Starting attack range of a Kudzu.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 1f;

    /// <summary>
    /// Maximum attack range of a Kudzu.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Kudzu.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Starting chase range of a Kudzu.
    /// </summary>
    public override float BASE_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Maximum chase range of a Kudzu.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a Kudzu.
    /// </summary>
    public override float MIN_CHASE_RANGE => float.MinValue;

    /// <summary>
    /// Starting movement animation duration of a Kudzu.
    /// </summary>
    public override float BaseMovementAnimationDuration => 0.3f;
 
    /// <summary>
    /// How many seconds a Kudzu's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// Starting movement speed of a Kudzu.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 5f;

    /// <summary>
    /// Maximum movement speed of a Kudzu.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minumum movement speed of a Kudzu.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// How fast a Kudzu moves when entering the game.
    /// </summary>
    public override float ENTERING_MOVEMENT_SPEED => GetMovementSpeed();

    #endregion

    #region Methods

    /// <summary>
    /// Returns the position at which an attacker will aim at when
    /// attacking this Kudzu.
    /// </summary>
    /// <returns>this Kuduu's attack position.</returns>
    public override Vector3 GetAttackPosition()
    {
        float adjustedY = transform.position.y + .25f;
        return new Vector3(transform.position.x, adjustedY, transform.position.z);
    }

    /// <summary>
    /// Returns the Kudzu's hop cooldown. This is equal to the movement speed 
    /// times a constant value.
    /// </summary>
    /// <returns>the Kudzu's hop cooldown.</returns>
    public float GetHopCooldown()
    {
        float baseMovementSpeed = BASE_MOVEMENT_SPEED;
        float currentMovementSpeed = GetMovementSpeed();

        // Scale the cooldown inversely based on speed ratio
        return BASE_HOP_COOLDOWN * (baseMovementSpeed / currentMovementSpeed);
    }

    #endregion
}
