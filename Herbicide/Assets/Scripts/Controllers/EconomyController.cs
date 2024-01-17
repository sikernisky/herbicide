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
    public const int MAX_MONEY = 100;

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
    /// All active Currency objects.
    /// </summary>
    private HashSet<Currency> activeCurrency;

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
        instance.activeCurrency = new HashSet<Currency>();
        currentMoney = instance.startingMoney;
    }

    /// <summary>
    /// Checks for and collects any currency the player clicked on.
    /// </summary>
    public static void CheckCurrencyPickup()
    {
        // Collectable c = InputController.CollectableClickedUp();
        // Currency curr = c as Currency;
        // if (curr == null) return;

        // //Handle logic here.
        // Assert.IsTrue(instance.activeCurrency.Contains(curr));
        // curr.OnCollect();
        // instance.activeCurrency.Remove(curr);
        // Deposit(curr.BASE_VALUE);
        // Destroy(curr.gameObject);
    }

    /// <summary>
    /// Main update loop for the EconomyController.<br></br>
    /// 
    /// (1) Updates all active Currencies in the game.<br></br>
    /// (2) Displays the current currency to a text component.
    /// </summary>
    public static void UpdateEconomy()
    {
        instance.currencyText.text = currentMoney.ToString();
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
}
