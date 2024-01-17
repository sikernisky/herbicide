using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages the SkillTree and its SkillSlots.
/// </summary>
public class SkillTreeManager : MonoBehaviour
{
    /// <summary>
    /// All SkillSlots in the Tree. 
    /// </summary>
    [SerializeField]
    private List<SkillSlot> skillSlots;

    /// <summary>
    /// Reference to the SkillTreeManager singleton.
    /// </summary>
    private static SkillTreeManager instance;


    /// <summary>
    /// Finds and sets the SkillTreeManager singleton for the SkillMenu.
    /// </summary>
    /// <param name="skillMenuController">The SkillMenuController singleton.</param>
    public static void SetSingleton(CollectionMenuController skillMenuController)
    {
        if (skillMenuController == null) return;
        if (instance != null) return;

        SkillTreeManager[] skillTreeManagers = FindObjectsOfType<SkillTreeManager>();
        Assert.IsNotNull(skillTreeManagers, "Array of EconomyControllers is null.");
        Assert.AreEqual(1, skillTreeManagers.Length);
        instance = skillTreeManagers[0];
    }

    /// <summary>
    /// Populates each SkillSlot with the correct information.
    /// </summary>
    /// <returns>A list of the SkillSlotControllers holding the created
    /// SkillSlots. </returns>
    public static List<SkillSlotController> LoadSkillSlots()
    {
        Assert.IsNotNull(instance.skillSlots);
        instance.skillSlots.ForEach(ss => Assert.IsNotNull(ss));

        List<SkillSlotController> controllers = new List<SkillSlotController>();
        foreach (SkillSlot ss in instance.skillSlots)
        {
            SkillSlotController ssc = new SkillSlotController(ss);
            controllers.Add(ssc);
        }

        return controllers;
    }
}
