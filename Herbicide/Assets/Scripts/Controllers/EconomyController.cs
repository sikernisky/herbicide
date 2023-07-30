using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

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
    /// Prefab for a Seed Token.
    /// </summary>
    [SerializeField]
    private GameObject currencyPrefab;

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
    /// Creates a SeedToken at a given position.
    /// </summary>
    /// <param name="position">The position at which to create the Seed Token.
    /// </param>
    public static void SpawnSeedToken(Vector2 position)
    {
        GameObject newToken = Instantiate(instance.currencyPrefab);
        Assert.IsNotNull(newToken);
        SeedToken tokenComp = newToken.GetComponent<SeedToken>();
        Assert.IsNotNull(tokenComp);

        Vector3 tokenPos = new Vector3(position.x, position.y, 1);
        newToken.transform.position = tokenPos;
        newToken.transform.SetParent(instance.transform);
        instance.activeCurrency.Add(tokenComp);
        tokenComp.OnSpawn();
    }

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
    }

    /// <summary>
    /// Checks for and collects any currency the player clicked on.
    /// </summary>
    public static void CheckCurrencyPickup()
    {
        Collectable c = InputController.CollectableClickedUp();
        Currency curr = c as Currency;
        if (curr == null) return;

        //Handle logic here.
        Assert.IsTrue(instance.activeCurrency.Contains(curr));
        curr.OnCollect();
        instance.activeCurrency.Remove(curr);
        Deposit(curr.VALUE);
        Destroy(curr.gameObject);
    }

    /// <summary>
    /// Main update loop for the EconomyController.<br></br>
    /// 
    /// (1) Updates all active Currencies in the game.<br></br>
    /// (2) Displays the current currency to a text component.
    /// </summary>
    public static void UpdateEconomy()
    {
        foreach (Currency c in instance.activeCurrency)
        {
            c.UpdateCurrency();
        }

        if (currentMoney >= 0) instance.currencyText.text = currentMoney.ToString();
    }


    /// <summary>
    /// Adds to the player's currency balance.
    /// </summary>
    /// <param name="amount">How much to add.</param>
    private static void Deposit(int amount)
    {
        if (amount < 0) return;
        int incremented = GetMoney() + amount;
        currentMoney = Mathf.Clamp(incremented, MIN_MONEY, MAX_MONEY);
    }

    /// <summary>
    /// Subtracts from the player's current balance.
    /// </summary>
    /// <param name="amount">How much to subtract.</param>
    private static void Withdraw(int amount)
    {
        if (amount < 0) return;
        int decremented = GetMoney() - amount;
        currentMoney = Mathf.Clamp(decremented, MIN_MONEY, MAX_MONEY);
    }

    /// <summary>
    /// Returns the amount of money the player currently has.
    ///  </summary>
    /// <returns>how much money the player has.</returns>
    public static int GetMoney()
    {
        return currentMoney;
    }
}
