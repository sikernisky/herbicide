using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents an effect that lasts for a certain duration.
/// </summary>
public abstract class TimedEffect : Effect
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
    public override bool IsEffectActive => timer > 0;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new SpeedEffect with the given duration and effect magnitude.
    /// </summary>
    /// <param name="duration">how long the effect lasts</param>
    public TimedEffect(float duration) : base()
    {
        Assert.IsTrue(duration > 0, "Duration must be greater than 0.");

        this.duration = duration;
        timer = duration;
    }

    /// <summary>
    /// Apply this effect to the given Model.
    /// </summary>
    /// <param name="model">the Model to afflict. </param>
    public override void OnApplyEffect(Model model)
    {
        base.OnApplyEffect(model);
        if (!IsEffectActive) timer = duration;
    }

    /// <summary>
    /// Called when this effect expires.
    /// </summary>
    /// <param name="model"></param>
    public override void OnExpire(Model model) { }

    /// <summary>
    /// Updates this effect, decreasing the timer if the effect is active.
    /// </summary>
    /// <param name="model"></param>
    public override void UpdateEffect(Model model)
    {
        base.UpdateEffect(model);
        if (timer > 0) timer -= Time.deltaTime;
    }

    #endregion
}
