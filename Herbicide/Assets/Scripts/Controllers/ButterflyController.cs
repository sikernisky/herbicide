using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for a Butterfly Defender.
/// </summary>
public class ButterflyController : DefenderController<ButterflyController.ButterflyState>
{
    /// <summary>
    /// The number of Butterflys spawned so far this scene.
    /// </summary>
    private static int NUM_BUTTERFLIES;

    /// <summary>
    /// State of a Butterfly. 
    /// </summary>
    public enum ButterflyState
    {
        SPAWN,
        IDLE,
        ATTACK,
        INVALID
    }


    /// <summary>
    /// Makes a new ButterflyController.
    /// </summary>
    /// <param name="defender">The Butterfly Defender. </param>
    /// <returns>The created ButterflyController.</returns>
    public ButterflyController(Defender defender) : base(defender) { NUM_BUTTERFLIES++; }

    /// <summary>
    /// Returns this ButterflyController's Butterfly.
    /// </summary>
    /// <returns>this ButterflyController's Butterfly.</returns>
    private Butterfly GetButterfly() { return GetMob() as Butterfly; }

    /// <summary>
    /// Returns true if the Butterfly controlled by this ButterflyController
    /// is not null.
    /// </summary>
    /// <returns>true if the Butterfly controlled by this ButterflyController
    /// is not null; otherwise, returns false.</returns>
    public override bool ValidModel() { return GetButterfly() != null; }

    /// <summary>
    /// Plays the current animation of the Butterfly. Acts like a flipbook;
    /// keeps track of frames and increments this counter to apply
    /// the correct Sprites to the Butterfly's SpriteRenderer. <br></br>
    /// 
    /// This method is also responsible for choosing the correct animation
    /// based off the ButterflyState. 
    /// /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    protected override IEnumerator CoPlayAnimation()
    {
        while (true)
        {
            //(1): Choose the right animation based off the current state.
            //float animationTime;
            switch (GetState())
            {
                case ButterflyState.ATTACK:
                    throw new System.NotImplementedException("Butterfly not implemented.");
                case ButterflyState.IDLE:
                    throw new System.NotImplementedException("Butterfly not implemented.");
                default: //Default to Idle animation
                    throw new System.Exception("Animation not supported for " + GetState() + ".");
            }

            //(2): Flip the flipbook.
            // if (HasAnimationTrack())
            // {
            //     float waitTime = animationTime / (GetFrameCount() + 1);
            //     GetButterfly().SetSprite(GetSpriteAtCurrentFrame());
            //     yield return new WaitForSeconds(waitTime);
            //     NextFrame();
            // }
            // else yield return null;
        }
    }

    /// <summary>
    /// Handles all collisions between this controller's Butterfly
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other)
    {
        throw new System.NotImplementedException();
    }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Updates the state of this ButterflyController's Butterfly model.
    /// The transitions are TBD.
    /// </summary>
    protected override void UpdateStateFSM() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Runs logic relevant to the Butterfly's attacking state.
    /// </summary>
    protected override void ExecuteAttackState() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Runs logic relevant to the Butterfly's idle state.
    /// </summary>
    protected override void ExecuteIdleState() { throw new System.NotImplementedException(); }

    //---------------------END STATE LOGIC-----------------------//

}
