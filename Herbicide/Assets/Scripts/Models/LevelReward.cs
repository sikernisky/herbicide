using UnityEngine;
using UnityEngine.Assertions;
/// <summary>
/// Represents a dropped reward for completing a level.
/// </summary>
public class LevelReward : Collectable
{
    #region Fields

    #endregion

    #region Reward Sprites

    /// <summary>
    /// The Sprite that represents a Bunny reward.
    /// </summary>
    [SerializeField]
    private Sprite bunnyRewardSprite;

    /// <summary>
    /// The Sprite that represents a Bunny reward's shadow.
    /// </summary>
    [SerializeField]
    private Sprite bunnyRewardSpriteShadow;

    /// <summary>
    /// The Sprite that represents a Raccoon reward.
    /// </summary>
    [SerializeField]
    private Sprite raccoonRewardSprite;

    /// <summary>
    /// The Sprite that represents a Raccoon reward's shadow.
    /// </summary>
    [SerializeField]
    private Sprite raccoonRewardSpriteShadow;

    /// <summary>
    /// The Sprite that represents a Owl reward.
    /// </summary>
    [SerializeField]
    private Sprite owlRewardSprite;

    /// <summary>
    /// The Sprite that represents a Owl reward's shadow.
    /// </summary>
    [SerializeField]
    private Sprite owlRewardSpriteShadow;

    /// <summary>
    /// The Sprite that represents a Porcupine reward.
    /// </summary>
    [SerializeField]
    private Sprite porcupineRewardSprite;

    /// <summary>
    /// The Sprite that represents a Porcupine reward's shadow.
    /// </summary>
    [SerializeField]
    private Sprite porcupineRewardSpriteShadow;

    /// <summary>
    /// The Sprite that represents a Combination Badge reward.
    /// </summary>
    [SerializeField]
    private Sprite combinationBadgeRewardSprite;

    /// <summary>
    /// The Sprite that represents a Combination Badge reward's shadow.
    /// </summary>
    [SerializeField]
    private Sprite combinationBadgeRewardSpriteShadow;

    /// <summary>
    /// The Sprite that represents a Reroll Badge reward.
    /// </summary>
    [SerializeField]
    private Sprite rerollBadgeRewardSprite;

    /// <summary>
    /// The Sprite that represents a Reroll Badge reward's shadow.
    /// </summary>
    [SerializeField]
    private Sprite rerollBadgeRewardSpriteShadow;

    #endregion

    #region Stats

    /// <summary>
    /// Type of a LevelReward.
    /// </summary>
    public override ModelType TYPE => ModelType.LEVEL_REWARD;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the Sprite of this LevelReward to match the given reward.
    /// </summary>
    /// <param name="reward">the new ModelType the player is unlocking
    /// as a reward.</param>
    public void SetLevelRewardSpriteBasedOnReward(ModelType reward)
    {
        switch (reward)
        {
            case ModelType.BUNNY:
                SetSprite(bunnyRewardSprite);
                SetShadowSprite(bunnyRewardSpriteShadow);
                break;
            case ModelType.RACCOON:
                SetSprite(raccoonRewardSprite);
                SetShadowSprite(raccoonRewardSpriteShadow);
                break;
            case ModelType.OWL:
                SetSprite(owlRewardSprite);
                SetShadowSprite(owlRewardSpriteShadow); 
                break;
            case ModelType.PORCUPINE:
                SetSprite(porcupineRewardSprite);
                SetShadowSprite(porcupineRewardSpriteShadow);
                break;
            case ModelType.REROLL_BADGE:
                SetSprite(rerollBadgeRewardSprite);
                SetShadowSprite(rerollBadgeRewardSpriteShadow);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Called when the player collects this LevelReward.
    /// </summary>
    public override void OnCollect()
    {
        base.OnCollect();
        SceneController.LoadNextLevelWithFadeDelay();
    }

    #endregion
}
