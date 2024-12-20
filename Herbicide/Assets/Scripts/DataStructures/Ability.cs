using UnityEngine;

/// <summary>
/// Stores information about a Mob's ability.
/// </summary>
[System.Serializable]
public class Ability
{
    #region Fields

    /// <summary>
    /// The name of the ability.
    /// </summary>
    [SerializeField]
    private string abilityName;

    /// <summary>
    /// The Sprite icon of the ability.
    /// </summary>
    [SerializeField]
    private Sprite abilityIcon;

    /// <summary>
    /// The description of the ability.
    /// </summary>
    [SerializeField]
    private string abilityDescription;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the name of the ability.
    /// </summary>
    /// <returns>The name of the ability.</returns>
    public string GetAbilityName() => abilityName;

    /// <summary>
    /// Returns the Sprite icon of the ability.
    /// </summary>
    /// <returns>The Sprite icon of the ability.</returns>
    public Sprite GetAbilityIcon() => abilityIcon;

    /// <summary>
    /// Returns the description of the ability.
    /// </summary>
    /// <returns>The description of the ability.</returns>
    public string GetAbilityDescription() => abilityDescription;

    #endregion
}
