using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a Quill. <br></br>
/// 
/// The QuillController is responsible for manipulating its Quill and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="QuillState">]]>
public class QuillController : ProjectileController<QuillController.QuillState>
{
    #region Fields

    /// <summary>
    /// The Collider2D that the Quill has pierced and is stuck in.
    /// </summary>
    private Collider2D piercedCollider;

    /// <summary>
    /// The scale of a Quill when it is stuck in a Collider2D.
    /// </summary>
    private Vector3 STUCK_SIZE => new Vector3(1.0f, 1.0f, 1.0f);

    /// <summary>
    /// A small, random offset to apply to the Quill's stuck position.
    /// </summary>
    private Vector3 randomStuckPositionOffset;

    /// <summary>
    /// Possible states of a Quill over its lifetime.
    /// </summary>
    public enum QuillState
    {
        SPAWN,
        MOVING,
        STUCK
    }


    /// <summary>
    /// Quills angle towards their target.
    /// </summary>
    protected override bool angleTowardsTarget => true;

    #endregion

    #region Methods

    /// <summary>
    /// Gives an Quill an QuillController.
    /// </summary>
    /// <param name="quill">The quill which will get an QuillController.</param>
    /// <param name="start">Where the quill started.</param>
    /// <param name="destination">Where the quill should go.</param>
    /// <param name="isDoubleQuill">true if the Quill should split into two upon its target's death;
    /// otherwise, false.</param>
    public QuillController(Quill quill, Vector3 start, Vector3 destination, bool isDoubleQuill) :
        base(quill, start, destination)
    {
        if (isDoubleQuill) GetQuill().SetAsDoubleQuill();
    }

    /// <summary>
    /// Main update loop for the QuillController. Here's where
    /// it manipulates its Quill based off game state. 
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
        if(!ValidModel()) return;

        ExecuteStuckState();
    }

    /// <summary>
    /// Returns the Quill model.
    /// </summary>
    /// <returns>the Quill model.</returns>
    protected Quill GetQuill() => GetProjectile() as Quill;

    /// <summary>
    /// Processes events that occur when the Quill detonates at a given position.
    /// </summary>
    /// <param name="other">Collider2D the projectile collided with.</param>
    protected override void DetonateProjectile(Collider2D other)
    {
        GetQuill().GetCollider().enabled = false;
        GetQuill().SetSize(STUCK_SIZE);
        Quaternion currentRotation = GetQuill().transform.rotation;
        float randomAdjustment = Random.Range(-20, 21);
        Quaternion newRotation = Quaternion.Euler(0, 0, currentRotation.eulerAngles.z + randomAdjustment);
        GetQuill().SetRotation(newRotation);
        GetQuill().SetShadowActive(false);
        piercedCollider = other;
        randomStuckPositionOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 1);
    }

    /// <summary>
    /// Returns true if this controller's Quill should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Quill should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel()
    {
        if (GetQuill().Expired()) return false;
        if (!GetQuill().IsActive()) return false;
        if(GetQuill().HasExploded()) return false;

        return true;
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this QuillController's Quill model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case QuillState.SPAWN:
                SetState(QuillState.MOVING);
                break;
            case QuillState.MOVING:
                if(GetProjectile().Collided()) SetState(QuillState.STUCK);
                break;
            case QuillState.STUCK: 
                break;
        }
    }

    /// <summary>
    /// Returns true if two QuillStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two QuillStates are equal; otherwise, false.</returns>
    public override bool StateEquals(QuillState stateA, QuillState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic relevant to the Quill's MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != QuillState.MOVING) return;

        SetAnimation(GetQuill().MID_AIR_ANIMATION_DURATION, ProjectileFactory.GetMidAirAnimationTrack(GetQuill()));
        LinearShot();
    }

    /// <summary>
    /// Runs logic relevant to the Quill's STUCK state.
    /// </summary>
    private void ExecuteStuckState()
    {
        if (!ValidModel()) return;
        if (GetState() != QuillState.STUCK) return;
        if(piercedCollider == null) return;

        Vector3 piercedPosition;
        Enemy piercedEnemy = piercedCollider.gameObject.GetComponent<Model>() as Enemy;
        if (piercedEnemy != null) piercedPosition = piercedEnemy.GetAttackPosition();
        else piercedPosition = piercedCollider.transform.position;
        Vector3 stuckPosition = piercedPosition + randomStuckPositionOffset;
        GetQuill().SetWorldPosition(stuckPosition);
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        QuillState state = GetState();
        if (state == QuillState.MOVING) midAirAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        QuillState state = GetState();
        if (state == QuillState.MOVING) return midAirAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        QuillState state = GetState();
        if (state == QuillState.MOVING) midAirAnimationCounter = 0;
    }

    #endregion
}
