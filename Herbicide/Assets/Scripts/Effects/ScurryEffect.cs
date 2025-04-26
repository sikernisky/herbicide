/// <summary>
/// Represents an effect granted by completing the Acornol ticket,
/// which increases all Squirrels' attack speed.
/// </summary>
public class AcornolEffect : Effect, IAttackSpeedEffect
{
    /// <summary>
    /// Maximym number of stacks this effect can have.
    /// </summary>
    public override int MaxStacks => int.MaxValue;

    /// <summary>
    /// The magnitude of the effect.
    /// </summary>
    private const float ACORNOL_MAGNITUDE = 0.1f;

    /// <summary>
    /// The magnitude of the effect.
    /// </summary>
    public float AttackSpeedMagnitude { get; set; }

    /// <summary>
    /// Creates a new AcornolEffect with the default magnitude.
    /// </summary>
    public AcornolEffect() => AttackSpeedMagnitude = ACORNOL_MAGNITUDE;

    /// <summary>
    /// Creates a new AcornolEffect with the same values as the given AcornolEffect.
    /// </summary>
    /// <param name="other">the given Effect</param>
    protected AcornolEffect(AcornolEffect other) : base(other) => AttackSpeedMagnitude = other.AttackSpeedMagnitude;

    /// <summary>
    /// Returns a copy of this AcornolEffect.
    /// </summary>
    /// <returns>a copy of this AcornolEffect.</returns>
    public override Effect Clone() => new AcornolEffect(this);

    /// <summary>
    /// Returns true if the AcornolEffect can afflict the given Model.
    /// It can only afflict Squirrels.
    /// </summary>
    /// <param name="model">the model to check.</param>
    /// <returns>true if the AcornolEffect can afflict the given Model;
    /// otherwise, false. </returns>
    public override bool CanAfflict(Model model) => model is Squirrel;
}
