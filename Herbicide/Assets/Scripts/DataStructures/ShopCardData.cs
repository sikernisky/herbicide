using UnityEngine.Assertions;

public class ShopCardData
{
    /// <summary>
    /// The type of Defender represented by the ShopCard.
    /// </summary>
    public ModelType DefenderType { get; }

    /// <summary>
    /// The name of the ShopCard, based on the Defender's name.
    /// </summary>
    public string CardName { get; }

    /// <summary>
    /// The cost of the card in the shop.
    /// </summary>
    public int CardCost { get; set; }

    /// <summary>
    /// Creates a new instance of ShopCardData.
    /// </summary>
    /// <param name="type">The type of Defender represented by the ShopCard.</param>
    /// <param name="name">The name of the ShopCard, based on the Defender's name.</param>
    /// <param name="cost">The cost of the card in the shop.</param>
    public ShopCardData(ModelType type, string name, int cost)
    {
        AssertValidTicketInformation(type, name, cost);
        DefenderType = type;
        CardName = name;
        CardCost = cost;
    }

    /// <summary>
    /// Asserts that the given ShopCardData information is valid.
    /// </summary>
    /// <param name="type">The type of Defender represented by the ShopCard.</param>
    /// <param name="name">The name of the ShopCard, based on the Defender's name.</param>
    /// <param name="cost">The cost of the card in the shop.</param>
    private void AssertValidTicketInformation(ModelType type, string name, int cost)
    {
        Assert.IsTrue(ModelTypeHelper.IsDefender(type), "ModelType is not a Defender.");
        Assert.IsTrue(name != null && name.Length > 0, "Name is null or empty.");
        Assert.IsTrue(cost >= 0, "Cost is negative.");
    }
}
