using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls and updates sub-controller classes.
/// </summary>
public class ControllerController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the ControllerController singleton.
    /// </summary>
    private static ControllerController instance;

    /// <summary>
    /// Number of active Models of each ModelType.
    /// </summary>
    private ModelCounts counts;

    #endregion

    #region Controller Lists

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
    /// List of active EmanationControllers.
    /// </summary>
    private List<EmanationController> emanationControllers;

    #endregion

    #region Methods

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
        instance.emanationControllers = new List<EmanationController>();

        instance.counts = new ModelCounts();

        ShopManager.SubscribeToBuyDefenderDelegate(instance.OnPurchaseModelFromShop);
    }

    /// <summary>
    /// Makes a Controller of the given ModelType and adds it to the list
    /// of active controllers that are updated and tracked. 
    /// </summary>
    /// <param name="model">The Model that needs a ModelController.</param>
    public static void MakeModelController(Model model)
    {
        bool needsParameters = false;
        if (model as Projectile != null) needsParameters = true;
        if (model as Collectable != null) needsParameters = true;
        if (needsParameters) throw new Exception("You need to use AddModelController() for a " +
            "model with type " + model.TYPE + " that requires parameters.");
        bool hasNoController = false;
        if (model as Tile != null) hasNoController = true;
        if (hasNoController) throw new Exception("The model with type " + model.TYPE + " has no controller.");

        switch (model.TYPE)
        {
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
            case ModelType.KNOTWOOD:
                Knotwood knotwood = model as Knotwood;
                Assert.IsNotNull(knotwood, "Knotwood is null.");
                KnotwoodController kwc = new KnotwoodController(knotwood);
                instance.enemyControllers.Add(kwc);
                break;
            case ModelType.KUDZU:
                Kudzu kudzu = model as Kudzu;
                Assert.IsNotNull(kudzu, "Kudzu is null.");
                KudzuController kzc = new KudzuController(kudzu);
                instance.enemyControllers.Add(kzc);
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
            case ModelType.PORCUPINE:
                Porcupine porcupine = model as Porcupine;
                Assert.IsNotNull(porcupine, "Porcupine is null.");
                PorcupineController pc = new PorcupineController(porcupine);
                instance.defenderControllers.Add(pc);
                break;
            case ModelType.RACCOON:
                Raccoon raccoon = model as Raccoon;
                Assert.IsNotNull(raccoon, "Raccoon is null.");
                RaccoonController rc = new RaccoonController(raccoon);
                instance.defenderControllers.Add(rc);
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
            case ModelType.STONE_WALL:
                StoneWall stonewall = model as StoneWall;
                Assert.IsNotNull(stonewall);
                WallController swc = new WallController(stonewall);
                instance.structureControllers.Add(swc);
                break;
        }

        instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
    }

    /// <summary>
    /// Adds a pre-created ModelController to the list of active controllers.
    /// </summary>
    /// <param name="modelController">the ModelController to add</param>
    public static void AddModelController(ModelController modelController)
    {
        Assert.IsNotNull(modelController, "ModelController is null.");

        Model model = modelController.GetModel();
        if (model as Projectile != null) instance.projectileControllers.Add(modelController);
        else if (model as Collectable != null) instance.collectableControllers.Add(modelController);
        else if (model as Tile != null) return;
        else throw new Exception("ModelController has no list to add to.");
    }

    /// <summary>
    /// Adds an EmanationController to the list of active EmanationControllers.
    /// </summary>
    /// <param name="emanationController">The EmanationController to add. </param>
    public static void AddEmanationController(EmanationController emanationController)
    {
        Assert.IsNotNull(emanationController, "EmanationController is null.");

        instance.emanationControllers.Add(emanationController);
    }

    /// <summary>
    /// Returns the number of remaining and alive, active Enemies.
    /// </summary>
    /// <returns>the number of remaining and alive, active Enemies.</returns>
    public static int NumEnemiesRemainingInclusive()
    {
        int counter = 0;
        foreach (ModelController pc in instance.enemyControllers)
        {
            if (pc == null || !pc.ValidModel()) continue;
            Enemy model = pc.GetModel() as Enemy;
            if (model == null) continue;
            if (!model.Dead() && !model.Exited()) counter++;
        }
        return counter;
    }

    /// <summary>
    /// Returns the number of spawned and alive enemies.
    /// </summary>
    /// <returns> the number of spawned and alive enemies.</returns>
    public static int NumActiveEnemies()
    {
        int counter = 0;
        foreach (ModelController pc in instance.enemyControllers)
        {
            if (pc == null || !pc.ValidModel()) continue;
            Enemy model = pc.GetModel() as Enemy;
            if (model == null) continue;
            if (!model.Dead() && !model.Exited() && model.Spawned()) counter++;
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
            Nexus nexus = mc.GetModel() as Nexus;
            if (nexus != null && !nexus.CashedIn()) counter++;
        }
        return counter;
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
            enemyControllers.Where(ec => ec.ShouldRemoveController()).ToList();
        enemyControllersToRemove.ForEach(ec => DiscardController(ec));
        enemyControllers.RemoveAll(ec => enemyControllersToRemove.Contains(ec));

        // Tree Controllers
        List<ModelController> treeControllersToRemove =
            treeControllers.Where(tc => tc.ShouldRemoveController()).ToList();
        treeControllersToRemove.ForEach(tc => DiscardController(tc));
        treeControllers.RemoveAll(tc => treeControllersToRemove.Contains(tc));

        // Defender Controllers
        List<ModelController> defenderControllersToRemove = defenderControllers.Where(dc => dc.ShouldRemoveController()).ToList();
        defenderControllersToRemove.ForEach(dc => DiscardController(dc));
        defenderControllers.RemoveAll(dc => defenderControllersToRemove.Contains(dc));

        // Projectile Controllers
        List<ModelController> projectileControllersToRemove = projectileControllers.Where(pc => pc.ShouldRemoveController()).ToList();
        projectileControllersToRemove.ForEach(pc => DiscardController(pc));
        projectileControllers.RemoveAll(pc => projectileControllersToRemove.Contains(pc));

        // Hazard Controllers
        List<ModelController> hazardControllersToRemove = hazardControllers.Where(hc => hc.ShouldRemoveController()).ToList();
        hazardControllersToRemove.ForEach(hc => DiscardController(hc));
        hazardControllers.RemoveAll(hc => hazardControllersToRemove.Contains(hc));

        // Collectable Controllers
        List<ModelController> collectableControllersToRemove = collectableControllers.Where(cc => cc.ShouldRemoveController()).ToList();
        collectableControllersToRemove.ForEach(cc => DiscardController(cc));
        collectableControllers.RemoveAll(cc => collectableControllersToRemove.Contains(cc));

        // Structure Controllers
        List<ModelController> structureControllersToRemove = structureControllers.Where(sc => sc.ShouldRemoveController()).ToList();
        structureControllersToRemove.ForEach(sc => DiscardController(sc));
        structureControllers.RemoveAll(sc => structureControllersToRemove.Contains(sc));

        // Emanation Controllers
        List<EmanationController> emanationControllersToRemove = emanationControllers.Where(emc => emc.ShouldRemoveController()).ToList();
        enemyControllersToRemove.ForEach(emc => emc.OnRemoveController());
        emanationControllers.RemoveAll(emc => emc.ShouldRemoveController());
    }

    /// <summary>
    /// Discards the given Controller from the list of active Controllers and updates
    /// the ModelCounts object
    /// </summary>
    /// <param name="controllerToRemove">the controller to discard.</param>
    private void DiscardController(ModelController controllerToRemove)
    {
        counts.SetCount(instance, controllerToRemove.GetModel().TYPE, counts.GetCount(controllerToRemove.GetModel().TYPE) - 1);
        controllerToRemove.OnRemoveController();
    }

    /// <summary>
    /// Updates the Model Controllers managed by the ControllerController.
    /// </summary>
    /// <param name="gameState">Current game state</param>
    public static void UpdateModelControllers(GameState gameState)
    {
        // General updates
        instance.TryRemoveControllers();
        instance.UpdateModelCounts();

        // Update EnemyControllers
        instance.enemyControllers.ForEach(ec => ec.UpdateController(gameState));

        // Update DefenderControllers
        instance.defenderControllers.ForEach(dc => dc.UpdateController(gameState));

        // Update TreeControllers
        instance.treeControllers.ForEach(tc => tc.UpdateController(gameState));

        // Update ProjectileControllers
        instance.projectileControllers.ForEach(pc => pc.UpdateController(gameState));

        // Update HazardControllers
        instance.hazardControllers.ForEach(hc => hc.UpdateController(gameState));

        // Update CollectableControllers
        instance.collectableControllers.ForEach(cc => cc.UpdateController(gameState));

        // Update StructureControllers
        instance.structureControllers.ForEach(sc => sc.UpdateController(gameState));

        // Update EmanationControllers
        instance.emanationControllers.ForEach(emc => emc.UpdateEmanation());
    }

    /// <summary>
    /// Updates the number of active Models of each type.
    /// </summary>
    private void UpdateModelCounts()
    {
        instance.enemyControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.defenderControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.treeControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.projectileControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.hazardControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.collectableControllers.ForEach(c => c.InformOfModelCounts(counts));
        instance.structureControllers.ForEach(c => c.InformOfModelCounts(counts));
    }


    /// <summary>
    /// Called when the player buys a Model from the shop. Makes its controller
    /// and handles any upgrades needed.
    /// </summary>
    /// <param name="purchasedModel">the Model that was just purchased.</param>
    private void OnPurchaseModelFromShop(Model purchasedModel)
    {
        // Create the model controller for the newly purchased model
        MakeModelController(purchasedModel);

        Defender purchasedDefender = purchasedModel as Defender;
        if(purchasedDefender == null) return;

        StartCoroutine(CheckAndCombineDefenders(purchasedDefender));
    }
    
    /// <summary>
    /// Checks if there is a combination of Defenders that can be made.
    /// If so, triggers a combination event in PlacementController and
    /// makes a recursive call to check for more combinations.
    /// </summary>
    /// <param name="newDefender">The Defender that was just purchased
    /// or combined.</param>
    /// <returns>a reference to the coroutine.</returns>
    private IEnumerator CheckAndCombineDefenders(Defender newDefender)
    {
        Assert.IsNotNull(newDefender);
        yield return new WaitWhile(() => PlacementController.IsCombining());

        if(newDefender.GetTier() < 3)
        {
            List<Defender> defendersOfTypeAndTier = FindPlacedDefendersOfTypeAndTier(newDefender.TYPE, newDefender.GetTier());
            Assert.IsNotNull(defendersOfTypeAndTier);
            defendersOfTypeAndTier.Add(newDefender);

            if (defendersOfTypeAndTier.Count == 3)
            {
                ModelType defenderType = newDefender.TYPE;
                List<Defender> defendersToAnimate = defendersOfTypeAndTier.GetRange(0, defendersOfTypeAndTier.Count - 1);
                PlacementController.AnimateCombination(defendersToAnimate);
                yield return new WaitWhile(() => PlacementController.IsCombining());

                Defender combinationResult = CombineDefenders(defendersOfTypeAndTier, newDefender.GetTier());
                PlacementController.StopPlacingObject();
                PlacementController.StartPlacingObject(combinationResult);
                yield return StartCoroutine(CheckAndCombineDefenders(combinationResult));
            }
        }
    }

    /// <summary>
    /// Creates a new Defender and controller of the given type and tier.
    /// Removes the old Defenders from the scene.
    /// </summary>
    /// <param name="combinedDefenders">The Defenders that were just combined.</param>
    /// <param name="oldTier">The tier of the Defenders that were just combined.</param>
    /// <returns>the new, combined Defender. </returns>
    private Defender CombineDefenders(List<Defender> combinedDefenders, int oldTier)
    {
        Assert.IsNotNull(combinedDefenders);
        Assert.IsTrue(combinedDefenders.Count == 3, "Cannot combine less than 3 Defenders.");
        combinedDefenders.ForEach(cd => Assert.IsNotNull(cd));
        combinedDefenders.ForEach(cd => Assert.IsTrue(cd.GetTier() == oldTier));

        int newTier = combinedDefenders[0].GetTier() + 1;
        ModelType defenderType = combinedDefenders[0].TYPE;
        combinedDefenders.ForEach(d => RemoveDefenderFromScene(d));
        GameObject newDefenderOb = DefenderFactory.GetDefenderPrefab(defenderType);
        Assert.IsNotNull(newDefenderOb);
        Defender newDefenderComp = newDefenderOb.GetComponent<Defender>();
        Assert.IsNotNull(newDefenderComp);
        while(newDefenderComp.GetTier() < newTier) newDefenderComp.Upgrade();
        MakeModelController(newDefenderComp);

        return newDefenderComp;
    }

    /// <summary>
    /// Removes a Defender and its associated Controller from the scene.
    /// </summary>
    /// <param name="defenderToRemove">the Defender to remove. </param>
    /// <returns>true if the remove was successful; otherwise, false. </returns>
    private bool RemoveDefenderFromScene(Defender defenderToRemove)
    {
        Assert.IsNotNull(defenderToRemove);

        foreach (ModelController dc in defenderControllers)
        {
            if (dc == null) continue;
            Defender defender = dc.GetModel() as Defender;
            if (defender == null) continue;
            if (defender != defenderToRemove) continue;

            TileGrid.RemoveFromTile(defenderToRemove.GetPlacedCoords());
            DiscardController(dc);
            defenderControllers.Remove(dc);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns a list of all placed Defenders of the given type. Does
    /// not discriminate by tier.
    /// </summary>
    /// <param name="defenderType">The ModelType of Defender to search for.</param>
    /// <returns>a list of all placed Defenders of the given type.</returns>
    private List<Defender> FindPlacedDefendersOfTypeAndTier(ModelType defenderType, int tier)
    {
        List<Defender> placedDefenders = new List<Defender>();

        foreach (ModelController defenderController in defenderControllers)
        {
            if (defenderController == null) continue;
            Defender defender = defenderController.GetModel() as Defender;

            if (defender == null || !defender.IsPlaced() || defender.GetTier() != tier) continue;

            if (defender.TYPE == defenderType)
            {
                placedDefenders.Add(defender);
            }
        }

        return placedDefenders;
    }

    #endregion
}
