using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Knotwood Enemy.
/// </summary>
public class Knotwood : Enemy
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Starting attack range of a Knotwood. 
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 1f;

    /// <summary>
    /// Maximum attack range of a Knotwood.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Knotwood.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Starting attack speed of a Knotwood.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => 1f;

    /// <summary>
    /// Maximum attack speed of a Knotwood.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting main action animation duration of a Knotwood.
    /// </summary>
    public override float BASE_MAIN_ACTION_ANIMATION_DURATION => .25f;

    /// <summary>
    /// Starting chase range of a Knotwood.
    /// </summary>
    public override float BASE_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Max chase range of a Knotwood.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Min chase range of a Knotwood.
    /// </summary>
    public override float MIN_CHASE_RANGE => float.MinValue;

    /// <summary>
    /// Starting movement speed of a Knotwood.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 1.5f;

    /// <summary>
    /// Maximum movement speed of a Knotwood.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum movement speed of a Knotwood.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Starting health of a Knotwood.
    /// </summary>
    public override float BASE_HEALTH => 20;

    /// <summary>
    /// Maximum health of a Knotwood.
    /// </summary>
    public override float MAX_HEALTH => BASE_HEALTH;

    /// <summary>
    /// Minimum health of a Knotwood.
    /// </summary>
    public override float MIN_HEALTH => 0f;

    /// <summary>
    /// ModelType of a Knotwood.
    /// </summary>
    public override ModelType TYPE => ModelType.KNOTWOOD;

    /// <summary>
    /// Starting movement animation duration of a Knotwood.
    /// </summary>
    public override float BASE_MOVEMENT_ANIMATION_DURATION => 0.3f;

    /// <summary>
    /// How many seconds a Knotwood's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// Attack damage of a Knotwood
    /// </summary>
    public float KICK_DAMAGE => 20f;

    #endregion

    #region Methods

    /// <summary>
    /// Sets this Knotwood's Collider2D properties.
    /// </summary>
    public override void SetColliderProperties()
    {
        BoxCollider2D collider = GetCollider() as BoxCollider2D;
        Assert.IsNotNull(collider, "Collider is null.");
        collider.offset = new Vector2(0, .5f);
    }

    /// <summary>
    /// Returns the position at which an attacker will aim at when
    /// attacking this Knotwood.
    /// </summary>
    /// <returns>this Knotwood's attack position.</returns>
    public override Vector3 GetAttackPosition()
    {
        float adjustedY = transform.position.y + .25f;
        return new Vector3(transform.position.x, adjustedY, transform.position.z);
    }

    #endregion
}
