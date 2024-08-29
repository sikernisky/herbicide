using UnityEngine.Assertions;

/// <summary>
/// Represents a custom property from a Tiled JSON file, deserialized.
/// </summary>
[System.Serializable]
public class PropertiesData
{
    #region Fields

    /// <summary>
    /// Name of this custom property.
    /// </summary>
    public string name;

    /// <summary>
    /// Value of this custom property, in string form.
    /// </summary>
    public string value;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the name of this custom property.
    /// </summary>
    /// <returns>the name of this property.</returns>
    public string GetPropertyName()
    {
        Assert.IsNotNull(name, "This custom property's `name` is null.");
        return name;
    }

    /// <summary>
    /// Returns the value of this custom property, as a string.
    /// </summary>
    /// <returns>the value of this custom property as a string.</returns>
    public string GetPropertyValue() => value;

    #endregion
}
