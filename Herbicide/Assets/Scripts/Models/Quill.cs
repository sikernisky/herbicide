/// <summary>
/// Represents a Quill projectile.
/// </summary>
public class Quill : Projectile
{
    #region Fields

    /// <summary>
    /// true if the Quill has exploded from the target it
    /// was stuck in; otherwise, false.
    /// </summary>
    private bool exploded;

    /// <summary>
    /// true if the Quill will split into two upon its target's death;
    /// otherwise, false.
    /// </summary>
    private bool doubleQuill;

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an Quill.
    /// </summary>
    public override ModelType TYPE => ModelType.QUILL;

    /// <summary>
    /// Starting speed of an Quill.
    /// </summary>
    public override float BaseSpeed => 25f;

    /// <summary>
    /// Maximum speed of an Quill.
    /// </summary>
    public override float MaxSpeed => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Quill.
    /// </summary>
    public override float MinSpeed => 0f;

    /// <summary>
    /// Starting damage of an Quill.
    /// </summary>
    public override int BaseDamage => 2;

    /// <summary>
    /// Maximum damage of an Quill.
    /// </summary>
    public override int MaxDamage => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Quill.
    /// </summary>
    public override int MinDamage => 0;

    /// <summary>
    /// Lifespan of an Quill.
    /// </summary>
    public override float Lifespan => float.MaxValue;

    /// <summary>
    /// How many seconds an Quill's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MovementAnimationDuration => 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the Quill to exploded.
    /// </summary>
    public void SetExploded() => exploded = true;

    /// <summary>
    /// Returns true if the Quill has exploded; otherwise, false.
    /// </summary>
    /// <returns>true if the Quill has exploded; otherwise, false. </returns>
    public bool HasExploded() => exploded;

    /// <summary>
    /// Sets this Quill as a double Quill. The Quill will split into two
    /// upon its target's death.
    /// </summary>
    public void SetAsDoubleQuill() => doubleQuill = true;
    /// <summary>
    /// Sets this Quill as a single Quill. The Quill will not split into two
    /// upon its target's death.
    /// </summary>

    private void SetAsSingleQuill() => doubleQuill = false;

    /// <summary>
    /// Returns true if the Quill is a double Quill; otherwise, false.
    /// </summary>
    /// <returns>true if the Quill is a double Quill; otherwise, false.</returns>
    public bool IsDoubleQuill() => doubleQuill;

    /// <summary>
    /// Resets the Quill's model.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        exploded = false;
        SetAsSingleQuill();
    }

    #endregion
}
