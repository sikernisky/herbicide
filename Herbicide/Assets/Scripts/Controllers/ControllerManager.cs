using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages ModelControllers and updates sub-controller classes.
/// </summary>
public class ControllerManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the ControllerManager singleton.
    /// </summary>
    private static ControllerManager instance;

    /// <summary>
    /// Number of active Models of each ModelType.
    /// </summary>
    private ModelCounts counts;

    /// <summary>
    /// true if the LevelReward has been spawned; otherwise, false.
    /// </summary>
    private static bool spawnedReward;

    /// <summary>
    /// true if the LevelReward has been collected; otherwise, false.
    /// </summary>
    private static bool collectedReward;

    /// <summary>
    /// true if combining Defenders is enabled; otherwise, false.
    /// </summary>
    private readonly bool IS_COMBINATION_ENABLED = false;

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
    /// Finds and sets the ControllerManager singleton. Also initializes the
    /// controller lists.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        ControllerManager[] controllerManagers = FindObjectsOfType<ControllerManager>();
        Assert.IsNotNull(controllerManagers, "Array of ControllerManagers is null.");
        Assert.AreEqual(1, controllerManagers.Length);
        instance = controllerManagers[0];

        instance.defenderControllers = new List<ModelController>();
        instance.enemyControllers = new List<ModelController>();
        instance.treeControllers = new List<ModelController>();
        instance.projectileControllers = new List<ModelController>();
        instance.hazardControllers = new List<ModelController>();
        instance.collectableControllers = new List<ModelController>();
        instance.structureControllers = new List<ModelController>();
        instance.emanationControllers = new List<EmanationController>();

        instance.counts = new ModelCounts();
        collectedReward = false;
        spawnedReward = false;

        ShopManager.SubscribeToOnPurchaseShopCardDelegate(instance.OnPurchaseModelFromShop);
    }

    /// <summary>
    /// Creates an appropriate ProjectileController based on the projectile type.
    /// </summary>
    /// <param name="projectileType">The type of projectile to create a controller for.</param>
    /// <param name="projectile">The projectile to create a controller for.</param>
    /// <param name="mobPosition">The position of the Model firing the projectile.</param>
    /// <param name="targetPosition">The position of the target the projectile is firing towards.</param>
    public static void CreateProjectileController(ModelType projectileType, Projectile projectile, Vector3 mobPosition, Vector3 targetPosition)
    {
        ModelController controller = projectileType switch
        {
            ModelType.ACORN => new AcornController((Acorn)projectile, mobPosition, targetPosition),
            ModelType.BLACKBERRY => new BlackberryController((Blackberry)projectile, mobPosition, targetPosition),
            _ => throw new ArgumentException("Unsupported projectile type", nameof(projectileType))
        };
        AddModelController(controller);
    }

    /// <summary>
    /// Creates an appropriate CollectableController based on the collectable type.
    /// </summary>
    /// <param name="collectableType">the type of collectable to create a controller for.</param>
    /// <param name="currencyComp">the Currency component of the collectable.</param>
    /// <param name="spawnPosition">the position to spawn the collectable at.</param>
    public static void CreateCollectableController(ModelType collectableType, Currency currencyComp, Vector2 spawnPosition)
    {
        ModelController controller = collectableType switch
        {
            ModelType.DEW => new DewController((Dew)currencyComp, spawnPosition),
            _ => null
        };
        AddModelController(controller);
    }

    /// <summary>
    /// Returns a Controller of the given ModelType and adds it to the list
    /// of active controllers that are updated and tracked. If a Defender is
    /// passed, it is assumed to be of tier 1.
    /// </summary>
    /// <param name="model">The Model that needs a ModelController.</param>
    /// <returns>A new ModelController reference of the given type of Model.</returns>
    public static ModelController MakeModelController(Model model)
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
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.treeControllers.Add(btc);
                return btc;
            case ModelType.BEAR:
                Bear bear = model as Bear;
                Assert.IsNotNull(bear);
                BearController bc = MakeDefenderController(bear, 1) as BearController;
                return bc;
            case ModelType.BUNNY:
                Bunny bunny = model as Bunny;
                Assert.IsNotNull(bunny);
                BunnyController bnc = MakeDefenderController(bunny, 1) as BunnyController;
                return bnc;
            case ModelType.GOAL_HOLE:
                GoalHole goalHole = model as GoalHole;
                Assert.IsNotNull(goalHole, "GoalHole is null.");
                GoalHoleController ghc = new GoalHoleController(goalHole);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.structureControllers.Add(ghc);
                return ghc;
            case ModelType.KNOTWOOD:
                Knotwood knotwood = model as Knotwood;
                Assert.IsNotNull(knotwood, "Knotwood is null.");
                KnotwoodController kwc = new KnotwoodController(knotwood);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.enemyControllers.Add(kwc);
                return kwc;
            case ModelType.KUDZU:
                Kudzu kudzu = model as Kudzu;
                Assert.IsNotNull(kudzu, "Kudzu is null.");
                KudzuController kzc = new KudzuController(kudzu);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.enemyControllers.Add(kzc);
                return kzc;
            case ModelType.SPAWN_HOLE:
                SpawnHole SpawnHole = model as SpawnHole;
                Assert.IsNotNull(SpawnHole, "SpawnHole is null.");
                SpawnHoleController nhc = new SpawnHoleController(SpawnHole);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.structureControllers.Add(nhc);
                return nhc;
            case ModelType.OWL:
                Owl owl = model as Owl;
                Assert.IsNotNull(owl, "Owl is null.");
                OwlController oc = MakeDefenderController(owl, 1) as OwlController;
                return oc;
            case ModelType.PORCUPINE:
                Porcupine porcupine = model as Porcupine;
                Assert.IsNotNull(porcupine, "Porcupine is null.");
                PorcupineController pc = MakeDefenderController(porcupine, 1) as PorcupineController;
                return pc;
            case ModelType.RACCOON:
                Raccoon raccoon = model as Raccoon;
                Assert.IsNotNull(raccoon, "Raccoon is null.");
                RaccoonController rc = MakeDefenderController(raccoon, 1) as RaccoonController;
                return rc;
            case ModelType.SOIL_FLOORING:
                SoilFlooring soilFlooring = model as SoilFlooring;
                Assert.IsNotNull(soilFlooring);
                FlooringController sfc = new FlooringController(soilFlooring);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.structureControllers.Add(sfc);
                return sfc;
            case ModelType.SPEED_TREE:
                SpeedTree speedTree = model as SpeedTree;
                Assert.IsNotNull(speedTree, "SpeedTree is null.");
                SpeedTreeController stc = new SpeedTreeController(speedTree);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.treeControllers.Add(stc);
                return stc;
            case ModelType.SPURGE:
                Spurge spurge = model as Spurge;
                Assert.IsNotNull(spurge, "Spurge is null.");
                SpurgeController spc = new SpurgeController(spurge);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.enemyControllers.Add(spc);
                return spc;
            case ModelType.SPURGE_MINION:
                SpurgeMinion spurgeMinion = model as SpurgeMinion;
                Assert.IsNotNull(spurgeMinion, "SpurgeMinion is null.");
                SpurgeMinionController smc = new SpurgeMinionController(spurgeMinion);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.enemyControllers.Add(smc);
                return smc;
            case ModelType.SQUIRREL:
                Squirrel squirrel = model as Squirrel;
                Assert.IsNotNull(squirrel);
                SquirrelController sc = MakeDefenderController(squirrel, 1) as SquirrelController;
                return sc;
            case ModelType.STONE_WALL:
                StoneWall stonewall = model as StoneWall;
                Assert.IsNotNull(stonewall);
                WallController swc = new WallController(stonewall);
                instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
                instance.structureControllers.Add(swc);
                return swc;
            default:
                throw new Exception("ModelType " + model.TYPE + " has no implementation in ControllerManager.");
        }
    }

    /// <summary>
    /// Returns a Controller of the given Defender's type and adds it to the list
    /// of active controllers that are updated and tracked. 
    /// </summary>
    /// <param name="defender">The Defender that needs a DefenderController.</param>
    /// <returns>A new DefenderController reference of the given type of Defender.</returns>
    public static ModelController MakeDefenderController(Defender defender, int tier)
    {
        Assert.IsTrue(defender != null, "Defender is null.");

        switch (defender.TYPE)
        {
            case ModelType.BEAR:
                Bear bear = defender as Bear;
                Assert.IsNotNull(bear);
                BearController bc = new BearController(bear, tier);
                instance.defenderControllers.Add(bc);
                return bc;
            case ModelType.BUNNY:
                Bunny bunny = defender as Bunny;
                Assert.IsNotNull(bunny);
                BunnyController bnc = new BunnyController(bunny, tier);
                instance.defenderControllers.Add(bnc);
                return bnc;
            case ModelType.OWL:
                Owl owl = defender as Owl;
                Assert.IsNotNull(owl, "Owl is null.");
                OwlController oc = new OwlController(owl, tier);
                instance.defenderControllers.Add(oc);
                return oc;
            case ModelType.PORCUPINE:
                Porcupine porcupine = defender as Porcupine;
                Assert.IsNotNull(porcupine, "Porcupine is null.");
                PorcupineController pc = new PorcupineController(porcupine, tier);
                instance.defenderControllers.Add(pc);
                return pc;
            case ModelType.RACCOON:
                Raccoon raccoon = defender as Raccoon;
                Assert.IsNotNull(raccoon, "Raccoon is null.");
                RaccoonController rc = new RaccoonController(raccoon, tier);
                instance.defenderControllers.Add(rc);
                return rc;
            case ModelType.SQUIRREL:
                Squirrel squirrel = defender as Squirrel;
                Assert.IsNotNull(squirrel);
                SquirrelController sc = new SquirrelController(squirrel, tier);
                instance.defenderControllers.Add(sc);
                return sc;
            default:
                throw new Exception("ModelType " + defender.TYPE + " has no implementation in ControllerManager.");
        }
    }

    /// <summary>
    /// Adds a pre-created ModelController to the list of active controllers.
    /// </summary>
    /// <param name="modelController">the ModelController to add</param>
    public static void AddModelController(ModelController modelController)
    {
        Assert.IsNotNull(modelController, "ModelController is null.");

        Model model = modelController.GetModel();

        if (model as LevelReward != null)
        {
            Assert.IsFalse(spawnedReward, "LevelReward has already been spawned.");
            spawnedReward = true;
        }

        if (model as Projectile != null) instance.projectileControllers.Add(modelController);
        else if (model as Collectable != null) instance.collectableControllers.Add(modelController);
        else if (model as Tile != null) return;
        else throw new Exception("ModelController has no list to add to.");

        instance.counts.SetCount(instance, model.TYPE, instance.counts.GetCount(model.TYPE) + 1);
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
            if (pc == null) continue;
            Enemy model = pc.GetModel() as Enemy;
            if (model == null) continue;
            if (!pc.ValidModel()) continue;
            if (!model.Dead() && !model.IsEscaped()) counter++;
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
            if (!model.Dead() && !model.IsEscaped() && model.Spawned()) counter++;
        }
        return counter;
    }

    /// <summary>
    /// Returns the number of placed Defenders.
    /// </summary>
    /// <returns>the number of placed Defenders.</returns>
    public static int NumPlacedDefenders()
    {
        int counter = 0;
        foreach (ModelController mc in instance.defenderControllers)
        {
            Defender defender = mc.GetModel() as Defender;
            if (defender != null && defender.PlacedOnSurface) counter++;
        }
        return counter;
    }

    /// <summary>
    /// Returns true if there is space for a Model of the given type  and tier
    /// on some Tree. Assumes a tier of 1. 
    /// </summary>
    /// <param name="modelType">the type of model to check. </param>
    /// <returns>true if there is space for a Model of the given type  and tier
    /// on some Tree; otherwise, false.</returns>
    public static bool IsSpaceForModelOnSomeTree(ModelType modelType)
    {
        int defendersOfSameTypeAndTier = 0;
        foreach (ModelController treeController in instance.treeControllers)
        {
            if (!treeController.ValidModel()) continue;
            Tree tree = treeController.GetModel() as Tree;
            if (tree == null) continue;
            
            // There is an empty tree, and we can place the defender on it
            if(!tree.IsOccupied()) return true;

            // Check if the defender on the tower is of the same class and tier
            Defender defender = tree.Occupant.Object as Defender;
            if(defender == null) continue;

            if (defender.TYPE == modelType && defender.GetTier() == 1)
            {
                defendersOfSameTypeAndTier++;
                // There are already two defenders of the same type and tier, so combining opens up a Tree
                if (defendersOfSameTypeAndTier == 2) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if purchasing a defender of the given type will trigger a combination.
    /// Assumes the defender has a tier of 1.
    /// </summary>
    /// <param name="modelType">The type of defender to check.</param>
    /// <returns>true if purchasing this defender triggers a combination; otherwise, false.</returns>
    public static bool WillTriggerCombination(ModelType modelType)
    {
        if(!instance.IS_COMBINATION_ENABLED) return false;
        int defendersOfSameTypeAndTier = 0;
        foreach (ModelController treeController in instance.treeControllers)
        {
            if (!treeController.ValidModel()) continue;
            Tree tree = treeController.GetModel() as Tree;
            if (tree == null) continue;
            if (!tree.IsOccupied()) continue;
            Defender defender = tree.Occupant as Defender;
            if (defender == null) continue;

            if (defender.TYPE == modelType && defender.GetTier() == 1)
            {
                defendersOfSameTypeAndTier++;
                if (defendersOfSameTypeAndTier == 2) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Sets the color of all Trees in the scene. This method is
    /// called by the TutorialLevelBehaviourController.
    /// </summary>
    /// <param name="tutorialLevelBehaviourController">the TutorialLevelBehaviourController singleton</param>
    /// <param name="color">the color to set to</param>
    public static void SetColorOfAllTrees(TutorialLevelBehaviourController tutorialLevelBehaviourController, Color32 color)
    {
        Assert.IsNotNull(tutorialLevelBehaviourController, "TutorialLevelBehaviourController is null.");

        List<ModelController> treeControllers = instance.treeControllers;
        List<ModelController> basicTreeControllers = treeControllers.Where(tc => tc.GetModel().TYPE == ModelType.BASIC_TREE).ToList();
        foreach (ModelController tc in basicTreeControllers)
        {
            BasicTreeController btc = tc as BasicTreeController;
            Assert.IsNotNull(btc, "BasicTreeController is null.");
            btc.GetModel().SetColor(color);
        }
    }

    /// <summary>
    /// Sets the tier of all Squirrels in the scene.
    /// </summary>
    /// <param name="tier">the tier to set to.</param>
    public static void SetTierOfAllSquirrels(int tier)
    {
        foreach (ModelController dc in instance.defenderControllers)
        {
            Squirrel squirrel = dc.GetModel() as Squirrel;
            if (squirrel == null) continue;
            squirrel.SetTier(tier);
        }
    }

    /// <summary>
    /// Applies the given Effect to all Models of the given ModelType. Takes in a constructed
    /// Effect object and clones it to each Model of the given ModelType.
    /// </summary>
    /// <param name="effect">the effect to apply.</param>
    /// <param name="modelType">the type of Model to apply to</param>
    public static void ApplyEffectToAllModelsOfType(Effect effect, ModelType modelType)
    {
        Assert.IsNotNull(effect, "Effect is null.");
        foreach (ModelController mc in instance.enemyControllers)
        {
            if (mc.GetModel().TYPE == modelType)
            {
                Model model = mc.GetModel();
                model.AddEffect(effect.Clone());
            }
        }
    }

    /// <summary>
    /// Forces each Bunny to generate a resource at its location.
    /// </summary>
    public static void MakeAllBunniesGenerateResource()
    {
        foreach (ModelController mc in instance.defenderControllers)
        {
            BunnyController bc = mc as BunnyController;
            if (bc == null) continue;
            bc.GenerateResource();
        }
    }

    /// <summary>
    /// Returns true if the LevelReward has been collected; otherwise, false.
    /// </summary>
    /// <returns>true if the LevelReward has been collected; otherwise, false.</returns>
    public static bool LevelRewardCollected() => collectedReward;

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
        Model modelToRemove = controllerToRemove.GetModel();
        if(modelToRemove as LevelReward != null)
        {
            Assert.IsTrue(spawnedReward, "LevelReward has not been spawned.");
            Assert.IsFalse(collectedReward, "LevelReward has already been collected.");
            collectedReward = true;
        }
        controllerToRemove.OnRemoveController();
    }

    /// <summary>
    /// Updates the Model Controllers managed by the ControllerManager.
    /// </summary>
    /// <param name="gameState">Current game state</param>
    public static void UpdateModelControllers(GameState gameState)
    {
        // General updates
        instance.TryRemoveControllers();
        instance.UpdateModelCounts();

        // Update EnemyControllers
        for (int i = 0; i < instance.enemyControllers.Count; i++)
        {
            instance.enemyControllers[i].UpdateController(gameState);
        }

        // Update DefenderControllers
        for (int i = 0; i < instance.defenderControllers.Count; i++)
        {
            instance.defenderControllers[i].UpdateController(gameState);
        }

        // Update TreeControllers
        for (int i = 0; i < instance.treeControllers.Count; i++)
        {
            instance.treeControllers[i].UpdateController(gameState);
        }

        // Update ProjectileControllers
        for (int i = 0; i < instance.projectileControllers.Count; i++)
        {
            instance.projectileControllers[i].UpdateController(gameState);
        }

        // Update HazardControllers
        for (int i = 0; i < instance.hazardControllers.Count; i++)
        {
            instance.hazardControllers[i].UpdateController(gameState);
        }

        // Update CollectableControllers
        for (int i = 0; i < instance.collectableControllers.Count; i++)
        {
            instance.collectableControllers[i].UpdateController(gameState);
        }

        // Update StructureControllers
        for (int i = 0; i < instance.structureControllers.Count; i++)
        {
            instance.structureControllers[i].UpdateController(gameState);
        }

        // Update EmanationControllers
        for (int i = 0; i < instance.emanationControllers.Count; i++)
        {
            instance.emanationControllers[i].UpdateEmanation();
        }
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
    /// Called when the player buys a Defender from the shop. Makes its controller
    /// and handles any upgrades needed.
    /// </summary>
    /// <param name="defenderType">the type of Defender that was just purchased.</param>
    private Defender OnPurchaseModelFromShop(ModelType defenderType)
    {
        Assert.IsTrue(IsSpaceForModelOnSomeTree(defenderType), "You need to ensure there is space to place this Model");
        GameObject defenderOb = DefenderFactory.GetDefenderPrefab(defenderType);
        Assert.IsNotNull(defenderOb);
        Defender purchasedDefender = defenderOb.GetComponent<Defender>();
        Assert.IsNotNull(purchasedDefender);
        MakeModelController(purchasedDefender);
        return purchasedDefender;

        // Create the model controller for the newly purchased model
        // if(purchasedDefender == null) return;
        // if(!IS_COMBINATION_ENABLED) return;
        // StartCoroutine(CheckAndCombineDefenders(purchasedDefender));
    }
    
/*    /// <summary>
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
                //PlacementController.StopPlacing();
                PlacementManager.StartPlacing(combinationResult);
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
        ModelController modelController = MakeDefenderController(newDefenderComp, newTier);
        List<Model> combinedDefenderModels = combinedDefenders.Cast<Model>().ToList();
        modelController.AquireStatsOfCombiningModels(combinedDefenderModels);

        return newDefenderComp;
    }*/

   /* /// <summary>
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
    }*/

    #endregion
}
