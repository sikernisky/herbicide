using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

/// <summary>
/// Manages assets for Models.
/// </summary>
public class ModelFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the ModelFactory singleton.
    /// </summary>
    private static ModelFactory instance;

    /// <summary>
    /// All ScriptableObjects containing data about
    /// different Models.
    /// </summary>
    [SerializeField]
    private List<ModelScriptable> modelScriptables;


    /// <summary>
    /// Finds and sets the ModelFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        ModelFactory[] modelFactories = FindObjectsOfType<ModelFactory>();
        ModelFactory[] specificFactories = modelFactories.Where(factory => factory.GetType() == typeof(ModelFactory)).ToArray();
        Assert.IsNotNull(modelFactories, "Array of DefenderFactories is null.");
        Assert.AreEqual(1, modelFactories.Length);
        instance = modelFactories[0];
    }

    /// <summary>
    /// Returns the Sprite track that represents this Model on a ShopBoat.
    /// </summary>
    /// <returns>the Sprite component that represents a Defender in the inventory</returns>
    /// <param name="modelType">Which type of Model to get the Boat track of.</param>
    /// <param name="direction">The direction of the Model.</param>
    public static Sprite[] GetBoatTrack(ModelType modelType, Direction direction)
    {
        ModelScriptable data = instance.modelScriptables.Find(
            x => x.GetModelType() == modelType);

        if (data != null) return data.GetBoatTrack(direction);

        return null;
    }

    /// <summary>
    /// Returns the prefab Model for a given type.
    /// </summary>
    /// <returns>the prefab Model for a given type.</returns>
    /// <param name="modelType">Type of model.</param>
    public static GameObject GetModelPrefab(ModelType modelType)
    {
        ModelScriptable data = instance.modelScriptables.Find(
            x => x.GetModelType() == modelType);

        if (data != null) return data.GetModelPrefab();

        return null;
    }


    /// <summary>
    /// Returns the prefab Model for a given type.
    /// </summary>
    /// <returns>the prefab Model for a given type.</returns>
    /// <param name="modelType">Type of model.</param>
    public static GameObject GetModelPrefab(string modelType)
    {
        ModelScriptable data = instance.modelScriptables.Find(
            x => x.GetModelType().ToString().ToLower() == modelType.ToLower());

        if (data != null) return data.GetModelPrefab();

        throw new System.NotSupportedException(modelType + " not supported.");
    }
}
