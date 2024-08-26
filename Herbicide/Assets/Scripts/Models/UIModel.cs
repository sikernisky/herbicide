using UnityEngine;

/// <summary>
/// Represents something tangible and physical in the game. All
/// UIModels reside under the Canvas component.
/// </summary>
public abstract class UIModel : MonoBehaviour
{
    #region Fields

    #endregion

    #region Methods

    /// <summary>
    /// Returns this UIModel's ModelType.
    /// </summary>
    /// <returns>this UIModel's ModelType.</returns>
    public abstract ModelType GetModelType();

    #endregion
}
