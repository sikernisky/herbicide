/// <summary>
/// Represents an Ice Chunk effect that can be applied to a Model.
/// Slows down the Model's movement speed.
/// </summary>
public class IceChunkEffect : SpeedEffect
{
    /// <summary>
    /// How long this effect lasts.
    /// </summary>
    private const float ICE_CHUNK_DURATION = 3.0f;

    /// <summary>
    /// The magnitude of the effect.
    /// </summary>
    private const float ICE_CHUNK_MAGNITUDE = -0.2f;

    /// <summary>
    /// Makes a new IceChunkEffect with the default duration and magnitude.
    /// </summary>
    public IceChunkEffect() : base(ICE_CHUNK_DURATION, ICE_CHUNK_MAGNITUDE) { }

    /// <summary>
    /// Makes a new IceChunkEffect with the given duration and magnitude.
    /// </summary>
    /// <param name="duration">How long the effect lasts.</param>
    /// <param name="effectMagnitude">The magnitude of the effect. </param>
    public IceChunkEffect(float duration, float effectMagnitude) : base(duration, effectMagnitude) { }
}
