using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A SeedToken is a type of currency dropped by Enemies.
/// </summary>
public class SeedToken : Currency
{
    /// <summary>
    /// Name of a SeedToken.
    /// </summary>
    public override string NAME => "Seed Token";

    /// <summary>
    /// CollectableType of a SeedToken.
    /// </summary>
    public override CollectableType TYPE => CollectableType.SEED_TOKEN;

    /// <summary>
    /// Returns the Sprite component that represents this SeedToken in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this SeedToken in
    /// the Inventory.</returns>
    public override Sprite GetInventorySprite()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a Sprite that represents this SeedToken when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this SeedToken when it is
    /// being placed.</returns>
    public override Sprite GetPlacementSprite()
    {
        throw new System.NotImplementedException();
    }
}
