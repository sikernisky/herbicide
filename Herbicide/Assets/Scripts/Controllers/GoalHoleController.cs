public class GoalHoleController : MobController<GoalHoleController.GoalHoleState>
{
    #region Fields

    /// <summary>
    /// States of a GoalHole.
    /// </summary>
    public enum GoalHoleState
    {
        SPAWN
    }

    /// <summary>
    /// Maximum number of targets a GoalHole can have.
    /// </summary>
    protected override int MAX_TARGETS => 0;

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a GoalHole to a controller.
    /// </summary>
    /// <param name="GoalHole">The GoalHole to assign.</param>
    public GoalHoleController(GoalHole GoalHole) : base(GoalHole) { }

    /// <summary>
    /// Returns true if the GoalHole can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Placeable object to check for targetability.</param>
    /// <returns>true if the GoalHole can target the Model; otherwise, false. </returns>
    protected override bool CanTargetOtherModel(Model target) => false;

    /// <summary>
    /// Returns true if the GoalHole should be removed.
    /// </summary>
    /// <returns>true if the GoalHole should be removed; otherwise, false.</returns>
    public override bool ValidModel() => true;

    /// <summary>
    /// Returns the GoalHole model.
    /// </summary>
    /// <returns>the GoalHole model.</returns>
    private GoalHole GetGoalHole() => GetMob() as GoalHole;

    /// <summary>
    /// Returns the GoalHole prefab to the GoalHoleFactory singleton.
    /// </summary>
    public override void ReturnModelToFactory() => HoleFactory.ReturnHolePrefab(GetGoalHole().gameObject);

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of the GoalHole. The transitions are: <br></br>
    /// 
    /// SPAWN --> EMPTY : always
    /// EMPTY --> FILLED : when nexus dropped in hole
    /// FILLED --> EMPTY : when nexus removed from hole 
    /// </summary>
    public override void UpdateFSM()
    {
        switch (GetState())
        {
            case GoalHoleState.SPAWN:
                break;
        }
    }

    /// <summary>
    /// Returns true if two GoalHoleStates are equal.
    /// </summary>
    /// <param name="stateA">The first GoalHoleState.</param>
    /// <param name="stateB">The second GoalHoleState.</param>
    /// <returns>true if the two GoalHoleStates are equal.</returns>
    public override bool StateEquals(GoalHoleState stateA, GoalHoleState stateB) => stateA == stateB;

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
