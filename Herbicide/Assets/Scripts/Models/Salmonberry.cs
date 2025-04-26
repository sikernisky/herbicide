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
    public override float BaseSpeed => 1f;

    /// <summary>
    /// Maximum speed of an Salmonberry.
    /// </summary>
    public override float MaxSpeed => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Salmonberry.
    /// </summary>
    public override float MinSpeed => 0f;

    /// <summary>
    /// Starting damage of an Salmonberry.
    /// </summary>
    public override int BaseDamage => 10; //default: 7

    /// <summary>
    /// Maximum damage of an Salmonberry.
    /// </summary>
    public override int MaxDamage => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Salmonberry.
    /// </summary>
    public override int MinDamage => 0;

    /// <summary>
    /// Lifespan of an Salmonberry.
    /// </summary>
    public override float Lifespan => float.MaxValue;

    /// <summary>
    /// How many seconds an Salmonberry's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MovementAnimationDuration => 0f;

    #endregion

    #region Methods

    #endregion
}
