using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Emanations.
/// </summary>
public class EmanationFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the EmanationFactory singleton.
    /// </summary>
    private static EmanationFactory instance;

    /// <summary>
    /// Animation track for the Bear Chomp emanation.
    /// </summary>
    [SerializeField]
    private Sprite[] bearChompTrack;


    /// <summary>
    /// Finds and sets the EmanationFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        EmanationFactory[] emanationFactories = FindObjectsOfType<EmanationFactory>();
        Assert.IsNotNull(emanationFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, emanationFactories.Length);
        instance = emanationFactories[0];
    }

    /// <summary>
    /// Returns the animation track for a given Emanation.
    /// </summary>
    /// <param name="emanationType">The Emanation track to get.</param>
    /// <returns>the animation track for a given Emanation.</returns>
    public static Sprite[] GetEmanationTrack(EmanationController.EmanationType emanationType)
    {
        switch (emanationType)
        {
            case EmanationController.EmanationType.BEAR_CHOMP:
                return instance.bearChompTrack;
            default:
                break;
        }

        throw new System.NotSupportedException(emanationType + " not supported.");
    }

    /// <summary>
    /// Returns the time it takes to complete one cycle of an animation track
    /// for a given Emanation.
    /// </summary>
    /// <param name="emanationType">The Emanation time to get.</param>
    /// <returns> the time it takes to complete one cycle of an animation track
    /// for a given Emanation.</returns>
    public static float GetEmanationCycleTime(EmanationController.EmanationType emanationType)
    {
        switch (emanationType)
        {
            case EmanationController.EmanationType.BEAR_CHOMP:
                return .075f;
            default:
                break;
        }

        throw new System.NotSupportedException(emanationType + " not supported.");
    }
}
