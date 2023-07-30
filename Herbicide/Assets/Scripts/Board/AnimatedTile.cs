using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class AnimatedTile : Tile
{
    /// <summary>
    /// The Sprites that make up this AnimatedTile's animation.
    /// </summary>
    private Sprite[] animationTrack;

    /// <summary>
    /// How long to wait before switching between frames in this
    /// AnimatedTile's animation.
    /// </summary>
    protected virtual float ANIMATION_DELAY => .5f;


    /// <summary>
    /// Defines this AnimatedTile. Starts its animation.
    /// </summary>
    /// <param name="x">The X-Coordinate of this AnimatedTile.</param>
    /// <param name="y">The Y-Coordinate of this AnimatedTile.</param>
    public override void Define(int x, int y)
    {
        base.Define(x, y);

        StartCoroutine(CoAnimateTile());
    }

    /// <summary>
    /// Starts this AnimatedTile's animation loop. This plays indefinitely. 
    /// </summary>
    /// <returns>A reference to this coroutine.</returns>
    private IEnumerator CoAnimateTile()
    {
        Assert.IsNotNull(animationTrack);
        int counter = Random.Range(0, animationTrack.Length);
        while (true)
        {
            SetSprite(animationTrack[counter]);
            if (counter + 1 == animationTrack.Length) counter = 0;
            else counter++;
            yield return new WaitForSeconds(ANIMATION_DELAY);
        }
    }

    /// <summary>
    /// Sets this AnimatedTile's animation track.
    /// </summary>
    /// <param name="newTrack">the new animation track.</param>
    protected void SetAnimationTrack(Sprite[] newTrack)
    {
        if (newTrack == null || newTrack.Length == 0) return;
        animationTrack = newTrack;
    }
}
