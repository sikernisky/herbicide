using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Represents a slot in the skill tree.
/// </summary>
public class SkillSlot : MonoBehaviour
{
    /// <summary>
    /// The Skill occupying this slot.
    /// </summary>
    [SerializeField]
    private SkillSlotScriptable occupant;

    /// <summary>
    /// This SkillSlot's Image component.
    /// </summary>
    [SerializeField]
    private Image slotImage;

    /// <summary>
    /// Sets this SkillSlot's sprite to its occupant.
    /// </summary>
    public void SetOccupantSprite() { GetOccupantImage().sprite = GetOccupant().GetIcon(); }

    /// <summary>
    /// Completely blacks out this SkillSlot's image sprite.
    /// </summary>
    public void Blackout() { GetOccupantImage().color = Color.black; }

    /// <summary>
    /// Returns the name of this slot's occupant.
    /// </summary>
    /// <returns>the name of this slot's occupant.</returns>
    public string GetOccupantName() { return GetOccupant().GetName(); }

    /// <summary>
    /// Returns this SkillSlot's occupant scriptable.
    /// </summary>
    /// <returns>this SkillSlot's occupant scriptable.</returns>
    private SkillSlotScriptable GetOccupant() { return occupant; }

    /// <summary>
    /// Returns this SkillSlot's Image component.
    /// </summary>
    /// <returns>this SkillSlot's Image component.</returns>
    private Image GetOccupantImage() { return slotImage; }

    /// <summary>
    /// Returns true if all Skills in this SkillSlot's
    /// prerequisite list have been unlocked. 
    /// </summary>
    /// <param name="unlockedSkills">The skills unlocked so far.</param>
    /// <returns></returns>
    public bool Unlocked(List<SkillSlotScriptable.SkillSlotType> unlockedSkills)
    {
        Assert.IsNotNull(unlockedSkills);
        foreach (SkillSlotScriptable.SkillSlotType skill in GetOccupant().GetPrereqs())
        {
            if (!unlockedSkills.Contains(skill)) return false;
        }
        return true;
    }
}
