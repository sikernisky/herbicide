/// <summary>
/// Contract for effects that increase or decrease a Mob's movement speed.
/// </summary>
public interface IMovementSpeedEffect: IEffect
{
    /// <summary>
    /// The magnitude of the movement speed effect.
    /// </summary>
    public float MovementSpeedMagnitude { get; set; }
}
