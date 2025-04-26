/// <summary>
/// Represents a Speed Tree Seed currency.
/// </summary>
public class SpeedTreeSeed : Currency
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Starting value of a BasicTreeSeed.
    /// </summary>
    public override int Value => 0;

    /// <summary>
    /// Maximum value of a BasicTreeSeed.
    /// </summary>
    public override int MAX_VALUE => int.MaxValue;

    /// <summary>
    /// Mimimum value of a BasicTreeSeed.
    /// </summary>
    public override int MIN_VALUE => 0;

    /// <summary>
    /// Type of a BasicTreeSeed.
    /// </summary>
    public override ModelType TYPE => ModelType.SPEED_TREE_SEED;

    #endregion
}
