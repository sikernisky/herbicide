using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    /// List of active EmanationControllers.
    /// </summary>
    private List<EmanationController> emanationControllers;

    /// <summary>
    /// Reference to the ControllerController singleton.
    /// </summary>
    private static ControllerController instance;

    /// <summary>
    /// Number of active Models of each ModelType.
    /// </summary>
    private ModelCounts counts;

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
        instance.emanationControllers = new List<EmanationController>();

        instance.counts = new ModelCounts();
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
            case ModelType.BEAR:
                Bear bear = model as Bear;
                Assert.IsNotNull(bear);
                BearController bc = new BearController(bear);
                instance.defenderControllers.Add(bc);
                break;
            case ModelType.BOMB:
                break;
            case ModelType.BUTTERFLY:
                Butterfly butterfly = model as Butterfly;
                Assert.IsNotNull(butterfly);
                ButterflyController bfc = new ButterflyController(butterfly);
                instance.defenderControllers.Add(bfc);
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
            case ModelType.HEDGEHOG:
                Hedgehog hedgehog = model as Hedgehog;
                Assert.IsNotNull(hedgehog);
                HedgehogController hc = new HedgehogController(hedgehog);
                instance.defenderControllers.Add(hc);
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
            case ModelType.SHORE_TILE:
                break;
            case ModelType.SOIL_FLOORING:
                SoilFlooring soilFlooring = model as SoilFlooring;
                Assert.IsNotNull(soilFlooring);
                FlooringController sfc = new FlooringController(soilFlooring);
                instance.structureControllers.Add(sfc);
                break;
            case ModelType.SQUIRREL:
                Squirrel squirrel = model as Squirrel;
                Assert.IsNotNull(squirrel);
                SquirrelController sc = new SquirrelController(squirrel);
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
    /// Returns true if all NexusHoles are filled.
    /// </summary>
    /// <returns>true if all NexusHoles are filled; otherwise,
    /// false.</returns>
    public static bool AllHolesFilled()
    {
        foreach (ModelController mc in instance.structureControllers)
        {
            NexusHole nexusHoleModel = mc.GetModel() as NexusHole;
            if (nexusHoleModel == null || !mc.ValidModel()) continue;
            if (!nexusHoleModel.Filled()) return false;
        }

        return true;
    }

    /// <summary>
    /// Attempts to remove any unused or defunct Controllers. 
    /// For those Controllers that are to be removed, extricates any
    /// additional Controllers that Controller produced on death.
    /// </summary>
    private void TryRemoveControllers()
    {
        // Enemy Controllers
        List<ModelController> enemyControllersToRemove =
            enemyControllers.Where(ec => ec.TryRemoveController()).ToList();
        instance.AddCollectedControllers(enemyControllersToRemove);
        enemyControllers.RemoveAll(ec => enemyControllersToRemove.Contains(ec));

        // Tree Controllers
        List<ModelController> treeControllersToRemove =
            treeControllers.Where(tc => tc.TryRemoveController()).ToList();
        instance.AddCollectedControllers(treeControllersToRemove);
        treeControllers.RemoveAll(tc => treeControllersToRemove.Contains(tc));

        // Defender Controllers
        List<ModelController> defenderControllersToRemove = defenderControllers.Where(dc => dc.TryRemoveController()).ToList();
        instance.AddCollectedControllers(defenderControllersToRemove);
        defenderControllers.RemoveAll(dc => defenderControllersToRemove.Contains(dc));

        // Projectile Controllers
        List<ModelController> projectileControllersToRemove = projectileControllers.Where(pc => pc.TryRemoveController()).ToList();
        instance.AddCollectedControllers(projectileControllersToRemove);
        projectileControllers.RemoveAll(pc => projectileControllersToRemove.Contains(pc));

        // Hazard Controllers
        List<ModelController> hazardControllersToRemove = hazardControllers.Where(hc => hc.TryRemoveController()).ToList();
        instance.AddCollectedControllers(hazardControllersToRemove);
        hazardControllers.RemoveAll(hc => hazardControllersToRemove.Contains(hc));

        // Collectable Controllers
        List<ModelController> collectableControllersToRemove = collectableControllers.Where(cc => cc.TryRemoveController()).ToList();
        instance.AddCollectedControllers(collectableControllersToRemove);
        collectableControllers.RemoveAll(cc => collectableControllersToRemove.Contains(cc));

        // Structure Controllers
        List<ModelController> structureControllersToRemove = structureControllers.Where(sc => sc.TryRemoveController()).ToList();
        instance.AddCollectedControllers(structureControllersToRemove);
        structureControllers.RemoveAll(sc => structureControllersToRemove.Contains(sc));

        // Boat Controllers
        List<ModelController> boatControllersToRemove = boatControllers.Where(bc => bc.TryRemoveController()).ToList();
        instance.AddCollectedControllers(boatControllersToRemove);
        boatControllers.RemoveAll(bc => boatControllersToRemove.Contains(bc));

        // Emanation Controllers
        emanationControllers.RemoveAll(emc => emc.ShouldRemoveController());
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
        List<EmanationController> emControllers = new List<EmanationController>();

        foreach (ModelController controller in controllers)
        {
            if (!controller.NeedsExtricating()) continue;

            // First do ModelControllers
            List<ModelController> extricatedModelControllers =
                controller.ExtricateModelControllers();
            foreach (ModelController extricatedController in extricatedModelControllers)
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

            // Then do Emanations
            List<EmanationController> extricatedEmanationControllers =
                controller.ExtricateEmanationControllers();
            foreach (EmanationController extricatedController in extricatedEmanationControllers)
            {
                emControllers.Add(extricatedController);
            }
        }

        defenderControllers.AddRange(defControllers);
        enemyControllers.AddRange(emyControllers);
        treeControllers.AddRange(treControllers);
        hazardControllers.AddRange(hazControllers);
        projectileControllers.AddRange(proControllers);
        collectableControllers.AddRange(colControllers);
        emanationControllers.AddRange(emControllers);
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
        instance.UpdateModelCounts();

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

        // Update EmanationControllers
        instance.emanationControllers.ForEach(emc => emc.UpdateEmanation());
    }

    /// <summary>
    /// Informs the ControllerController of the most recent GameState
    /// so that it knows how to update its sub controllers.
    /// </summary>
    /// <param name="state">The most recent game state. </param>
    public static void InformOfGameState(GameState state) { instance.gameState = state; }

    /// <summary>
    /// Updates the number of active Models of each type.
    /// </summary>
    private void UpdateModelCounts()
    {
        counts.WipeCounts(instance);

        // Find EnemyControllers
        foreach (ModelController modelController in enemyControllers)
        {
            ModelType modelType = modelController.GetModel().TYPE;
            counts.SetCount(instance, modelType, counts.GetCount(modelType) + 1);
        }

        // Find DefenderControllers
        foreach (ModelController modelController in defenderControllers)
        {
            ModelType modelType = modelController.GetModel().TYPE;
            counts.SetCount(instance, modelType, counts.GetCount(modelType) + 1);
        }

        // Find TreeControllers
        foreach (ModelController modelController in treeControllers)
        {
            ModelType modelType = modelController.GetModel().TYPE;
            counts.SetCount(instance, modelType, counts.GetCount(modelType) + 1);
        }

        // Find ProjectileControllers
        foreach (ModelController modelController in enemyControllers)
        {
            ModelType modelType = modelController.GetModel().TYPE;
            counts.SetCount(instance, modelType, counts.GetCount(modelType) + 1);
        }

        // Find HazardControllers
        foreach (ModelController modelController in hazardControllers)
        {
            ModelType modelType = modelController.GetModel().TYPE;
            counts.SetCount(instance, modelType, counts.GetCount(modelType) + 1);
        }

        // Find CollectableControllers
        foreach (ModelController modelController in collectableControllers)
        {
            ModelType modelType = modelController.GetModel().TYPE;
            counts.SetCount(instance, modelType, counts.GetCount(modelType) + 1);
        }

        // Find StructureControllers
        foreach (ModelController modelController in structureControllers)
        {
            ModelType modelType = modelController.GetModel().TYPE;
            counts.SetCount(instance, modelType, counts.GetCount(modelType) + 1);
        }

        // Find BoatControllers
        foreach (ModelController modelController in boatControllers)
        {
            ModelType modelType = modelController.GetModel().TYPE;
            counts.SetCount(instance, modelType, counts.GetCount(modelType) + 1);
        }

        // Update all
        instance.enemyControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.defenderControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.treeControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.projectileControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.hazardControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.collectableControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.structureControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.boatControllers.ForEach(c => c.InformOfModelCounts(counts));
    }
}
