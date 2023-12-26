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
        INACTIVE, // Not spawned yet.
        SPAWN, // Just spawned,
        IDLE, // Static, nothing to do.
        CHASE, // Pursuing target.
        ATTACK, // Attacking target.
        ESCAPE, // Running back to start.
        EXIT, // Leaving map.
        INVALID // Something went wrong.
    }

    /// <summary>
    /// The maximum number of targets a Kudzu can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

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
    /// Counts the number of seconds in the escape animation; resets
    /// on step.
    /// </summary>
    private float escapeAnimationCounter;

    /// <summary>
    /// Counts the number of seconds until the Kudzu can hop again.
    /// </summary>
    private float hopCooldownCounter;


    /// <summary>
    /// Makes a new KudzuController.
    /// </summary>
    /// <param name="kudzu">The Kudzu Enemy. </param>
    /// <returns>The created KudzuController.</returns>
    public KudzuController(Kudzu kudzu) : base(kudzu) { NUM_KUDZUS++; }

    /// <summary>
    /// Updates the Kudzu controlled by this KudzuController.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();

        if (!ValidModel()) return;
        ExecuteIdleState();
        ExecuteChaseState();
        ExecuteAttackState();
        ExecuteEscapeState();
        ExecuteExitState();
    }

    /// <summary>
    /// Returns this KudzuController's Kudzu model.
    /// </summary>
    /// <returns>this KudzuController's Kudzu model.</returns>
    private Kudzu GetKudzu() { return GetModel() as Kudzu; }

    /// <summary>
    /// Returns the Kudzu's target if it has one. 
    /// </summary>
    /// <returns>the Kudzu's target; null if it doesn't have one.</returns>
    private PlaceableObject GetTarget() { return NumTargets() == 1 ? GetTargets()[0] : null; }

    /// <summary>
    /// Performs logic right before the Kudzu is destroyed.
    /// </summary>
    protected override void OnDestroyModel()
    {

        foreach (PlaceableObject heldTarget in GetHeldTargets())
        {
            // Drop all held Nexii.
            Nexus nexusTarget = heldTarget as Nexus;
            if (nexusTarget != null)
            {
                nexusTarget.Drop();
                int xCoord = TileGrid.PositionToCoordinate(nexusTarget.GetPosition().x);
                int yCoord = TileGrid.PositionToCoordinate(nexusTarget.GetPosition().y);
                TileGrid.PlaceOnTile(new Vector2Int(xCoord, yCoord), nexusTarget, true);
            }
        }
    }




    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two KudzuStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two KudzuStates are equal; otherwise, false.</returns>
    public override bool StateEquals(KudzuState stateA, KudzuState stateB)
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
    /// ATTACK --> ESCAPE : if grabbed a target and running away with it <br></br>
    /// ESCAPE --> CHASE : if dropped target <br></br>
    /// ESCAPE --> EXIT : if made it back to start pos with target<br></br>
    /// CHASE --> IDLE : if target not in chase range <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(KudzuState.IDLE);
            return;
        }


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
                else if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetChaseRange())
                    SetState(KudzuState.CHASE);
                break;
            case KudzuState.CHASE:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                if (GetNextMovePos() != null && GetNextMovePos() != GetKudzu().GetPosition()) break;
                else if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange()
                        && GetKudzu().GetAttackCooldown() <= 0) SetState(KudzuState.ATTACK);
                else if (GetKudzu().DistanceToTarget(GetTarget()) > GetKudzu().GetChaseRange())
                    SetState(KudzuState.IDLE);
                break;
            case KudzuState.ATTACK:
                if (GetTarget() == null) break;
                if (GetAnimationCounter() > 0) break;
                if (NumTargetsHolding() == HOLDING_LIMIT) SetState(KudzuState.ESCAPE);
                else if (GetKudzu().GetAttackCooldown() > 0) SetState(KudzuState.CHASE);
                else if (GetKudzu().DistanceToTarget(GetTarget()) > GetKudzu().GetAttackRange())
                    SetState(KudzuState.CHASE);
                break;
            case KudzuState.ESCAPE:
                if (GetNextMovePos() != null && GetNextMovePos() != GetKudzu().GetPosition()) break;
                if (NumTargetsHolding() == 0) SetState(KudzuState.CHASE);
                else if (GetKudzu().GetPosition() == GetKudzu().GetSpawnPos()) SetState(KudzuState.EXIT);
                break;
            case KudzuState.EXIT:
                break;
            case KudzuState.INVALID:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Runs logic for the Kudzu model's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != KudzuState.IDLE) return;

        //Set up the animation
        GetKudzu().SetAnimationDuration(GetKudzu().MOVE_ANIMATION_DURATION);
        Enemy.EnemyHealthState healthState = GetKudzu().GetHealthState();
        Direction direction = GetKudzu().GetDirection();
        Sprite[] idleTrack = KudzuFactory.GetIdleTrack(direction, healthState);
        if (GetAnimationState() != KudzuState.IDLE) GetKudzu().SetAnimationTrack(idleTrack);
        else GetKudzu().SetAnimationTrack(idleTrack, GetKudzu().CurrentFrame);
        SetAnimationState(KudzuState.IDLE);
        GetKudzu().FaceDirection(Direction.SOUTH);

        //Step the animation.
        StepAnimation();
        GetKudzu().SetSprite(GetKudzu().GetSpriteAtCurrentFrame());
    }

    /// <summary>
    /// Runs logic for the Kudzu model's chase state. 
    /// </summary>
    protected virtual void ExecuteChaseState()
    {
        if (GetState() != KudzuState.CHASE) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;

        // Set up the animation.
        GetKudzu().SetAnimationDuration(GetKudzu().MOVE_ANIMATION_DURATION);
        Enemy.EnemyHealthState healthState = GetKudzu().GetHealthState();
        Direction direction = GetKudzu().GetDirection();
        Sprite[] chaseTrack = KudzuFactory.GetMovementTrack(direction, healthState);
        if (GetAnimationState() != KudzuState.CHASE) GetKudzu().SetAnimationTrack(chaseTrack);
        else GetKudzu().SetAnimationTrack(chaseTrack, GetKudzu().CurrentFrame);
        SetAnimationState(KudzuState.CHASE);

        // Step the animation.
        StepAnimation();
        GetKudzu().SetSprite(GetKudzu().GetSpriteAtCurrentFrame());

        // Move to target.
        if (GetNextMovePos() != null)
        {
            Vector3 adjusted = new Vector3(GetNextMovePos().Value.x, GetNextMovePos().Value.y, 1);
            float step = GetKudzu().GetMovementSpeed() * Time.deltaTime;
            step = Mathf.Clamp(step, 0f, step);
            Vector3 newPosition = Vector3.MoveTowards(GetKudzu().GetPosition(), adjusted, step);
            if (GetKudzu().GetPosition() != adjusted) GetKudzu().FaceTarget(adjusted);
            GetKudzu().SetWorldPosition(newPosition);
        }

        // Decrement hop counter.
        hopCooldownCounter -= Time.deltaTime;
        if (hopCooldownCounter > 0) return;

        // We reached our move target, so we need a new one.
        if (GetNextMovePos() != null && GetKudzu().GetPosition() != GetNextMovePos()) return;
        if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange()) return;
        Vector3 closest = ClosestPositionToTarget(GetTarget());
        SetNextMovePos(TileGrid.NextTilePosTowardsGoal(GetKudzu().GetPosition(), closest));
        hopCooldownCounter = GetKudzu().HOP_COOLDOWN;
    }

    /// <summary>
    /// Runs logic for the Kudzu model's attack state. 
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (GetState() != KudzuState.ATTACK) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;

        //Animation Logic.
        GetKudzu().SetAnimationDuration(GetKudzu().ATTACK_ANIMATION_DURATION);
        Enemy.EnemyHealthState healthState = GetKudzu().GetHealthState();
        Direction direction = GetKudzu().GetDirection();
        Sprite[] attackTrack = KudzuFactory.GetAttackTrack(direction, healthState);
        if (GetAnimationState() != KudzuState.ATTACK) GetKudzu().SetAnimationTrack(attackTrack);
        else GetKudzu().SetAnimationTrack(attackTrack, GetKudzu().CurrentFrame);
        SetAnimationState(KudzuState.ATTACK);

        //Step the animation.
        StepAnimation();
        GetKudzu().SetSprite(GetKudzu().GetSpriteAtCurrentFrame());

        //Attack Logic
        if (!CanAttack()) return;
        GetKudzu().FaceTarget(GetTarget());

        if (CanHoldTarget(GetTarget())) HoldTarget(GetTarget()); // Hold.
        else GetTarget().AdjustHealth(-GetKudzu().BONK_DAMAGE); // Bonk.

        GetKudzu().ResetAttackCooldown();
    }

    /// <summary>
    /// Runs logic for the Kudzu model's escaping state. 
    /// </summary>
    protected virtual void ExecuteEscapeState()
    {
        if (GetState() != KudzuState.ESCAPE) return;

        // Set up the animation.
        GetKudzu().SetAnimationDuration(GetKudzu().MOVE_ANIMATION_DURATION);
        Enemy.EnemyHealthState healthState = GetKudzu().GetHealthState();
        Direction direction = GetKudzu().GetDirection();
        Sprite[] escapeTrack = KudzuFactory.GetMovementTrack(direction, healthState);
        if (GetAnimationState() != KudzuState.ESCAPE) GetKudzu().SetAnimationTrack(escapeTrack);
        else GetKudzu().SetAnimationTrack(escapeTrack, GetKudzu().CurrentFrame);
        SetAnimationState(KudzuState.ESCAPE);

        // Step the animation.
        StepAnimation();
        GetKudzu().SetSprite(GetKudzu().GetSpriteAtCurrentFrame());

        // Move to target.
        if (GetNextMovePos() != null)
        {
            Vector3 adjusted = new Vector3(GetNextMovePos().Value.x, GetNextMovePos().Value.y, 1);
            float step = GetKudzu().GetMovementSpeed() * Time.deltaTime;
            step = Mathf.Clamp(step, 0f, step);
            Vector3 newPosition = Vector3.MoveTowards(GetKudzu().GetPosition(), adjusted, step);
            if (GetKudzu().GetPosition() != adjusted) GetKudzu().FaceTarget(adjusted);
            GetKudzu().SetWorldPosition(newPosition);
        }


        // Decrement hop counter.
        hopCooldownCounter -= Time.deltaTime;
        if (hopCooldownCounter > 0) return;

        // We reached our move target, so we need a new one.
        if (GetNextMovePos() != null && GetKudzu().GetPosition() != GetNextMovePos()) return;
        SetNextMovePos(TileGrid.NextTilePosTowardsGoal(GetKudzu().GetPosition(), GetTarget().GetPosition()));
        hopCooldownCounter = GetKudzu().HOP_COOLDOWN;
    }

    /// <summary>
    /// Runs logic for the Kudzu's Exit state.
    /// </summary>
    protected virtual void ExecuteExitState()
    {
        if (GetState() != KudzuState.EXIT) return;

        GetKudzu().SetEscaped();
        Assert.IsTrue(NumTargetsHolding() > 0, "You need to hold targets to exit.");
    }

    /// <summary>
    /// Returns true if the Kudzu can bonk a target.
    /// </summary>
    /// <returns>true if the Kudzu can bonk a target; otherwise,
    /// false.</returns>
    public override bool CanAttack()
    {
        if (!base.CanAttack()) return false; //Cooldown
        if (GetState() != KudzuState.ATTACK) return false; //Not in the attack state.
        if (GetTarget() == null || !GetTarget().Targetable()) return false; //Invalid target
        return true;
    }

    //---------------------END STATE LOGIC-----------------------//

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.ESCAPE) escapeAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.IDLE) return idleAnimationCounter;
        else if (state == KudzuState.CHASE) return chaseAnimationCounter;
        else if (state == KudzuState.ATTACK) return attackAnimationCounter;
        else if (state == KudzuState.ESCAPE) return escapeAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.IDLE) idleAnimationCounter = 0;
        else if (state == KudzuState.CHASE) chaseAnimationCounter = 0;
        else if (state == KudzuState.ATTACK) attackAnimationCounter = 0;
        else if (state == KudzuState.ESCAPE) escapeAnimationCounter = 0;
    }
}
