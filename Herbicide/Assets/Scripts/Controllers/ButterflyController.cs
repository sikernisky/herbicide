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
    /// The maximum number of targets a Butterfly can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

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
    /// Counts the number of seconds in the backup animation; resets
    /// on step.
    /// </summary>
    private float backupAnimationCounter;

    /// <summary>
    /// State of a Butterfly. 
    /// </summary>
    public enum ButterflyState
    {
        SPAWN,
        IDLE,
        CHASE,
        ATTACK,
        BACKUP,
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
    /// Updates the Butterfly controlled by this ButterflyController.
    /// </summary>
    /// <param name="targets">A complete list of ITargetables in the scene.</param>
    protected override void UpdateMob()
    {
        if (!ValidModel()) return;
        base.UpdateMob();
        ExecuteIdleState();
        ExecuteChaseState();
        ExecuteBackupState();
        ExecuteAttackState();
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

    /// <summary>
    /// Returns the Butterfly's target if it has one. 
    /// </summary>
    /// <returns>the Butterfly's target; null if it doesn't have one.</returns>
    private ITargetable GetTarget() { return NumTargets() == 1 ? GetTargets()[0] : null; }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two ButterflyStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two ButterflyStates are equal; otherwise, false.</returns>
    public override bool StateEquals(ButterflyState stateA, ButterflyState stateB)
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
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(ButterflyState.CHASE);
            return;
        }


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
                else if (GetButterfly().DistanceToTarget(GetTarget()) <= GetButterfly().GetAttackRange()
                        && GetButterfly().GetAttackCooldown() <= 0) { SetState(ButterflyState.BACKUP); }
                else if (GetButterfly().DistanceToTarget(GetTarget()) > GetButterfly().GetChaseRange())
                    SetState(ButterflyState.IDLE);
                break;
            case ButterflyState.ATTACK:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                if (GetAnimationCounter() > 0) break;
                if (GetButterfly().GetAttackCooldown() > 0)
                {
                    if (GetButterfly().DistanceToTarget(GetTarget()) > GetButterfly().GetChaseRange())
                    {
                        SetState(ButterflyState.CHASE);
                    }
                    else SetState(ButterflyState.BACKUP);
                }
                break;
            case ButterflyState.BACKUP:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                float epsilon = .01f;
                float difference = GetButterfly().GetAttackRange() - GetButterfly().DistanceToTarget(GetTarget());
                bool backedUp = Mathf.Abs(difference) <= epsilon;
                if (backedUp && GetButterfly().GetAttackCooldown() <= 0) SetState(ButterflyState.ATTACK);
                else if (backedUp && GetButterfly().GetAttackCooldown() > 0) SetState(ButterflyState.CHASE);
                break;
            case ButterflyState.INVALID:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Butterfly's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != ButterflyState.IDLE) return;

        GetButterfly().SetAnimationDuration(GetButterfly().IDLE_ANIMATION_DURATION);
        Sprite[] idleTrack = DefenderFactory.GetMovementTrack(
            GetButterfly().TYPE,
            Direction.NORTH); //NORTH animation supported.
        GetButterfly().SetAnimationTrack(idleTrack);
        if (GetAnimationState() != ButterflyState.IDLE) GetButterfly().SetAnimationTrack(idleTrack);
        else GetButterfly().SetAnimationTrack(idleTrack, GetButterfly().CurrentFrame);
        SetAnimationState(ButterflyState.IDLE);

        //Step the animation.
        StepAnimation();
        GetButterfly().SetSprite(GetButterfly().GetSpriteAtCurrentFrame());
    }

    /// <summary>
    /// Runs logic relevant to the Buttefly's chasing state.
    /// </summary>
    protected virtual void ExecuteChaseState()
    {
        if (GetState() != ButterflyState.CHASE) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;

        //Set up the animation
        GetButterfly().SetAnimationDuration(GetButterfly().MOVE_ANIMATION_DURATION);
        Sprite[] chaseTrack = DefenderFactory.GetMovementTrack(
            GetButterfly().TYPE,
            Direction.NORTH); //NORTH animation supported.
        if (GetAnimationState() != ButterflyState.CHASE) GetButterfly().SetAnimationTrack(chaseTrack);
        else GetButterfly().SetAnimationTrack(chaseTrack, GetButterfly().CurrentFrame);
        SetAnimationState(ButterflyState.CHASE);

        //Step the animation.
        StepAnimation();
        GetButterfly().SetSprite(GetButterfly().GetSpriteAtCurrentFrame());

        float distance = GetButterfly().DistanceToTarget(GetTarget());
        if (distance <= GetButterfly().GetAttackRange()) return;

        Vector2 targetPosition = GetTarget().GetPosition();
        Vector2 currentPosition = GetButterfly().GetPosition();
        Vector2 direction = (targetPosition - currentPosition).normalized;
        Vector2 newPosition = currentPosition + direction * GetButterfly().GetMovementSpeed() * Time.deltaTime;
        GetButterfly().SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Runs logic relevant to the Buttefly's backing up state.
    /// </summary>
    protected virtual void ExecuteBackupState()
    {
        if (GetState() != ButterflyState.BACKUP) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;

        //Set up the animation
        GetButterfly().SetAnimationDuration(GetButterfly().MOVE_ANIMATION_DURATION);
        Sprite[] backupTrack = DefenderFactory.GetMovementTrack(
            GetButterfly().TYPE,
            Direction.NORTH); //NORTH animation supported.
        if (GetAnimationState() != ButterflyState.BACKUP) GetButterfly().SetAnimationTrack(backupTrack);
        else GetButterfly().SetAnimationTrack(backupTrack, GetButterfly().CurrentFrame);
        SetAnimationState(ButterflyState.BACKUP);

        //Step the animation.
        StepAnimation();
        GetButterfly().SetSprite(GetButterfly().GetSpriteAtCurrentFrame());

        Vector2 targetPosition = GetTarget().GetPosition();
        float radius = GetButterfly().GetAttackRange();

        Vector2 northBackup = new Vector3(
                    targetPosition.x + radius * Mathf.Cos(90f),
                    targetPosition.y + radius * Mathf.Sin(90f),
                    1);

        Vector2 currentPosition = GetButterfly().GetPosition();
        Vector2 direction = (northBackup - currentPosition).normalized;
        Vector2 newPosition = currentPosition + direction * GetButterfly().GetMovementSpeed() * Time.deltaTime;
        GetButterfly().SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Runs logic relevant to the Butterfly's attacking state.
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (GetTarget() == null || !GetTarget().Targetable()) return;
        if (GetState() != ButterflyState.ATTACK) return;

        //Set up the animation
        GetButterfly().SetAnimationDuration(GetButterfly().ATTACK_ANIMATION_DURATION);
        Sprite[] attackTrack = DefenderFactory.GetAttackTrack(
            GetButterfly().TYPE,
            Direction.NORTH); //NORTH animation supported.
        if (GetAnimationState() != ButterflyState.ATTACK) GetButterfly().SetAnimationTrack(attackTrack);
        else GetButterfly().SetAnimationTrack(attackTrack, GetButterfly().CurrentFrame);
        SetAnimationState(ButterflyState.ATTACK);

        //Step the animation.
        StepAnimation();
        GetButterfly().SetSprite(GetButterfly().GetSpriteAtCurrentFrame());

        if (!CanAttack()) return;
        GameObject bombPrefab = ProjectileFactory.GetProjectilePrefab(Projectile.ProjectileType.BOMB);
        Assert.IsNotNull(bombPrefab);
        GameObject clonedBomb = GameObject.Instantiate(bombPrefab);
        Assert.IsNotNull(clonedBomb);
        Bomb clonedBombComp = clonedBomb.GetComponent<Bomb>();
        Vector3 targetPosition = GetTarget().GetAttackPosition();
        BombController bombController =
            new BombController(clonedBombComp, GetButterfly().GetPosition(), targetPosition);
        AddController(bombController);

        GetButterfly().ResetAttackCooldown();
    }

    /// <summary>
    /// Returns true if the Butterfly can lob a bomb.
    /// </summary>
    /// <returns>true if the Butterfly can lob a bomb; 
    /// otherwise, false.</returns>
    public override bool CanAttack()
    {
        if (!base.CanAttack()) return false; //Cooldown
        if (GetTarget() == null || !GetTarget().Targetable()) return false;
        if (GetState() != ButterflyState.ATTACK) return false;
        return true;
    }

    //---------------------END STATE LOGIC-----------------------//

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        ButterflyState state = GetState();
        if (state == ButterflyState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == ButterflyState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == ButterflyState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == ButterflyState.BACKUP) backupAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        ButterflyState state = GetState();
        if (state == ButterflyState.IDLE) return idleAnimationCounter;
        else if (state == ButterflyState.CHASE) return chaseAnimationCounter;
        else if (state == ButterflyState.ATTACK) return attackAnimationCounter;
        else if (state == ButterflyState.BACKUP) return backupAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        ButterflyState state = GetState();
        if (state == ButterflyState.IDLE) idleAnimationCounter = 0;
        else if (state == ButterflyState.CHASE) chaseAnimationCounter = 0;
        else if (state == ButterflyState.ATTACK) attackAnimationCounter = 0;
        else if (state == ButterflyState.BACKUP) backupAnimationCounter = 0;
    }

}
