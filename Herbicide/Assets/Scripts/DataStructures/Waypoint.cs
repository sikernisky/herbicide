using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a milestone within a path that an Enemy
/// can follow.
/// </summary>
public class Waypoint
{
    #region Fields

    /// <summary>
    /// The number of paths in the level.
    /// </summary>
    private static int numPaths;

    /// <summary>
    /// Returns the world position of the waypoint.
    /// </summary>
    public Vector3 WorldPosition { get; private set; }

    /// <summary>
    /// Returns the coordinates of the waypoint in the grid.
    /// </summary>
    public Vector2Int Coords { get; private set; }

    /// <summary>
    /// Holds the ids of the paths that the waypoint is part of.
    /// </summary>
    public int[] PathIds { get; private set; }

    /// <summary>
    /// Holds the order of the waypoint in the paths it is part of. The
    /// index of the path id in PathIds corresponds to the index of the
    /// order in this array.
    /// </summary>
    public int[] OrderInPaths { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new Waypoint instance.
    /// </summary>
    /// <param name="worldPosition">The world position of the waypoint.</param>
    /// <param name="coords">The coordinates of the waypoint in the grid.</param>
    /// <param name="unparsedPathIds">The unparsed string representing the ids of the paths that the waypoint is part of.</param>
    /// <param name="unparsedOrderInPaths">The unparsed string representing order of the waypoint in the paths it is part of.</param>
    public Waypoint(Vector3 worldPosition, Vector2Int coords, string unparsedPathIds, string unparsedOrderInPaths)
    {
        PathIds = ParseCommaSeparatedIntegers(unparsedPathIds);
        OrderInPaths = ParseCommaSeparatedIntegers(unparsedOrderInPaths);
        Assert.AreEqual(PathIds.Length, OrderInPaths.Length, "The number of PathIds does not match the number of OrderInPaths.");
        WorldPosition = worldPosition;
        Coords = coords;
        CalculateNumPaths(PathIds);
    }

    /// <summary>
    /// Parses a string of integers separated by commas into an array of integers.
    /// </summary>
    /// <param name="input">The string to parse (e.g., "1,2,3").</param>
    /// <returns>An array of integers parsed from the input string.</returns>
    private int[] ParseCommaSeparatedIntegers(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Input string is null, empty, or whitespace.");
        try { return input.Split(',').Select(item => int.Parse(item.Trim())).ToArray(); }
        catch (FormatException ex) { throw new ArgumentException($"Input contains invalid integer values: {input}", ex); }
        catch (OverflowException ex) { throw new ArgumentException($"Input contains out-of-range integer values: {input}", ex); }
    }

    /// <summary>
    /// Finds and sets the number of paths in the level.
    /// </summary>
    /// <param name="pathIds">The parsed integer array of path ids.</param>
    private void CalculateNumPaths(int[] pathIds)
    {
        if (pathIds == null) throw new ArgumentNullException(nameof(pathIds));
        if (pathIds.Length == 0) throw new ArgumentException("PathIds array is empty.");
        numPaths = Math.Max(numPaths, pathIds.Length);
    }

    /// <summary>
    /// Returns the number of paths in the level.
    /// </summary>
    /// <returns>The number of paths in the level.</returns>
    public static int GetNumPaths() => numPaths;

    #endregion
}
