using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Data Structure for a Damage Over Time effect.
/// </summary>
public class DamageOverTime
{
    #region Fields

    /// <summary>
    /// Type of DamageOverTime effect.
    /// </summary>
    public enum DOTType
    {
        BEAR_CHOMP
    }

    /// <summary>
    /// Total damage the effect will deal.
    /// </summary>
    private float totalDamage;

    /// <summary>
    /// Total time the effect lasts.
    /// </summary>
    private float duration;

    /// <summary>
    /// How many times this DamageOverTime will tick.
    /// </summary>
    private int numTicks;

    /// <summary>
    /// Number of seconds since the effect started.
    /// </summary>
    private float elapsedTime;

    /// <summary>
    /// Number of seconds since the last tick.
    /// </summary>
    private float tickTimer;

    /// <summary>
    /// The type of DamageOverTime effect.
    /// </summary>
    private DOTType dotType;

    #endregion

    #region Methods

    /// <summary>
    /// Constructs a new DamageOverTime effect.
    /// </summary>
    /// <param name="totalDamage">The total damage the effect will deal.</param>
    /// <param name="duration">How many seconds the effect will last. </param>
    /// <param name="numTicks">How many times this DOT will tick. </param>
    /// <param name="dotType">The type of DamageOverTime effect. </param>
    public DamageOverTime(float totalDamage, float duration, int numTicks, DOTType dotType)
    {
        Assert.IsTrue(duration > 0, "Duration must be greater than 0.");

        this.totalDamage = totalDamage;
        this.duration = duration;
        this.numTicks = numTicks;
        this.dotType = dotType;
    }

    /// <summary>
    /// Returns true if the victim of this effect should take damage.
    /// Updates the effect's internal timers.
    /// </summary>
    /// <returns>true if the victim of this effect should take damage; otherwise,
    /// false. </returns>
    public bool UpdateDamageOverTime()
    {
        elapsedTime += Time.deltaTime;
        tickTimer += Time.deltaTime;

        float tickInterval = duration / numTicks;

        if (tickTimer >= tickInterval)
        {
            tickTimer = 0;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true if the effect is finished.
    /// </summary>
    /// <returns>true if the effect is finished; otherwise, false. </returns>
    public bool IsFinished() => elapsedTime >= duration;

    /// <summary>
    /// Returns the amount of damage to deal per tick.
    /// </summary>
    /// <returns>the amount of damage to deal per tick.</returns>
    public float GetDamage() => totalDamage / numTicks;

    /// <summary>
    /// Returns true if the effect stacks.
    /// </summary>
    /// <returns>true if the effect stacks; otherwise, false. </returns>
    public bool DoesStack()
    {
        switch (dotType)
        {
            case DOTType.BEAR_CHOMP:
                return false;
            default:
                return false;
        }
    }

    /// <summary>
    /// Returns the type of DamageOverTime effect.
    /// </summary>
    /// <returns>the type of DamageOverTime effect.</returns>
    public DOTType GetDOTType() => dotType;

    #endregion
}
