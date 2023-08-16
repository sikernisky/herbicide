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
    /// The position that the MovingEnemyController's MovingEnemy is trying to
    /// move to.
    /// </summary>
    private Vector3 movementTarget;

    /// <summary>
    /// Type of movement. 
    /// </summary>
    private enum MovementType
    {
        HOP,
        WALK
    }

    /// <summary>
    /// Initializes this MovingEnemyController with the MovingEnemy it controls.
    /// </summary>
    /// <param name="movingEnemy">The MovingEnemy this MovingEnemyController controls.</param>
    /// <param name="spawnPosition">The position at which to spawn the MovingEnemy. </param>
    ///<param name="id">the unique ID of this EnemyController</param>
    public MovingEnemyController(MovingEnemy movingEnemy) : base(movingEnemy)
    {
    }

    /// <summary>
    /// Updates the MovingEnemy controlled by this Controller.
    /// </summary>
    /// <param name="targets">All potential targets for this MovingEnemy</param>
    public override void UpdateEnemy(List<ITargetable> targets)
    {
        base.UpdateEnemy(targets);
        if (targets == null) return;
        if (!ValidEnemy()) return;
        MovingEnemy enemy = GetEnemy() as MovingEnemy;
        Assert.IsNotNull(enemy);

        SelectMovementTarget();

        if (GetState() == EnemyState.CHASE) enemy.MoveTo(GetMovementTarget());
        else if (GetState() == EnemyState.ATTACK) enemy.Attack(GetTarget());
    }

    //here, logic for target priority (for enemies)

    /// <summary>
    /// Sets position to which the MovingEnemy should select as its
    /// movement target.
    /// </summary>
    private void SelectMovementTarget()
    {
        if (!ValidEnemy()) return;
        if (!ValidTarget()) return;

        //Default to be itself
        else movementTarget = new Vector3(
            GetEnemy().GetPosition().x,
            GetEnemy().GetPosition().y,
            1
            );
    }

    /// <summary>
    /// Returns a Vector3 world position that represents the furthest position
    /// at which an Enemy can attack its target. Since the attack range is represented
    /// as a circle around an Enemy, this world position is often in between Tiles.
    /// That is fine, since pathfinding algorithms solve this issue. 
    ///  </summary>
    /// <param name="target">The target to attack.</param>
    /// <returns>the furthest position at which an Enemy can attack its target.</returns>
    private Vector3 GetEdgePositionInAttackRange(Vector3 target)
    {
        Vector3 enemyPos = GetEnemy().GetPosition();
        float attackRange = GetEnemy().GetAttackRange();

        Vector3 directionToTarget = (target - enemyPos).normalized;
        Vector3 edgePosition = enemyPos + directionToTarget * attackRange;

        return new Vector3(
            edgePosition.x,
            edgePosition.y,
            1
        );
    }

    /// <summary>
    /// Returns the position at which the MovingEnemy controlled by this MovingEnemyController
    /// is targeting / moving to.
    /// </summary>
    /// <returns>the position at which the MovingEnemy controlled by this 
    /// MovingEnemyController
    /// is targeting / moving to.</returns>
    private Vector3 GetMovementTarget()
    {
        if (!ValidTarget() || !ValidEnemy()) return default;

        return movementTarget;
    }

    /// <summary>
    /// Returns true if the MovingEnemy controlled by this MovingEnemyController
    /// can target a PlaceableObject.
    /// </summary>
    /// <param name="candidate">the target to check</param>
    /// <returns>true if the MovingEnemy controlled by this MovingEnemyController
    /// can target a PlaceableObject; otherwise, false.</returns>
    protected override bool CanTarget(PlaceableObject candidate)
    {
        if (!ValidEnemy()) return false;
        if (!base.CanTarget(candidate)) return false;

        return true;
    }

    /// <summary>
    /// Updates this MovingEnemyController's state.
    /// </summary>
    protected override void UpdateState()
    {
        if (!ValidEnemy()) return;

        float distanceToTarget = GetEnemy().DistanceToTarget(GetTarget());
        //Debug.Log(distanceToTarget);

        switch (GetState())
        {
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
