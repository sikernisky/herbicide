/// <summary>
/// Contract for an effect that can be applied to a Model.
/// </summary>
public interface IEffect
{
    /// <summary>
    /// Called when this effect is Applied to the given Model.
    /// </summary>
    /// <param name="model">The Model on which to apply the effect.</param>
    void OnApplyEffect(Model model);

    /// <summary>
    /// Called when this effect is Removed from the given Model.
    /// </summary>
    /// <param name="model">The Model on which to remove the effect. </param>
    void OnExpire(Model model);

    /// <summary>
    /// Update this effect on the given Model.
    /// </summary>
    /// <param name="model">The Model on which to update the effect.</param>
    void UpdateEffect(Model model);

    /// <summary>
    /// Returns true if this effect can afflict the given Model;
    /// otherwise, false.
    /// </summary>
    /// <param name="model">The Model on which to apply the effect.</param>
    /// <returns>true if this effect can afflict the given Model;
    /// otherwise, false.</returns>
    bool CanAfflict(Model model);

    /// <summary>
    /// true if this effect is currently active; otherwise, false.
    /// </summary>
    bool IsEffectActive { get; }
}
