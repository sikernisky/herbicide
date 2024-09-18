/// <summary>
/// Represents a Basic Tree Seed currency.
/// </summary>
public class BasicTreeSeed : Currency
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Starting value of a BasicTreeSeed.
    /// </summary>
    public override int BASE_VALUE => 0;

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
    public override ModelType TYPE => ModelType.BASIC_TREE_SEED;

    #endregion
}
