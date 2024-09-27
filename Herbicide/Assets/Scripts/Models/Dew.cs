using UnityEngine;
/// <summary>
/// Represents a Dew currency.
/// </summary>
public class Dew : Currency
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Starting value of a Dew.
    /// </summary>
    public override int BASE_VALUE => 25;

    /// <summary>
    /// Maximum value of a Dew.
    /// </summary>
    public override int MAX_VALUE => int.MaxValue;

    /// <summary>
    /// Mimimum value of a Dew.
    /// </summary>
    public override int MIN_VALUE => 0;

    /// <summary>
    /// Type of a Dew.
    /// </summary>
    public override ModelType TYPE => ModelType.DEW;

    #endregion
}
