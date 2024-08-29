using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DataStructure to store frequently requested A* Pathfinding calls.
/// </summary>
public static class PathfindingCache
{
    #region CacheEntry

    /// <summary>
    /// Represents a cache entry.
    /// </summary>
    private class CacheEntry
    {
        /// <summary>
        /// true if the goal is reachable from the start position;
        /// otherwise, false. 
        /// </summary>
        public bool IsReachable { get; set; }

        /// <summary>
        /// The position of the next tile in the path.
        /// </summary>
        public Vector2Int NextTilePosition { get; set; }
    }

    #endregion

    #region Fields

    /// <summary>
    /// The cache for pathfinding results.
    /// </summary>
    private static Dictionary<(Vector3 start, Vector3 goal), CacheEntry> cache = new Dictionary<(Vector3, Vector3), CacheEntry>();

    #endregion

    #region Methods

    /// <summary>
    /// Updates the cache with the pathfinding results.
    /// </summary>
    /// <param name="startPos">The starting position.</param>
    /// <param name="goalPos">The goal position.</param>
    /// <param name="reachable">True if the goal is reachable from the start position.</param>
    /// <param name="nextPos">The position of the next tile in the path.</param>
    public static void UpdateCache(Vector3 startPos, Vector3 goalPos, bool reachable, Vector2Int nextPos)
    {
        var key = (start: startPos, goal: goalPos);
        cache[key] = new CacheEntry
        {
            IsReachable = reachable,
            NextTilePosition = nextPos
        };
    }

    /// <summary>
    /// Checks if the cache has valid data for the given start and goal positions.
    /// </summary>
    /// <param name="startPos">The starting position.</param>
    /// <param name="goalPos">The goal position.</param>
    /// <returns>True if the cache has valid data for the given positions; otherwise, false.</returns>
    public static bool IsCacheValid(Vector3 startPos, Vector3 goalPos)
    {
        var key = (start: startPos, goal: goalPos);
        return cache.ContainsKey(key);
    }

    /// <summary>
    /// Gets the reachability status from the cache.
    /// </summary>
    /// <param name="startPos">The starting position.</param>
    /// <param name="goalPos">The goal position.</param>
    /// <returns>True if the goal is reachable from the start position; otherwise, false.</returns>
    public static bool GetIsReachable(Vector3 startPos, Vector3 goalPos)
    {
        var key = (start: startPos, goal: goalPos);
        return cache[key].IsReachable;
    }

    /// <summary>
    /// Gets the next tile position from the cache.
    /// </summary>
    /// <param name="startPos">The starting position.</param>
    /// <param name="goalPos">The goal position.</param>
    /// <returns>The position of the next tile in the path.</returns>
    public static Vector2Int GetNextTilePosition(Vector3 startPos, Vector3 goalPos)
    {
        var key = (start: startPos, goal: goalPos);
        return cache[key].NextTilePosition;
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public static void ClearCache() => cache.Clear();

    #endregion
}
