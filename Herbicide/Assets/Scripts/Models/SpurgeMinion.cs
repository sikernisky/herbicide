using UnityEngine;

public class SpurgeMinion : Enemy
{
    #region Fields

    /// <summary>
    /// The transform of the Spurge that this SpurgeMinion is following.
    /// </summary>
    private Transform spurgeTransform;

    #endregion

    #region Stats

    /// <summary>
    /// Cost of placing a SpurgeMinion from the inventory. 
    /// </summary>
    public override int COST => 0;

    /// <summary>
    /// Type of a SpurgeMinion.
    /// </summary>
    public override ModelType TYPE => ModelType.SPURGE_MINION;

    /// <summary>
    /// Base health of a SpurgeMinion.
    /// </summary>
    public override float BASE_HEALTH => 10;

    /// <summary>
    /// Upper bound of a SpurgeMinion's health. 
    /// </summary>
    public override float MAX_HEALTH => BASE_HEALTH;

    /// <summary>
    /// Minimum health of a SpurgeMinion
    /// </summary>
    public override float MIN_HEALTH => 0;

    /// <summary>
    /// Amount of attack cooldown this SpurgeMinion starts with.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => 1f;

    /// <summary>
    /// Most amount of attack cooldown this SpurgeMinion can have.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting main action animation duration of a SpurgeMinion.
    /// </summary>
    public override float BASE_MAIN_ACTION_ANIMATION_DURATION => .25f;

    /// <summary>
    /// Starting attack range of a SpurgeMinion.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 1f;

    /// <summary>
    /// Maximum attack range of a SpurgeMinion.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a SpurgeMinion.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Starting chase range of a SpurgeMinion.
    /// </summary>
    public override float BASE_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Maximum chase range of a SpurgeMinion.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a SpurgeMinion.
    /// </summary>
    public override float MIN_CHASE_RANGE => float.MinValue;

    /// <summary>
    /// Starting movement animation duration of a SpurgeMinion.
    /// </summary>
    public override float BASE_MOVEMENT_ANIMATION_DURATION => 0.4f;

    /// <summary>
    /// How many seconds a SpurgeMinion's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// Starting movement speed of a SpurgeMinion.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => .75f;

    /// <summary>
    /// Maximum movement speed of a SpurgeMinion.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minumum movement speed of a SpurgeMinion.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    #endregion

    #region Methods

    #endregion
}
