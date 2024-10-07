using UnityEngine;

/// <summary>
/// Represents an Acorn projectile.
/// </summary>
public class Acorn : Projectile
{
    #region Fields

    /// <summary>
    /// Number of times this Acorn will split.
    /// </summary>
    private int numSplits;

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an Acorn.
    /// </summary>
    public override ModelType TYPE => ModelType.ACORN;

    /// <summary>
    /// Starting speed of an Acorn.
    /// </summary>
    public override float BASE_SPEED => 9f;

    /// <summary>
    /// Maximum speed of an Acorn.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Acorn.
    /// </summary>
    public override float MIN_SPEED => 0f;

    /// <summary>
    /// Starting damage of an Acorn.
    /// </summary>
    public override int BASE_DAMAGE => 4; //default: 4

    /// <summary>
    /// Maximum damage of an Acorn.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Acorn.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of an Acorn.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;

    /// <summary>
    /// How many seconds an Acorn's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the number of splits this Acorn has left.
    /// </summary>
    /// <param name="numSplits">the number of splits this Acorn has left.</param>
    public void SetNumSplits(int numSplits)
    {
        if(numSplits < 0) numSplits = 0;
        this.numSplits = numSplits;
    }
    
    /// <summary>
    /// Reduces the number of splits this Acorn has left by one.
    /// </summary>
    public void Split() => numSplits = numSplits > 0 ? numSplits - 1 : 0;

    /// <summary>
    /// Returns the number of splits this Acorn has left.
    /// </summary>
    /// <returns>the number of splits this Acorn has left.</returns>
    public int GetNumSplits() => numSplits;

    #endregion
}
