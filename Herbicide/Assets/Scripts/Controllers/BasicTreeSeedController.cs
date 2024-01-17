using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a BasicTreeSeed Projectile.
/// 
/// The BasicTreeSeedController is responsible for manipulating its Acorn and bringing
///// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public class BasicTreeSeedController : ProjectileController<BasicTreeSeedController.BasicTreeSeedState>
{

    /// <summary>
    /// States of a BasicTreeSeed.
    /// </summary>
    public enum BasicTreeSeedState
    {
        SPAWN,
        MOVING,
        COLLIDING,
        DEAD
    }

    /// <summary>
    /// Gives a BasicTreeSeed a controller.
    /// </summary>
    /// <param name="basicTreeSeed">The BasicTreeSeed model.</param>
    /// <param name="start">Where the BasicTreeSeed starts.</param>
    /// <param name="destination">Where the BasicTreeSeed is going.</param>
    public BasicTreeSeedController(BasicTreeSeed basicTreeSeed,
     Vector3 start, Vector3 destination) : base(basicTreeSeed, start, destination)
    {
    }

    /// <summary>
    /// Returns the BasicTreeSeed model.
    /// </summary>
    /// <returns>the BasicTreeSeed model.</returns>
    protected BasicTreeSeed GetBasicTreeSeed() { return GetProjectile() as BasicTreeSeed; }

    //-----------------------STATE LOGIC------------------------//


    /// <summary>
    /// Updates the state of this BasicTreeSeedController's BasicTreeSeed model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source <br></br>
    /// MOVING --> COLLIDING : when hits valid target <br></br>
    /// COLLIDING --> DEAD : when all effects have been applied to valid target <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case BasicTreeSeedState.SPAWN:
                SetState(BasicTreeSeedState.MOVING);
                break;
            case BasicTreeSeedState.MOVING:
                if (GetBasicTreeSeed().HasReachedTarget()) SetState(BasicTreeSeedState.COLLIDING);
                break;
            case BasicTreeSeedState.COLLIDING:
                if (AppliedEffect()) SetState(BasicTreeSeedState.DEAD);
                break;
            case BasicTreeSeedState.DEAD:
                break;
        }
    }

    /// <summary>
    /// Returns true if two BasicTreeSeedStateS are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two BasicTreeSeedStates are equal; otherwise, false.</returns>
    public override bool StateEquals(BasicTreeSeedState stateA, BasicTreeSeedState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Performs logic for the MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != BasicTreeSeedState.MOVING) return;
        if (GetBasicTreeSeed().HasReachedTarget()) return;

        ParabolicShot(1f, 1.5f);
    }

    /// <summary>
    /// Performs logic for the COLLIDING state.
    /// </summary>
    public override void ExecuteCollidingState()
    {
        if (GetState() != BasicTreeSeedState.COLLIDING) return;
        if (!ValidModel()) return;

        // Make the seed invisible.
        GetBasicTreeSeed().SetColor(new Color32(255, 255, 255, 0));

        Vector2Int tilePos = new Vector2Int(GetBasicTreeSeed().GetX(), GetBasicTreeSeed().GetY());
        GameObject flooringPrefab = FlooringFactory.GetSoilFlooringPrefab();
        Assert.IsNotNull(flooringPrefab);
        SoilFlooring soilFlooringComp = flooringPrefab.GetComponent<SoilFlooring>();
        Assert.IsNotNull(soilFlooringComp);
        TileGrid.FloorTile(tilePos, soilFlooringComp);

        ApplyEffect();
    }

    /// <summary>
    /// Performs logic for the DEAD state.
    /// </summary>
    public override void ExecuteDeadState()
    {
        if (GetState() != BasicTreeSeedState.DEAD) return;

        GetBasicTreeSeed().SetAsInactive();
    }

    //---------------------ANIMATION LOGIC----------------------//

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() { throw new System.NotImplementedException(); }
}
