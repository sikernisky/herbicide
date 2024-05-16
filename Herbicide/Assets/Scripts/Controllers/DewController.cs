using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Controls a Dew Collectable.
/// 
/// The DewController is responsible for manipulating its Dew and bringing
/// it to life. This includes moving it playing animations, and more.
/// </summary>
public class DewController : CollectableController<DewController.DewState>
{
    /// <summary>
    /// Number of Dew assigned since the scene began.
    /// </summary>
    private static int NUM_DEW;

    /// <summary>
    /// State of a Dew.
    /// </summary>
    public enum DewState
    {
        SPAWN,
        BOBBING,
        COLLECTING
    }

    /// <summary>
    /// Assigns a Dew to a DewController.
    /// </summary>
    /// <param name="dew">The Dew to assign.</param>
    /// <param name="dropPos">Where the Dew first dropped.</param>
    /// <param name="value">How much the Dew collectable is worth..</param>
    public DewController(Dew dew, Vector3 dropPos, int value) : base(dew, dropPos)
    {
        NUM_DEW++;
        GetDew().SetWorldPosition(dropPos);
        GetDew().AdjustValue(value);
    }

    /// <summary>
    /// Main update loop for the Dew.
    /// </summary>
    public override void UpdateController()
    {
        base.UpdateController();
        ExecuteBobbingState();
        ExecuteCollectingState();
    }

    /// <summary>
    /// Returns this controller's Dew model.
    /// </summary>
    /// <returns>this controller's Dew model.</returns>
    private Dew GetDew() { return GetCollectable() as Dew; }

    /// <summary>
    /// Returns the Dew prefab to the DewFactory singleton.
    /// </summary>
    public override void DestroyModel() { DewFactory.ReturnDewPrefab(GetDew().gameObject); }

    //-----------------------STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two DewStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if the two DewStates are equal.</returns>
    public override bool StateEquals(DewState stateA, DewState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this DewController's Dew model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> BOBBING : when dropped from source <br></br>
    /// BOBBING --> COLLECTING : when being collected <br></br>
    /// COLLECTING --> DEAD : when collected. <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        switch (GetState())
        {
            case DewState.SPAWN:
                SetState(DewState.BOBBING);
                break;
            case DewState.BOBBING:
                if (InHomingRange()) SetState(DewState.COLLECTING);
                break;
            case DewState.COLLECTING:
                break;
        }
    }

    /// <summary>
    /// Runs logic for the Dew's bobbing state.
    /// </summary>
    protected virtual void ExecuteBobbingState()
    {
        if (!ValidModel()) return;
        if (GetState() != DewState.BOBBING) return;

        BobUpAndDown();

    }

    /// <summary>
    /// Runs logic for the Dew's collecting state.
    /// </summary>
    protected virtual void ExecuteCollectingState()
    {
        if (!ValidModel()) return;
        if (GetState() != DewState.COLLECTING) return;

        if (InCollectionRange())
        {
            EconomyController.CashIn(GetDew());
            GetDew().OnCollect();
        }
        else
        {
            MoveTowardsCursor();

        }
    }

    //-----------------------ANIM LOGIC----------------------//

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() { return; }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() { return default; }
    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() { return; }
}
