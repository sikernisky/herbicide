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
    /// <param name="position">The position to move to.</param>
    public virtual void MoveTo(Vector2 position)
    {
        throw new System.NotImplementedException();
    }
}
