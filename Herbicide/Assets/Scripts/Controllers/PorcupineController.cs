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
    /// Creates a PorcupineController reference.
    /// </summary>
    /// <param name="porcupine">The Porcupine defender.</param>
    /// <returns>a new PorcupineController reference.</returns>
    public PorcupineController(Porcupine porcupine) : base(porcupine) { }

    /// <summary>
    /// Maximum number of targets a Porcupine can have at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    public override void AgeAnimationCounter()
    {
        throw new System.NotImplementedException();
    }

    public override void DestroyModel()
    {
        throw new System.NotImplementedException();
    }

    public override float GetAnimationCounter()
    {
        throw new System.NotImplementedException();
    }

    public override void ResetAnimationCounter()
    {
        throw new System.NotImplementedException();
    }

    public override bool StateEquals(PorcupineState stateA, PorcupineState stateB)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateStateFSM()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleCollision(Collider2D other)
    {
        throw new System.NotImplementedException();
    }


}
