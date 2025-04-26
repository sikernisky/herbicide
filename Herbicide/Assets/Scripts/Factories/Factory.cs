using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Base class for all Factories. Provides essential methods
/// for object pooling and defines fields that all Factories
/// must implement.
/// </summary>
public abstract class Factory : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// This Factory's (non-instantiated) list of model prefabs.
    /// </summary>
    [SerializeField]
    protected List<GameObject> prefabs;

    /// <summary>
    /// An ObjectPool for each prefab type this Factory pumps out.
    /// </summary>
    private List<ObjectPool> pools;

    /// <summary>
    /// How many prefabs to fill each ObjectPool with at start.
    /// </summary>
    protected virtual int poolStartingCount => 10;

    #endregion

    #region Methods

    /// <summary>
    /// Spawns a pool for each of this Factory's prefab. These objects will
    /// be granted upon request and recycled when their user does
    /// not need them anymore.
    /// </summary>
    protected void SpawnPools()
    {
        Assert.IsTrue(poolStartingCount > 0);

        pools = new List<ObjectPool>();
        foreach (GameObject prefab in prefabs)
        {
            Assert.IsNotNull(prefab);
            ObjectPool pool = new ObjectPool(prefab, poolStartingCount, GetTransform());
            pools.Add(pool);
        }
    }

    /// <summary>
    /// Returns this Factory instance's Transform component.
    /// </summary>
    /// <returns>this Factory instance's Transform component.</returns>
    protected abstract Transform GetTransform();

    /// <summary>
    /// Returns and sets active a prefab of this Factory's type. 
    /// If there are no more prefabs remaning, instantiates a new one 
    /// and adds it to the Object pool before returning. 
    /// </summary>
    /// <param name="modelType">The type of Model to get.</param>
    /// <returns> a prefab of this Factory's type</returns>
    protected GameObject RequestObject(ModelType modelType)
    {
        Assert.IsNotNull(pools);
        ObjectPool objectPool = pools.Find(op => op.GetPoolType() == modelType);
        Assert.IsNotNull(objectPool, name + " has no support for model type " + modelType);
        return objectPool.TakeFromPool();
    }

    /// <summary>
    /// Accepts a prefab of this Factory's type and adds it back
    /// into the object pool. Sets it as inactive.
    /// </summary>
    /// <param name="returnedPrefab">The prefab to accept.</param>
    protected void ReturnObject(GameObject returnedPrefab)
    {
        Assert.IsNotNull(pools);
        Assert.IsNotNull(returnedPrefab);
        Assert.IsNotNull(prefabs);

        Model model = returnedPrefab.GetComponent<Model>();
        UIModel uIModel = returnedPrefab.GetComponent<UIModel>();
        ModelType returnedType = model != null ? model.TYPE : uIModel.GetModelType();
        ObjectPool objectPool = pools.Find(op => op.GetPoolType() == returnedType);
        Assert.IsNotNull(objectPool, "No pool found for model type " + returnedType);

        if(model != null) model.ResetModel();
        objectPool.ReturnToPool(returnedPrefab);
    }

    #endregion
}
