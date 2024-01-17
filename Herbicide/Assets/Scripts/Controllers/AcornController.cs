using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls an Acorn Projectile.
/// 
/// The AcornController is responsible for manipulating its Acorn and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public class AcornController : ProjectileController<AcornController.AcornState>
{
    /// <summary>
    /// Possible states of an Acorn over its lifetime.
    /// </summary>
    public enum AcornState
    {
        SPAWN,
        MOVING,
        COLLIDING,
        DEAD
    }


    /// <summary>
    /// Gives an Acorn an AcornController.
    /// </summary>
    /// <param name="acorn">The acorn which will get an AcornController.</param>
    /// <param name="destination">Where the acorn started.</param>
    /// <param name="destination">Where the acorn should go.</param>
    public AcornController(Acorn acorn, Vector3 start, Vector3 destination) :
        base(acorn, start, destination)
    { }

    //-----------------------STATE LOGIC------------------------//

    /// <summary>
    /// Updates the state of this AcornController's Acorn model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source <br></br>
    /// MOVING --> COLLIDING : when hits valid target <br></br>
    /// COLLIDING --> DEAD : when all effects have been applied to valid target <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case AcornState.SPAWN:
                SetState(AcornState.MOVING);
                break;
            case AcornState.MOVING:
                break;
            case AcornState.COLLIDING:
                break;
            case AcornState.DEAD:
                break;
        }
    }

    /// <summary>
    /// Returns true if two AcornStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two AcornStates are equal; otherwise, false.</returns>
    public override bool StateEquals(AcornState stateA, AcornState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Runs logic relevant to the Acorn's MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != AcornState.MOVING) return;

        //Call LinearShot here
        LinearShot();
    }

    /// <summary>
    /// Runs logic relevant to the Acorn's COLLIDING state.
    /// </summary>
    public override void ExecuteCollidingState() { return; }

    /// <summary>
    /// Runs logic relevant to the Acorn's DEAD state.
    /// </summary>
    public override void ExecuteDeadState() { return; }

    //---------------------ANIMATION LOGIC----------------------//

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() { throw new System.NotImplementedException(); }
}
