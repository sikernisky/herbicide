using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something tangible and physical in the game. All
/// UIModels reside under the Canvas component.
/// </summary>
public abstract class UIModel : MonoBehaviour
{
    /// <summary>
    /// ModelType of this UIModel.
    /// </summary>
    [SerializeField]
    protected ModelType modelType;

    /// <summary>
    /// Returns this UIModel's ModelType.
    /// </summary>
    /// <returns>this UIModel's ModelType.</returns>
    public ModelType GetModelType() { return modelType; } //TODO -- Is this good idk
}
