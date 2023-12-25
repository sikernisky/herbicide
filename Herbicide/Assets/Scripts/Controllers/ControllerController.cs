using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller that controls the scene's controllers.
/// </summary>
public class ControllerController : MonoBehaviour
{
    /// <summary>
    /// List of active DefenderControllers.
    /// </summary>
    private List<ModelController> defenderControllers;

    /// <summary>
    /// List of active EnemyControllers.
    /// </summary>
    private List<ModelController> enemyControllers;

    /// <summary>
    /// List of active TreeControllers.
    /// </summary>
    private List<ModelController> treeControllers;

    /// <summary>
    /// List of active ProjectileControllers.
    /// </summary>
    private List<ModelController> projectileControllers;

    /// <summary>
    /// List of active HazardControllers.
    /// </summary>
    private List<ModelController> hazardControllers;

    /// <summary>
    /// List of active CollectableControllers.
    /// </summary>
    private List<ModelController> collectableControllers;

    /// <summary>
    /// List of active StructureControllers.
    /// </summary>
    private List<ModelController> structureControllers;

    /// <summary>
    /// List of active ShopBoatControllers.
    /// </summary>
    private List<ModelController> boatControllers;

    /// <summary>
    /// Reference to the ControllerController singleton.
    /// </summary>
    private static ControllerController instance;

    /// <summary>
    /// The most recent GameState.
    /// </summary>
    private GameState gameState;


    /// <summary>
    /// Finds and sets the ControllerController singleton. Also initializes the
    /// controller lists.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        ControllerController[] controllerQueues = FindObjectsOfType<ControllerController>();
        Assert.IsNotNull(controllerQueues, "Array of ControllerQueues is null.");
        Assert.AreEqual(1, controllerQueues.Length);
        instance = controllerQueues[0];

        instance.defenderControllers = new List<ModelController>();
        instance.enemyControllers = new List<ModelController>();
        instance.treeControllers = new List<ModelController>();
        instance.projectileControllers = new List<ModelController>();
        instance.hazardControllers = new List<ModelController>();
        instance.collectableControllers = new List<ModelController>();
        instance.structureControllers = new List<ModelController>();
        instance.boatControllers = new List<ModelController>();
    }

    /// <summary>
    /// Makes a Controller of the given ModelType and adds it to the list
    /// of active controllers that are updated and tracked. 
    /// </summary>
    /// <param name="modelType">The Model that needs a ModelController.</param>
    public static void MakeController(Model model)
    {
        switch (model.TYPE)
        {
            case ModelType.ACORN:
                break;
            case ModelType.BASIC_TREE:
                BasicTree basicTree = model as BasicTree;
                Assert.IsNotNull(basicTree, "BasicTree is null");
                BasicTreeController btc = new BasicTreeController(basicTree);
                instance.treeControllers.Add(btc);
                break;
            case ModelType.BOMB:
                break;
            case ModelType.BUTTERFLY:
                Butterfly butterfly = model as Butterfly;
                Assert.IsNotNull(butterfly);
                ButterflyController bc = new ButterflyController(butterfly);
                instance.defenderControllers.Add(bc);
                break;
            case ModelType.BOMB_SPLAT:
                BombSplat bombSplat = model as BombSplat;
                Assert.IsNotNull(bombSplat, "BombSplat is null.");
                SlowZoneController szc = new SlowZoneController(bombSplat);
                instance.hazardControllers.Add(szc);
                break;
            case ModelType.DEW:
                break;
            case ModelType.GRASS_TILE:
                break;
            case ModelType.KUDZU:
                Kudzu kudzu = model as Kudzu;
                Assert.IsNotNull(kudzu, "Kudzu is null.");
                KudzuController kc = new KudzuController(kudzu);
                instance.enemyControllers.Add(kc);
                break;
            case ModelType.NEXUS:
                Nexus nexus = model as Nexus;
                Assert.IsNotNull(nexus, "Nexus is null.");
                NexusController nxc = new NexusController(nexus);
                instance.structureControllers.Add(nxc);
                break;
            case ModelType.NEXUS_HOLE:
                NexusHole nexusHole = model as NexusHole;
                Assert.IsNotNull(nexusHole, "NexusHole is null.");
                NexusHoleController nhc = new NexusHoleController(nexusHole);
                instance.structureControllers.Add(nhc);
                break;
            case ModelType.SEED_TOKEN:
                break;
            case ModelType.SHOP_BOAT:
                ShopBoat shopBoat = model as ShopBoat;
                Assert.IsNotNull(shopBoat);
                ShopBoatController sbc = new ShopBoatController(shopBoat);
                instance.structureControllers.Add(sbc);
                break;
            case ModelType.SHORE_TILE:
                break;
            case ModelType.SOIL_FLOORING:
                break;
            case ModelType.SQUIRREL:
                Squirrel squirrel = model as Squirrel;
                Assert.IsNotNull(squirrel);
                SquirrelController sc = new SquirrelController(squirrel as Squirrel);
                instance.defenderControllers.Add(sc);
                break;
            case ModelType.WATER_TILE:
                break;
        }
    }

    /// <summary>
    /// Returns the number of alive, active Enemies.
    /// </summary>
    /// <returns>the number of alive, active Enemies.</returns>
    public static int NumActiveEnemies()
    {
        int counter = 0;
        foreach (ModelController pc in instance.enemyControllers)
        {
            if (pc == null || !pc.ValidModel()) continue;
            Enemy model = pc.GetModel() as Enemy;
            if (model == null) continue;
            if (!model.Dead()) counter++;
        }
        return counter;
    }

    /// <summary>
    /// Returns the number of alive, active Trees
    /// </summary>
    /// <returns>the number of alive, active Trees </returns>
    public static int NumActiveTrees()
    {
        int counter = 0;
        foreach (ModelController mc in instance.treeControllers)
        {
            if (mc == null || !mc.ValidModel()) continue;
            Tree model = mc.GetModel() as Tree;
            if (model == null) continue;
            if (!model.Dead()) counter++;
        }
        return counter;
    }

    /// <summary>
    /// Returns the number of alive, active Squirrels.
    /// </summary>
    /// <returns>the number of alive, active Squirrels.</returns>
    public static int NumActiveSquirrels()
    {
        int counter = 0;
        foreach (ModelController mc in instance.defenderControllers)
        {
            if (mc == null || !mc.ValidModel()) continue;
            if (mc.GetModel() as Squirrel != null) counter++;
        }
        return counter;
    }

    /// <summary>
    /// Returns the number of alive, active Nexii.
    /// </summary>
    /// <returns>the number of alive, active Nexii.</returns>
    public static int NumActiveNexii()
    {
        int counter = 0;
        foreach (ModelController mc in instance.structureControllers)
        {
            if (mc == null || !mc.ValidModel()) continue;
            if (mc.GetModel() as Nexus != null) counter++;
        }
        return counter;
    }

    /// <summary>
    /// Attempts to remove any unused or defunct Controllers.
    /// </summary>
    private void TryRemoveControllers()
    {
        enemyControllers.RemoveAll(ec => ec.ShouldRemoveController());
        treeControllers.RemoveAll(tc => tc.ShouldRemoveController());
        defenderControllers.RemoveAll(dc => dc.ShouldRemoveController());
        projectileControllers.RemoveAll(pc => pc.ShouldRemoveController());
        hazardControllers.RemoveAll(hc => hc.ShouldRemoveController());
        collectableControllers.RemoveAll(cc => cc.ShouldRemoveController());
        structureControllers.RemoveAll(sc => sc.ShouldRemoveController());
        boatControllers.RemoveAll(bc => bc.ShouldRemoveController());
    }

    /// <summary>
    /// Informs all of this ControllerController's controller references
    /// of the current GameState.
    /// </summary>
    private void InformControllersOfGameState()
    {
        enemyControllers.ForEach(ec => ec.InformOfGameState(gameState));
        defenderControllers.ForEach(dc => dc.InformOfGameState(gameState));
        treeControllers.ForEach(tc => tc.InformOfGameState(gameState));
        projectileControllers.ForEach(pc => pc.InformOfGameState(gameState));
        hazardControllers.ForEach(hc => hc.InformOfGameState(gameState));
        collectableControllers.ForEach(cc => cc.InformOfGameState(gameState));
        structureControllers.ForEach(sc => sc.InformOfGameState(gameState));
        boatControllers.ForEach(bc => bc.InformOfGameState(gameState));
    }

    /// <summary>
    /// Runs through the list of uncollected ModelControllers to identify
    /// their most downcasted version. Adds it to the correct list.
    /// </summary>
    /// <param name="controllers">The list of ModelControllers to filter.</param>
    private void AddCollectedControllers(List<ModelController> controllers)
    {

        List<ModelController> defControllers = new List<ModelController>();
        List<ModelController> emyControllers = new List<ModelController>();
        List<ModelController> treControllers = new List<ModelController>();
        List<ModelController> proControllers = new List<ModelController>();
        List<ModelController> hazControllers = new List<ModelController>();
        List<ModelController> colControllers = new List<ModelController>();

        foreach (ModelController controller in controllers)
        {
            List<ModelController> extricatedControllers =
                controller.ExtricateControllers();

            foreach (ModelController extricatedController in extricatedControllers)
            {
                if (extricatedController.GetModel() as Defender != null)
                    defControllers.Add(extricatedController);
                else if (extricatedController.GetModel() as Enemy != null)
                    emyControllers.Add(extricatedController);
                else if (extricatedController.GetModel() as Projectile != null)
                    proControllers.Add(extricatedController);
                else if (extricatedController.GetModel() as Hazard != null)
                    hazControllers.Add(extricatedController);
                else if (extricatedController.GetModel() as Tree != null)
                    treControllers.Add(extricatedController);
                else if (extricatedController.GetModel() as Collectable != null)
                    colControllers.Add(extricatedController);
            }
        }

        defenderControllers.AddRange(defControllers);
        enemyControllers.AddRange(emyControllers);
        treeControllers.AddRange(treControllers);
        hazardControllers.AddRange(hazControllers);
        projectileControllers.AddRange(proControllers);
        collectableControllers.AddRange(colControllers);
    }

    /// <summary>
    /// Updates all Controllers managed by the ControllerController.
    /// </summary>
    /// <param name="dt">Current game time</param>
    public static void UpdateAllControllers()
    {
        // General updates
        instance.TryRemoveControllers();
        instance.InformControllersOfGameState();

        // Update EnemyControllers
        instance.AddCollectedControllers(instance.enemyControllers);
        instance.enemyControllers.ForEach(ec => ec.UpdateModel());

        // Update DefenderControllers
        instance.AddCollectedControllers(instance.defenderControllers);
        instance.defenderControllers.ForEach(dc => dc.UpdateModel());

        // Update TreeControllers
        instance.AddCollectedControllers(instance.treeControllers);
        instance.treeControllers.ForEach(tc => tc.UpdateModel());

        // Update ProjectileControllers
        instance.AddCollectedControllers(instance.projectileControllers);
        instance.projectileControllers.ForEach(pc => pc.UpdateModel());

        // Update HazardControllers
        instance.AddCollectedControllers(instance.hazardControllers);
        instance.hazardControllers.ForEach(hc => hc.UpdateModel());

        // Update CollectableControllers
        instance.AddCollectedControllers(instance.collectableControllers);
        instance.collectableControllers.ForEach(cc => cc.UpdateModel());

        // Update StructureControllers
        instance.structureControllers.ForEach(sc => sc.UpdateModel());

        // Update BoatControllers
        instance.boatControllers.ForEach(bc => bc.UpdateModel());
    }

    /// <summary>
    /// Informs the ControllerController of the most recent GameState
    /// so that it knows how to update its sub controllers.
    /// </summary>
    /// <param name="state">The most recent game state. </param>
    public static void InformOfGameState(GameState state) { instance.gameState = state; }
}
