/// <summary>
/// Represents a Blackberry Projectile.
/// </summary>
public class Blackberry : Projectile
{
    #region Fields

    /// <summary>
    /// Explosion radius of an Blackberry.
    /// </summary>
    public float EXPLOSION_RADIUS => 1.5f;

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an Blackberry.
    /// </summary>
    public override ModelType TYPE => ModelType.BLACKBERRY;

    /// <summary>
    /// Starting speed of an Blackberry.
    /// </summary>
    public override float BaseSpeed => 1f;

    /// <summary>
    /// Maximum speed of an Blackberry.
    /// </summary>
    public override float MaxSpeed => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Blackberry.
    /// </summary>
    public override float MinSpeed => 0f;

    /// <summary>
    /// Starting damage of an Blackberry.
    /// </summary>
    public override int BaseDamage => 2; 

    /// <summary>
    /// Maximum damage of an Blackberry.
    /// </summary>
    public override int MaxDamage => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Blackberry.
    /// </summary>
    public override int MinDamage => 0;

    /// <summary>
    /// Lifespan of an Blackberry.
    /// </summary>
    public override float Lifespan => float.MaxValue;

    /// <summary>
    /// How many seconds an Blackberry's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MovementAnimationDuration => 0f;

    #endregion

    #region Methods

    #endregion
}
