using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents deserialized custom properties from a Tiled
/// JSON file.
/// </summary>
[System.Serializable]
public class PropertiesData
{
    /// <summary>
    /// Name of this custom property.
    /// </summary>
    public string name;

    /// <summary>
    /// Value of this custom property, in string form.
    /// </summary>
    public string value;

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
    public string GetPropertyValue()
    {
        return value;
    }
}
