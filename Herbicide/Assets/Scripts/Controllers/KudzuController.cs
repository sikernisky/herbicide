using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

/// <summary>
/// Controller for a Kudzu Enemy.
/// </summary>
public class KudzuController : EnemyController<KudzuController.KudzuState>
{
    /// <summary>
    /// State of a Kudzu.
    /// </summary>
    public enum KudzuState
    {
        INACTIVE,
        SPAWN,
        IDLE,
        CHASE,
        ATTACK,
        INVALID
    }

    /// <summary>
    /// The number of Kudzus spawned so far this scene.
    /// </summary>
    private static int NUM_KUDZUS;

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    private float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the chase animation; resets
    /// on step.
    /// </summary>
    private float chaseAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the attack animation; resets
    /// on step.
    /// </summary>
    private float attackAnimationCounter;


    /// <summary>
    /// Makes a new KudzuController.
    /// </summary>
    /// <param name="kudzu">The Kudzu Enemy. </param>
    /// <param name="spawnTime">The time at which the Kudzu Enemy spawns. </param>
    /// <param name="spawnCoords">The (X, Y) coordinates where the Kudzu spawns. </param>
    /// <returns>The created KudzuController.</returns>
    public KudzuController(Kudzu kudzu, float spawnTime, Vector2 spawnCoords)
    : base(kudzu) { NUM_KUDZUS++; }

    /// <summary>
    /// Updates the Kudzu controlled by this KudzuController.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();

        //STATE LOGIC (UpdateFSM() is called in base())
        //Debug.Log(GetState());
        if (!ValidModel()) return;
        ExecuteAttackState();
        ExecuteChaseState();
    }

    /// <summary>
    /// Returns this KudzuController's Kudzu model.
    /// </summary>
    /// <returns>this KudzuController's Kudzu model.</returns>
    private Kudzu GetKudzu() { return GetModel() as Kudzu; }

    /// <summary>
    /// Returns true if this KudzuController's has a not NULL Kudzu model.
    /// </summary>
    /// <returns>true if this KudzuController's has a not NULL Kudzu model</returns>
    public override bool ValidModel() { return GetKudzu() != null; }

    /// <summary>
    /// Plays the current animation of the Kudzu. Acts like a flipbook;
    /// keeps track of frames and increments this counter to apply
    /// the correct Sprites to the Kudzu's SpriteRenderer. <br></br>
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    protected override IEnumerator CoPlayAnimation()
    {
        yield return null;
    }

    /// <summary>
    /// Handles all collisions between this controller's Kudzu
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other)
    {
        if (other == null) return;

        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null && projectile.IsActive())
        {
            SoundController.PlaySoundEffect("kudzuHit");
            GetKudzu().AdjustHealth(-projectile.GetDamage());
            projectile.SetAsInactive();
        }
    }

    /// <summary>
    /// Does not move the Kudzu model. This is overriden to ensure any Kudzu 
    /// movement executes in CoHop(). 
    public override void MoveModel(Vector3 targetPosition, float duration, AnimationCurve lerp = null)
    {
        return;
    }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two KudzuStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two KudzuStates are equal; otherwise, false.</returns>
    protected override bool StateEquals(KudzuState stateA, KudzuState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this KudzuController's Kudzu model.
    /// The transitions are: <br></br>
    /// 
    /// INACTIVE --> SPAWN : if ready to spawn <br></br>
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> CHASE : if target in chase range <br></br>
    /// CHASE --> ATTACK : if target in attack range and attack off cooldown <br></br>
    /// ATTACK --> CHASE : if target not in attack range, or attack on cooldown <br></br>
    /// CHASE --> IDLE : if target not in chase range <br></br>
    /// </summary>
    protected override void UpdateStateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(KudzuState.CHASE);
            return;
        }

        KudzuState stateBefore = GetState();
        switch (GetState())
        {
            case KudzuState.INACTIVE:
                if (GetKudzu().GetSpawnTime() <= SceneController.GetTimeElapsed())
                    SetState(KudzuState.SPAWN);
                break;
            case KudzuState.SPAWN:
                SetState(KudzuState.IDLE);
                break;
            case KudzuState.IDLE:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                else if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange())
                    SetState(KudzuState.ATTACK);
                else if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetChaseRange())
                    SetState(KudzuState.CHASE);
                break;
            case KudzuState.CHASE:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                else if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange()
                        && GetKudzu().GetAttackCooldown() <= 0) SetState(KudzuState.ATTACK);
                else if (GetKudzu().DistanceToTarget(GetTarget()) > GetKudzu().GetChaseRange())
                    SetState(KudzuState.IDLE);
                break;
            case KudzuState.ATTACK:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                if (GetAnimationCounter() > 0) break;
                if (GetKudzu().GetAttackCooldown() > 0) SetState(KudzuState.CHASE);
                else if (GetKudzu().DistanceToTarget(GetTarget()) > GetKudzu().GetAttackRange())
                    SetState(KudzuState.CHASE);
                break;
            case KudzuState.INVALID:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Runs logic for the Kudzu model's idle state.
    /// </summary>
    protected override void ExecuteIdleState()
    {
        if (GetState() != KudzuState.IDLE) return;

        //Setup animations
        SetAnimationDuration(GetKudzu().MOVE_ANIMATION_DURATION);
        Sprite[] chaseTrack = EnemyFactory.GetMovementTrack(
            GetKudzu().TYPE,
            GetKudzu().GetHealthState(),
            GetKudzu().GetDirection());
        SetAnimationTrack(chaseTrack, GetState());

        //Step the animation.
        StepAnimation();
    }

    /// <summary>
    /// Runs logic for the Kudzu model's chase state. 
    /// </summary>
    protected override void ExecuteChaseState()
    {
        if (GetState() != KudzuState.CHASE) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;

        //Set up the animation.
        SetAnimationDuration(GetKudzu().MOVE_ANIMATION_DURATION);
        Sprite[] chaseTrack = EnemyFactory.GetMovementTrack(
            GetKudzu().TYPE,
            GetKudzu().GetHealthState(),
            GetKudzu().GetDirection());
        SetAnimationTrack(chaseTrack, GetState());
        GetKudzu().SetSprite(GetSpriteAtCurrentFrame());

        //Step the animation.
        StepAnimation();

        if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange()) return;

        Vector3 currentPos = GetKudzu().GetPosition();
        Vector3 targetPosition = TileGrid.NextTilePosTowardsGoal(currentPos, GetTarget().GetPosition());

        //Only update the target position if the Kudzu has reached its last one.
        if (GetKudzu().GetPosition() == GetNextMovePos() || GetNextMovePos() == null)
            SetNextMovePos(targetPosition);

        // Smooth movement
        Vector3 adjusted = new Vector3(GetNextMovePos().Value.x, GetNextMovePos().Value.y, 1);
        GetKudzu().FaceTarget(adjusted);
        float step = GetKudzu().GetMovementSpeed() * Time.deltaTime;
        Vector3 newPosition = Vector3.MoveTowards(currentPos, adjusted, step);
        GetKudzu().SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Runs logic for the Kudzu model's attack state. 
    /// </summary>
    protected override void ExecuteAttackState()
    {
        if (GetState() != KudzuState.ATTACK) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;
        GetKudzu().FaceTarget(GetTarget());

        //Animation Logic.
        SetAnimationDuration(GetKudzu().ATTACK_ANIMATION_DURATION);
        Sprite[] attackTrack = EnemyFactory.GetAttackTrack(
            GetKudzu().TYPE,
            GetKudzu().GetHealthState(),
            GetKudzu().GetDirection());
        SetAnimationTrack(attackTrack, GetState());
        GetKudzu().SetSprite(GetSpriteAtCurrentFrame());

        //Step the animation.
        StepAnimation();

        //Attack Logic
        if (!CanAttack()) return;
        GetTarget().AdjustHealth(-GetKudzu().BONK_DAMAGE);
        GetKudzu().ResetAttackCooldown();
    }

    /// <summary>
    /// Returns true if the Kudzu can bonk a target.
    /// </summary>
    /// <returns>true if the Kudzu can bonk a target; otherwise,
    /// false.</returns>
    protected override bool CanAttack()
    {
        if (!base.CanAttack()) return false; //Cooldown
        if (GetState() != KudzuState.ATTACK) return false; //Not in the attack state.
        if (GetTarget() == null || !GetTarget().Targetable()) return false; //Invalid target
        return true;
    }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    protected override void AgeAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    protected override float GetAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.IDLE) return idleAnimationCounter;
        else if (state == KudzuState.CHASE) return chaseAnimationCounter;
        else if (state == KudzuState.ATTACK) return attackAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    protected override void ResetAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.IDLE) idleAnimationCounter = 0;
        else if (state == KudzuState.CHASE) chaseAnimationCounter = 0;
        else if (state == KudzuState.ATTACK) attackAnimationCounter = 0;
    }

    //---------------------END STATE LOGIC-----------------------//
}
