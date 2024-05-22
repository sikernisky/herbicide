using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System;

/// <summary>
/// Handles economy and currency related events.
/// </summary>
public class EconomyController : MonoBehaviour
{

    /// <summary>
    /// Upper bound of how much money the player can have at once.
    /// </summary>
    public const int MAX_MONEY = 9999;

    /// <summary>
    /// Lower bound of how much money the player can have at once.
    /// </summary>
    public const int MIN_MONEY = 0;

    /// <summary>
    /// Text component that displays the current amount of currency.
    /// </summary>
    [SerializeField]
    private TMP_Text currencyText;

    /// <summary>
    /// Reference to the EconomyController singleton.
    /// </summary>
    private static EconomyController instance;

    /// <summary>
    /// How much money the player has.
    /// </summary>
    private static int currentMoney;

    /// <summary>
    /// Starting money for this level. (TODO: Data-Driven Design)
    /// </summary>
    [SerializeField]
    private int startingMoney;

    /// <summary>
    /// How much currency the player gets per tick.
    /// </summary>
    private static readonly int PASSIVE_INCOME_AMOUNT = 10;

    /// <summary>
    /// The number of seconds the player must wait until they
    /// recieve another passive income tick.
    /// </summary>
    private static readonly float PASSIVE_INCOME_FREQUENCY = 10f;

    /// <summary>
    /// Number of seconds since the last passive income tick occured.
    /// </summary>
    private float timeSinceLastPassiveIncomeTick;

    /// <summary>
    /// The most recent GameState.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// Finds and sets the EconomyController singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        EconomyController[] economyControllers = FindObjectsOfType<EconomyController>();
        Assert.IsNotNull(economyControllers, "Array of EconomyControllers is null.");
        Assert.AreEqual(1, economyControllers.Length);
        instance = economyControllers[0];
        currentMoney = instance.startingMoney;
    }

    /// <summary>
    /// Checks for and collects any currency the player clicked on.
    /// </summary>
    public static void CheckCurrencyPickup() { return; }

    /// <summary>
    /// Main update loop for the EconomyController.<br></br>
    /// 
    /// (1) Updates all active Currencies in the game.<br></br>
    /// (2) Displays the current currency to a text component.
    /// </summary>
    public static void UpdateEconomy()
    {
        instance.UpdateCurrencyText();
        instance.PassiveIncome();
    }

    /// <summary>
    /// Updates the value and size of the text component that displays
    /// the player's current currency balance.
    /// </summary>
    private void UpdateCurrencyText()
    {
        currencyText.text = currentMoney.ToString();
        if (GetBalance() > 999) currencyText.fontSize = 13;
        else currencyText.fontSize = 16;
    }

    /// <summary>
    /// Adds to the player's currency balance.
    /// </summary>
    /// <param name="amount">How much to add.</param>
    private static void Deposit(int amount)
    {
        int incremented = GetBalance() + amount;
        currentMoney = Mathf.Clamp(incremented, MIN_MONEY, MAX_MONEY);
    }

    /// <summary>
    /// Cashes in a currency, potentially modifying the player's
    /// balance.
    /// </summary>
    /// <param name="currency">The currency to cash in.</param>
    public static void CashIn(Currency currency)
    {
        Assert.IsNotNull(currency, "Currency is null.");
        Deposit(currency.GetValue());
    }

    /// <summary>
    /// Removes some amount of money from the player's balance.
    ///</summary>
    /// <param name="amount">The amount of money to remove. </param>
    public static void Withdraw(int amount)
    {
        Assert.IsTrue(amount >= 0);
        currentMoney = Math.Clamp(currentMoney - amount, 0, int.MaxValue);
    }

    /// <summary>
    /// Returns the amount of money the player currently has.
    ///  </summary>
    /// <returns>how much money the player has.</returns>
    public static int GetBalance() { return currentMoney; }

    /// <summary>
    /// Informs the EconomyController of the most recent GameState.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public static void InformOfGameState(GameState gameState)
    {
        instance.gameState = gameState;
    }

    /// <summary>
    /// Updates the passive income counter and awards the player currency
    /// if it is time.
    /// </summary>
    private void PassiveIncome()
    {
        if (gameState != GameState.ONGOING) return;

        timeSinceLastPassiveIncomeTick += Time.deltaTime;

        if (timeSinceLastPassiveIncomeTick >= PASSIVE_INCOME_FREQUENCY)
        {
            timeSinceLastPassiveIncomeTick = 0;
            Deposit(PASSIVE_INCOME_AMOUNT);
        }
    }
}
