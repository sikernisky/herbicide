using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Emanations.
/// </summary>
public class EmanationFactory : MonoBehaviour
{
    #region Fields

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
    /// Animation track for the Quill Pierce emanation.
    /// </summary>
    [SerializeField]
    private Sprite[] quillPierceTrack;

    /// <summary>
    /// Animation track for the Blackberry Explosion emanation.
    /// </summary>
    [SerializeField]
    private Sprite[] blackberryExplosionTrack;

    #endregion

    #region Methods

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
            case EmanationController.EmanationType.QUILL_PIERCE:
                return instance.quillPierceTrack;
            case EmanationController.EmanationType.BLACKBERRY_EXPLOSION:
                return instance.blackberryExplosionTrack;
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
                return 0.075f;
            case EmanationController.EmanationType.QUILL_PIERCE:
                return 0.200f;
            case EmanationController.EmanationType.BLACKBERRY_EXPLOSION:
                return 0.100f;
            default:
                break;
        }

        throw new System.NotSupportedException(emanationType + " not supported.");
    }

    #endregion
}
