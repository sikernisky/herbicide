using UnityEngine;
/// <summary>
/// Represents a Dew currency.
/// </summary>
public class Dew : Currency
{
    #region Fields

    /// <summary>
    /// Value of a Dew.
    /// </summary>
    public override int Value => ModelStatConstants.CollectableDewValue;

    /// <summary>
    /// Type of a Dew.
    /// </summary>
    public override ModelType TYPE => ModelType.DEW;

    #endregion
}
