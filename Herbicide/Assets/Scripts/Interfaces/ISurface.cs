using UnityEngine;

/// <summary>
/// Contract for a model that can host a ISurfacePlaceable. In other
/// words, a ISurfacePlaceable can be placed on an ISurface.
/// </summary>
public interface ISurface
{
    /// <summary>
    /// The PlaceableObject that is currently on this ISurface.
    /// </summary>
    PlaceableObject Occupant { get; }

    /// <summary>
    /// The Ghost GameObject that is currently on this ISurface.
    /// </summary>
    GameObject GhostOccupant { get; }

    /// <summary>
    /// The LineRenderer component of this ISurface.
    /// </summary>
    LineRenderer LineRenderer { get; }

    /// <summary>
    /// true if this ISurface is walkable by default; otherwise, false.
    /// </summary>
    bool IsTraversable { get; }

    /// <summary>
    /// Returns true if a PlaceableObject can be placed on this ISurface.
    /// If it can, places the PlaceableObject.
    /// </summary>
    /// <param name="candidate">The PlaceableObject to place.</param>
    /// <returns>true if a PlaceableObject can be placed on this ISurface;
    /// otherwise, false.</returns>
    bool Place(ISurfacePlaceable candidate);

    /// <summary>
    /// Returns true if this ISurface is occupied; otherwise, returns false.
    /// </summary>
    /// <returns> true if this ISurface is occupied; otherwise, returns false.
    ///</returns>
    bool IsOccupied();

    /// <summary>
    /// Returns true if the given Model type lies on this Surface or
    /// any of its occupants.
    /// </summary>
    /// <typeparam name="T">The Model type to check for.</typeparam>
    /// <returns> true if the given Model type lies on this Surface or
    /// any of its occupants; otherwise, false.</returns>
    bool HasModel<T>();

    /// <summary>
    /// Returns true if the given Model type lies on this Surface or
    /// any of its occupants.
    /// </summary>
    /// <param name="modelType">The Model type to check for.</param>
    /// <returns>true if the given Model type lies on this Surface or
    /// any of its occupants; otherwise, false.</returns>
    bool HasModel(ModelType modelType);

    /// <summary>
    /// Removes the PlaceableObject on this ISurface.
    /// </summary>
    /// <returns>the PlaceableObject that was removed; null if there was none.</returns>
    PlaceableObject Remove();

    /// <summary>
    /// Provides a visual simulation of placing a candidate on
    /// this ISurface and is called during a hover / placement action.
    /// This method does not carry out actual placement of the candidate on
    /// the ISurface. Instead, it displays a potential placement scenario.
    /// </summary>
    /// <param name="candidate">The candidate object that we are
    /// trying to virtually place on the ISurface.</param>
    /// <returns> true if the ghost place was successful; otherwise,
    /// false. </returns> 
    bool GhostPlace(ISurfacePlaceable candidate);

    /// <summary>
    /// Draws the range indicator for a given Defender on this ISurface.
    /// </summary>
    /// <param name="defender">the given Defender</param>
    void DrawRangeIndicator(Defender defender);

    /// <summary>
    /// Returns true if the ghost occupant on this ISurface is not null.
    /// Ghost occupants may become null if the ISurface is destroyed
    /// while a ghost occupant sits on it.
    /// </summary>
    /// <returns>true if the ghost occupant on this ISurface is not null;
    /// otherwise, false.</returns>
    bool IsGhostOccupied();

    /// <summary>
    /// Removes all visual simulations of placing a PlaceableObject on this
    /// ISurface. If there are none, does nothing.
    /// </summary>
    void GhostRemove();

    /// <summary>
    /// Returns true if a pathfinder can walk across this ISurface.
    /// </summary>
    /// <returns>true if a pathfinder can walk across this ISurface;
    /// otherwise, false. </returns>
    bool IsCurrentlyWalkable();
}
