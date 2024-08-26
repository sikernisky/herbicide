using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls synergies and their effects among Defenders.
/// </summary>
public class SynergyController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// All synergies.
    /// </summary>
    public enum Synergy
    {
        TRIPLE_THREAT
    }

    /// <summary>
    /// Reference to the SynergyController singleton.
    /// </summary>
    private static SynergyController instance;

    /// <summary>
    /// Up to date list of active PlaceableObjects.
    /// </summary>
    private static List<PlaceableObject> placeableObjects;

    /// <summary>
    /// The Synergy slots.
    /// </summary>
    [SerializeField]
    private List<SynergySlot> slots;

    /// <summary>
    /// Curve that the SynergySlots will use when lerping.
    /// </summary>
    [SerializeField]
    private AnimationCurve lerpCurve;

    /// <summary>
    /// The Player's selected synergies for this level.
    /// </summary>
    private HashSet<Synergy> chosenSynergies;

    #endregion

    #region Methods

    /// <summary>
    /// Gives the SynergyController the set of Synergies chosen by the player
    /// this level.
    /// </summary>
    /// <param name="chosenSynergies">The set of chosen Synergies.</param> 
    public static void LoadAllSynergies(HashSet<Synergy> chosenSynergies)
    {
        Assert.IsNotNull(chosenSynergies, "Set of chosen Synergies is null.");
        Assert.IsTrue(chosenSynergies.Count > 0, "Set of chosen Synergies is empty.");

        instance.chosenSynergies = chosenSynergies;
        instance.slots.ForEach(s => s.SetupSynergySlot());
        // TODO: SynergyFactory gives slot prefab, set it in its correct position.
    }

    /// <summary>
    /// Finds and sets the SynergyController singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        SynergyController[] synergyControllers = FindObjectsOfType<SynergyController>();
        Assert.IsNotNull(synergyControllers, "Array of SynergyControllers is null.");
        Assert.AreEqual(1, synergyControllers.Length);
        instance = synergyControllers[0];
    }

    /// <summary>
    /// Main update loop for the SynergyController. Gives it a list of active
    /// PlaceableObjects it needs to calculate current synergies.
    /// </summary>
    /// <param name="targetables">The most recent list of active PlaceableObject
    /// objects.</param>
    public static void UpdateSynergyController(List<PlaceableObject> targetables)
    {
        Assert.IsNotNull(targetables);

        SynergyController.placeableObjects = targetables;
        UpdateSynergySlots();
    }

    /// <summary>
    /// Returns the tier of a synergy that the player has met,
    /// or 0 if they have not met any tier of the synergy or if the 
    /// synergy is not active.
    /// </summary>
    /// <param name="synergy">The synergy tier to get. </param>
    /// <returns>the current tier for the given synergy; 0 if no tier 
    /// has been met. or if the synergy is not active.</returns>
    public static int GetSynergyTier(SynergyController.Synergy synergy)
    {
        Assert.IsNotNull(instance.chosenSynergies);
        if (!instance.chosenSynergies.Contains(synergy)) return 0;

        if (synergy == Synergy.TRIPLE_THREAT) return GetTripleThreatTier();
        else throw new System.Exception();
    }

    /// <summary>
    /// Returns the current tier of the active TripleThreat synergy.
    /// </summary>
    /// <returns>the current tier of the active TripleThreat synergy, or
    /// 0 if no tier has been met.</returns>
    private static int GetTripleThreatTier()
    {
        Assert.IsNotNull(placeableObjects);
        Assert.IsTrue(instance.chosenSynergies.Contains(Synergy.TRIPLE_THREAT));

        int numSquirrels = 0;
        foreach (PlaceableObject target in placeableObjects)
        {
            if (target as Squirrel != null) numSquirrels++;
        }

        if (numSquirrels == 3) return 1;
        if (numSquirrels == 6) return 2;
        if (numSquirrels == 9) return 3;
        return 0;
    }

    /// <summary>
    /// Updates the active SynergySlots.
    /// </summary>
    private static void UpdateSynergySlots()
    {
        foreach (SynergySlot slot in instance.slots)
        {
            Synergy synergy = slot.GetSynergy();
            int tier = GetSynergyTier(synergy);
            bool hovering = InputController.IsHoveringUIElement(slot.gameObject);
            slot.UpdateSynergySlot(tier, hovering, instance.lerpCurve);
        }
    }

    #endregion
}
