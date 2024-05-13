using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Data structure to store a pool of GameObjects with Models of the same
/// ModelType. Backed by a stack.
/// </summary>
public class ObjectPool
{
    /// <summary>
    /// The type of Model that occupies this ObjectPool.
    /// </summary>
    private ModelType poolType;

    /// <summary>
    /// The the prefabs with Model components that occupy this ObjectPool.
    /// </summary>
    private GameObject prefab;

    /// <summary>
    /// The Transform component of the GameObject storing this ObjectPool.
    /// </summary>
    private Transform transform;

    /// <summary>
    /// The object pool.
    /// </summary>
    private Stack<GameObject> pool;

    /// <summary>
    /// The InstanceIds of GameObject prefabs that are part of this pool.
    /// </summary>
    private HashSet<int> objectIds;


    /// <summary>
    /// Creates a new ObjectPool, spawning a finite number of the GameObject prefab.
    /// </summary>
    /// <param name="prefab">The prefab that occupies this pool. </param>
    /// <param name="startingSize">How many prefabs to instantiate inside the pool
    /// upon initialization.</param>
    /// <param name="transform">The transform of the script that is storing this pool.</param>
    public ObjectPool(GameObject prefab, int startingSize, Transform transform)
    {
        Assert.IsNotNull(prefab);
        Assert.IsTrue(startingSize > 0);
        Assert.IsNotNull(transform);

        Model model = prefab.GetComponent<Model>();
        UIModel uiModel = prefab.GetComponent<UIModel>();
        Assert.IsFalse(model == null && uiModel == null);

        poolType = model == null ? uiModel.GetModelType() : model.TYPE;
        this.prefab = prefab;
        this.transform = transform;

        pool = new Stack<GameObject>();
        objectIds = new HashSet<int>();
        for (int i = 0; i < startingSize; i++) { AddNewPrefabToPool(); }
        Assert.IsTrue(pool.Count == startingSize);
    }

    /// <summary>
    /// Instantiates this ObjectPool's prefab GameObject and adds it to the pool.
    /// This method should only be called upon the pool's initialization and 
    /// when there are no more objects left but one is needed.
    /// </summary>
    private void AddNewPrefabToPool()
    {
        GameObject copy = GameObject.Instantiate(prefab);
        copy.transform.SetParent(transform);
        copy.SetActive(false);
        pool.Push(copy);
        objectIds.Add(copy.GetInstanceID());
    }

    /// <summary>
    /// Returns a GameObject from the object pool. If the pool is empty,
    /// creates and returns new GameObject prefab. 
    /// </summary>
    /// <returns>a fresh GameObject prefab.</returns>
    public GameObject TakeFromPool()
    {
        if (pool.Count < 1) AddNewPrefabToPool();
        GameObject selectedPrefab = pool.Pop();
        selectedPrefab.SetActive(true);
        return selectedPrefab;
    }

    /// <summary>
    /// Accepts a GameObject prefab that was previously borrowed from the
    /// object pool. 
    /// </summary>
    /// <param name="prefab">The GameObject prefab to return. </param> 
    public void ReturnToPool(GameObject prefab)
    {
        Assert.IsNotNull(prefab);
        prefab.transform.SetParent(transform);
        prefab.SetActive(false);
        pool.Push(prefab);
    }

    /// <summary>
    /// Returns the ModelType of GameObject prefabs that occupy this ObjectPool.
    /// </summary>
    /// <returns>the ModelType of GameObject prefabs that occupy this ObjectPool.</returns>
    public ModelType GetPoolType() { return poolType; }
}
