using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Projectiles. 
/// </summary>
public class ProjectileFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the ProjectileFactory singleton.
    /// </summary>
    private static ProjectileFactory instance;

    /// <summary>
    /// The animation set for an Acorn.
    /// </summary>
    [SerializeField]
    private ProjectileAnimationSet acornAnimationSet;

    /// <summary>
    /// The animation set for a Quill.
    /// </summary>
    [SerializeField]
    private QuillAnimationSet quillAnimationSet;

    /// <summary>
    /// The animation set for a Blackberry.
    /// </summary>
    [SerializeField]
    private ProjectileAnimationSet blackberryAnimationSet;

    /// <summary>
    /// The animation set for a Raspberry.
    /// </summary>
    [SerializeField]
    private ProjectileAnimationSet raspberryAnimationSet;

    /// <summary>
    /// The animation set for a Salmonberry.
    /// </summary>
    [SerializeField]
    private ProjectileAnimationSet salmonberryAnimationSet;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the ProjectileFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        ProjectileFactory[] projectileFactories = FindObjectsOfType<ProjectileFactory>();
        Assert.IsNotNull(projectileFactories, "Array of ProjectileFactories is null.");
        Assert.AreEqual(1, projectileFactories.Length);
        instance = projectileFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a prefab for a given Projectile type from the object pool.
    /// </summary>
    /// <param name="modelType">The ModelType of the Projectile to get</param>
    /// <returns>a prefab for a given Projectile type from the object pool.</returns>
    public static GameObject GetProjectilePrefab(ModelType modelType) => instance.RequestObject(modelType);

    /// <summary>
    /// Accepts a Projectile prefab and puts it back in the object pool.
    /// </summary>
    /// <param name="prefab">the prefab to accept.</param>
    public static void ReturnProjectilePrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the animation track that represents this Projectile when placing.
    /// </summary>
    /// <param name="m">The ModelType of the Projectile to get</param>
    /// <returns>the animation track that represents this Projectile when placing.</returns>
    public static Sprite[] GetPlacementTrack(Model m)
    {
        switch (m.TYPE)
        {
            case ModelType.ACORN:
                return instance.acornAnimationSet.GetPlacementAnimation();
            case ModelType.QUILL:
                Quill quill = m as Quill;
                Assert.IsNotNull(quill, "Quill is null.");
                if(quill.IsDoubleQuill()) return instance.quillAnimationSet.GetDoubleQuillPlacementAnimation();
                else return instance.quillAnimationSet.GetPlacementAnimation();
            case ModelType.BLACKBERRY:
                return instance.blackberryAnimationSet.GetPlacementAnimation();
            case ModelType.RASPBERRY:
                return instance.raspberryAnimationSet.GetPlacementAnimation();
            case ModelType.SALMONBERRY:
                return instance.salmonberryAnimationSet.GetPlacementAnimation();
            default:
                throw new System.Exception("Invalid ModelType");
        }
    }

    /// <summary>
    /// Returns the animation track that represents this Projectile when mid air.
    /// </summary>
    /// <param name="m">The ModelType of the Projectile to get</param>
    /// <returns>the animation track that represents this Projectile when mid air.</returns>
    public static Sprite[] GetMidAirAnimationTrack(Model m)
    {
        switch (m.TYPE)
        {
            case ModelType.ACORN:
                return instance.acornAnimationSet.GetMidAirAnimation();
            case ModelType.QUILL:
                Quill quill = m as Quill;
                Assert.IsNotNull(quill, "Quill is null.");
                if(quill.IsDoubleQuill()) return instance.quillAnimationSet.GetDoubleQuillMidAirAnimation();
                else return instance.quillAnimationSet.GetMidAirAnimation();
            case ModelType.BLACKBERRY:
                return instance.blackberryAnimationSet.GetMidAirAnimation();
            case ModelType.RASPBERRY:
                return instance.raspberryAnimationSet.GetMidAirAnimation();
            case ModelType.SALMONBERRY:
                return instance.salmonberryAnimationSet.GetMidAirAnimation();
            default:
                throw new System.Exception("Invalid ModelType");
        }
    }

    /// <summary>
    /// Returns the ProjectileFactory's transform component.
    /// </summary>
    /// <returns>the ProjectileFactory's transform component.</returns>
    protected override Transform GetTransform() => instance.transform;

    #endregion
}
