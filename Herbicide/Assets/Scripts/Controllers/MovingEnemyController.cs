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
    /// Initializes this MovingEnemyController with the MovingEnemy it controls.
    /// </summary>
    /// <param name="movingEnemy">The MovingEnemy this MovingEnemyController controls.</param>
    /// <param name="spawnPosition">The position at which to spawn the MovingEnemy. </param>
    ///<param name="id">the unique ID of this EnemyController</param>
    public MovingEnemyController(MovingEnemy movingEnemy, Vector3 spawnPosition, int id)
     : base(movingEnemy, spawnPosition, id)
    {
    }

    /// <summary>
    /// Updates the MovingEnemy controlled by this Controller.
    /// </summary>
    /// <param name="targets">All potential targets for this MovingEnemy</param>
    public override void UpdateEnemy(List<PlaceableObject> targets)
    {
        base.UpdateEnemy(targets);
        if (targets == null) return;
        if (!ValidEnemy()) return;
        MovingEnemy enemy = GetEnemy() as MovingEnemy;
        Assert.IsNotNull(enemy);


        SelectTarget(targets);
        SelectMovementTarget();
        UpdateState();

        if (GetState() == EnemyState.CHASE) enemy.MoveTo(GetMovementTarget());
    }

    /// <summary>
    /// Selects the MovingEnemy's target.
    /// </summary>
    /// <param name="targets"> all possible targets for this MovingEnemy to select.
    /// </param>
    protected override void SelectTarget(List<PlaceableObject> targets)
    {
        if (!ValidEnemy()) return;
        if (GetTarget() != null) return;
        if (targets == null) return;
        if (targets.Count == 0) return;

        Debug.Log("Selected: " + targets[0]);
        SetTarget(targets[0]);
    }

    /// <summary>
    /// Sets position to which the MovingEnemy should select as its
    /// movement target.
    /// </summary>
    private void SelectMovementTarget()
    {
        if (!ValidEnemy()) return;

        //Move to the right by default
        movementTarget = new Vector3(GetEnemy().transform.position.x + 1,
            GetEnemy().transform.position.y,
            GetEnemy().transform.position.z);


        if (GetTarget() != null && DistanceToTarget() > GetEnemy().GetAttackRange())
        {
            movementTarget = TileGrid.GetNextPositionInPath(GetEnemy().transform.position,
                GetTarget().transform.position);
        }
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
        if (!ValidEnemy()) return default;

        return movementTarget;
    }

    /// <summary>
    /// Sets the MovingEnemy's movement target.
    /// </summary>
    /// <param name="targetPos">the target position.</param>
    private void SetMovementTarget(Vector3 targetPos)
    {
        if (!ValidEnemy()) return;

        movementTarget = targetPos;
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

        switch (GetState())
        {
            case EnemyState.SPAWN:
                SetState(EnemyState.CHASE);
                break;
            case EnemyState.CHASE:
                if (DistanceToTarget() <= GetEnemy().GetAttackRange())
                {
                    SetState(EnemyState.ATTACK);
                }
                break;
            case EnemyState.ATTACK:
                if (DistanceToTarget() > GetEnemy().GetAttackRange())
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
