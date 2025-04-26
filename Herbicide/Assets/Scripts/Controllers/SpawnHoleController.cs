using UnityEngine;

/// <summary>
/// Controls a SpawnHole.
/// 
/// The SpawnHoleController is responsible for manipulating its SpawnHole and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="SpawnHoleState">]]>
public class SpawnHoleController : MobController<SpawnHoleController.SpawnHoleState>
{
    #region Fields

    /// <summary>
    /// States of a SpawnHole.
    /// </summary>
    public enum SpawnHoleState
    {
        SPAWN
    }

    /// <summary>
    /// Maximum number of targets a SpawnHole can have.
    /// </summary>
    protected override int MAX_TARGETS => 0;

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a SpawnHole to a controller.
    /// </summary>
    /// <param name="SpawnHole">The SpawnHole to assign.</param>
    public SpawnHoleController(SpawnHole SpawnHole) : base(SpawnHole) { }

    /// <summary>
    /// Returns true if the SpawnHole can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Placeable object to check for targetability.</param>
    /// <returns>true if the SpawnHole can target the Model; otherwise, false. </returns>
    protected override bool CanTargetOtherModel(Model target) => false;

    /// <summary>
    /// Returns true if the SpawnHole should be removed.
    /// </summary>
    /// <returns>true if the SpawnHole should be removed; otherwise, false.</returns>
    public override bool ValidModel() => true;

    /// <summary>
    /// Returns the SpawnHole model.
    /// </summary>
    /// <returns>the SpawnHole model.</returns>
    private SpawnHole GetSpawnHole() => GetMob() as SpawnHole;

    /// <summary>
    /// Returns the SpawnHole prefab to the SpawnHoleFactory singleton.
    /// </summary>
    public override void ReturnModelToFactory() => HoleFactory.ReturnHolePrefab(GetSpawnHole().gameObject);

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of the SpawnHole. The transitions are: <br></br>
    /// 
    /// SPAWN --> EMPTY : always
    /// EMPTY --> FILLED : when nexus dropped in hole
    /// FILLED --> EMPTY : when nexus removed from hole 
    /// </summary>
    public override void UpdateFSM()
    {
        switch (GetState())
        {
            case SpawnHoleState.SPAWN:
                break;
        }
    }

    /// <summary>
    /// Returns true if two SpawnHoleStates are equal.
    /// </summary>
    /// <param name="stateA">The first SpawnHoleState.</param>
    /// <param name="stateB">The second SpawnHoleState.</param>
    /// <returns>true if the two SpawnHoleStates are equal.</returns>
    public override bool StateEquals(SpawnHoleState stateA, SpawnHoleState stateB) => stateA == stateB;

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
