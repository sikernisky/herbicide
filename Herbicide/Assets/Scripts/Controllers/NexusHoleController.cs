using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a NexusHole.
/// 
/// The NexusHoleController is responsible for manipulating its NexusHole and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public class NexusHoleController : StructureController<NexusHoleController.NexusHoleState>
{
    /// <summary>
    /// States of a NexusHole.
    /// </summary>
    public enum NexusHoleState
    {
        SPAWN,
        EMPTY,
        FILLED
    }

    /// <summary>
    /// Maximum number of targets a NexusHole can have.
    /// </summary>
    protected override int MAX_TARGETS => 0;


    /// <summary>
    /// Assigns a NexusHole to a controller.
    /// </summary>
    /// <param name="nexusHole">The NexusHole to assign.</param>
    public NexusHoleController(NexusHole nexusHole) : base(nexusHole) { }


    /// <summary>
    /// Returns true if the NexusHole can target the PlaceableObject passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Placeable object to check for targetability.</param>
    /// <returns></returns>
    protected override bool CanTarget(PlaceableObject target)
    {
        return false;
    }


    /// <summary>
    /// Returns true if the NexusHole should be removed.
    /// </summary>
    /// <returns>true if the NexusHole should be removed; otherwise, false.</returns>
    protected override bool ShouldRemoveModel() { return false; }

    /// <summary>
    /// Returns the NexusHole model.
    /// </summary>
    /// <returns>the NexusHole model.</returns>
    private NexusHole GetNexusHole() { return GetMob() as NexusHole; }

    /// <summary>
    /// Handles a collision between the NexusHole model and some other
    /// collider.
    /// </summary>
    /// <param name="other">The other 2D Collider.</param>
    protected override void HandleCollision(Collider2D other) { return; }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() { throw new System.NotImplementedException(); }


    //--------------------- STATE LOGIC-----------------------//

    /// <summary>
    /// Updates the state of the NexusHole. The transitions are: <br></br>
    /// 
    /// SPAWN --> EMPTY : always
    /// EMPTY --> FILLED : when nexus dropped in hole
    /// FILLED --> EMPTY : when nexus removed from hole 
    /// </summary>
    public override void UpdateStateFSM()
    {
        switch (GetState())
        {
            case NexusHoleState.SPAWN:
                SetState(NexusHoleState.EMPTY);
                break;
            case NexusHoleState.EMPTY:
                break;
            case NexusHoleState.FILLED:
                break;
        }
    }

    /// <summary>
    /// Returns true if two NexusHoleStates are equal.
    /// </summary>
    /// <param name="stateA">The first NexusHoleState.</param>
    /// <param name="stateB">The second NexusHoleState.</param>
    /// <returns>true if the two NexusHoleStates are equal.</returns>
    public override bool StateEquals(NexusHoleState stateA, NexusHoleState stateB)
    {
        return stateA == stateB;
    }
}
