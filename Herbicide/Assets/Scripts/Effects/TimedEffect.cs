using UnityEngine;
using UnityEngine.Assertions;

public abstract class TimedEffect : IEffect
{
    #region Fields

    /// <summary>
    /// How long this effect lasts.
    /// </summary>
    private float duration;

    /// <summary>
    /// Timer to manage the duration of this effect.
    /// </summary>
    private float timer;

    /// <summary>
    /// Returns true if this effect is currently active; otherwise, false.
    /// </summary>
    public bool IsEffectActive => timer > 0;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new SpeedEffect with the given duration and effect magnitude.
    /// </summary>
    /// <param name="duration">how long the effect lasts</param>
    public TimedEffect(float duration)
    {
        Assert.IsTrue(duration > 0, "Duration must be greater than 0.");

        this.duration = duration;
        timer = duration;
    }

    /// <summary>
    /// Apply this effect to the given Model.
    /// </summary>
    /// <param name="model">the Model to afflict. </param>
    public void OnApplyEffect(Model model)
    {
        Assert.IsTrue(CanAfflict(model), "Cannot afflict Model with this effect.");
        if (!IsEffectActive) timer = duration;
    }

    /// <summary>
    /// Called when this effect expires.
    /// </summary>
    /// <param name="model"></param>
    public void OnExpire(Model model) { }

    /// <summary>
    /// Updates this effect, decreasing the timer if the effect is active.
    /// </summary>
    /// <param name="model"></param>
    public void UpdateEffect(Model model)
    {
        if (timer > 0) timer -= Time.deltaTime;
        else if (IsEffectActive) OnExpire(model);
    }

    /// <summary>
    /// Returns true if this effect can afflict the given Model.
    /// </summary>
    /// <param name="model">the Model to check. </param>
    /// <returns> true if this effect can afflict the given Model;
    /// otherwise, false. </returns>
    public abstract bool CanAfflict(Model model);

    #endregion
}
