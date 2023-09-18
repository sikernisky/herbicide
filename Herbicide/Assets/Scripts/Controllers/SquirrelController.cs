using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for a Squirrel Defender.
/// </summary>
public class SquirrelController : DefenderController
{
    /// <summary>
    /// Makes a new SquirrelController.
    /// </summary>
    /// <param name="defender">The Squirrel Defender. </param>
    /// <returns></returns>
    public SquirrelController(Defender defender) : base(defender)
    {
        Assert.IsNotNull(defender as Squirrel, "SquirrelControllers must be " +
            "assigned Squirrel Defenders.");
    }
}
