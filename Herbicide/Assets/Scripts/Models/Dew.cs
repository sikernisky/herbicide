using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Dew currency.
/// </summary>
public class Dew : Currency
{
    /// <summary>
    /// Starting value of a Dew.
    /// </summary>
    public override int BASE_VALUE => 1;

    /// <summary>
    /// Maximum value of a Dew.
    /// </summary>
    public override int MAX_VALUE => int.MaxValue;

    /// <summary>
    /// Mimimum value of a Dew.
    /// </summary>
    public override int MIN_VALUE => int.MinValue;

    /// <summary>
    /// Type of a Dew.
    /// </summary>
    public override ModelType TYPE => ModelType.DEW;

    /// <summary>
    /// Name of a Dew.
    /// </summary>
    public override string NAME => "Dew";
}
