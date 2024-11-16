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
    /// Cost of placing a Kudzu from the inventory. 
    /// </summary>
    public override int COST => 0;

    /// <summary>
    /// Type of a Kudzu.
    /// </summary>
    public override ModelType TYPE => ModelType.KUDZU;

    /// <summary>
    /// Base health of a Kudzu.
    /// </summary>
    public override float BASE_HEALTH => 40;

    /// <summary>
    /// Upper bound of a Kudzu's health. 
    /// </summary>
    public override float MAX_HEALTH => BASE_HEALTH;

    /// <summary>
    /// Minimum health of a Kudzu
    /// </summary>
    public override float MIN_HEALTH => 0;

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
    public override float BASE_MAIN_ACTION_ANIMATION_DURATION => .25f;

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
    public override float BASE_MOVEMENT_ANIMATION_DURATION => 0.4f;
 
    /// <summary>
    /// How many seconds a Kudzu's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// Starting movement speed of a Kudzu.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 4f;

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

    /// <summary>
    /// The offset of the Model held by this Kudzu.
    /// </summary>
    public override Vector2 HOLDER_OFFSET => new Vector2(0, .25f);

    #endregion

    #region Methods

    /// <summary>
    /// Sets this Kudzu's Collider2D properties.
    /// </summary>
    public override void SetColliderProperties()
    {
        BoxCollider2D collider = GetCollider() as BoxCollider2D;
        Assert.IsNotNull(collider, "Collider is null.");
        collider.offset = new Vector2(0, .5f);
    }

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
        float hopCooldownMultiplier = baseMovementSpeed / currentMovementSpeed;

        return baseMovementSpeed * hopCooldownMultiplier;
    }

    #endregion
}
