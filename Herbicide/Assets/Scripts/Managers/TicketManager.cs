using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages all tickets in the game.
/// </summary>
public class TicketManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the TicketManager singleton.
    /// </summary>
    private static TicketManager Instance { get; set; }

    /// <summary>
    /// The number of completed tickets since the start of the game.
    /// </summary>
    public static int NumCompletedTickets { get; private set; }

    /// <summary>
    /// The spawned tickets in the scene that the player can complete.
    /// </summary>
    private List<Ticket> ActiveTickets { get; set; }

    /// <summary>
    /// The tickets that should be active and loaded this level.
    /// </summary>
    private List<ModelType> TicketsToLoad { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the TicketManager singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        TicketManager[] ticketManagers = FindObjectsOfType<TicketManager>();
        Assert.IsNotNull(ticketManagers, "Array of TicketManagers is null.");
        Assert.AreEqual(1, ticketManagers.Length);
        Instance = ticketManagers[0];
        Instance.ActiveTickets = new List<Ticket>();
    }

    /// <summary>
    /// Main update loop for the TicketManager.
    /// </summary>
    /// <param name="gameState">The most recent GameState. </param>
    public static void UpdateTicketManager(GameState gameState)
    {
        Instance.CheckAndHandleTicketHoverEvents();
    }

    /// <summary>
    /// Checks for and handles ticket hover events.
    /// </summary>
    private void CheckAndHandleTicketHoverEvents()
    {
        if(InputManager.TryGetPreviouslyHoveredUIModel<Ticket>(out var ticketExited)) OnMouseExitTicket(ticketExited);
        if (InputManager.TryGetNewlyHoveredUIModel<Ticket>(out var ticketEntered)) OnMouseEnterTicket(ticketEntered);
    }

    /// <summary>
    /// Called when a ticket is hovered over.
    /// </summary>
    /// <param name="ticket">the ticket that was hovered over.</param>
    private void OnMouseEnterTicket(Ticket ticket)
    {
        if(ticket == null) return;
        ticket.DisplayDescriptionAndHideIcons();
    }

    /// <summary>
    /// Called when a ticket is no longer hovered over.
    /// </summary>
    /// <param name="ticket">the ticket that was exited.</param>
    private void OnMouseExitTicket(Ticket ticket)
    {
        if(ticket == null) return;
        ticket.DisplayIconsAndHideDescription();
    }

    /// <summary>
    /// Returns the number of satisfied tickets in the scene.
    /// </summary>
    /// <returns>the number of satisfied tickets in the scene.</returns>
    public static int GetNumberOfSatisfiedTickets()
    {
        int satisfiedTickets = 0;
        foreach (Ticket ticket in Instance.ActiveTickets)
        {
            if (ticket.IsComplete()) satisfiedTickets++;
        }
        return satisfiedTickets;
    }

    /// <summary>
    /// Informs the Tickets that a Model has been purchased or used from the 
    /// Inventory. Updates its Tickets accordingly.
    /// </summary>
    /// <param name="purchasedModelType">The ModelType that was purchased.</param>
    public static void OnPurchaseOrUseModel(ModelType purchasedModelType) {

        Instance.ActiveTickets.ForEach(t => t.ProcessObtainedModel(purchasedModelType));
        Instance.UpdateActiveTicketPositionsAndScale();
    }

    /// <summary>
    /// Updates the positions of all active tickets in the scene based on
    /// their index in the activeTickets queue.
    /// </summary>
    private void UpdateActiveTicketPositionsAndScale()
    {
        SortActiveTicketsBasedOnCompletion();
        for (int i = 0; i < ActiveTickets.Count; i++)
        {
            Vector2 position = UIConstants.FirstTicketPosition + i * UIConstants.TicketPositionOffset;
            ActiveTickets[i].transform.localPosition = position;
            ActiveTickets[i].transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Adds all available tickets to the availableTickets HashSet. Currently,
    /// this represents all unlocked tickets.
    /// </summary>
    public static void InitializeTickets()
    {
        Instance.UnlockTickets();
        Instance.GatherUnlockedTickets();
        Instance.SpawnTickets();
        Instance.SupplyTicketsWithEvents();
        Instance.UpdateActiveTicketPositionsAndScale();
    }

    /// <summary>
    /// Unlocks all tickets in the game. This will be replaced
    /// with a more sophisticated unlocking system in the future.
    /// </summary>
    private void UnlockTickets()
    {
        CollectionManager.UnlockModel(ModelType.TICKET_ACORNOL);
        if (SaveLoadManager.GetLoadedGameLevel() == 1) CollectionManager.UnlockModel(ModelType.TICKET_BUNADRYL);
    }

    /// <summary>
    /// Sorts the active tickets based on their completion status such that
    /// the most completed tickets are first in the list of active tickets.
    /// </summary>
    private void SortActiveTicketsBasedOnCompletion() =>
        ActiveTickets.Sort((t1, t2) =>
        {
            // Prioritize completed tickets first
            if (t1.IsComplete() && !t2.IsComplete()) return -1;
            if (!t1.IsComplete() && t2.IsComplete()) return 1;

            // If both are complete or both are incomplete, sort by NumRequirementsSatisfied descending
            return t2.NumRequirementsSatisfied().CompareTo(t1.NumRequirementsSatisfied());
        });

    /// <summary>
    /// Loads all tickets that should be active during this level.
    /// </summary>
    private void GatherUnlockedTickets()
    {
        Instance.TicketsToLoad = new List<ModelType>();
        foreach (ModelType unlockedTicket in CollectionManager.GetAllUnlockedTickets()) { Instance.TicketsToLoad.Add(unlockedTicket); }
        foreach (ModelType ticket in Instance.TicketsToLoad) { Assert.IsTrue(ModelTypeHelper.IsTicket(ticket), "ModelType is not a ticket."); }
    }

    /// <summary>
    /// Spawns all loaded tickets that should exist during this level. 
    /// </summary>
    private void SpawnTickets()
    { 
        for(int i = 0; i < TicketsToLoad.Count; i++)
        {
            GameObject ticketPrefab = TicketFactory.GetTicketPrefab(ModelType.TICKET);
            ticketPrefab.transform.SetParent(transform);
            Ticket ticketComp = ticketPrefab.GetComponent<Ticket>();
            Assert.IsNotNull(ticketComp, "Ticket component is null.");
            ActiveTickets.Add(ticketComp);
            ticketComp.DefineAndResetTicket(TicketFactory.GetTicketData(TicketsToLoad[i]));
        }
    }

    /// <summary>
    /// Supplies all active tickets with their respective on-click events.
    /// </summary>
    private void SupplyTicketsWithEvents()
    {
        Assert.IsNotNull(ActiveTickets, "Active tickets is null.");

        foreach(Ticket ticket in ActiveTickets)
        {
            Assert.IsNotNull(ticket, "Ticket is null.");
            ticket.SupplyTicketWithOnClickEvents(OnTicketClick, PostChooseCompletedTicket);
        }
    }

    /// <summary>
    /// Called after a ticket is selected to be added to the
    /// Inventory.
    /// </summary>
    /// <param name="ticket">the clicked ticket</param>
    private void PostChooseCompletedTicket(Ticket ticket)
    {
        foreach(Ticket t in ActiveTickets)
        {
            if(t.IsComplete()) InventoryManager.CompleteTicket(t);
            t.ResetTicket();
            NumCompletedTickets++;
        }
    }

    /// <summary>
    /// Called when a ticket is clicked. Future idea: create ticket
    /// specific methods if a ticket has an effect when it's crafted
    /// that is different from the other tickets.
    /// </summary>
    /// <param name="ticket">the clicked ticket.</param>
    private void OnTicketClick(Ticket ticket) { }

    #endregion
}
