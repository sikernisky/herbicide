using System;
using System.Collections.Generic;
using StageOfDay = StageController.StageOfDay;

/// <summary>
/// Helper class for the ModelType enum.
/// </summary>
public static class ModelTypeHelper {

    #region Fields

    /// <summary>
    /// Dictionary of purchasable stages for each ModelType.
    /// </summary>
    private static readonly Dictionary<ModelType, List<StageOfDay>> PURCHASABLE_STAGES = new Dictionary<ModelType, List<StageOfDay>>
    {
        { ModelType.SQUIRREL, new List<StageOfDay> { StageOfDay.MORNING, StageOfDay.NOON, StageOfDay.EVENING, StageOfDay.NIGHT } },
        { ModelType.BEAR, new List<StageOfDay> { StageOfDay.MORNING, StageOfDay.NOON, StageOfDay.EVENING, StageOfDay.NIGHT } },
        { ModelType.BUNNY, new List<StageOfDay> { StageOfDay.MORNING, StageOfDay.NOON, StageOfDay.EVENING, StageOfDay.NIGHT } },
        { ModelType.PORCUPINE, new List<StageOfDay> { StageOfDay.MORNING, StageOfDay.NOON, StageOfDay.EVENING, StageOfDay.NIGHT } },
        { ModelType.RACCOON, new List<StageOfDay> { StageOfDay.MORNING, StageOfDay.NOON, StageOfDay.EVENING, StageOfDay.NIGHT } },
        { ModelType.OWL, new List<StageOfDay> { StageOfDay.MORNING, StageOfDay.EVENING, StageOfDay.NIGHT } }
    };

    #endregion

    #region Methods

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
    /// Returns the ModelType based on the given ShopCard ModelType.
    /// </summary>
    /// <param name="shopCardModelType">The ShopCard ModelType to convert to a ModelType.</param>
    /// <returns>the ModelType based on the given ShopCard ModelType.</returns>
    public static ModelType GetModelTypeFromShopCardModelType(ModelType shopCardModelType)
    {
        HashSet<ModelType> modelTypes = new HashSet<ModelType>
        {
            ModelType.SQUIRREL,
            ModelType.BEAR,
            ModelType.BUNNY,
            ModelType.PORCUPINE,
            ModelType.RACCOON,
            ModelType.OWL,
        };

        switch (shopCardModelType)
        {
            case ModelType.SHOP_CARD_SQUIRREL:
                return ModelType.SQUIRREL;
            case ModelType.SHOP_CARD_BEAR:
                return ModelType.BEAR;
            case ModelType.SHOP_CARD_BUNNY:
                return ModelType.BUNNY;
            case ModelType.SHOP_CARD_PORCUPINE:
                return ModelType.PORCUPINE;
            case ModelType.SHOP_CARD_RACCOON:
                return ModelType.RACCOON;
            case ModelType.SHOP_CARD_OWL:
                return ModelType.OWL;
            default:
                break;
        }

        throw new System.Exception("ModelType " + shopCardModelType + " does not have a corresponding ModelType.");
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

    /// <summary>
    /// Returns true if the given ModelType can be purchased during the given stage; false otherwise.
    /// </summary>
    /// <param name="modelType">The ModelType to check.</param>
    /// <param name="stage">The stage to check.</param>
    /// <returns>true if the given ModelType can be purchased during the given stage; false otherwise.</returns>
    public static bool CanPurchaseDuringStage(ModelType modelType, StageOfDay stage)
    {
        if(!PURCHASABLE_STAGES.ContainsKey(modelType)) return false;
        return PURCHASABLE_STAGES[modelType].Contains(stage);
    }

    #endregion
}
