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
public class NexusController : MobController<NexusController.NexusState>
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
        PICKED_UP,
        CASHED_IN
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
        ExecuteIdleState();
        ExecutePickedUpState();
    }

    /// <summary>
    /// Returns true if the Nexus can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the Nexus can target the Model passed
    /// into this method; otherwise, false.</returns>
    protected override bool CanTarget(Model target) { return false; }

    /// <summary>
    /// Returns true if the Nexus should be removed.
    /// </summary>
    /// <returns>true if the Nexus should be removed; otherwise, false.</returns>
    public override bool ValidModel() { return !GetNexus().CashedIn(); }

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

    /// <summary>
    /// Returns the Nexus prefab to the NexusFactory singleton.
    /// </summary>
    public override void DestroyModel()
    {
        NexusFactory.ReturnNexusPrefab(GetNexus().gameObject);
    }
    //--------------------- STATE LOGIC-----------------------//

    /// <summary>
    /// Updates the state of the Nexus. The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case NexusState.SPAWN:
                SetState(NexusState.IDLE);
                break;
            case NexusState.IDLE:
                if (GetNexus().PickedUp()) SetState(NexusState.PICKED_UP);
                break;
            case NexusState.PICKED_UP:
                if (!GetNexus().PickedUp() && !GetNexus().CashedIn()) SetState(NexusState.IDLE);
                if (GetNexus().CashedIn()) SetState(NexusState.CASHED_IN);
                break;
            case NexusState.CASHED_IN:
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
    /// Executes logic for the Nexus' idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != NexusState.IDLE) return;
        if (!ValidModel()) return;
        if (GetNexus().PickedUp()) return;
        if (GetNexus().CashedIn()) return;

        if(TileGrid.IsNexusHole(GetNexus().GetPosition()))
        {
            Vector3 nexusHolePosition = GetNexus().GetPosition();
            Vector3 jumpPosition = nexusHolePosition;
            jumpPosition.y -= 2;

            SetNextMovePos(jumpPosition);
            FallIntoMovePos(3f);

            if (!ReachedMovementTarget()) return;
            GetNexus().CashIn();
        }
    }

    /// <summary>
    /// Executes logic for the Nexus' picked up state.
    /// </summary>
    protected virtual void ExecutePickedUpState()
    {
        if (GetState() != NexusState.PICKED_UP) return;
        if (!ValidModel()) return;
        if (!GetNexus().PickedUp()) return;
    }

   
}
