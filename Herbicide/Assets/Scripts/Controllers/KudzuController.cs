using System.Collections;
using System.Collections.Generic;
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
    /// Makes a new KudzuController.
    /// </summary>
    /// <param name="kudzu">The Kudzu Enemy. </param>
    /// <param name="spawnTime">The time at which the Kudzu Enemy spawns. </param>
    /// <param name="spawnCoords">The (X, Y) coordinates where the Kudzu spawns. </param>
    /// <returns>The created KudzuController.</returns>
    public KudzuController(Kudzu kudzu, float spawnTime, Vector2 spawnCoords)
    : base(kudzu, spawnTime, spawnCoords) { NUM_KUDZUS++; }

    /// <summary>
    /// Updates the Kudzu controlled by this KudzuController.
    /// </summary>
    /// <param name="targets">A complete list of ITargetables in the scene.</param>
    protected override void UpdateMob(List<ITargetable> targets)
    {
        base.UpdateMob(targets);

        //STATE LOGIC (UpdateFSM() is called in base())
        ExecuteAttackState();
        ExecuteChaseState();
    }

    /// <summary>
    /// Sets the Kuduz model's next movement position.
    /// </summary>
    protected override void DetermineEnemyMovePos()
    {
        switch (GetState())
        {
            case KudzuState.INACTIVE:
                GetKudzu().SetNextMovePos(GetKudzu().GetPosition());
                break;
            case KudzuState.SPAWN:
                GetKudzu().SetNextMovePos(GetKudzu().GetPosition());
                break;
            case KudzuState.IDLE:
                GetKudzu().SetNextMovePos(GetKudzu().GetPosition());
                break;
            case KudzuState.CHASE:
                GetKudzu().SetNextMovePos(
                        TileGrid.NextTilePosTowardsGoal(
                            GetKudzu().GetPosition(),
                            GetTarget().GetPosition())
                        );
                break;
            case KudzuState.ATTACK:
                GetKudzu().SetNextMovePos(GetKudzu().GetPosition());
                break;
            default:
                GetKudzu().SetNextMovePos(GetKudzu().GetPosition());
                break;
        }
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
    /// 
    /// This method is also responsible for choosing the correct animation
    /// based off the KudzuState. 
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
                case KudzuState.SPAWN:
                    break;
                case KudzuState.ATTACK:
                    animationTime = GetKudzu().ATTACK_ANIMATION_DURATION;
                    Sprite[] attackTrack = EnemyFactory.GetAttackTrack(
                        GetKudzu().TYPE,
                        GetKudzu().GetHealthState(),
                        GetKudzu().GetDirection());
                    SetAnimationTrack(attackTrack);
                    break;
                case KudzuState.CHASE:
                    animationTime = GetKudzu().MOVE_ANIMATION_DURATION;
                    Sprite[] chaseTrack = EnemyFactory.GetMovementTrack(
                        GetKudzu().TYPE,
                        GetKudzu().GetHealthState(),
                        GetKudzu().GetDirection());
                    SetAnimationTrack(chaseTrack);
                    break;
                case KudzuState.IDLE:
                    animationTime = GetKudzu().IDLE_ANIMATION_DURATION;
                    Sprite[] idleTrack = EnemyFactory.GetIdleTrack(
                        GetKudzu().TYPE,
                        GetKudzu().GetHealthState(),
                        GetKudzu().GetDirection());
                    break;
                case KudzuState.INACTIVE:
                    break;
                default:
                    throw new System.Exception("Animation not supported for " + GetState() + ".");
            }

            //(2): Flip the flipbook.
            if (HasAnimationTrack() && animationTime > 0)
            {
                float waitTime = animationTime / (GetFrameCount() + 1);
                GetKudzu().SetSprite(GetSpriteAtCurrentFrame());
                yield return new WaitForSeconds(waitTime);
                NextFrame();
            }
            else yield return null;
        }
    }

    /// <summary>
    /// Handles all collisions between this controller's Kudzu
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other)
    {
        Debug.Log(-1);
        if (other == null) return;

        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            SoundController.PlaySoundEffect("kudzuHit");
            // Debug.Log(GetKudzu().GetHealth());
            GetKudzu().AdjustHealth(-projectile.GetDamage());
            // Debug.Log(GetKudzu().GetHealth());
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
    /// Updates the state of this KudzuController's Kudzu model.
    /// The transitions are: <br></br>
    /// 
    /// INACTIVE --> SPAWN : if ready to spawn <br></br>
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> CHASE : if target & not in attack range <br></br>
    /// CHASE --> ATTACK : if target & in attack range <br></br>
    /// ATTACK --> CHASE : if target & not in attack range <br></br>
    /// ATTACK --> CHASE : if target & not in attack range <br></br>
    /// CHASE --> IDLE : if no target <br></br>
    /// </summary>
    protected override void UpdateStateFSM()
    {
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
                if (GetTarget() != null) SetState(KudzuState.CHASE);
                break;
            case KudzuState.CHASE:
                if (GetTarget() == null) SetState(KudzuState.IDLE);
                else if (GetKudzu().DistanceToTarget(GetTarget()) <= GetKudzu().GetAttackRange())
                    SetState(KudzuState.ATTACK);
                break;
            case KudzuState.ATTACK:
                if (GetTarget() == null) SetState(KudzuState.IDLE);
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
    protected override void ExecuteIdleState() { return; }

    /// <summary>
    /// Runs logic for the Kudzu model's attack state. 
    /// </summary>
    private void ExecuteChaseState()
    {
        if (GetState() != KudzuState.CHASE) return;
        Vector3 moveTarget = GetKudzu().GetNextMovePos();
        if (GetKudzu().GetHopCooldown() <= 0 && !GetKudzu().IsHopping())
        {
            GetKudzu().FaceTarget(GetTarget());
            SceneController.BeginCoroutine(CoHop(moveTarget));
        }
    }

    /// <summary>
    /// Runs logic for the Kudzu model's attack state. 
    /// </summary>
    private void ExecuteAttackState() { if (GetState() != KudzuState.ATTACK) return; }

    //---------------------END STATE LOGIC-----------------------//

    /// <summary>
    /// Coroutine to make this KudzuController's Kudzu model hop. 
    /// </summary>
    /// <param name="targetPosition">The position to hop towards.</param>
    /// <returns>A reference to the coroutine.</returns>
    private IEnumerator CoHop(Vector3 targetPosition)
    {
        GetKudzu().SetHopping(true);
        GetKudzu().ResetHopCooldown();
        float progress = 0f;
        Vector3 startPosition = GetKudzu().GetPosition();
        while (progress < GetKudzu().MOVE_ANIMATION_DURATION)
        {
            float t = progress / GetKudzu().MOVE_ANIMATION_DURATION;
            Vector3 step = Vector3.MoveTowards(
                startPosition,
                targetPosition,
                t * Vector3.Distance(startPosition, targetPosition)
            );
            GetKudzu().SetWorldPosition(step);
            progress += Time.deltaTime;
            yield return null;
        }
        GetKudzu().SetWorldPosition(targetPosition);
        while (GetKudzu().GetHopCooldown() > 0)
        {
            GetKudzu().DecrementHopCooldown();
            yield return null;
        }
        GetKudzu().SetHopping(false);
    }
}
