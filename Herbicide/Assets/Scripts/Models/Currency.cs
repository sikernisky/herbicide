using UnityEngine;

/// <summary>
/// Represents a Collectable, that when collected, gives the player
/// a form of currency.
/// </summary>
public abstract class Currency : Collectable
{
    #region Fields

    /// <summary>
    /// Current value of this Currency.
    /// </summary>
    private int value;

    #endregion

    #region Stats

    /// <summary>
    /// Starting value of this currency.
    /// </summary>
    public virtual int BASE_VALUE => 1;

    /// <summary>
    /// Maximum value of this currency.
    /// </summary>
    public virtual int MAX_VALUE => int.MaxValue;

    /// <summary>
    /// Mimimum value of this currency.
    /// </summary>
    public virtual int MIN_VALUE => int.MinValue;

    #endregion

    #region Methods

    /// <summary>
    /// Performs actions when this Currency spawns in game. Sets the
    /// starting position for the Bob animation.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetValue();
    }

    /// <summary>
    /// Adds some amount to this Currency's value.
    /// </summary>
    /// <param name="amount">The amount to add.</param>
    public void AdjustValue(int amount) => value = Mathf.Clamp(value + amount, MIN_VALUE, MAX_VALUE);

    /// <summary>
    /// Resets this Currency's value to its starting amount.
    /// </summary>
    public void ResetValue() => value = BASE_VALUE;

    /// <summary>
    /// Returns this Currency's current value.
    /// </summary>
    /// <returns>this Currency's current value.</returns>
    public int GetValue() => value;

    #endregion
}
