using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an Enemy that moves in some format.
/// </summary>
public abstract class MovingEnemy : Enemy
{
    /// <summary>
    /// Base speed of this MovingEnemy.
    /// </summary>
    public abstract float BASE_SPEED { get; }

    /// <summary>
    /// Current speed of this MovingEnemy.
    /// </summary>
    private float speed;


    /// <summary>
    /// Base method for MovingEnemy object's movement logic. This method
    /// only sets its direction based on the movement target; subclasses
    /// override this method to implement actual movement. 
    /// </summary>
    /// <param name="movePosition">The position to move to.</param>

    public virtual void MoveTo(Vector3 movePosition)
    {
        if (Attacking()) return;

        float preMoveX = GetPosition().x;
        float preMoveY = GetPosition().y;
        float horizontalDifference = movePosition.x - preMoveX;
        float verticalDifference = movePosition.y - preMoveY;

        //Handle diagonals
        if (Mathf.Approximately(Mathf.Abs(horizontalDifference), Mathf.Abs(verticalDifference)))
        {
            if (verticalDifference > 0) SetDirection(Direction.NORTH);
            else SetDirection(Direction.SOUTH);
            return;
        }

        //Handle non-diagonals
        if (horizontalDifference != 0)
        {
            if (horizontalDifference > 0) SetDirection(Direction.EAST);
            else SetDirection(Direction.WEST);
        }
        else
        {
            if (verticalDifference > 0) SetDirection(Direction.NORTH);
            else SetDirection(Direction.SOUTH);
        }
    }

    /// <summary>
    /// Sets the non-negative movement speed of this MovingEnemy.
    /// </summary>
    protected void SetMoveSpeed(float newMoveSpeed)
    {
        if (newMoveSpeed < 0) return;
        speed = newMoveSpeed;
    }

    /// <summary>
    /// Resets the stats of this MovingEnemy to its starting/base values.
    /// </summary>
    public override void ResetStats()
    {
        base.ResetStats();
        SetMoveSpeed(BASE_SPEED);
    }

    /// <summary>
    /// Returns this MovingEnemy's current movement speed. 
    /// </summary>
    /// <returns>this MovingEnemy's current movement speed. </returns>
    protected float GetMoveSpeed()
    {
        return speed;
    }
}
