using System.Collections.Generic;

/// <summary>
/// Stores cached pathfinding data to reduce the number of pathfinding calls.
/// </summary>
public static class PathfindingCache
{
    /// <summary>
    /// Cache for the next position of a pathfinding call.
    /// </summary>
    private static Dictionary<(int, int, int, int), (int, int)> nextPositionCache = new Dictionary<(int, int, int, int), (int, int)>();
    
    /// <summary>
    /// Cache for the reachability of a pathfinding call.
    /// </summary>
    private static Dictionary<(int, int, int, int), bool> reachabilityCache = new Dictionary<(int, int, int, int), bool>();

    /// <summary>
    /// Caches the next position of a pathfinding call.
    /// </summary>
    /// <param name="goalX">The x-coordinate of the goal.</param>
    /// <param name="goalY">The y-coordinate of the goal.</param>
    /// <param name="nextX">The x-coordinate of the next position.</param>
    /// <param name="nextY">The y-coordinate of the next position.</param>
    /// <param name="startX">The x-coordinate of the start.</param>
    /// <param name="startY">The y-coordinate of the start.</param>
    public static void CacheNextPosition(int startX, int startY, int goalX, int goalY, int nextX, int nextY)
    {
        nextPositionCache[(startX, startY, goalX, goalY)] = (nextX, nextY);
    }

    /// <summary>
    /// Returns true if the next position of a pathfinding call is cached.
    /// </summary>
    /// <param name="startX">The x-coordinate of the start.</param>
    /// <param name="startY">The y-coordinate of the start.</param>
    /// <param name="goalX">The x-coordinate of the goal.</param>
    /// <param name="goalY">The y-coordinate of the goal.</param>
    /// <returns>true if the next position of a pathfinding call is cached;
    /// otherwise, false. </returns>
    public static bool HasCachedNextPosition(int startX, int startY, int goalX, int goalY)
    {
        return nextPositionCache.ContainsKey((startX, startY, goalX, goalY));
    }

    /// <summary>
    /// Returns the cached next position of a pathfinding call.
    /// </summary>
    /// <param name="startX">The x-coordinate of the start.</param>
    /// <param name="startY">The y-coordinate of the start.</param>
    /// <param name="goalX">The x-coordinate of the goal.</param>
    /// <param name="goalY">The y-coordinate of the goal.</param>
    /// <returns>the cached next position of a pathfinding call.</returns>
    public static (int, int) GetCachedNextPosition(int startX, int startY, int goalX, int goalY)
    {
        return nextPositionCache[(startX, startY, goalX, goalY)];
    }

    /// <summary>
    /// Caches the reachability of a pathfinding call.
    /// </summary>
    /// <param name="startX">The x-coordinate of the start.</param>
    /// <param name="startY">The y-coordinate of the start.</param>
    /// <param name="goalX">The x-coordinate of the goal.</param>
    /// <param name="goalY">The y-coordinate of the goal.</param>
    /// <param name="isReachable">true if the goal is reachable from the start;
    /// otherwise, false.</param>   
    public static void CacheReachability(int startX, int startY, int goalX, int goalY, bool isReachable)
    {
        reachabilityCache[(startX, startY, goalX, goalY)] = isReachable;
    }

    /// <summary>
    /// Returns true if the reachability of a pathfinding call is cached.
    /// </summary>
    /// <param name="startX">The x-coordinate of the start.</param>
    /// <param name="startY">The y-coordinate of the start.</param>
    /// <param name="goalX">The x-coordinate of the goal.</param>
    /// <param name="goalY">The y-coordinate of the goal.</param>
    /// <returns>true if the reachability of a pathfinding call is cached;
    /// otherwise, false. </returns>    
    public static bool HasCachedReachability(int startX, int startY, int goalX, int goalY)
    {
        return reachabilityCache.ContainsKey((startX, startY, goalX, goalY));
    }

    /// <summary>
    /// Returns the cached reachability of a pathfinding call.
    /// </summary>
    /// <param name="startX">The x-coordinate of the start.</param>
    /// <param name="startY">The y-coordinate of the start.</param>
    /// <param name="goalX">The x-coordinate of the goal.</param>
    /// <param name="goalY">The y-coordinate of the goal.</param>
    /// <returns>the cached reachability of a pathfinding call.</returns>
    public static bool GetCachedReachability(int startX, int startY, int goalX, int goalY)
    {
        return reachabilityCache[(startX, startY, goalX, goalY)];
    }
}
