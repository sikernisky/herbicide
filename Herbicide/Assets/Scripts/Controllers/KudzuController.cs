using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using System.Linq;
using System.Net.Http.Headers;

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
        EXITING, // Leaving map.
        DEAD, // Dead.
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
    /// Counts the number of seconds in the spawn animation; resets
    /// on step.
    /// </summary>
    private float spawnAnimationCounter;

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
    /// Counts the number of seconds in the exiting animation; resets
    /// on step.
    /// </summary>
    private float exitingAnimationCounter;

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
        ExecuteInactiveState();
        ExecuteSpawnState();
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
    private PlaceableObject GetTarget() { return NumTargets() == 1 ? GetTargets()[0] as PlaceableObject : null; }

    /// <summary>
    /// Drops a resource at the Kudzu's resource drop rate.
    /// </summary>
    protected override void DropDeathLoot()
    {
        if (DroppedDeathLoot()) return;
        if (GetKudzu().Exited()) return;

        Dew dew = DewFactory.GetDewPrefab().GetComponent<Dew>();
        Vector3 lootPos = GetKudzu().Exited() ? GetKudzu().GetExitPos() : GetKudzu().GetPosition();
        int value = GetKudzu().CURRENCY_VALUE_ON_DEATH;
        DewController dewController = new DewController(dew, lootPos, value);
        AddModelControllerForExtrication(dewController);
        base.DropDeathLoot();
    }

    /// <summary>
    /// Returns the spawn position of the Kudzu when in a NexusHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the NexusHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Kudzu when in a NexusHole.</returns>
    protected override Vector3 NexusHoleSpawnPos(Vector3 originalSpawnPos)
    {
        originalSpawnPos.y -= 2;
        return originalSpawnPos;
    }

    /// <summary>
    /// Returns true if this controller's Kudzu should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Kudzu should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel()
    {
        if (!GetEnemy().Spawned()) return true;

        HashSet<KudzuState> immuneStates = new HashSet<KudzuState>()
        {
            KudzuState.INACTIVE,
            KudzuState.SPAWN,
            KudzuState.EXITING,
            KudzuState.INVALID
        };

        bool isImmune = immuneStates.Contains(GetState());
        if (!isImmune && !TileGrid.OnWalkableTile(GetEnemy().GetPosition())) return false;
        else if (GetEnemy().Dead()) return false;
        else if (GetEnemy().Exited()) return false;

        return true;
    }

    /// <summary>
    /// Returns the Kudzu prefab to the KudzuFactory singleton.
    /// </summary>
    public override void DestroyModel()
    {
        KudzuFactory.ReturnKudzuPrefab(GetKudzu().gameObject);
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
                DecideFSMFromInactive();
                break;
            case KudzuState.SPAWN:
                DecideFSMFromSpawn();
                break;
            case KudzuState.IDLE:
                DecideFSMFromIdle();
                break;
            case KudzuState.CHASE:
                DecideFSMFromChase();
                break;
            case KudzuState.ATTACK:
                DecideFSMFromAttack();
                break;
            case KudzuState.ESCAPE:
                DecideFSMFromEscape();
                break;
            case KudzuState.EXITING:
                DecideFSMFromExit();
                break;
            case KudzuState.INVALID:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Runs logic for the Kudzu model's Inactive state.
    /// </summary>
    protected virtual void ExecuteInactiveState()
    {
        if (GetState() != KudzuState.INACTIVE) return;
    }

    /// <summary>
    /// Runs logic for the Kudzu model's Spawn state.
    /// </summary>
    protected virtual void ExecuteSpawnState()
    {
        if (GetState() != KudzuState.SPAWN) return;

        GetKudzu().SetAnimationDuration(GetKudzu().MOVE_ANIMATION_DURATION);
        Enemy.EnemyHealthState healthState = GetKudzu().GetHealthState();
        Direction direction = GetKudzu().GetDirection();
        Sprite[] spawnTrack = KudzuFactory.GetSpawnTrack(direction, healthState);
        if (GetAnimationState() != KudzuState.SPAWN) GetKudzu().SetAnimationTrack(spawnTrack);
        else GetKudzu().SetAnimationTrack(spawnTrack, GetKudzu().CurrentFrame);
        SetAnimationState(KudzuState.SPAWN);

        // Step the animation.
        StepAnimation();
        GetKudzu().SetSprite(GetKudzu().GetSpriteAtCurrentFrame());

        PopOutOfMovePos(GetKudzu().GetMovementSpeed(), NexusHoleSpawnPos(GetKudzu().GetSpawnPos()));
        GetKudzu().FaceDirection(Direction.SOUTH);
        if (!ReachedMovementTarget()) return;
        SetNextMovePos(GetKudzu().GetSpawnPos());

    }

    /// <summary>
    /// Runs logic for the Kudzu model's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != KudzuState.IDLE) return;

        // Set up the animation
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
        MoveLinearlyTowardsMovePos(GetKudzu().GetMovementSpeed());

        // Decrement hop counter.
        hopCooldownCounter -= Time.deltaTime;
        if (hopCooldownCounter > 0) return;

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange()) return;
        Vector3 closest = ClosestPositionToTarget(GetTarget());
        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetKudzu().GetPosition(), closest);
        SetNextMovePos(nextMove);
        hopCooldownCounter = GetKudzu().HOP_COOLDOWN;
    }

    /// <summary>
    /// Runs logic for the Kudzu model's attack state. 
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (GetState() != KudzuState.ATTACK) return;

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

        //Attack Logic : Only if target is valid.
        bool validTarget = GetTarget() != null && GetTarget().Targetable();
        if (!CanAttack()) return;
        if (!validTarget) return;

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
        if (!ValidModel()) return;
        if (GetTarget() == null) return;

        // Set up the animation.
        GetKudzu().SetAnimationDuration(GetKudzu().MOVE_ANIMATION_DURATION);
        Enemy.EnemyHealthState healthState = GetKudzu().GetHealthState();
        Direction direction = GetKudzu().GetDirection();
        Sprite[] escapeTrack = KudzuFactory.GetEscapeTrack(direction, healthState);
        if (GetAnimationState() != KudzuState.ESCAPE) GetKudzu().SetAnimationTrack(escapeTrack);
        else GetKudzu().SetAnimationTrack(escapeTrack, GetKudzu().CurrentFrame);
        SetAnimationState(KudzuState.ESCAPE);

        // Step the animation.
        StepAnimation();
        GetKudzu().SetSprite(GetKudzu().GetSpriteAtCurrentFrame());

        // Move to target.
        if (TileGrid.IsNexusHole(GetNextMovePos())) MoveParabolicallyTowardsMovePos();
        else MoveLinearlyTowardsMovePos(GetKudzu().GetMovementSpeed());

        // Decrement hop counter.
        hopCooldownCounter -= Time.deltaTime;
        if (hopCooldownCounter > 0) return;

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        SetNextMovePos(TileGrid.NextTilePosTowardsGoal(
            GetKudzu().GetPosition(), GetTarget().GetPosition()));
        hopCooldownCounter = GetKudzu().HOP_COOLDOWN;
    }

    /// <summary>
    /// Runs logic for the Kudzu's Exit state.
    /// </summary>
    protected virtual void ExecuteExitState()
    {
        if (GetState() != KudzuState.EXITING) return;
        if (GetKudzu().Exited()) return;
        NexusHole nexusHoleTarget = GetTarget() as NexusHole;
        if (nexusHoleTarget == null) return;
        if (nexusHoleTarget.Filled()) return;

        Assert.IsTrue(GetTarget() as NexusHole != null, "exit target needs to be a NH.");
        Vector3 nexusHolePosition = GetTarget().GetPosition();
        Vector3 jumpPosition = nexusHolePosition;
        jumpPosition.y -= 2;

        if (!GetKudzu().IsExiting() && !GetKudzu().Exited()) GetKudzu().SetExiting(nexusHolePosition);

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
        FallIntoMovePos(GetKudzu().GetMovementSpeed(), 3f);

        if (!ReachedMovementTarget()) return;
        SetNextMovePos(jumpPosition);
        if (GetKudzu().IsExiting() && GetKudzu().GetPosition() == jumpPosition)
            GetKudzu().SetExited();
        Assert.IsTrue(NumTargetsHolding() > 0, "You need to hold targets to exit.");
    }

    /// <summary>
    /// Examines the current INACTIVE state and decides which state
    /// it should switch to next.
    /// </summary>
    private void DecideFSMFromInactive()
    {
        if (!ValidModel()) return;

        // TO SPAWN
        if (GetKudzu().Spawned()) SetState(KudzuState.SPAWN);
    }

    /// <summary>
    /// Examines the current SPAWN state and decides which state
    /// it should switch to next.
    /// </summary>
    private void DecideFSMFromSpawn()
    {
        if (!ValidModel()) return;
        if (!ReachedMovementTarget()) return;

        // TO IDLE
        SetState(KudzuState.IDLE);
    }

    /// <summary>
    /// Examines the current IDLE state and decides which state
    /// it should switch to next.
    /// </summary>
    private void DecideFSMFromIdle()
    {
        if (!ValidModel()) return;

        // TO CHASE
        bool validTarget = GetTarget() != null && GetTarget().Targetable();
        bool inChaseRange = validTarget ? GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetChaseRange() : false;
        if (inChaseRange) SetState(KudzuState.CHASE);

        // TO ESCAPE
        if (ShouldEscape()) SetState(KudzuState.ESCAPE);
    }

    /// <summary>
    /// Examines the current CHASE state and decides which state
    /// it should switch to next.
    /// </summary>
    private void DecideFSMFromChase()
    {
        if (!ValidModel()) return;
        if (!ReachedMovementTarget()) return;

        // TO ATTACK
        bool validTarget = GetTarget() != null && GetTarget().Targetable();
        bool inAttackRange = validTarget ? GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange() : false;
        bool readyToAttack = validTarget ? GetKudzu().GetAttackCooldown() <= 0 : false;
        if (readyToAttack && validTarget && inAttackRange) SetState(KudzuState.ATTACK);

        // TO IDLE
        bool inChaseRange = validTarget ? GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetChaseRange() : false;
        if (!validTarget || !inChaseRange) SetState(KudzuState.IDLE);

        // TO ESCAPE
        if (ShouldEscape()) SetState(KudzuState.ESCAPE);
    }

    /// <summary>
    /// Examines the current ATTACK state and decides which state
    /// it should switch to next.
    /// </summary>
    private void DecideFSMFromAttack()
    {
        if (!ValidModel()) return;

        // FINISH ANIMATION FIRST
        if (GetAnimationCounter() > 0) return;

        // TO IDLE
        bool attackOnCooldown = GetKudzu().GetAttackCooldown() > 0;
        bool validTarget = GetTarget() != null && GetTarget().Targetable();
        bool inAttackRange = validTarget ? GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange() : false;
        if (!validTarget || !inAttackRange || attackOnCooldown) SetState(KudzuState.IDLE);

        // TO ESCAPE
        if (ShouldEscape()) SetState(KudzuState.ESCAPE);
    }

    /// <summary>
    /// Examines the current ESCAPE state and decides which state
    /// it should switch to next.
    /// </summary>
    private void DecideFSMFromEscape()
    {
        if (!ValidModel()) return;
        if (!ReachedMovementTarget()) return;

        // TO IDLE
        if (!ShouldEscape()) SetState(KudzuState.IDLE);

        // TO EXIT
        if (ShouldExit()) SetState(KudzuState.EXITING);
    }

    /// <summary>
    /// Examines the current EXIT state and decides which state
    /// it should switch to next.
    /// </summary>
    private void DecideFSMFromExit()
    {
        if (!ValidModel()) return;
    }


    //---------------------END STATE LOGIC-----------------------//

    /// <summary>
    /// Returns true if the Enemy can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns></returns>
    protected override bool CanTarget(Model target)
    {
        if (!GetKudzu().Spawned()) return false;
        if (GetState() == KudzuState.SPAWN) return false;

        Nexus nexusTarget = target as Nexus;
        NexusHole nexusHoleTarget = target as NexusHole;

        // If escaping, only target NexusHoles.
        if (GetState() == KudzuState.ESCAPE || GetState() == KudzuState.EXITING)
        {
            if (nexusHoleTarget == null) return false;
            if (!nexusHoleTarget.Targetable()) return false;
            if (nexusHoleTarget.Filled()) return false;
            if (nexusHoleTarget.PickedUp()) return false;
            if (!GetPathfindingCache().IsReachable(nexusHoleTarget)) return false;
            if (!ClosestValidNexusHole(nexusHoleTarget)) return false;

            return true;
        }

        // If not escaping, only target Nexii.
        else
        {
            if (nexusTarget == null) return false;
            if (!nexusTarget.Targetable()) return false;
            if (nexusTarget.PickedUp()) return false;
            if (nexusTarget.CashedIn()) return false;
            if (!GetPathfindingCache().IsReachable(nexusTarget)) return false;
            if (!ClosestValidNexus(nexusTarget)) return false;

            return true;
        }

        //// If it's a Tree, check to see if there are no targetable nexii first.
        //if (treeTarget != null)
        //{
        //    foreach (Model model in GetAllModels())
        //    {
        //        Nexus nexusPlaceable = model as Nexus;
        //        NexusHole nexusHolePlaceable = model as NexusHole;

        //        // YEAH BITCH!! RECURSION!!!! SCIENCE MR. WHITE!!
        //        if (nexusPlaceable != null && CanTarget(nexusPlaceable)) return false;
        //        if (nexusHolePlaceable != null && CanTarget(nexusHolePlaceable)) return false;
        //    }
        //    if (!GetPathfindingCache().IsReachable(treeTarget)) return false;
        //    return true;
        //}

        //if (GetState() == KudzuState.ESCAPE || GetState() == KudzuState.EXITING)
        //{
        //    if (nexusHoleTarget == null) return false;
        //    if (!nexusHoleTarget.Targetable()) return false;
        //    if (nexusHoleTarget.Filled()) return false;
        //    if (nexusHoleTarget.PickedUp()) return false;
        //    if (!GetPathfindingCache().IsReachable(nexusHoleTarget)) return false;
        //    if (!ClosestValidNexusHole(nexusHoleTarget)) return false;

        //    return true;
        //}

        //else
        //{
        //    if (nexusTarget == null) return false;
        //    if (!nexusTarget.Targetable()) return false;
        //    if (nexusTarget.PickedUp()) return false;
        //    if (nexusTarget.CashedIn()) return false;
        //    if (!GetPathfindingCache().IsReachable(nexusTarget)) return false;
        //    if (!ClosestValidNexus(nexusTarget)) return false;

        //    return true;
        //}

        throw new System.Exception("Something wrong happened.");
    }

    /// <summary>
    /// Returns true if the Kudzu's position is at the spot it
    /// is trying to move towards.
    /// </summary>
    /// <returns>true if the Kudzu made it to its movement destination;
    /// otherwise, false. </returns>
    private bool ReachedMovementTarget()
    {
        if (GetNextMovePos() == null) return true;

        Vector3 nextMovePos = new Vector3(
            GetNextMovePos().Value.x,
            GetNextMovePos().Value.y,
            1
        );

        return Vector3.Distance(GetKudzu().GetPosition(), nextMovePos) < 0.1f;
    }

    /// <summary>
    /// Returns true if the Kudzu should stop trying to steal 
    /// targets and escape. This is when the Kudzu has grabbed
    /// as many targets as possible and there are no more it
    /// can pick up. 
    /// </summary>
    /// <returns>true if the Kudzu should escape; otherwise, false.</returns>
    protected virtual bool ShouldEscape()
    {
        if (NumTargetsHolding() >= HOLDING_LIMIT) return true;
        if (NumTargetsHolding() == 0) return false;

        if (GetTarget() != null) return false;
        if (GetAllModels().Count == 0) return false;

        foreach (PlaceableObject placeableObject in GetAllModels())
        {
            Nexus nexusTarget = placeableObject as Nexus;
            if (nexusTarget == null) continue;
            if (nexusTarget.PickedUp()) continue;
            if (nexusTarget.CashedIn()) continue;
            if (!GetPathfindingCache().IsReachable(nexusTarget)) continue;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Returns true if the Kudzu should drop its last Nexus
    /// and die. 
    /// /// </summary>
    /// <returns>true if the Kudzu should escape; otherwise, false.</returns>
    protected virtual bool ShouldExit()
    {
        NexusHole nexusHoleTarget = GetTarget() as NexusHole;
        if (nexusHoleTarget == null) return false;
        if (nexusHoleTarget.Filled()) return false;
        Vector3 kudzuPos = GetKudzu().GetPosition();
        Vector3 holePos = GetTarget().GetPosition();
        if (Vector3.Distance(kudzuPos, holePos) > 0.1f) return false;

        return true;
    }

    /// <summary>
    /// Returns true if a given Nexus is the closest Nexus to the
    /// Kudzu that it can target.
    /// </summary>
    /// <param name="nexus">The Nexus to check.</param>
    /// <returns>true if a given Nexus is the closest Nexus to the
    /// Kudzu that it can target; otherwise, false. </returns>
    private bool ClosestValidNexus(Nexus nexus)
    {
        Assert.IsNotNull(nexus);
        if (nexus.CashedIn()) return false;
        if (nexus.PickedUp()) return false;

        List<Nexus> nexii = new List<Nexus>();
        foreach (Model model in GetAllModels())
        {
            Nexus nexusObject = model as Nexus;
            if (nexusObject != null && !nexusObject.CashedIn() && !nexusObject.PickedUp())
                nexii.Add(nexusObject);
        }

        nexii.Sort((n1, n2) =>
            n1.DistanceToTarget(GetKudzu()).CompareTo(n2.DistanceToTarget(GetKudzu())));

        return nexii[0] == nexus;
    }

    /// <summary>
    /// Returns true if a given NexusHole is the closest NexusHole to the
    /// Kudzu that it can target.
    /// </summary>
    /// <param name="nexusHole">The NexusHole to check.</param>
    /// <returns>true if a given NexusHole is the closest NexusHole to the
    /// Kudzu that it can target; otherwise, false. </returns>
    private bool ClosestValidNexusHole(NexusHole nexusHole)
    {
        Assert.IsNotNull(nexusHole);

        List<NexusHole> holes = new List<NexusHole>();
        foreach (Model model in GetAllModels())
        {
            NexusHole nexusHoleObject = model as NexusHole;
            if (nexusHoleObject != null && !nexusHoleObject.Filled())
                holes.Add(nexusHoleObject);
        }

        holes.Sort((n1, n2) =>
            n1.DistanceToTarget(GetKudzu()).CompareTo(n2.DistanceToTarget(GetKudzu())));

        return holes[0] == nexusHole;
    }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.SPAWN) spawnAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.ESCAPE) escapeAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.EXITING) exitingAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.SPAWN) return spawnAnimationCounter;
        else if (state == KudzuState.IDLE) return idleAnimationCounter;
        else if (state == KudzuState.CHASE) return chaseAnimationCounter;
        else if (state == KudzuState.ATTACK) return attackAnimationCounter;
        else if (state == KudzuState.ESCAPE) return escapeAnimationCounter;
        else if (state == KudzuState.EXITING) return exitingAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.SPAWN) spawnAnimationCounter = 0;
        else if (state == KudzuState.IDLE) idleAnimationCounter = 0;
        else if (state == KudzuState.CHASE) chaseAnimationCounter = 0;
        else if (state == KudzuState.ATTACK) attackAnimationCounter = 0;
        else if (state == KudzuState.ESCAPE) escapeAnimationCounter = 0;
        else if (state == KudzuState.EXITING) exitingAnimationCounter = 0;
    }
}
