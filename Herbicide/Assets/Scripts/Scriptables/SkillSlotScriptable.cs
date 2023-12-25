using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores data for SkillSlots.
/// </summary>
[CreateAssetMenu(fileName = "SkillSlotScriptable", menuName = "Skill Scriptable", order = 0)]
public class SkillSlotScriptable : ScriptableObject
{
    /// <summary>
    /// Types of Skill/Defender/Occupants.
    /// </summary>
    public enum SkillSlotType
    {
        SQUIRREL,
        BUTTERFLY
    }

    /// <summary>
    /// The Skills the player needs to unlock before they can unlock
    /// this Skill.
    /// </summary>
    [SerializeField]
    private List<SkillSlotType> prerequisites;

    /// <summary>
    /// Name of this Skill.
    /// </summary>
    [SerializeField]
    private string skillName;

    /// <summary>
    /// How much resource it costs to unlock this Skill.
    /// </summary>
    private int cost;

    /// <summary>
    /// This Skill's icon.
    /// </summary>
    [SerializeField]
    private Sprite skillIcon;


    /// <summary>
    /// Returns the name of this Skill.
    /// </summary>
    /// <returns>the name of this Skill.</returns>
    public string GetName() { return skillName; }

    /// <summary>
    /// Returns the Sprite that represents this Skill in the
    /// SkillTree.
    /// </summary>
    /// <returns>the Sprite that represents this Skill in the
    /// SkillTree.</returns>
    public Sprite GetIcon() { return skillIcon; }

    /// <summary>
    /// Returns a new list containing this SkillSlotScriptable's prerequisite skills.
    /// </summary>
    /// <returns> a new list containing this SkillSlotScriptable's prerequisite skills.</returns>
    public List<SkillSlotType> GetPrereqs() { return new List<SkillSlotType>(prerequisites); }
}
