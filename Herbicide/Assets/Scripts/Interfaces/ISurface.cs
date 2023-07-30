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
    /// Returns true if this ISurface is occupied; otherwise, returns false.
    /// </summary>
    /// <returns> true if this ISurface is occupied; otherwise, returns false.
    ///</returns>
    bool Occupied();

    /// <summary>
    /// Informs this ISurface whether an Enemy is on it or not.
    /// </summary>
    void SetOccupiedByEnemy(bool occupiedByEnemy);

    /// <summary>
    /// Returns true if an Enemy is on this ISurface.
    /// </summary>
    /// <returns>true if an Enemy is on this ISurface; otherwise, false.
    /// </returns>
    bool OccupiedByEnemy();

    /// <summary>
    /// Notifies this ISurface of its new neighboring ISurface components.
    /// </summary>
    /// <param name="newNeighbors">this ISurface's new neighbors.</param>
    void UpdateSurfaceNeighbors(ISurface[] newNeighbors);

    /// <summary>
    /// Returns this ISurface's four neighbors. 
    /// </summary>
    /// <returns>this ISurface's four neighbors.</returns>
    ISurface[] GetSurfaceNeighbors();

    /// <summary>
    /// Returns this ISurface's four neighbors' PlaceableObjects.
    /// </summary>
    /// <returns>this ISurface's four neighbors' PlaceableObjects.</returns>
    PlaceableObject[] GetPlaceableObjectNeighbors();

    /// <summary>
    /// Returns the PlaceableObject on this ISurface.
    /// </summary>
    /// <returns> the PlaceableObject on this ISurface;
    /// null if it has none.</returns>
    PlaceableObject GetPlaceableObject();

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

    /// <summary>
    /// Provides a visual simulation of placing an IPlaceable on
    /// this ISurface and is called during a hover / placement action.
    /// This method does not carry out actual placement of the IPlaceable on
    /// the ISurface. Instead, it displays a potential placement scenario. If
    /// the IPlaceable is allowed to be positioned  on this ISurface, the ISurface
    /// appears highlighted in blue. Conversely, if the placement is disallowed, 
    /// the ISurface will be highlighted in red.
    /// </summary>
    /// <param name="ghost">The IPlaceable object that we are
    /// trying to virtually place on the ISurface.</param>
    /// <returns> true if the ghost place was successful; otherwise,
    /// false. </returns> 
    bool GhostPlace(IPlaceable ghost);


    /// <summary>
    /// Determines whether an IPlaceable object can be potentially placed
    /// on the given ISurface. This method is invoked alongside GhostPlace()
    /// during a hover or placement action to validate the placement feasibility.
    /// </summary>
    /// <param name="ghost">The IPlaceable object that we are
    /// trying to virtually place on the ISurface.</param>
    /// <returns>true if the IPlaceable object can be placed on the ISurface;
    /// otherwise, false.</returns>
    bool CanGhostPlace(IPlaceable ghost);

    /// <summary>
    /// Removes all visual simulations of placing an IPlaceable on this
    /// ISurface. If there are none, does nothing.
    /// </summary>
    void GhostRemove();

    /// <summary>
    /// Returns true if the ghost occupant on this ISurface is not null.
    /// Ghost occupants may become null if the ISurface is destroyed
    /// while a ghost occupant sits on it.
    /// </summary>
    /// <returns>true if the ghost occupant on this ISurface is not null;
    /// otherwise, false.</returns>
    bool HasActiveGhostOccupant();

    /// <summary>
    /// Returns true if a pathfinder can walk across this ISurface.
    /// </summary>
    /// <returns>true if a pathfinder can walk across this ISurface;
    /// otherwise, false. </returns>
    bool IsWalkable();
}
