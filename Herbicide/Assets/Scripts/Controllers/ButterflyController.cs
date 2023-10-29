using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for a Butterfly Defender.
/// </summary>
public class ButterflyController : DefenderController
{
    /// <summary>
    /// Makes a new ButterflyController.
    /// </summary>
    /// <param name="defender">The Butterfly Defender. </param>
    /// <returns>The created ButterflyController.</returns>
    public ButterflyController(Defender defender) : base(defender)
    {
        Assert.IsNotNull(defender as Butterfly, "ButterflyControllers must be " +
            "assigned Butterfly Defenders.");
    }
}
