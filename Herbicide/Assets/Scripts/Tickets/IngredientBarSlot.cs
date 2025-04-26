using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Displays an ingredient that the player has submitted.
/// </summary>
public class IngredientBarSlot : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// true if the slot is empty, false otherwise.
    /// </summary>
    public bool IsEmpty => !iconImage.enabled;

    /// <summary>
    /// The icon image of the ingredient.
    /// </summary>
    [SerializeField]
    private Image iconImage;

    #endregion

    /// <summary>
    /// Sets the icon of the ingredient.
    /// </summary>
    /// <param name="icon">the icon to set.</param>
    public void SetIcon(Sprite icon)
    {
        Assert.IsNotNull(icon, "Icon is null.");
        iconImage.sprite = icon;
        iconImage.enabled = true;
    }

    /// <summary>
    /// Clears the icon of the ingredient.
    /// </summary>
    public void ClearIcon()
    {
        iconImage.enabled = false;
        iconImage.sprite = null;
    }
}
