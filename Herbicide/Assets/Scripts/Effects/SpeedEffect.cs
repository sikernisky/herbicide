/// <summary>
/// Represents an effect that increases or decreases a Model's movement speed.
/// </summary>
public class SpeedEffect : TimedEffect
{
    #region Fields

    /// <summary>
    /// How much to change the Model's move speed. For example, a value of 1.0f
    /// increases the Model's movement speed by 100%. A value of
    /// -0.5f decreases the Model's movement speed by 50%.
    /// </summary>
    private float speedAdjustmentMagnitude;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new SpeedEffect with the given duration and effect magnitude.
    /// </summary>
    /// <param name="duration">how long the effect lasts</param>
    /// <param name="effectMagnitude">how much to change the Model's move speed</param>
    public SpeedEffect(float duration, float effectMagnitude) : base(duration)
    {
        speedAdjustmentMagnitude = effectMagnitude;
    }

    /// <summary>
    /// Returns true if the SpeedEffect can afflict the given Model.
    /// SpeedEffects can only afflict Mobs.
    /// </summary>
    /// <param name="model">the Model to check.</param>
    /// <returns>true if the SpeedEffect can afflict the given Model;
    /// otherwise, false. </returns>
    public override bool CanAfflict(Model model) => model is Mob;

    /// <summary>
    /// Returns the magnitude of this effect.
    /// </summary>
    /// <returns>the magitude of this effect. </returns>
    public float GetEffectMagnitude() => speedAdjustmentMagnitude;

    #endregion
}
