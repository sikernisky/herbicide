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
    public override bool StateEquals(BasicTreeState stateA, BasicTreeState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this BasicTreeController's Tree model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    public override void UpdateStateFSM()
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
        if (!ValidModel()) return;

        EmitResources();
    }

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

    //---------------------END STATE LOGIC-----------------------//

    /// <summary>
    /// Drops one prefab of the BasicTree's resource.
    /// </summary>
    protected override void DropResources()
    {
        GameObject dewPrefab = CollectableFactory.GetCollectablePrefab(Collectable.CollectableType.DEW);
        GameObject clonedDew = GameObject.Instantiate(dewPrefab);
        Assert.IsNotNull(clonedDew);
        Dew dewComp = clonedDew.GetComponent<Dew>();
        Assert.IsNotNull(dewComp);
        Vector3 dropPosition = new Vector3(
            GetTree().GetX() + GetTree().DEFENDER_OFFSET_X,
            GetTree().GetY() + GetTree().DEFENDER_OFFSET_Y,
            1
        );
        DewController dewController = new DewController(dewComp, dropPosition);
        AddController(dewController);
    }
}
