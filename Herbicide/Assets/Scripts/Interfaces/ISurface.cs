using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a GameObject that can host an IPlaceable. In other
/// words, an IPlaceable can be placed on an ISurface.
/// </summary>
public interface ISurface
{
    /// <summary>
    /// Returns true if an IPlaceable can be placed on this ISurface.
    /// If it can, places the IPlaceable.
    /// </summary>
    /// <param name="candidate">The IPlaceable to place.</param>
    /// <param name="neighbors">This ISurface's neighbors.</param>
    /// <returns>true if an IPlaceable can be placed on this ISurface;
    /// otherwise, false.</returns>
    bool Place(IPlaceable candidate, ISurface[] neighbors);

    /// <summary>
    /// Returns true if an IPlaceable can be placed on this ISurface.
    /// </summary>
    /// <param name="candidate">The IPlaceable to place.</param>
    /// <param name="neighbors">This ISurface's neighbors.</param>
    /// <returns>true if an IPlaceable can be placed on this ISurface;
    /// otherwise, false.</returns>
    bool CanPlace(IPlaceable candidate, ISurface[] neighbors);

    /// <summary>
    /// Returns true if there is an IPlaceable on this ISurface that can
    /// be removed. If so, removes the IPlaceable.
    /// </summary>
    /// <param name="neighbors">This ISurface's neighbors.</param>
    /// <returns>true if an IPlaceable can be removed from this ISurface;
    /// otherwise, false.</returns>
    bool Remove(ISurface[] neighbors);

    /// <summary>
    /// Returns true if there is an IPlaceable on this ISurface that can
    /// be removed.
    /// </summary>
    /// <param name="neighbors">This ISurface's neighbors.</param>
    /// <returns>true if an IPlaceable can be removed from this ISurface;
    /// otherwise, false.</returns>
    bool CanRemove(IPlaceable candidate, ISurface[] neighbors);

    /// <summary>
    /// Returns true if this ISurface is occupied; otherwise, returns false.
    /// </summary>
    /// <returns> true if this ISurface is occupied; otherwise, returns false.
    ///</returns>
    bool Occupied();

    /// <summary>
    /// Notifies this ISurface of its new neighboring ISurface components.
    /// </summary>
    /// <param name="newNeighbors">this ISurface's new neighbors.</param>
    void UpdateNeighbors(ISurface[] newNeighbors);

    /// <summary>
    /// Returns this ISurface's four neighbors. 
    /// </summary>
    /// <returns>this ISurface's four neighbors.</returns>
    ISurface[] GetNeighbors();

    /// <summary>
    /// Asserts that this ISurface is defined.
    /// </summary>
    void AssertDefined();

    /// <summary>
    /// Returns the X-coordinate of this ISurface.
    /// </summary>
    /// <returns>the X-coordinate of this ISurface.</returns>
    int GetX();

    /// <summary>
    /// Returns the Y-coordinate of this ISurface.
    /// </summary>
    /// <returns>the Y-coordinate of this ISurface.</returns>
    int GetY();
}
