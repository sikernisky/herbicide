using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Mob that doesn't move and is often
/// already placed on the TileGrid when the level begins.
/// </summary>
public abstract class Structure : Mob
{
    /// <summary>
    /// Different types of Structures.
    /// </summary>
    public enum StructureType
    {
        NEXUS,
        NEXUS_HOLE,
    }

    /// <summary>
    /// Type of this Structure.
    /// </summary>
    public abstract StructureType TYPE { get; }
}
