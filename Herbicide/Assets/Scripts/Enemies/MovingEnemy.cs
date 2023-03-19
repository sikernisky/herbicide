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
    protected abstract float BASE_SPEED { get; }

    /// <summary>
    /// Current speed of this MovingEnemy.
    /// </summary>
    private float speed;


    /// <summary>
    /// Moves this MovingEnemy to a position.
    /// </summary>
    /// <param name="targetPosition">The position to move to.</param>
    public virtual void MoveTo(Vector3 targetPosition)
    {
        Vector3 movedPosition = Vector3.MoveTowards(transform.position, targetPosition,
            GetMoveSpeed() * Time.deltaTime);
        transform.position = movedPosition;
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
    /// Called when this MovingEnemy spawns. Sets its movement speed, health, and
    /// attack range.
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();
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
