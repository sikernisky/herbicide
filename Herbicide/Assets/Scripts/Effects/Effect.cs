using UnityEngine.Assertions;

/// <summary>
/// Represents an effect that can be applied to a Model.
/// </summary>
public abstract class Effect : IEffect
{
    #region Fields

    /// <summary>
    /// Returns true if this effect is currently active; otherwise, false.
    /// </summary>
    public virtual bool IsEffectActive => true;

    /// <summary>
    /// The number of Effect instances that can be applied to a Model.
    /// </summary>
    public abstract int MaxStacks { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new Effect.
    /// </summary>
    public Effect() { }

    /// <summary>
    /// Apply this Effect to the given Model.
    /// </summary>
    /// <param name="model">the Model to afflict. </param>
    public virtual void OnApplyEffect(Model model) => Assert.IsTrue(CanAfflict(model), "Cannot afflict Model with this effect.");

    /// <summary>
    /// Called when this Effect expires.
    /// </summary>
    /// <param name="model"></param>
    public virtual void OnExpire(Model model) { }

    /// <summary>
    /// Updates this Effect, decreasing the timer if the Effect is active.
    /// </summary>
    /// <param name="model"></param>
    public virtual void UpdateEffect(Model model)
    {
       if (!IsEffectActive) OnExpire(model);
    }

    /// <summary>
    /// Returns true if this Effect can afflict the given Model.
    /// </summary>
    /// <param name="model">the Model to check. </param>
    /// <returns> true if this Effect can afflict the given Model;
    /// otherwise, false. </returns>
    public abstract bool CanAfflict(Model model);

    #endregion
}
