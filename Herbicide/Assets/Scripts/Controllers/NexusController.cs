using System;
using UnityEngine;

/// <summary>
/// Controls a Nexus. <br></br>
/// 
/// The NexusController is responsible for manipulating its Nexus and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="NexusState">]]>
public class NexusController : MobController<NexusController.NexusState>
{
    #region Fields

    /// <summary>
    /// Different states of a Nexus.
    /// /// </summary>
    public enum NexusState
    {
        SPAWN,
        IDLE,
        PICKED_UP
    }

    /// <summary>
    /// Maximum number of targets a Nexus can have.
    /// </summary>
    protected override int MAX_TARGETS => 0;

    /// <summary>
    /// The number of seconds that this Nexus has been dropped.
    /// </summary>
    private float droppedCounter;

    /// <summary>
    /// How long it takes for a Nexus to return to its spawn position when dropped.
    /// </summary>
    private const float RESPAWN_RATE = 5f;

    /// <summary>
    /// true if the Nexus resets; otherwise, false.
    /// </summary>
    private bool resets = false;

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a Nexus to this NexusController.
    /// </summary>
    /// <param name="nexus"></param>The Nexus to assign. <summary>
    public NexusController(Nexus nexus) : base(nexus) { }

    /// <summary>
    /// Main update loop for the Nexus.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        ExecuteSpawnState();
        ExecuteIdleState();
        ExecutePickedUpState();
    }

    /// <summary>
    /// Returns true if the Nexus can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the Nexus can target the Model passed
    /// into this method; otherwise, false.</returns>
    protected override bool CanTargetOtherModel(Model target) => false;

    /// <summary>
    /// Returns true if the Nexus should be removed.
    /// </summary>
    /// <returns>true if the Nexus should be removed; otherwise, false.</returns>
    public override bool ValidModel() => !GetNexus().DroppedByMob();

    /// <summary>
    /// Returns the Nexus model.
    /// </summary>
    /// <returns>the Nexus model.</returns>
    private Nexus GetNexus() => GetMob() as Nexus;

    /// <summary>
    /// Returns the Nexus prefab to the NexusFactory singleton.
    /// </summary>
    public override void ReturnModelToFactory() => NexusFactory.ReturnNexusPrefab(GetNexus().gameObject);

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of the Nexus. The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case NexusState.SPAWN:
                SetState(NexusState.IDLE);
                break;
            case NexusState.IDLE:
                if (GetNexus().PickedUp()) SetState(NexusState.PICKED_UP);
                break;
            case NexusState.PICKED_UP:
                if (!GetNexus().PickedUp() && !GetNexus().DroppedByMob()) SetState(NexusState.IDLE);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns true if two NexusStates are equal.
    /// </summary>
    /// <param name="stateA">The first NexusState.</param>
    /// <param name="stateB">The second NexusState.</param>
    /// <returns>true if the two NexusStates are equal.</returns>
    public override bool StateEquals(NexusState stateA, NexusState stateB) => stateA == stateB;

    /// <summary>
    /// Executes logic for the Nexus' spawn state.
    /// </summary>
    protected virtual void ExecuteSpawnState()
    {
        if(GetState() != NexusState.SPAWN) return;
    }

    /// <summary>
    /// Executes logic for the Nexus' idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != NexusState.IDLE) return;
        if (!ValidModel()) return;
        if (GetNexus().PickedUp()) return;
        if (GetNexus().DroppedByMob()) return;

        int xPosCoord = TileGrid.PositionToCoordinate(GetNexus().GetPosition().x);
        int yPosCoord = TileGrid.PositionToCoordinate(GetNexus().GetPosition().y);
        int xSpawn = TileGrid.PositionToCoordinate(GetNexus().GetSpawnWorldPosition().x);
        int ySpawn = TileGrid.PositionToCoordinate(GetNexus().GetSpawnWorldPosition().y);

        if (TileGrid.IsNexusHole(GetNexus().GetPosition()))
        {
            Vector3 nexusHolePosition = GetNexus().GetPosition();
            Vector3 jumpPosition = nexusHolePosition;
            jumpPosition.y -= 2;

            SetNextMovePos(jumpPosition);
            FallIntoMovePos(3f);

            if (!ReachedMovementTarget()) return;
            GetNexus().Drop();
        }

        else if(resets && (xPosCoord != xSpawn || yPosCoord != ySpawn))
        {
            if(droppedCounter >= RESPAWN_RATE)
            {
                TileGrid.RemoveFromTile(new Vector2Int(xPosCoord, yPosCoord));
                TileGrid.PlaceOnTileUsingCoordinates(new Vector2Int(xSpawn, ySpawn), GetNexus());
                droppedCounter = 0;
            }
            else droppedCounter += Time.deltaTime;
        }
    }

    /// <summary>
    /// Executes logic for the Nexus' picked up state.
    /// </summary>
    protected virtual void ExecutePickedUpState()
    {
        if (GetState() != NexusState.PICKED_UP) return;
        if (!ValidModel()) return;
        if (!GetNexus().PickedUp()) return;

        GetNexus().SetWorldPosition(GetNexus().GetHeldPosition());
        droppedCounter = 0;
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() { throw new System.NotImplementedException(); }

    #endregion
}
