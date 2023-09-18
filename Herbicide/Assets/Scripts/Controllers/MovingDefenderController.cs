using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for a Defender that moves around the TileGrid.
/// </summary>
public class MovingDefenderController : DefenderController
{
    /// <summary>
    /// Makes a new MovingDefenderController. 
    /// </summary>
    /// <param name="defender">The Defender to assign.</param>
    /// <returns></returns>
    public MovingDefenderController(Defender defender) : base(defender)
    {

    }
}
