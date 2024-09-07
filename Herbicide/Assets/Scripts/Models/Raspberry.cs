/// <summary>
/// Represents a Raspberry Projectile.
/// </summary>
public class Raspberry : Projectile
{
    #region Fields

    /// <summary>
    /// Explosion radius of an Raspberry.
    /// </summary>
    public float EXPLOSION_RADIUS => 2f;

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an Raspberry.
    /// </summary>
    public override ModelType TYPE => ModelType.RASPBERRY;

    /// <summary>
    /// Starting speed of an Raspberry.
    /// </summary>
    public override float BASE_SPEED => 1f;

    /// <summary>
    /// Maximum speed of an Raspberry.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Raspberry.
    /// </summary>
    public override float MIN_SPEED => 0f;

    /// <summary>
    /// Starting damage of an Raspberry.
    /// </summary>
    public override int BASE_DAMAGE => 6; //default: 7

    /// <summary>
    /// Maximum damage of an Raspberry.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Raspberry.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of an Raspberry.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;

    /// <summary>
    /// How many seconds an Raspberry's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;

    #endregion

    #region Methods

    #endregion
}
