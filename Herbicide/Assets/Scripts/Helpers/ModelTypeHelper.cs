using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for the ModelType enum.
/// </summary>
public static class ModelTypeHelper {

    /// <summary>
    /// Converts a string to the corresponding ModelType enum.
    /// </summary>
    /// <param name="modelTypeString">The string to convert.</param>
    public static ModelType ConvertStringToModelType(string modelTypeString)
    {
        if (Enum.TryParse(modelTypeString, true, out ModelType result))
        {
            return result;
        }
        throw new ArgumentException($"Invalid ModelType: {modelTypeString}");
    }

    /// <summary>
    /// Returns the ShopCard ModelType based on the given ModelType.
    /// </summary>
    /// <param name="modelType">The ModelType to convert to a ShopCard ModelType.</param>
    /// <returns>the ShopCard ModelType based on the given ModelType.</returns>
    public static ModelType GetShopCardModelTypeFromModelType(ModelType modelType)
    {
        HashSet<ModelType> shopCardTypes = new HashSet<ModelType>
        {
            ModelType.SHOP_CARD_SQUIRREL,
            ModelType.SHOP_CARD_BEAR,
            ModelType.SHOP_CARD_BUNNY,
            ModelType.SHOP_CARD_PORCUPINE,
            ModelType.SHOP_CARD_RACCOON,
            ModelType.SHOP_CARD_OWL,
            ModelType.SHOP_CARD_BLANK
        };

        if (shopCardTypes.Contains(modelType)) return modelType;

        switch (modelType)
        {
            case ModelType.SQUIRREL:
                return ModelType.SHOP_CARD_SQUIRREL;
            case ModelType.BEAR:
                return ModelType.SHOP_CARD_BEAR;
            case ModelType.BUNNY:
                return ModelType.SHOP_CARD_BUNNY;
            case ModelType.PORCUPINE:
                return ModelType.SHOP_CARD_PORCUPINE;
            case ModelType.RACCOON:
                return ModelType.SHOP_CARD_RACCOON;
            case ModelType.OWL:
                return ModelType.SHOP_CARD_OWL;
            default:
                break;
        }

        throw new System.Exception("ModelType " + modelType + " does not have a corresponding ShopCard ModelType.");
    }

    /// <summary>
    /// Returns true if the given ModelType is a defender; false otherwise.
    /// </summary>
    /// <param name="modelType">The ModelType to check.</param>
    /// <returns>true if the given ModelType is a defender; false otherwise.</returns>
    public static bool IsDefender(ModelType modelType)
    {
        HashSet<ModelType> defenderTypes = new HashSet<ModelType>
        {
            ModelType.SQUIRREL,
            ModelType.BEAR,
            ModelType.BUNNY,
            ModelType.PORCUPINE,
            ModelType.RACCOON,
            ModelType.OWL
        };

        return defenderTypes.Contains(modelType);
    }

}
