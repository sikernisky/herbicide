using UnityEngine;

/// <summary>
/// Controls a LevelReward. <br></br>
/// 
/// The LevelRewardController is responsible for manipulating its LevelReward and bringing
/// it to life. This includes moving it playing animations, and more.
/// </summary>
/// <![CDATA[<param name="LevelRewardState">]]>
public class LevelRewardController : CollectableController<LevelRewardController.LevelRewardState>
{
    #region Fields

    /// <summary>
    /// State of a LevelReward.
    /// </summary>
    public enum LevelRewardState
    {
        SPAWN,
        BOBBING,
        COLLECTING
    }

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a LevelReward to a LevelRewardController.
    /// </summary>
    /// <param name="levelReward">The LevelReward to assign.</param>
    /// <param name="dropPos">Where the LevelReward first dropped.</param>
    public LevelRewardController(LevelReward levelReward, Vector3 dropPos) : base(levelReward, dropPos)
    {
        GetLevelReward().SetLevelRewardSpriteBasedOnReward(GetModelTypeRewardBasedOnLevel());
        GetLevelReward().SetWorldPosition(dropPos);
    }

    /// <summary>
    /// Main update loop for the LevelReward.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
        ExecuteBobbingState();
        ExecuteCollectingState();
    }

    /// <summary>
    /// Returns this controller's LevelReward model.
    /// </summary>
    /// <returns>this controller's LevelReward model.</returns>
    private LevelReward GetLevelReward() => GetCollectable() as LevelReward;

    /// <summary>
    /// Returns the type of Model the player is unlocking as a reward
    /// based on the current level.
    /// </summary>
    /// <returns>the type of Model the player is unlocking as a reward
    /// based on the current level.</returns>
    private ModelType GetModelTypeRewardBasedOnLevel()
    {
        switch (SaveLoadManager.GetGameLevel())
        {
            case 0:
                return ModelType.BUNNY;
            case 1:
                return ModelType.RACCOON;
            case 2:
                return ModelType.OWL;
            case 3:
                return ModelType.PORCUPINE;
            default:
                return ModelType.BUNNY;
        }
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two LevelRewardStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if the two LevelRewardStates are equal.</returns>
    public override bool StateEquals(LevelRewardState stateA, LevelRewardState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this LevelRewardController's LevelReward model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> BOBBING : when dropped from source <br></br>
    /// BOBBING --> COLLECTING : when being collected <br></br>
    /// COLLECTING --> DEAD : when collected. <br></br>
    /// </summary>
    public override void UpdateFSM()
    {
        switch (GetState())
        {
            case LevelRewardState.SPAWN:
                SetState(LevelRewardState.BOBBING);
                break;
            case LevelRewardState.BOBBING:
                if (InHomingRange()) SetState(LevelRewardState.COLLECTING);
                break;
            case LevelRewardState.COLLECTING:
                break;
        }
    }

    /// <summary>
    /// Runs logic for the LevelReward's bobbing state.
    /// </summary>
    protected virtual void ExecuteBobbingState()
    {
        if (!ValidModel()) return;
        if (GetState() != LevelRewardState.BOBBING) return;

        BobUpAndDown();
    }

    /// <summary>
    /// Runs logic for the LevelReward's collecting state.
    /// </summary>
    protected virtual void ExecuteCollectingState()
    {
        if (!ValidModel()) return;
        if (GetState() != LevelRewardState.COLLECTING) return;

        if (InCollectionRange())
        {
            GetLevelReward().OnCollect();
            CollectionManager.UnlockModel(GetModelTypeRewardBasedOnLevel());
        }
        else MoveTowardsCursor();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() => throw new System.NotImplementedException();

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() => throw new System.NotImplementedException();
    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() => throw new System.NotImplementedException();

    #endregion
}
