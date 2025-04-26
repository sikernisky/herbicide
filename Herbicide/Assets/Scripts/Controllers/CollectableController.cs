using UnityEngine;
using System;

/// <summary>
/// Controls a Collectable. <br></br>
/// 
/// The CollectableController is responsible for manipulating its Collectable and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Collectable.</typeparam>
public abstract class CollectableController<T> : ModelController, IStateTracker<T> where T : Enum
{
    #region Fields

    /// <summary>
    /// Current state of the Collectable.
    /// </summary>
    private T state;

    /// <summary>
    /// Current animation state of the Collectable.
    /// </summary>
    private T animationState;

    /// <summary>
    /// Starting position of the Collectable when it bobs up and down.
    /// </summary>
    private Vector3 bobStartPosition;

    /// <summary>
    /// Time offset to create varying bobbing animations.
    /// </summary>
    private float timeOffset;

    /// <summary>
    /// true if the Collectable got close enough to the cursor and should
    /// complete its homing movement. 
    /// </summary>
    private bool isHoming;

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a CollectableController to a Collectable Model.
    /// </summary>
    /// <param name="collectable">The Collectable to assign.</param>
    /// <param name="dropPos">Where the Collectable first dropped.</param>

    public CollectableController(Collectable collectable, Vector3 dropPos) : base(collectable)
    {
        bobStartPosition = dropPos;
        timeOffset = timeOffset = UnityEngine.Random.Range(0f, Mathf.PI * 2);
    }

    /// <summary>
    /// Main update loop for the Collectable.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
        UpdateFSM();
    }

    /// <summary>
    /// Returns true if this controller's Collectable should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if the Collectable should be nullified and destroyed;
    /// otherwise, false. </returns>
    public override bool ValidModel() => GetCollectable().Collected() ? false : true;

    /// <summary>
    /// Returns this controller's Collectable model.
    /// </summary>
    /// <returns>this controller's Collectable model.</returns>
    protected Collectable GetCollectable() => GetModel() as Collectable;

    /// <summary>
    /// Returns true if the Collectable is within homing range of the cursor.
    /// </summary>
    /// <returns>true if the Collectable is within homing range of the cursor;
    /// otherwise, false. /// </returns>
    protected bool InHomingRange() => Vector2.Distance(InputManager.WorldMousePosition, GetCollectable().GetWorldPosition()) < GetCollectable().HomingRange;

    /// <summary>
    /// Returns true if the Collectable is within collection range of the cursor.
    /// </summary>
    /// <returns>true if the Collectable is within collection range of the cursor;
    /// otherwise, false. /// </returns>
    protected bool InCollectionRange() => Vector2.Distance(InputManager.WorldMousePosition, GetCollectable().GetWorldPosition()) < GetCollectable().CollectingRange;

    /// <summary>
    /// Bobs the Collectable up and down.
    /// </summary>
    protected virtual void BobUpAndDown()
    {
        float t = (Mathf.Cos((Time.time + timeOffset) * GetCollectable().BobbingSpeed) + 1) * 0.5f;
        float newY = Mathf.Lerp(-GetCollectable().BobbingHeight, GetCollectable().BobbingHeight, t);
        Vector3 newPosition = new Vector3(
            bobStartPosition.x,
            bobStartPosition.y + newY,
            bobStartPosition.z);
        GetCollectable().SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Homes the collectable towards the cursor.
    /// </summary>
    protected virtual void MoveTowardsCursor()
    {
        if (!InHomingRange() && !isHoming) return;
        isHoming = true;
        float minSpeed = 4f;
        Vector2 mousePos = InputManager.WorldMousePosition;
        float distanceToCursor = Vector2.Distance(GetCollectable().GetWorldPosition(), mousePos);
        float normalizedDistance = Mathf.Clamp01(distanceToCursor / GetCollectable().HomingRange);
        float speedMultiplier = GetCollectable().GetHomingCurve().Evaluate(normalizedDistance);
        float movementSpeed = Mathf.Lerp(minSpeed, GetCollectable().HomingSpeed, speedMultiplier) * Time.deltaTime;
        Vector2 currentPosition = GetCollectable().GetWorldPosition();
        float distanceToMouse = Vector2.Distance(currentPosition, mousePos);
        float epsilon = 0.01f;
        if (distanceToMouse > epsilon)
        {
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, mousePos, movementSpeed);
            GetCollectable().SetWorldPosition(newPosition);
        }
    }

    /// <summary>
    /// Returns the Collectable prefab to the CollectableFactory object pool.
    /// </summary>
    public override void ReturnModelToFactory() => CollectableFactory.ReturnCollectablePrefab(GetCollectable().gameObject);

    #endregion

    #region State Logic

    /// <summary>
    /// Sets the state of the Collectable.
    /// </summary>
    /// <param name="state">The state to set to.</param>
    public void SetState(T state) => this.state = state;

    /// <summary>
    /// Returns true if two states are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two states are equal; otherwise, false.</returns>
    public abstract bool StateEquals(T stateA, T stateB);

    /// <summary>
    /// Returns the state of the Collectable.
    /// </summary>
    /// <returns>the state of the Collectable.</returns>
    public T GetState() => state;

    /// <summary>
    /// Updates the state of this CollectableController's Collectable model.
    /// </summary>
    public abstract void UpdateFSM();

    #endregion

    #region Animation Logic

    /// <summary>
    /// Returns the Collectable's current animation state.
    /// </summary>
    /// <returns>the Collectable's current animation state.</returns>
    public T GetAnimationState() => animationState;

    /// <summary>
    /// Sets the Collectable's current animation state.
    /// </summary>
    /// <param name="animationState">The animation state to set.</param>
    public void SetAnimationState(T animationState) => this.animationState = animationState;

    #endregion
}
