using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Data structure to store the current number of each
/// ModelType.
/// </summary>
public class ModelCounts
{
    /// <summary>
    /// Current number of each ModelType.
    /// </summary>
    private Dictionary<ModelType, int> data;

    /// <summary>
    /// Creates a new MobCounts object. Initializes the
    /// dictionary.
    /// </summary>
    public ModelCounts() { data = new Dictionary<ModelType, int>(); }

    /// <summary>
    /// Sets the current count of a certain ModelType.
    /// </summary>
    /// <param name="controller">ControllerController reference.</param>
    /// <param name="type">The type to set.</param>
    /// <param name="count">The amount of the ModelType active.</param>
    public void SetCount(ControllerController controller, ModelType type, int count)
    {
        Assert.IsNotNull(controller);
        data[type] = count;
    }

    /// <summary>
    /// Returns the number of active Models of some ModelType.
    /// </summary>
    /// <param name="type">The ModelType to check.</param>
    /// <returns>the number of active Models of some ModelType.</returns>
    public int GetCount(ModelType type)
    {
        return data.TryGetValue(type, out int count) ? count : 0;
    }

    /// <summary>
    /// Sets the number of all active Models to 0.
    /// </summary>
    /// <param name="controller">ControllerController reference.</param>
    public void WipeCounts(ControllerController controller)
    {
        Assert.IsNotNull(controller);
        data.Clear();
    }
}
