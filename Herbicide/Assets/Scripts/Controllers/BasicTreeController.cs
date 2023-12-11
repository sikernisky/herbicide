using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for a BasicTree.
/// </summary>
public class BasicTreeController : TreeController<BasicTreeController.BasicTreeState>
{
    /// <summary>
    /// State of a BasicTree.
    /// </summary>
    public enum BasicTreeState
    {
        SPAWN,
        IDLE,
        INVALID
    }

    /// <summary>
    /// Number of BasicTrees spawned since the scene began.
    /// </summary>
    private static int NUM_BASIC_TREES;


    /// <summary>
    /// Makes a new BasicTreeController.
    /// </summary>
    /// <param name="basicTree">The BasicTree model.</param>
    public BasicTreeController(BasicTree basicTree) : base(basicTree)
    {
        Assert.IsNotNull(basicTree, "Basic Tree cannot be null.");
        NUM_BASIC_TREES++;
    }

    /// <summary>
    /// Main update loop for the BasicTree.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        if (!ValidModel()) return;

        ExecuteIdleState();
    }

    /// <summary>
    /// Handles all collisions between this controller's BasicTree
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other)
    {
        throw new System.NotImplementedException();
    }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two BasicTreeStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two BasicTreeStates are equal; otherwise, false.</returns>
    protected override bool StateEquals(BasicTreeState stateA, BasicTreeState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this BasicTreeController's Tree model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    protected override void UpdateStateFSM()
    {
        switch (GetState())
        {
            case BasicTreeState.SPAWN:
                SetState(BasicTreeState.IDLE);
                break;
            case BasicTreeState.IDLE:
                break;
            case BasicTreeState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the BasicTree's IDLE state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != BasicTreeState.IDLE) return;
    }

    protected override void AgeAnimationCounter()
    {
        throw new System.NotImplementedException();
    }

    protected override float GetAnimationCounter()
    {
        throw new System.NotImplementedException();
    }

    protected override void ResetAnimationCounter()
    {
        throw new System.NotImplementedException();
    }

    //---------------------END STATE LOGIC-----------------------//

}
