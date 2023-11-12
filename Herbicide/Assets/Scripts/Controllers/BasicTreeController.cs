using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        NUM_BASIC_TREES++;
    }


    /// <summary>
    /// Plays the current animation of the BasicTree. Acts like a flipbook;
    /// keeps track of frames and increments this counter to apply
    /// the correct Sprites to the BasicTree's SpriteRenderer. <br></br>
    /// 
    /// This method is also responsible for choosing the correct animation
    /// based off the BasicTreeState. 
    /// /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    protected override IEnumerator CoPlayAnimation()
    {
        while (true) { yield return null; }
        //TODO: Implement factory functionality for Trees
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
    /// Runs logic relevant to the BasicTrees's idle state.
    /// </summary>
    protected override void ExecuteIdleState()
    {
        if (GetState() != BasicTreeState.IDLE) return;
    }

    //---------------------END STATE LOGIC-----------------------//

}
