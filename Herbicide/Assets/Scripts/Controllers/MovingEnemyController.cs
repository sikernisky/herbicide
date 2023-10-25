using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a MovingEnemy.
/// </summary>
public class MovingEnemyController : EnemyController
{

    /// <summary>
    /// Initializes this MovingEnemyController with the MovingEnemy it controls.
    /// </summary>
    /// <param name="movingEnemy">The MovingEnemy this MovingEnemyController controls.</param>
    /// <param name="spawnTime">when the MovingEnemy should spawn in the level. </param>
    /// <param name="spawnCoords">where this MovingEnemy should spawn</param>
    public MovingEnemyController(MovingEnemy movingEnemy, float spawnTime, Vector2 spawnCoords)
        : base(movingEnemy, spawnTime, spawnCoords)
    {
    }

    /// <summary>
    /// Updates the MovingEnemy controlled by this Controller.
    /// </summary>
    /// <param name="targets">All potential targets for this MovingEnemy</param>
    /// <param name="dt">Current game time.</param>
    public override void UpdateEnemy(List<ITargetable> targets, float dt)
    {
        base.UpdateEnemy(targets, dt);

        if (targets == null) return;
        if (!ValidEnemy()) return;

        MovingEnemy enemy = GetEnemy() as MovingEnemy;
        Assert.IsNotNull(enemy);

        if (GetState() == EnemyState.CHASE) enemy.MoveTo(CalculateNextMovementPosition());
        else if (GetState() == EnemyState.ATTACK) enemy.Attack(GetTarget());
    }

    /// <summary>
    /// Returns the Vector3 position that represents where this MovingEnemyController's
    /// MovingEnemy should move towards. This method relies that on the precondition
    /// that there is indeed a valid path towards its target.
    /// </summary>
    private Vector3 CalculateNextMovementPosition()
    {
        if (!ValidEnemy()) return GetEnemy().GetPosition();
        if (!ValidTarget()) return GetEnemy().GetPosition();

        return TileGrid.NextTilePosTowardsGoal(
            GetEnemy().GetPosition(),
            GetTarget().GetPosition()
        );
    }

    /// <summary>
    /// Returns true if the Enemy controlled by this MovingEnemyController can
    /// target some ITargetable.
    /// <param name="target">the ITargetable to check.</param>
    /// <returns>true if this MovingEnemyController's MovingEnemy can
    /// target the ITargetable; otherwise, false. </returns>
    protected override bool CanTarget(ITargetable t)
    {
        if (!base.CanTarget(t)) return false;

        Vector3 nextTilePos = TileGrid.NextTilePosTowardsGoal(
            GetEnemy().GetPosition(),
            t.GetPosition()
        );

        //No path and not in attack range
        if (nextTilePos == GetEnemy().GetPosition() &&
            !GetEnemy().InAttackRange(t)) return false;

        return true;
    }
    /// <summary>
    /// Runs all code relevant to the Enemy's chasing state.
    /// </summary>
    private void ChaseState()
    {
        if (!ValidEnemy()) return;
        Assert.IsNotNull(GetTarget(), "Cannot chase a null target.");

        if (GetState() != EnemyState.CHASE) return;
    }

    /// <summary>
    /// Updates this MovingEnemyController's state.
    /// </summary>
    protected override void UpdateState()
    {
        if (!ValidEnemy()) return;

        float distanceToTarget = GetEnemy().DistanceToTarget(GetTarget());

        switch (GetState())
        {
            case EnemyState.INACTIVE:
                break;
            case EnemyState.SPAWN:
                SetState(EnemyState.CHASE);
                break;
            case EnemyState.CHASE:

                if (distanceToTarget <= GetEnemy().GetAttackRange())
                {
                    SetState(EnemyState.ATTACK);
                }
                break;
            case EnemyState.ATTACK:
                if (distanceToTarget > GetEnemy().GetAttackRange())
                {
                    SetState(EnemyState.CHASE);
                }
                break;
            default:
                Assert.IsTrue(false);//should not get here
                break;
        }
    }
}
