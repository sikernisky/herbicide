using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Controls a Structure.
/// 
/// The StructureController is responsible for manipulating its Structure and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public abstract class StructureController<T> : MobController<T> where T : Enum
{
    /// <summary>
    /// Assigns a Structure to a StructureController.
    /// </summary>
    /// <param name="structure">The Structure that needs a controller.</param>
    protected StructureController(Structure structure) : base(structure) { }
}
