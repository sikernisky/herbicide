using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Defenders.
/// </summary>
public class DefenderFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the DefenderFactory singleton.
    /// </summary>
    private static DefenderFactory instance;

    /// <summary>
    /// Animation set for a Squirrel.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet squirrelAnimationSet;

    /// <summary>
    /// Animation set for a Bear.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet bearAnimationSet;

    /// <summary>
    /// Animation set for a Porcupine.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet porcupineAnimationSet;

    /// <summary>
    /// Animation set for a Raccoon.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet raccoonAnimationSet;

    /// <summary>
    /// Animation set for a Owl.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet owlAnimationSet;

    /// <summary>
    /// Animation set for a Bunny.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet bunnyAnimationSet;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the DefenderFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        DefenderFactory[] defenderFactories = FindObjectsOfType<DefenderFactory>();
        Assert.IsNotNull(defenderFactories, "Array of DefenderFactories is null.");
        Assert.AreEqual(1, defenderFactories.Length);
        instance = defenderFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a prefab for a given Defender type from the object pool.
    /// </summary>
    /// <param name="modelType">The ModelType of the Defender to get</param>
    /// <returns>a prefab for a given Defender type from the object pool.</returns>
    public static GameObject GetDefenderPrefab(ModelType modelType) => instance.RequestObject(modelType);

    /// <summary>
    /// Accepts a Defender prefab and puts it back in the object pool.
    /// </summary>
    /// <param name="prefab">the prefab to accept.</param>
    public static void ReturnDefenderPrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the animation track that represents the Defender during its main action.
    /// </summary>
    /// <returns>the animation track that represents the Defender during its main action.</returns>
    /// <param name="m">The ModelType of the Defender.</param>
    /// <param name="d">The Defender's Direction.</param>
    /// <param name="tier">The Defender's tier.</param>
    public static Sprite[] GetMainActionTrack(ModelType m, Direction d, int tier)
    {
        Assert.IsTrue(tier >= Defender.MIN_TIER && tier <= Defender.MAX_TIER, "Invalid tier.");

        switch (m)
        {
            case ModelType.SQUIRREL:
                return instance.squirrelAnimationSet.GetMainActionAnimation(d, tier);
            case ModelType.BEAR:
                return instance.bearAnimationSet.GetMainActionAnimation(d, tier);
            case ModelType.PORCUPINE:
                return instance.porcupineAnimationSet.GetMainActionAnimation(d, tier);
            case ModelType.RACCOON:
                return instance.raccoonAnimationSet.GetMainActionAnimation(d, tier);
            case ModelType.OWL:
                return instance.owlAnimationSet.GetMainActionAnimation(d, tier);
            case ModelType.BUNNY:
                return instance.bunnyAnimationSet.GetMainActionAnimation(d, tier);
            default:
                throw new System.Exception("Invalid ModelType");
        }
    }

    /// <summary>
    /// Returns the animation track that represents the Defender when Idle.
    /// </summary>
    /// <returns>the animation track that represents the Defender when Idle.</returns>
    /// <param name="m">The ModelType of the Defender.</param>
    /// <param name="d">The Defender's Direction.</param>
    /// <param name="tier">The Defender's tier.</param>
    public static Sprite[] GetIdleTrack(ModelType m, Direction d, int tier)
    {
        Assert.IsTrue(tier >= Defender.MIN_TIER && tier <= Defender.MAX_TIER, "Invalid tier.");

        switch (m)
        {
            case ModelType.SQUIRREL:
                return instance.squirrelAnimationSet.GetIdleAnimation(d, tier);
            case ModelType.BEAR:
                return instance.bearAnimationSet.GetIdleAnimation(d, tier);
            case ModelType.PORCUPINE:
                return instance.porcupineAnimationSet.GetIdleAnimation(d, tier);
            case ModelType.RACCOON:
                return instance.raccoonAnimationSet.GetIdleAnimation(d, tier);
            case ModelType.OWL:
                return instance.owlAnimationSet.GetIdleAnimation(d, tier);
            case ModelType.BUNNY:
                return instance.bunnyAnimationSet.GetIdleAnimation(d, tier);
            default:
                throw new System.Exception("Invalid ModelType");
        }
    }

    /// <summary>
    /// Returns the animation track that represents this Defender when placing.
    /// </summary>
    /// <param name="m">The ModelType of the Defender to get</param>
    /// <param name="tier">The tier of the Defender to get</param>
    /// <returns>the animation track that represents this Defender when placing.</returns>
    public static Sprite[] GetPlacementTrack(ModelType m, int tier)
    {
        switch (m)
        {
            case ModelType.SQUIRREL:
                return instance.squirrelAnimationSet.GetPlacementAnimation(tier);
            case ModelType.BEAR:
                return instance.bearAnimationSet.GetPlacementAnimation(tier);
            case ModelType.PORCUPINE:
                return instance.porcupineAnimationSet.GetPlacementAnimation(tier);
            case ModelType.RACCOON:
                return instance.raccoonAnimationSet.GetPlacementAnimation(tier);
            case ModelType.OWL:
                return instance.owlAnimationSet.GetPlacementAnimation(tier);
            case ModelType.BUNNY:
                return instance.bunnyAnimationSet.GetPlacementAnimation(tier);
            default:
                throw new System.Exception("Invalid ModelType");
        }
    }

    /// <summary>
    /// Returns the DefenderFactory's transform component.
    /// </summary>
    /// <returns>the DefenderFactory's transform component.</returns>
    protected override Transform GetTransform() => instance.transform;

    #endregion
}
