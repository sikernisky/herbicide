using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Controls insights; tooltips, ability icons and descriptions,
/// etc.
/// </summary>
public class InsightManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the InsightManager singleton.
    /// </summary>
    private static InsightManager instance;

    /// <summary>   
    /// Backing field for the EquippedItemImages property.
    /// </summary>
    [SerializeField]
    private Image[] _equippedItemImages;

    /// <summary>   
    /// The image components that display the equipped items.
    /// </summary>
    private Image[] EquippedItemImages { get { return _equippedItemImages; } }

    #endregion

    #region Methods

    /// <summary>
    /// Sets the InsightManager singleton.
    /// </summary>
    /// <param name="levelController">the LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        InsightManager[] insightManagers = FindObjectsOfType<InsightManager>();
        Assert.IsNotNull(insightManagers, "Array of LightManagers is null.");
        Assert.AreEqual(1, insightManagers.Length);
        instance = insightManagers[0];
        Assert.IsTrue(instance.EquippedItemImages.Length == GameConstants.MaxEquipmentSlots, "EquippedItemImages array is not the correct length.");
    }

    /// <summary>
    /// Main update loop for the InsightManager.
    /// </summary>
    /// <param name="gameState">The current game state</param>
    public static void UpdateInsightManager(GameState gameState)
    {
        if(gameState != GameState.ONGOING) return;
        instance.CheckAndHandleMouseHoverModel();
        instance.CheckAndHandleMouseExitModel();
    }

    /// <summary>
    /// Checks if the mouse has entered a model and handles the event.
    /// </summary>
    private void CheckAndHandleMouseHoverModel()
    {
        OnMouseHoverModel(InputManager.CurrentHoveredWorldModel);
    }

    /// <summary>
    /// Handles the event when the mouse hovers a Model. Called every frame.
    /// </summary>
    /// <param name="modelHovering">the Model that the mouse is hovering.</param>
    private void OnMouseHoverModel(Model modelHovering)
    {
        if (modelHovering == null) return;
        DisplayEquipmentImages(modelHovering);
    }

    /// <summary>
    /// Activates, positions, and defines the dummy equipment images
    /// such that they display a Model's list of Equipment.
    /// </summary>
    /// <param name="modelHovering">the Model to display Equipment for.</param>
    private void DisplayEquipmentImages(Model modelHovering)
    {
        if (modelHovering == null) return;
        List<ModelType> equippedItems = modelHovering.EquippedItems;        
        for(int i = 0; i < equippedItems.Count; i++)
        {
            EquippedItemImages[i].gameObject.SetActive(true);
            EquippedItemImages[i].sprite = InventoryFactory.GetInventoryItemIcon(equippedItems[i]);
            EquippedItemImages[i].transform.position = Camera.main.WorldToScreenPoint(modelHovering.transform.position + UIConstants.InsightEquipmentImageScreenOffset);
        }
    }

    /// <summary>
    /// Checks if the mouse has exited a model and handles the event.
    /// </summary>
    private void CheckAndHandleMouseExitModel()
    {
        if (InputManager.TryGetPreviouslyHoveredWorldModel<Model>(out var modelExited)) OnMouseExitModel(modelExited);
    }

    /// <summary>
    /// Handles the event when the mouse exits a Model.
    /// </summary>
    /// <param name="modelExited">the Model that the mouse exited.</param>
    private void OnMouseExitModel(Model modelExited)
    {
        if (modelExited == null) return;
        HideEquipmentImages(modelExited);
    }

    /// <summary>
    /// Hides the dummy equipment images.
    /// </summary>
    /// <param name="modelExited">the Model to hide Equipment for.</param>
    private void HideEquipmentImages(Model modelExited)
    {
        if (modelExited == null) return;
        for(int i = 0; i < EquippedItemImages.Length; i++)
        {
            EquippedItemImages[i].gameObject.SetActive(false);
        }
    }


    #endregion
}
