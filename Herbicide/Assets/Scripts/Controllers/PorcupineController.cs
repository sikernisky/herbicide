using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a Porcupoine.
/// </summary>
public class PorcupineController : DefenderController<PorcupineController.PorcupineState>
{

    /// <summary>
    /// States of a Porcupine.
    /// </summary>
    public enum PorcupineState
    {
        SPAWN,
        IDLE,
        ATTACK
    }

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    private float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the attack animation; resets
    /// on step.
    /// </summary>
    private float attackAnimationCounter;


    /// <summary>
    /// Creates a PorcupineController reference.
    /// </summary>
    /// <param name="porcupine">The Porcupine defender.</param>
    /// <returns>a new PorcupineController reference.</returns>
    public PorcupineController(Porcupine porcupine) : base(porcupine) { }

    /// <summary>
    /// Maximum number of targets a Porcupine can have at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// Main update loop for the Porcupine.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        if (!ValidModel()) return;

        ExecuteSpawnState();
    }

    /// <summary>
    /// Returns the Porcupine prefab to the DefenderFactory singleton.
    /// </summary>
    public override void DestroyModel() { DefenderFactory.ReturnDefenderPrefab(GetPorcupine().gameObject); }

    /// <summary>
    /// Returns this PorcupineController's Porcupine.
    /// </summary>
    /// <returns>this PorcupineController's Porcupine.</returns>
    private Porcupine GetPorcupine() { return GetMob() as Porcupine; }

    /// <summary>
    /// Returns true if two PorcupineStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two PorcupineStates are equal; otherwise, false.</returns>
    public override bool StateEquals(PorcupineState stateA, PorcupineState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this PorcupineController's Porcupine model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(PorcupineState.IDLE);
            return;
        }
    }

    /// <summary>
    /// Handles all collisions between this controller's Porcupine
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other) { throw new System.NotImplementedException(); }


    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        PorcupineState state = GetState();
        if (state == PorcupineState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == PorcupineState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        PorcupineState state = GetState();
        if (state == PorcupineState.IDLE) return idleAnimationCounter;
        else if (state == PorcupineState.ATTACK) return attackAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        PorcupineState state = GetState();
        if (state == PorcupineState.IDLE) idleAnimationCounter = 0;
        else if (state == PorcupineState.ATTACK) attackAnimationCounter = 0;
    }
}
