public class Spurge : Enemy
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Cost of placing a Spurge from the inventory. 
    /// </summary>
    public override int COST => 0;

    /// <summary>
    /// Type of a Spurge.
    /// </summary>
    public override ModelType TYPE => ModelType.SPURGE;

    /// <summary>
    /// Base health of a Spurge.
    /// </summary>
    public override float BASE_HEALTH => 100; // NORMAL VALUE: 100

    /// <summary>
    /// Upper bound of a Spurge's health. 
    /// </summary>
    public override float MAX_HEALTH => 100; // NORMAL VALUE: 100

    /// <summary>
    /// Minimum health of a Spurge
    /// </summary>
    public override float MIN_HEALTH => 0;

    /// <summary>
    /// Amount of attack cooldown this Spurge starts with.
    /// </summary>
    public override float BASE_ATTACK_SPEED => 1f;

    /// <summary>
    /// Most amount of attack cooldown this Spurge can have.
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Starting attack range of a Spurge.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 1f;

    /// <summary>
    /// Maximum attack range of a Spurge.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Spurge.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0;

    /// <summary>
    /// Starting chase range of a Spurge.
    /// </summary>
    public override float BASE_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Maximum chase range of a Spurge.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a Spurge.
    /// </summary>
    public override float MIN_CHASE_RANGE => float.MinValue;

    /// <summary>
    /// How many seconds a Spurge's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public float MOVE_ANIMATION_DURATION => .4f;

    /// <summary>
    /// How many seconds a Spurge's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .25f;

    /// <summary>
    /// How many seconds a Spurge's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// Starting movement speed of a Spurge.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 1f;

    /// <summary>
    /// Maximum movement speed of a Spurge.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minumum movement speed of a Spurge.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    #endregion

    #region Methods

    #endregion
}
