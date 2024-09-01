using UnityEngine;

public class Blackberry : Projectile
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an Blackberry.
    /// </summary>
    public override ModelType TYPE => ModelType.BLACKBERRY;

    /// <summary>
    /// Starting speed of an Blackberry.
    /// </summary>
    public override float BASE_SPEED => 9f;

    /// <summary>
    /// Maximum speed of an Blackberry.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Blackberry.
    /// </summary>
    public override float MIN_SPEED => 0f;

    /// <summary>
    /// Starting damage of an Blackberry.
    /// </summary>
    public override int BASE_DAMAGE => 5; //default: 5

    /// <summary>
    /// Maximum damage of an Blackberry.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Blackberry.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of an Blackberry.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;

    /// <summary>
    /// How many seconds an Blackberry's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;

    #endregion
}
