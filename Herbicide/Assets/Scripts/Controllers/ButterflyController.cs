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
    /// true if the Butterfly has attacked during its current attack sequence;
    /// otherwise, false.
    /// </summary>
    private bool bombed;

    /// <summary>
    /// State of a Butterfly. 
    /// </summary>
    public enum ButterflyState
    {
        SPAWN,
        IDLE,
        CHASE,
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
    /// Updates the Butterfly controlled by this ButterflyController.
    /// </summary>
    /// <param name="targets">A complete list of ITargetables in the scene.</param>
    protected override void UpdateMob()
    {
        if (!ValidModel()) return;
        base.UpdateMob();
        ExecuteChaseState();
    }

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
            float animationTime = 0;
            switch (GetState())
            {
                case ButterflyState.SPAWN:
                    GetButterfly().FaceDirection(Direction.NORTH);
                    break;
                case ButterflyState.CHASE:
                    animationTime = GetButterfly().ATTACK_ANIMATION_DURATION;
                    Sprite[] chaseTrack = DefenderFactory.GetMovementTrack(
                        GetButterfly().TYPE,
                        GetButterfly().GetDirection());
                    SetAnimationTrack(chaseTrack, GetState());
                    break;
                case ButterflyState.ATTACK:
                    animationTime = GetButterfly().ATTACK_ANIMATION_DURATION;
                    Sprite[] attackTrack = DefenderFactory.GetAttackTrack(
                        GetButterfly().TYPE,
                        GetButterfly().GetDirection());
                    SetAnimationTrack(attackTrack, GetState());
                    break;
                case ButterflyState.IDLE:
                    animationTime = GetButterfly().IDLE_ANIMATION_DURATION;
                    Sprite[] idleTrack = DefenderFactory.GetMovementTrack(
                        GetButterfly().TYPE,
                        GetButterfly().GetDirection());
                    SetAnimationTrack(idleTrack, GetState());
                    break;
                default: //Default to Idle animation
                    throw new System.Exception("Animation not supported for " + GetState() + ".");
            }

            //(2): Flip the flipbook.
            if (HasAnimationTrack() && animationTime > 0)
            {
                float waitTime = animationTime / (GetFrameCount() + 1);
                GetButterfly().SetSprite(GetSpriteAtCurrentFrame());
                yield return new WaitForSeconds(waitTime);
                NextFrame();
                bombed = false;
            }
            else yield return null;
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
    /// Returns true if two ButterflyStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two ButterflyStates are equal; otherwise, false.</returns>
    protected override bool StateEquals(ButterflyState stateA, ButterflyState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this ButterflyController's Butterfly model.<br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> CHASE : if target in chase range <br></br>
    /// CHASE --> ATTACK : if target in attack range <br></br>
    /// ATTACK --> CHASE : if target not in attack range <br></br>
    /// CHASE --> IDLE : if target not in chase range <br></br>
    /// </summary>
    protected override void UpdateStateFSM()
    {
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(ButterflyState.IDLE);
            return;
        }

        //Debug.Log(GetState());

        switch (GetState())
        {
            case ButterflyState.SPAWN:
                SetState(ButterflyState.IDLE);
                break;
            case ButterflyState.IDLE:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                else if (GetButterfly().DistanceToTarget(GetTarget()) <= GetButterfly().GetChaseRange())
                    SetState(ButterflyState.CHASE);
                break;
            case ButterflyState.CHASE:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                else if (GetButterfly().DistanceToTarget(GetTarget()) <= GetButterfly().GetAttackRange())
                    SetState(ButterflyState.ATTACK);
                else if (GetButterfly().DistanceToTarget(GetTarget()) > GetButterfly().GetChaseRange())
                    SetState(ButterflyState.IDLE);
                break;
            case ButterflyState.ATTACK:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                if (GetButterfly().DistanceToTarget(GetTarget()) > GetButterfly().GetAttackRange())
                    SetState(ButterflyState.CHASE);
                break;
            case ButterflyState.INVALID:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Buttefly's chasing state.
    /// </summary>
    protected override void ExecuteChaseState()
    {
        if (GetState() != ButterflyState.CHASE) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;

        Vector2 targetPosition = GetTarget().GetPosition();
        Vector2 currentPosition = GetButterfly().GetPosition();
        Vector2 direction = (targetPosition - currentPosition).normalized;
        Vector2 newPosition = currentPosition + direction * GetButterfly().GetMovementSpeed() * Time.deltaTime;
        GetButterfly().SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Runs logic relevant to the Butterfly's attacking state.
    /// </summary>
    protected override void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;
        if (GetState() != ButterflyState.ATTACK) return;

        if (!CanAttack()) return;
        Vector3 targetPosition = GetTarget().GetAttackPosition();
        ProjectileController.ProjectileType bomb = ProjectileController.ProjectileType.BUTTERFLY_BOMB;
        ProjectileController.LobShot(bomb, GetButterfly().transform, GetTarget().GetTransform(), .2f, 1f);
        bombed = true;

    }

    /// <summary>
    /// Returns true if the Butterfly can lob a bomb.
    /// </summary>
    /// <returns>true if the Butterfly can lob a bomb; 
    /// otherwise, false.</returns>
    protected override bool CanAttack()
    {
        if (!base.CanAttack()) return false; //Cooldown
        if (GetTarget() == null || !GetTarget().Targetable()) return false;
        if (GetFrameNumber() != 0) return false;
        if (bombed) return false;
        if (GetState() != ButterflyState.ATTACK) return false;
        return true;
    }

    /// <summary>
    /// Runs logic relevant to the Butterfly's idle state.
    /// </summary>
    protected override void ExecuteIdleState() { return; }

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
