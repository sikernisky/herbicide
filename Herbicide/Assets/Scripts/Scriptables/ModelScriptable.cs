using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores data for Models.
/// </summary>
public class ModelScriptable : ScriptableObject
{
    /// <summary>
    /// This Model's prefab.
    /// </summary>
    [SerializeField]
    private GameObject modelPrefab;

    /// <summary>
    /// Type of the Model.
    /// </summary>
    [SerializeField]
    private ModelType modelType;

    /// <summary>
    /// Boat track when this Model is facing North.
    /// </summary>
    [SerializeField]
    private Sprite[] boatAnimationNorth;

    /// <summary>
    /// Boat track when this Model is facing East.
    /// </summary>
    [SerializeField]
    private Sprite[] boatAnimationEast;

    /// <summary>
    /// Boat track when this Model is facing South.
    /// </summary>
    [SerializeField]
    private Sprite[] boatAnimationSouth;

    /// <summary>
    /// Boat track when this Model is facing West.
    /// </summary>
    [SerializeField]
    private Sprite[] boatAnimationWest;


    /// <summary>
    /// Returns the Model's type.
    /// </summary>
    /// <returns>the Model's type.</returns>
    public ModelType GetModelType() { return modelType; }

    /// <summary>
    /// Returns this Model's prefab.
    /// </summary>
    /// <returns>this Model's prefab.</returns>
    public GameObject GetModelPrefab() { return modelPrefab; }

    /// <summary>
    /// Returns the Model's boat animation for a given direction.
    /// </summary>
    /// <param name="direction">The direction of the animation to get.</param>
    /// <returns>the Model's boat animation.</returns>
    public Sprite[] GetBoatTrack(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return boatAnimationNorth;
            case Direction.EAST:
                return boatAnimationEast;
            case Direction.SOUTH:
                return boatAnimationSouth;
            case Direction.WEST:
                return boatAnimationWest;
        }

        throw new System.Exception();
    }
}
