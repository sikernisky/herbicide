using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Nexus.
/// 
/// The NexusController is responsible for manipulating its Nexus and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public class NexusController : StructureController<NexusController.NexusState>
{
    /// <summary>
    /// Maximum number of targets a Nexus can have.
    /// </summary>
    protected override int MAX_TARGETS => 0;

    /// <summary>
    /// Different states of a Nexus.
    /// /// </summary>
    public enum NexusState
    {
        SPAWN,
        IDLE,
        PICKED_UP
    }

    /// <summary>
    /// Assigns a Nexus to this NexusController.
    /// </summary>
    /// <param name="nexus"></param>The Nexus to assign. <summary>
    public NexusController(Nexus nexus) : base(nexus) { }

    /// <summary>
    /// Main update loop for the Nexus.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        ExecutePickedUpState();
    }

    /// <summary>
    /// Filters the PlaceableObjects that the Nexus can target. Nexii can target
    /// nothing, so this returns an empty list.
    /// </summary>
    /// <param name="targetables">All targetables in the scene.</param>
    /// <returns>An empty list (the Nexus can target nothing).</returns>
    protected override List<PlaceableObject> FilterTargets(List<PlaceableObject> targetables)
    {
        Assert.IsNotNull(targetables, "List of targets is null.");
        return new List<PlaceableObject>();
    }

    /// <summary>
    /// Returns true if the Nexus should be removed.
    /// </summary>
    /// <returns>true if the Nexus should be removed; otherwise, false.</returns>
    protected override bool ShouldRemoveModel() { return GetNexus().Dead(); }

    /// <summary>
    /// Returns the Nexus model.
    /// </summary>
    /// <returns>the Nexus model.</returns>
    private Nexus GetNexus() { return GetMob() as Nexus; }

    /// <summary>
    /// Handles a collision between the Nexus model and some other
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
    /// Updates the state of the Nexus. The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always
    /// </summary>
    public override void UpdateStateFSM()
    {
        switch (GetState())
        {
            case NexusState.SPAWN:
                SetState(NexusState.IDLE);
                break;
            case NexusState.IDLE:
                if (GetNexus().PickedUp()) SetState(NexusState.PICKED_UP);
                break;
            case NexusState.PICKED_UP:
                if (!GetNexus().PickedUp()) SetState(NexusState.IDLE);
                break;
        }
    }

    /// <summary>
    /// Returns true if two NexusStates are equal.
    /// </summary>
    /// <param name="stateA">The first NexusState.</param>
    /// <param name="stateB">The second NexusState.</param>
    /// <returns>true if the two NexusStates are equal.</returns>
    public override bool StateEquals(NexusState stateA, NexusState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Executes logic for the Nexus' picked up state.
    /// </summary>
    protected virtual void ExecutePickedUpState()
    {
        if (GetState() != NexusState.PICKED_UP) return;
        if (!ValidModel()) return;
        if (!GetNexus().PickedUp()) return;

        GetNexus().SetWorldPosition(GetNexus().GetHolderPosition());
    }
}
