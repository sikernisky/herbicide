/// <summary>
/// Contract for an effect that modifies a Mob's attack speed.
/// </summary>
public interface IAttackSpeedEffect : IEffect
{
    /// <summary>
    /// Amount of attack speed this effect modifies.
    /// </summary>
    float AttackSpeedMagnitude { get; set; }    
}
