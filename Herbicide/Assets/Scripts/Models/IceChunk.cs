public class IceChunk : Projectile
{
    #region Fields

    /// <summary>
    /// The percentage by which an IceChunk slows down its target.
    /// 1 = 100% slow, 0.5 = 50% slow, etc.
    /// </summary>
    public float SLOW_PERCENTAGE => 0.25f;

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an IceChunk.
    /// </summary>
    public override ModelType TYPE => ModelType.ICE_CHUNK;

    /// <summary>
    /// Starting speed of an IceChunk.
    /// </summary>
    public override float BaseSpeed => 14f;

    /// <summary>
    /// Maximum speed of an IceChunk.
    /// </summary>
    public override float MaxSpeed => float.MaxValue;

    /// <summary>
    /// Minimum speed of an IceChunk.
    /// </summary>
    public override float MinSpeed => 0f;

    /// <summary>
    /// Starting damage of an IceChunk.
    /// </summary>
    public override int BaseDamage => 3;

    /// <summary>
    /// Maximum damage of an IceChunk.
    /// </summary>
    public override int MaxDamage => int.MaxValue;

    /// <summary>
    /// Minimum damage of an IceChunk.
    /// </summary>
    public override int MinDamage => 0;

    /// <summary>
    /// Lifespan of an IceChunk.
    /// </summary>
    public override float Lifespan => float.MaxValue;

    /// <summary>
    /// How many seconds an IceChunk's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MovementAnimationDuration => 0f;

    #endregion

    #region Methods

    #endregion
}
