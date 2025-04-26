using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Represents a card that the player attempts to activate
/// during the game by completing certain actions.
/// </summary>
public class Ticket : UIModel
{
    #region Fields

    /// <summary>
    /// The data that defines this Ticket.
    /// </summary>
    public TicketData TicketData { get; private set; }

    /// <summary>
    /// The index of the current ModelType the player must obtain.
    /// </summary>
    private int CurrentIndex { get; set; }

    /// <summary>
    /// true if the Ticket is defined; otherwise, false.
    /// </summary>
    public bool IsDefined => TicketData != null;

    /// <summary>
    /// The icons that represent the ModelTypes the player must
    /// obtain to make progress on this ticket. Ordered from left
    /// to right.
    /// </summary>
    private List<TicketIcon> TicketIcons;

    /// <summary>
    /// The text that displays the description of this ticket.
    /// </summary>
    [SerializeField]
    private TMP_Text descriptionText;

    /// <summary>
    /// The image that represents the rarity of this ticket.
    /// </summary>
    [SerializeField]
    private Image rarityIcon;

    /// <summary>
    /// The splash image that represents this ticket.
    /// </summary>
    [SerializeField]
    private Image splashImage;

    /// <summary>
    /// The image that represents the shadow of the splash image.
    /// </summary>
    [SerializeField]
    private Image splashShadowImage;

    /// <summary>
    /// The Ticket's button component.
    /// </summary>
    [SerializeField]
    private Button button;

    /// <summary>
    /// The text component that displays the name of this ticket.
    /// </summary>
    [SerializeField]
    private TMP_Text nameText;

    /// <summary>
    /// Event that is invoked when the Ticket is clicked.
    /// </summary>
    private UnityEvent<Ticket> onTicketClicked;

    #endregion

    #region Methods

    /// <summary>
    /// Defines this Ticket with the given data. Resets the Ticket.
    /// </summary>
    /// <param name="ticketData">the data that will define this Ticket.</param>
    public void DefineAndResetTicket(TicketData ticketData)
    {
        Assert.IsNotNull(ticketData, "TicketData is null.");
        TicketData = ticketData;
        SetTicketSplash();
        SetRarityIcon();
        SetDisplayName();
        SetupRequirementIcons();
        ResetTicket();
    }

    /// <summary>
    /// Sets the ticket to its initial state.
    /// </summary>
    public void ResetTicket()
    {
        Assert.IsTrue(IsDefined, "Ticket is not defined.");
        ResetRequirementIndex();
        SetIncomplete();
        DarkenRequirementIcons();
    }

    /// <summary>
    /// Sets the splash image of this ticket.
    /// </summary>
    private void SetTicketSplash()
    {
        Sprite splashArt = TicketFactory.GetTicketIcon(TicketData.TicketType);
        splashImage.sprite = splashArt;
        splashShadowImage.sprite = splashArt;
    }

    /// <summary>
    /// Sets the rarity icon of this ticket.
    /// </summary>
    private void SetRarityIcon() => rarityIcon.sprite = TicketFactory.GetRarityIcon(TicketData.Rarity);

    /// <summary>
    /// Sets the name of this ticket.
    /// </summary>
    private void SetDisplayName() => nameText.text = TicketData.TicketName;

    /// <summary>
    /// Sets the requirement index to 0.
    /// </summary>
    private void ResetRequirementIndex() => CurrentIndex = 0;

    /// <summary>
    /// Spawns the necessary amount of icons for this ticket.
    /// Positions them in the correct order. Sets the splash for each icon.
    /// </summary>
    private void SetupRequirementIcons()
    {
        ClearRequirementIcons();
        for(int i = 0; i < TicketData.Requirements.Count; i++)
        {
            GameObject ticketIconOb = TicketFactory.GetTicketPrefab(ModelType.TICKET_ICON);
            TicketIcon ticketIcon = ticketIconOb.GetComponent<TicketIcon>();
            SetRequirementIconPosition(i, ticketIcon);
            SetRequirementIconSplash(i, ticketIcon);
            AddTicketIcon(ticketIcon);
        }
    }

    /// <summary>
    /// Returns all icons to the object pool and clears the list of icons.
    /// </summary>
    private void ClearRequirementIcons()
    {
        if(TicketIcons == null) TicketIcons = new List<TicketIcon>();
        TicketIcons.ForEach(i => TicketFactory.ReturnTicketPrefab(i.gameObject));  
        TicketIcons.Clear();
    }

    /// <summary>
    /// Adds the given icon to the list of icons for this ticket.
    /// </summary>
    /// <param name="iconToAdd">the icon to add.</param>
    private void AddTicketIcon(TicketIcon iconToAdd)
    {
        Assert.IsNotNull(iconToAdd, "Icon to add is null.");
        Assert.IsNotNull(TicketIcons, "TicketIcons is null.");
        Assert.IsTrue(TicketData.Requirements.Count > TicketIcons.Count, "TicketIcons is full.");
        TicketIcons.Add(iconToAdd);
    }

    /// <summary>
    /// Sets the position of the requirement icon.
    /// </summary>
    /// <param name="requirementIndex">the index of the icon in the requirements list.</param>
    /// <param name="ticketIcon">the icon to position.</param>
    private void SetRequirementIconPosition(int requirementIndex, TicketIcon ticketIcon)
    {
        Assert.IsNotNull(ticketIcon, "TicketIcon is null.");
        Assert.IsTrue(IsValidRequirementIndex(requirementIndex), "Index out of range.");
        ticketIcon.transform.SetParent(transform);
        float xPos = UIConstants.TicketIconStartPosition.x + requirementIndex * UIConstants.TicketIconOffset.x;
        float yPos = UIConstants.TicketIconStartPosition.y + requirementIndex * UIConstants.TicketIconOffset.y;
        ticketIcon.transform.localPosition = new Vector3(xPos, yPos);
    }

    /// <summary>
    /// Sets the splashes of the requirement icons of this ticket.
    /// </summary>
    /// <param name="requirementIndex">the index of the icon in the requirements list.</param>
    /// <param name="ticketIcon">The ticket icon to set the splash for.</param>
    private void SetRequirementIconSplash(int requirementIndex, TicketIcon ticketIcon)
    {
        Assert.IsNotNull(ticketIcon, "TicketIcon is null.");
        Assert.IsTrue(IsValidRequirementIndex(requirementIndex), "Index out of range.");
        Sprite splash = TicketFactory.GetTicketIcon(TicketData.Requirements[requirementIndex]);
        ticketIcon.SetSplash(splash);
    }

    /// <summary>
    /// Returns whether the given requirement index is valid.
    /// </summary>
    /// <param name="requirementIndex">the index to check</param>
    /// <returns>true if the given requirement index is valid; otherwise, false. </returns>
    private bool IsValidRequirementIndex(int requirementIndex) => TicketData.Requirements.Count > requirementIndex && requirementIndex >= 0;

    /// <summary>
    /// Changes this Ticket's appearance to indicate that it is incomplete.
    /// </summary>
    private void SetIncomplete()
    {
        Assert.IsFalse(IsComplete());
        button.interactable = false;
        GetComponent<Image>().color = Color.white;
    }

    /// <summary>
    /// Returns whether the ticket is complete.
    /// </summary>
    /// <returns>true if the ticket is complete, false otherwise.</returns>
    public bool IsComplete() => IsDefined && CurrentIndex == TicketData.Requirements.Count;

    /// <summary>
    /// Darkens all icons.
    /// </summary>
    private void DarkenRequirementIcons() => TicketIcons.ForEach(i => i.Darken());

    /// <summary>
    /// Changes this Ticket's appearance to indicate that it is complete.
    /// </summary>
    private void DisplayAsComplete()
    {
        Assert.IsTrue(IsComplete());
        button.interactable = true;
        GetComponent<Image>().color = Color.green;
    }

    /// <summary>
    /// Processes the ModelType that the player has obtained. Makes
    /// progress on the ticket if the ModelType is the next required
    /// ModelType.
    /// </summary>
    /// <param name="modelType">The ModelType the player has obtained.</param>
    public void ProcessObtainedModel(ModelType modelType)
    {
        if (!IsDefined) return;
        if(IsComplete()) return;
        if (TicketData.Requirements[CurrentIndex] != modelType) return;
        TicketIcons[CurrentIndex].LightUp();
        CurrentIndex++;
        if (IsComplete()) DisplayAsComplete();
    }

    /// <summary>
    /// Gives this Ticket methods to execute when it is clicked.
    /// </summary>
    /// <param name="onClickAction">the method to execute on click.</param>
    /// <param name="postClickAction">the method to execute after the click.</param>
    public void SupplyTicketWithOnClickEvents(UnityAction<Ticket> onClickAction, UnityAction<Ticket> postClickAction)
    {
        Assert.IsTrue(IsDefined, "Ticket is not defined.");
        Assert.IsNotNull(onClickAction, "Action is null.");
        Assert.IsNotNull(button, "Button is null.");
        if (onTicketClicked == null) onTicketClicked = new UnityEvent<Ticket>();
        SetupButtonListeners(onClickAction, postClickAction);
    }

    /// <summary>
    /// Sets up the button listeners.
    /// </summary>
    private void SetupButtonListeners(UnityAction<Ticket> onClickAction, UnityAction<Ticket> postClickAction)
    {
        onTicketClicked.RemoveAllListeners();
        onTicketClicked.AddListener(onClickAction);
        onTicketClicked.AddListener(postClickAction);
        button.onClick.AddListener(() => onTicketClicked.Invoke(this));
    }

    /// <summary>
    /// Returns the number of requirements satisfied.
    /// </summary>
    /// <returns>the number of requirements satisfied.</returns>
    public int NumRequirementsSatisfied() => CurrentIndex;

    /// <summary>
    /// Returns the ModelType of this UIModel.
    /// </summary>
    /// <returns>the ModelType of this UIModel.</returns>
    public override ModelType GetModelType() => ModelType.TICKET;

    /// <summary>
    /// Hides all TicketIcons and displays the description of this ticket.
    /// </summary>
    public void DisplayDescriptionAndHideIcons()
    {
        TicketIcons.ForEach(i => i.Hide());
        descriptionText.enabled = true;
        descriptionText.text = TicketData.Description;

    }

    /// <summary>
    /// Removes the description of this ticket and displays the icons.
    /// </summary>
    public void DisplayIconsAndHideDescription()
    {
        descriptionText.text = string.Empty;
        descriptionText.enabled = false;
        TicketIcons.ForEach(i => i.Show());
    }

    #endregion
}
