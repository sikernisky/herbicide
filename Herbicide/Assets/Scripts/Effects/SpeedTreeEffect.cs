/// <summary>
/// Represents an effect on a Model that is on a SpeedTree.
/// </summary>
public class SpeedTreeEffect : Effect, IAttackSpeedEffect
{
    #region Fields

    /// <summary>
    /// The amount of attack speed this effect modifies.
    /// </summary>
    public float AttackSpeedMagnitude { get; set; }

    /// <summary>
    /// The magnitude of the effect.
    /// </summary>
    private const float SPEED_TREE_MAGNITUDE = 1f;

    /// <summary>
    /// The maximum number of stacks this effect can have.
    /// Can have one stack.
    /// </summary>
    public override int MaxStacks => 1;

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new IceChunkEffect with the default duration and magnitude.
    /// </summary>
    public SpeedTreeEffect() =>  AttackSpeedMagnitude = SPEED_TREE_MAGNITUDE;

    /// <summary>
    /// Makes a copy of this SpeedTreeEffect.
    /// </summary>
    /// <param name="other">the given SpeedTreeEffect</param>
    protected SpeedTreeEffect(SpeedTreeEffect other) : base(other) => AttackSpeedMagnitude = other.AttackSpeedMagnitude;

    /// <summary>
    /// Returns a copy of this SpeedTreeEffect.
    /// </summary>
    /// <returns>a copy of this SpeedTreeEffect.</returns>
    public override Effect Clone() => new SpeedTreeEffect(this);

    /// <summary>
    /// Returns true if this effect can afflict the given Model.
    /// This effect can only afflict Trebuchet Defenders.
    /// </summary>
    /// <param name="model">The model to check. </param>
    /// <returns>true if this effect can afflict the given Model;
    /// otherwise, false. </returns>
    public override bool CanAfflict(Model model)
    {
        Defender defender = model as Defender;
        return defender != null && defender.CLASS == Defender.DefenderClass.TREBUCHET;
    }

    #endregion
}
