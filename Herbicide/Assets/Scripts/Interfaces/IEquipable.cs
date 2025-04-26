using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contract for a model that can be equipped by an ISurface or
/// its occupants. 
/// </summary>
public interface IEquipable : IUseable
{
    /// <summary>
    /// The type of equipment that can be placed on this IEquipable.
    /// </summary>
    ModelType EquipmentType { get; }
}
