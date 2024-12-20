/// <summary>
/// Contract for an effect that modifies a Mob's attack speed.
/// </summary>
public interface IAttackSpeedEffect : IEffect
{
    /// <summary>
    /// Amount of attack speed this effect modifies. A modifier
    /// of 1.0f means an increase of 100%.
    /// </summary>
    float AttackSpeedMagnitude { get; set; }    
}
