using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Cache for expensive A* Pathfinding calls.
/// </summary>
public class PathfindingCache
{
    /// <summary>
    /// Class to represent our Cache metadata.
    /// </summary>
    private class CacheData
    {
        /// <summary>
        /// True if the target Model was reachable that last time we ran our
        /// pathfinding algorithm.
        /// </summary>
        public bool IsReachable { get; set; }

        /// <summary>
        /// The position of the target Model the last time we ran our
        /// pathfinding algorithm.
        /// </summary>
        public Vector3 LastKnownTargetPosition { get; set; }
    }

    /// <summary>
    /// Cache reference for Models.
    /// </summary>
    private Dictionary<Model, CacheData> cache;

    /// <summary>
    /// The Model to pathfind from.
    /// </summary>
    private Model model;


    /// <summary>
    /// Creates a new PathfindingCache.
    /// </summary>
    /// <param name="model">The Model to pathfind from.</param>
    public PathfindingCache(Model model)
    {
        Assert.IsNotNull(model);
        cache = new Dictionary<Model, CacheData>();
        this.model = model;
    }

    /// <summary>
    /// Returns true if a Model is reachable from this Model.
    /// </summary>
    /// <param name="model">The Model to check. </param>
    /// <returns>true if a Model is reachable from this Mob; otherwise,
    /// false. </returns>
    public bool IsReachable(Model model)
    {
        Assert.IsNotNull(model, "Model is null.");

        if (cache.TryGetValue(model, out CacheData data))
        {
            if (model.GetPosition() == data.LastKnownTargetPosition)
                return data.IsReachable;
        }


        bool isReachable = Reachable(model);
        cache[model] = new CacheData
        {
            IsReachable = isReachable,
            LastKnownTargetPosition = model.GetPosition(),
        };
        return isReachable;
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void ClearCache()
    {
        cache.Clear();
    }

    /// <summary>
    /// Returns the position of the next Tile in the path towards
    /// some Model.
    /// </summary>
    /// <param name="model">The Model to pathfind towards.</param>
    /// <returns>the position of the next Tile in the path towards
    /// some Model.</returns>
    private bool Reachable(Model model)
    {
        Assert.IsNotNull(model, "Model is null.");

        return TileGrid.CanReach(this, this.model.GetPosition(),
            model.GetPosition());
    }
}
