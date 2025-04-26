using System.Collections.Generic;
using UnityEngine.Assertions;

/// <summary>
/// Stores data related to a ticket.
/// </summary>
public class TicketData
{
    /// <summary>
    /// The type of ticket.
    /// </summary>
    public ModelType TicketType { get; }

    /// <summary>
    /// The name of the ticket.
    /// </summary>
    public string TicketName { get; }

    /// <summary>
    /// The description of the ticket.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The requirements for the ticket.
    /// </summary>
    public List<ModelType> Requirements { get; }

    /// <summary>
    /// The rarity of the ticket.
    /// </summary>
    public TicketRarity Rarity { get; }

    /// <summary>
    /// Creates a new instance of TicketData.
    /// </summary>
    /// <param name="type">The type of ticket.</param>
    /// <param name="name">The name of the ticket.</param>
    /// <param name="requirements">The requirements for the ticket.</param>
    /// <param name="rarity">The rarity of the ticket.</param>
    /// <param name="description">The description of the ticket.</param>
    public TicketData(ModelType type, string name, List<ModelType> requirements, TicketRarity rarity, string description)
    {
        AssertValidTicketInformation(type, name, requirements, description);
        TicketType = type;
        TicketName = name;
        Requirements = requirements;
        Rarity = rarity;
        Description = description;
    }

    /// <summary>
    /// Asserts that the given ticket information is valid.
    /// </summary>
    /// <param name="type">The ModelType of this Ticket.</param>
    /// <param name="name">The name of this Ticket.</param>
    /// <param name="requirements">The requirements of this Ticket.</param>
    /// <param name="rarity">The rarity of this Ticket.</param>
    /// <param name="description">The description of this Ticket.</param>
    private void AssertValidTicketInformation(ModelType type, string name, List<ModelType> requirements, string description)
    {
        Assert.IsTrue(ModelTypeHelper.IsTicket(type), "ModelType is not a Ticket.");
        Assert.IsTrue(name != null && name.Length > 0, "Name is null or empty.");
        Assert.IsNotNull(requirements, "Requirements is null.");
        Assert.IsTrue(requirements.Count > 0, "Requirements is empty.");
        Assert.IsNotNull(description, "Description is null.");
    }
}
