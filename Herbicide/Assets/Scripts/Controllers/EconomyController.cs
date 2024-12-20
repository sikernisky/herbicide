using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Controls player balance and currency related events.
/// </summary>
public class EconomyController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The bank of currencies. 
    /// </summary>
    private static Dictionary<ModelType, int> currencies;

    /// <summary>
    /// Text component that displays the current amount of Dew.
    /// </summary>
    [SerializeField]
    private TMP_Text dewBalanceText;

    /// <summary>
    /// Text component that displays the current amount of BasicTreeSeeds.
    /// </summary>
    [SerializeField]
    private TMP_Text basicTreeSeedBalanceText;

    /// <summary>
    /// Text component that displays the current amount of SpeedTreeSeeds.
    /// </summary>
    [SerializeField]
    private TMP_Text speedTreeSeedBalanceText;

    /// <summary>
    /// Reference to the EconomyController singleton.
    /// </summary>
    private static EconomyController instance;

    /// <summary>
    /// Defines a delegate for when the player's balance is updated.
    /// </summary>
    public delegate void BalanceUpdatedDelegate();

    /// <summary>
    /// Event triggered when the player's balance is updated.
    /// </summary>
    public event BalanceUpdatedDelegate OnBalanceUpdated;

    /// <summary>
    /// How much Dew the player gets per tick.
    /// </summary>
    private static readonly int DEW_PASSIVE_INCOME = 25;

    /// <summary>
    /// The number of seconds the player must wait until they
    /// recieve another Dew passive income tick.
    /// </summary>
    private static readonly float DEW_PASSIVE_INCOME_FREQUENCY = 30f;

    /// <summary>
    /// Number of seconds since the last passive income tick occured.
    /// </summary>
    private float timeSinceLastDewPassiveIncomeTick;

    /// <summary>
    /// true if passive income is enabled; otherwise, false.
    /// </summary>
    private bool passiveIncomeEnabled;

    /// <summary>
    /// The most recent GameState.
    /// </summary>
    private GameState gameState;

    #endregion

    #region Methods

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
        instance.passiveIncomeEnabled = false;
    }

    /// <summary>
    /// Subscribes to the SaveLoadManager's OnLoadRequested and OnSaveRequested events.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SubscribeToSaveLoadEvents(LevelController levelController)
    {
        Assert.IsNotNull(levelController, "LevelController is null.");

        SaveLoadManager.SubscribeToToLoadEvent(instance.LoadEconomyData);
        SaveLoadManager.SubscribeToToSaveEvent(instance.SaveEconomyData);
    }

    /// <summary>
    /// Main update loop for the EconomyController.<br></br>
    /// 
    /// (1) Updates all active Currencies in the game.<br></br>
    /// (2) Displays the current currency to a text component.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public static void UpdateEconomy(GameState gameState)
    {
        instance.gameState = gameState;
        instance.UpdateCurrencyText();
        if(instance.passiveIncomeEnabled) instance.UpdatePassiveIncome();
    }

    /// <summary>
    /// Updates the values of the currency text components.
    /// </summary>
    private void UpdateCurrencyText()
    {
        dewBalanceText.text = currencies[ModelType.DEW].ToString();
        basicTreeSeedBalanceText.text = currencies[ModelType.BASIC_TREE_SEED].ToString();
        speedTreeSeedBalanceText.text = currencies[ModelType.SPEED_TREE_SEED].ToString();
    }

    /// <summary>
    /// Adds to the player's currency balance. Precondition that
    /// the currency exists in the dictionary.
    /// </summary>
    ///<param name="currencyType">The type of currency to add. </param>
    /// <param name="amountToAdd">The amount of curenncy to add. </param>
    private static void Deposit(ModelType currencyType, int amountToDeposit)
    {
        Assert.IsTrue(currencies.ContainsKey(currencyType), "Currency not found: " + currencyType);
        if(amountToDeposit < 0)
        {
            Withdraw(currencyType, amountToDeposit);
            return;
        }
        int incremented = currencies[currencyType] + amountToDeposit;
        currencies[currencyType] = Mathf.Clamp(incremented, 0, int.MaxValue);
        instance.OnBalanceUpdated?.Invoke();
    }

    /// <summary>
    /// Cashes in a currency, potentially modifying the player's
    /// balance.
    /// </summary>
    /// <param name="currency">The currency to cash in.</param>
    public static void CashIn(Currency currency)
    {
        Assert.IsNotNull(currency, "Currency is null.");
        if (!currencies.ContainsKey(currency.TYPE)) return;

        if(currency.GetValue() > 0) Deposit(currency.TYPE, currency.GetValue());
        else Withdraw(currency.TYPE, currency.GetValue());
    }

    /// <summary>
    /// Cashes in an Enemy, potentially modifying the player's balance. Used to
    /// reward the player for defeating an enemy.
    /// </summary>
    /// <param name="enemy">The enemy to cash in.</param>
    public static void CashIn(Enemy enemy)
    {
        Assert.IsNotNull(enemy, "Enemy is null.");

        int value = enemy.CURRENCY_VALUE_ON_DEATH;
        if(value > 0) Deposit(ModelType.DEW, value);
        else Withdraw(ModelType.DEW, value);
    }

    /// <summary>
    /// Removes some amount of currency from the player's balance.
    ///</summary>
    ///<param name="currencyType">The type of currency to remove. </param>
    /// <param name="amountToWithdraw">The amount of curenncy to remove. </param>
    public static void Withdraw(ModelType currencyType, int amountToWithdraw)
    {
        if(amountToWithdraw < 0)
        {
            Deposit(currencyType, amountToWithdraw);
            return;
        }
        int decremented = currencies[currencyType] - amountToWithdraw;
        currencies[currencyType] = Mathf.Clamp(decremented, 0, int.MaxValue);
        instance.OnBalanceUpdated?.Invoke();
    }

    /// <summary>
    /// Returns the balance of a currency.
    /// </summary>
    /// <param name="currencyType">the type of currency from which to get the balance. </param>
    /// <returns>the current balance of a currency.</returns>
    public static int GetBalance(ModelType currencyType)
    {
        Assert.IsTrue(currencies.ContainsKey(currencyType), "Currency not found: " + currencyType);
        return currencies[currencyType];
    }

    /// <summary>
    /// Updates the passive income counter and awards the player currency
    /// if it is time.
    /// </summary>
    private void UpdatePassiveIncome()
    {
        if (gameState != GameState.ONGOING) return;

        timeSinceLastDewPassiveIncomeTick += Time.deltaTime;

        if (timeSinceLastDewPassiveIncomeTick >= DEW_PASSIVE_INCOME_FREQUENCY)
        {
            timeSinceLastDewPassiveIncomeTick = 0;
            Deposit(ModelType.DEW, DEW_PASSIVE_INCOME);
        }
    }

    /// <summary>
    /// Subscribes a handler to the balance updated event.
    /// </summary>
    /// <param name="handler">The handler to subscribe.</param>
    public static void SubscribeToBalanceUpdatedDelegate(BalanceUpdatedDelegate handler)
    {
        Assert.IsNotNull(handler, "Handler is null.");
        instance.OnBalanceUpdated += handler;
    }

    /// <summary>
    /// Loads the Economy data from the SaveLoadManager.
    /// </summary>
    private void LoadEconomyData()
    {
        int level = SaveLoadManager.GetLoadedGameLevel();
        currencies = new Dictionary<ModelType, int>();
        switch (level)
        {
            case 0:
                currencies.Add(ModelType.DEW, 500);
                currencies.Add(ModelType.BASIC_TREE_SEED, 0);
                currencies.Add(ModelType.SPEED_TREE_SEED, 0);
                break;
            default:
                currencies.Add(ModelType.DEW, 750);
                currencies.Add(ModelType.BASIC_TREE_SEED, 0);
                currencies.Add(ModelType.SPEED_TREE_SEED, 0);
                break;
        }
    }

    /// <summary>
    /// Saves the Economy data to the SaveLoadManager.
    /// </summary>
    private void SaveEconomyData() { }

    #endregion
}
