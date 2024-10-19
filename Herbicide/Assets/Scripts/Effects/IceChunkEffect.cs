/// <summary>
/// Represents an Ice Chunk effect that can be applied to a Model.
/// Slows down the Model's movement speed.
/// </summary>
public class IceChunkEffect : TimedEffect, IMovementSpeedEffect
{
    #region Fields

    /// <summary>
    /// How long this effect lasts.
    /// </summary>
    private const float ICE_CHUNK_DURATION = 5.0f;

    /// <summary>
    /// The magnitude of the effect.
    /// </summary>
    private const float ICE_CHUNK_MAGNITUDE = -0.3f;

    /// <summary>
    /// The maximum number of stacks this effect can have.
    /// Can have infinite stacks.
    /// </summary>
    public override int MaxStacks => 1;

    /// <summary>
    /// The magnitude of the movement speed effect.
    /// </summary>
    public float MovementSpeedMagnitude { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new IceChunkEffect with the default duration and magnitude.
    /// </summary>
    public IceChunkEffect() : base(ICE_CHUNK_DURATION)
    {
        MovementSpeedMagnitude = ICE_CHUNK_MAGNITUDE;
    }

    /// <summary>
    /// Returns true if the IceChunkEffect can afflict the given Model.
    /// </summary>
    /// <param name="model">the model to check. </param>
    /// <returns>true if the IceChunkEffect can afflict the given Model;
    /// otherwise, false. </returns>
    public override bool CanAfflict(Model model) => model is Mob;

    #endregion
}
