using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Bomb Projectile.
/// 
/// The BombController is responsible for manipulating its Bomb and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public class BombController : ProjectileController<BombController.BombState>
{
    /// <summary>
    /// Total number of Bombs created since the scene began.
    /// </summary>
    private static int NUM_BOMBS;

    /// <summary>
    /// Counts the number of seconds in the moving animation; resets
    /// on step.
    /// </summary>
    private float movingAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    private float collidingAnimationCounter;

    /// <summary>
    /// Possible states of a Bomb over its lifetime.
    /// </summary>
    public enum BombState
    {
        SPAWN,
        MOVING,
        COLLIDING,
        DEAD
    }


    /// <summary>
    /// Gives a Bomb a BombController.
    /// </summary>
    /// <param name="bomb">The Bomb which will get an BombController.</param>
    /// <param name="start">Where the Bomb starts.</param>
    /// <param name="destination">Where the Bomb should go.</param>
    public BombController(Projectile bomb, Vector3 start, Vector3 destination)
    : base(bomb, start, destination)
    {
        Assert.IsNotNull(bomb, "Bomb cannot be null.");
        NUM_BOMBS++;
    }

    /// <summary>
    /// Returns this BombController's Bomb model.
    /// </summary>
    /// <returns>this BombController's Bomb model.</returns>
    private Bomb GetBomb() { return GetProjectile() as Bomb; }

    //-----------------------STATE LOGIC----------------------//

    /// <summary>
    /// Performs logic for the Bomb's Moving state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != BombState.MOVING) return;

        GetBomb().SetAnimationDuration(GetBomb().MOVE_ANIMATION_DURATION);
        Sprite[] movingTrack = BombFactory.GetMidAirMovementTrack();
        if (GetAnimationState() != BombState.MOVING) GetBomb().SetAnimationTrack(movingTrack);
        else GetBomb().SetAnimationTrack(movingTrack, GetBomb().CurrentFrame);
        SetAnimationState(BombState.MOVING);

        //Step the animation.
        StepAnimation();
        GetBomb().SetSprite(GetBomb().GetSpriteAtCurrentFrame());
        GetBomb().GetTransform().Rotate(0, 0, 750 * Time.deltaTime);

        ParabolicShot(2, 1.8f);
    }

    /// <summary>
    /// Performs logic for the Bomb's Colliding state.
    /// </summary>
    public override void ExecuteCollidingState()
    {
        if (!ValidModel()) return;
        if (GetState() != BombState.COLLIDING) return;

        // GameObject splatterCopy = GameObject.Instantiate(GetBomb().GetSplatter());
        // splatterCopy.SetActive(true);

        Vector2Int tilePos = new Vector2Int(GetBomb().GetX(), GetBomb().GetY());
        GameObject bombSplatPrefab = BombFactory.GetBombPrefab();
        Assert.IsNotNull(bombSplatPrefab);
        BombSplat bombSplatComp = bombSplatPrefab.GetComponent<BombSplat>();
        Assert.IsNotNull(bombSplatComp);

        if (TileGrid.PlaceOnTile(tilePos, bombSplatComp))
        {

        }

        ApplyHazard();
    }

    /// <summary>
    /// Performs logic for the Bomb's Dead state.
    /// </summary>
    public override void ExecuteDeadState()
    {
        if (!ValidModel()) return;
        if (GetState() != BombState.DEAD) return;

        GetBomb().SetAsInactive();
    }

    /// <summary>
    /// Returns true if two BombStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two BombStates are equal; otherwise, false.</returns>
    public override bool StateEquals(BombState stateA, BombState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this BombController's Bomb model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source <br></br>
    /// MOVING --> COLLIDING : when hits valid target <br></br>
    /// COLLIDING --> DEAD : when all effects have been applied to valid target <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        switch (GetState())
        {
            case BombState.SPAWN:
                SetState(BombState.MOVING);
                break;
            case BombState.MOVING:
                if (GetBomb().HasReachedTarget()) SetState(BombState.COLLIDING);
                break;
            case BombState.COLLIDING:
                if (AppliedHazard()) SetState(BombState.DEAD);
                break;
            case BombState.DEAD:
                break;
        }
    }

    //---------------------ANIMATION LOGIC----------------------//

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        BombState state = GetState();
        if (state == BombState.MOVING) movingAnimationCounter += Time.deltaTime;
        else if (state == BombState.COLLIDING) collidingAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        BombState state = GetState();
        if (state == BombState.MOVING) return movingAnimationCounter;
        else if (state == BombState.COLLIDING) return collidingAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        BombState state = GetState();
        if (state == BombState.MOVING) movingAnimationCounter = 0;
        else if (state == BombState.COLLIDING) collidingAnimationCounter = 0;
    }
}
