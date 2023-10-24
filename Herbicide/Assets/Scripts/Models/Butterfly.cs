using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Buttefly Defender. It drops bombs on
/// enemies. 
/// </summary>
public class Butterfly : Defender
{
    /// <summary>
    /// Type of a Butterfly
    /// </summary>
    public override DefenderType TYPE => DefenderType.BUTTERFLY;

    /// <summary>
    /// Class of a Buttefly
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.ANIMYST;

    /// <summary>
    /// Name of a Butterfly
    /// </summary>
    protected override string NAME => "Butterfly";

    /// <summary>
    /// How much currency it takes to place a Butterfly
    /// </summary>
    protected override int COST => 1;

    /// <summary>
    /// Starting attack speed of a Butterfly
    /// </summary>
    public override float BASE_ATTACK_SPEED => 1f;

    /// <summary>
    /// Starting attack range of a Butterfly.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 10f;



    /// <summary>
    /// Returns the most updated DefenderState of this defender
    /// after considering its environment. 
    /// </summary>
    /// <param name="currentState">The current state of this Defender.</param>
    /// <param name="targetsInRange">The number of targets this Defender
    /// can see. </param>
    /// <returns>the correct, up to date DefenderState of this Defender. </returns>
    public override DefenderController.DefenderState DetermineState(
        DefenderController.DefenderState currentState,
        int targetsInRange)
    {
        return DefenderController.DefenderState.INVALID;
    }
}
