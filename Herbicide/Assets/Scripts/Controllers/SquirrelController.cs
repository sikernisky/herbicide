using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for a Squirrel Defender.
/// </summary>
public class SquirrelController : DefenderController<SquirrelController.SquirrelState>
{
    /// <summary>
    /// The number of Squirrels spawned so far this scene.
    /// </summary>
    private static int NUM_SQUIRRELS;

    /// <summary>
    /// State of a Squirrel. 
    /// </summary>
    public enum SquirrelState
    {
        SPAWN,
        IDLE,
        ATTACK,
        INVALID
    }

    /// <summary>
    /// Makes a new SquirrelController.
    /// </summary>
    /// <param name="defender">The Squirrel Defender. </param>
    /// <returns>The created SquirrelController.</returns>
    public SquirrelController(Squirrel squirrel) : base(squirrel)
    {
        NUM_SQUIRRELS++;
    }

    /// <summary>
    /// Returns this SquirrelController's Squirrel.
    /// </summary>
    /// <returns>this SquirrelController's Squirrel.</returns>
    private Squirrel GetSquirrel() { return GetMob() as Squirrel; }

    /// <summary>
    /// Returns true if the Squirrel controlled by this SquirrelController
    /// is not null.
    /// </summary>
    /// <returns>true if the Squirrel controlled by this SquirrelController
    /// is not null; otherwise, returns false.</returns>
    public override bool ValidModel() { return GetSquirrel() != null; }

    /// <summary>
    /// Plays the current animation of the Squirrel. Acts like a flipbook;
    /// keeps track of frames and increments this counter to apply
    /// the correct Sprites to the Squirrel's SpriteRenderer. <br></br>
    /// 
    /// This method is also responsible for choosing the correct animation
    /// based off the SquirrelState. 
    /// /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    protected override IEnumerator CoPlayAnimation()
    {
        while (true)
        {
            //(1): Choose the right animation based off the current state.
            float animationTime = 0;
            switch (GetState())
            {
                case SquirrelState.SPAWN:
                    GetSquirrel().FaceDirection(Direction.SOUTH);
                    break;
                case SquirrelState.ATTACK:
                    animationTime = GetSquirrel().ATTACK_ANIMATION_DURATION;
                    Sprite[] attackTrack = DefenderFactory.GetAttackTrack(
                        GetSquirrel().TYPE,
                        GetSquirrel().GetDirection());
                    SetAnimationTrack(attackTrack);
                    break;
                case SquirrelState.IDLE:
                    animationTime = GetSquirrel().IDLE_ANIMATION_DURATION;
                    Sprite[] idleTrack = DefenderFactory.GetIdleTrack(
                        GetSquirrel().TYPE,
                        GetSquirrel().GetDirection());
                    SetAnimationTrack(idleTrack);
                    break;
                default: //Default to Idle animation
                    throw new Exception("Animation not supported for " + GetState() + ".");
            }

            //(2): Flip the flipbook.
            if (HasAnimationTrack() && animationTime > 0)
            {
                //On attack animation, shoot the acorn.
                if (GetState() == SquirrelState.ATTACK && GetFrameNumber() == 1)
                {
                    Vector3 targetPosition = GetTarget().GetAttackPosition();
                    ProjectileController.ProjectileType acorn = ProjectileController.ProjectileType.ACORN;
                    ProjectileController.Shoot(acorn, GetSquirrel().transform, targetPosition);
                }
                float waitTime = animationTime / (GetFrameCount() + 1);
                GetSquirrel().SetSprite(GetSpriteAtCurrentFrame());
                yield return new WaitForSeconds(waitTime);
                NextFrame();
            }
            else yield return null;
        }
    }

    /// <summary>
    /// Handles all collisions between this controller's Squirrel
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other)
    {
        throw new NotImplementedException();
    }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Updates the state of this SquirrelController's Squirrel model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> ATTACK : if target in range <br></br>
    /// ATTACK --> IDLE : if no target in range
    /// </summary>
    protected override void UpdateStateFSM()
    {
        switch (GetState())
        {
            case SquirrelState.SPAWN:
                SetState(SquirrelState.IDLE);
                break;
            case SquirrelState.IDLE:
                if (GetSquirrel().DistanceToTarget(GetTarget())
                    <= GetSquirrel().GetAttackRange()) SetState(SquirrelState.ATTACK);
                break;
            case SquirrelState.ATTACK:
                if (GetSquirrel().DistanceToTarget(GetTarget())
                    > GetSquirrel().GetAttackRange()) SetState(SquirrelState.IDLE);
                break;
            case SquirrelState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's idle state.
    /// </summary>
    protected override void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != SquirrelState.IDLE) return;
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's attacking state.
    /// </summary>
    protected override void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        if (GetTarget() == null) return;
        if (GetState() != SquirrelState.ATTACK) return;

        GetSquirrel().FaceTarget(GetTarget());
    }

    //---------------------END STATE LOGIC-----------------------//
}
