/// <summary>
/// Represents a Salmonberry Projectile.
/// </summary>
public class Salmonberry : Projectile
{
    #region Fields

    /// <summary>
    /// Explosion radius of an Salmonberry.
    /// </summary>
    public float EXPLOSION_RADIUS => 2.5f;

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an Salmonberry.
    /// </summary>
    public override ModelType TYPE => ModelType.SALMONBERRY;

    /// <summary>
    /// Starting speed of an Salmonberry.
    /// </summary>
    public override float BASE_SPEED => 1f;

    /// <summary>
    /// Maximum speed of an Salmonberry.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Salmonberry.
    /// </summary>
    public override float MIN_SPEED => 0f;

    /// <summary>
    /// Starting damage of an Salmonberry.
    /// </summary>
    public override int BASE_DAMAGE => 10; //default: 7

    /// <summary>
    /// Maximum damage of an Salmonberry.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Salmonberry.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of an Salmonberry.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;

    /// <summary>
    /// How many seconds an Salmonberry's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;

    #endregion

    #region Methods

    #endregion
}
