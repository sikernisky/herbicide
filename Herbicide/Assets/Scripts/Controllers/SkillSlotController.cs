using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manipulates a SkillSlot GameObject.
/// </summary>
public class SkillSlotController
{
    /// <summary>
    /// This SkillSlotController's SkillSlot.
    /// </summary>
    private SkillSlot slot;

    /// <summary>
    /// Assigns a SkillSlot to a controller.
    /// </summary>
    /// <param name="slot">The SkillSlot to assign. </param>
    public SkillSlotController(SkillSlot slot)
    {
        Assert.IsNotNull(slot, "Skill Slot is null.");
        this.slot = slot;
        GetSlot().SetOccupantSprite();
    }

    /// <summary>
    /// Returns the SkillSlot model.
    /// </summary>
    /// <returns>the SkillSlot model.</returns>
    private SkillSlot GetSlot() { return slot; }

    /// <summary>
    /// Main update loop for the SkillSlot.
    /// </summary>
    /// <param name="unlockedSkills">All unlocked skills.</param>
    public void UpdateSlot(List<SkillSlotScriptable.SkillSlotType> unlockedSkills)
    {
        if (!GetSlot().Unlocked(unlockedSkills)) GetSlot().Blackout();
        else GetSlot().SetOccupantSprite();
    }
}
