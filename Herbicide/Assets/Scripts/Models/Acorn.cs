using UnityEngine;

/// <summary>
/// Represents an Acorn projectile.
/// </summary>
public class Acorn : Projectile
{
    #region Fields

    /// <summary>
    /// ModelType of an Acorn.
    /// </summary>
    public override ModelType TYPE => ModelType.ACORN;

    /// <summary>
    /// Starting speed of an Acorn.
    /// </summary>
    public override float BaseSpeed => ModelStatConstants.AcornBaseSpeed;

    /// <summary>
    /// Maximum speed of an Acorn.
    /// </summary>
    public override float MaxSpeed => ModelStatConstants.AcornMaxSpeed;

    /// <summary>
    /// Minimum speed of an Acorn.
    /// </summary>
    public override float MinSpeed => ModelStatConstants.AcornMinSpeed;

    /// <summary>
    /// Starting damage of an Acorn.
    /// </summary>
    public override int BaseDamage => ModelStatConstants.AcornBaseDamage;

    /// <summary>
    /// Maximum damage of an Acorn.
    /// </summary>
    public override int MaxDamage => ModelStatConstants.AcornMaxDamage;

    /// <summary>
    /// Minimum damage of an Acorn.
    /// </summary>
    public override int MinDamage => ModelStatConstants.AcornMinDamage;

    /// <summary>
    /// Lifespan of an Acorn.
    /// </summary>
    public override float Lifespan => float.MaxValue;

    /// <summary>
    /// How many seconds an Acorn's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MovementAnimationDuration => 0f;

    #endregion

    #region Methods

    #endregion
}
