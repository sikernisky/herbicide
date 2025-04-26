using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Tickets.
/// </summary>
public class TicketFactory : Factory
{
    #region Fields
    
    /// <summary>
    /// Reference to the TicketFactory singleton.
    /// </summary>
    private static TicketFactory instance;

    /// <summary>
    /// How many prefabs to fill each ObjectPool with at start.
    /// </summary>
    protected override int poolStartingCount => 1;

    /// <summary>
    /// Struct that holds a rarity and its corresponding icon.
    /// </summary>
    [System.Serializable]
    private struct RarityIcon
    {
        /// <summary>
        /// Rarity of the ticket.
        /// </summary>
        public TicketRarity ticketRarity;

        /// <summary>
        /// Icon that represents the rarity of the ticket.
        /// </summary>
        public Sprite icon;
    }

    /// <summary>
    /// All icons that represent the rarity of a ticket.
    /// </summary>
    [SerializeField]
    private RarityIcon[] rarityIcons;

    /// <summary>
    /// Struct that holds a model type and its corresponding Ticket icon.
    /// </summary>
    [System.Serializable]
    private struct ModelTypeIcon
    {
        /// <summary>
        /// Type of model.
        /// </summary>
        public ModelType modelType;

        /// <summary>
        /// The icon that represents the model.
        /// </summary>
        public Sprite icon;
    }   

    /// <summary>
    /// All icons that can appear on a ticket.
    /// </summary>
    [SerializeField]
    private ModelTypeIcon[] icons;

    /// <summary>
    /// All tickets and their data.
    /// </summary>
    private List<TicketData> ticketData;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the TicketFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        TicketFactory[] ticketFactories = FindObjectsOfType<TicketFactory>();
        Assert.IsNotNull(ticketFactories, "Array of TicketFactories is null.");
        Assert.AreEqual(1, ticketFactories.Length);
        instance = ticketFactories[0];
        instance.SpawnPools();
        instance.ConstructTicketData();
    }

    /// <summary>
    /// Constructs the ticket data dictionary. Eventually, we will replace
    /// this with JSON deserialization if needed.
    /// </summary>
    private void ConstructTicketData()
    {
        ticketData = new List<TicketData>();

        // Bunadryl Ticket
        TicketData bunadryl = new TicketData(
            ModelType.TICKET_BUNADRYL,
            "Bunadryl",
            new List<ModelType> { ModelType.BUNNY, ModelType.BUNNY },
            TicketRarity.ELEMENTARY,
            AbilityItemConstants.BunadrylDescription);
        AddTicketData(bunadryl);

        // Acornol Ticket
        TicketData acornol = new TicketData(
            ModelType.TICKET_ACORNOL,
            "Acornol",
            new List<ModelType> { ModelType.SQUIRREL, ModelType.SQUIRREL },
            TicketRarity.ELEMENTARY,
            AbilityItemConstants.AcornolDescription);
        AddTicketData(acornol);
    }

    /// <summary>
    /// Adds a TicketData object to the ticketData list.
    /// </summary>
    /// <param name="data">The TicketData object to add.</param>
    private void AddTicketData(TicketData data)
    {
        Assert.IsNotNull(data, "TicketData is null.");
        Assert.IsNotNull(ticketData, "TicketData list is null.");
        ticketData.ForEach(t => Assert.AreNotEqual(data.TicketType, t.TicketType));
        ticketData.Add(data);
    }

    /// <summary>
    /// Returns a fresh Ticket prefab for a given type from its object pool.
    /// </summary>
    /// <param name="modelType">The type of Ticket model to get.</param>
    /// <returns>a GameObject with a Ticket component attached to it</returns>
    public static GameObject GetTicketPrefab(ModelType modelType)
    {
        Assert.IsTrue(ModelTypeHelper.IsTicket(modelType));
        return instance.RequestObject(modelType);
    }

    /// <summary>
    /// Accepts a Ticket prefab that the caller no longer needs. Adds it back
    /// to its object pool.
    /// </summary>
    /// <param name="prefab">The Ticket prefab to return.</param>
    public static void ReturnTicketPrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the TicketFactory instance's Transform component.
    /// </summary>
    /// <returns></returns>
    protected override Transform GetTransform() { return instance.transform; }

    /// <summary>
    /// Returns the Sprite that represents the given TicketRarity.
    /// </summary>
    /// <param name="ticket">the given TicketRarity.</param>
    /// <returns>the Sprite that represents the given TicketRarity.</returns>
    public static Sprite GetRarityIcon(TicketRarity rarity)
    {
        foreach (RarityIcon rarityIcon in instance.rarityIcons)
        {
            if (rarityIcon.ticketRarity == rarity) return rarityIcon.icon;
        }
        return null;
    }

    /// <summary>
    /// Returns the Sprite that represents the given ModelType on a ticket.
    /// </summary>
    /// <param name="modelType">the given model type. </param>
    /// <returns>the Sprite that represents the given ModelType on a ticket.</returns>
    public static Sprite GetTicketIcon(ModelType modelType)
    {
        foreach (ModelTypeIcon icon in instance.icons)
        {
            if (icon.modelType == modelType) return icon.icon;
        }
        return null;
    }

    /// <summary>
    /// Returns the TicketData object for a given Ticket type.
    /// </summary>
    /// <param name="ticketType">the Ticket type.</param>
    /// <returns> the TicketData object for a given Ticket type.</returns>
    public static TicketData GetTicketData(ModelType ticketType)
    {
        Assert.IsTrue(ModelTypeHelper.IsTicket(ticketType), "ModelType is not a ticket.");
        Assert.IsTrue(instance.ticketData.Exists(t => t.TicketType == ticketType), "TicketData does not exist.");
        return instance.ticketData.Find(t => t.TicketType == ticketType);
    }

    #endregion
}
