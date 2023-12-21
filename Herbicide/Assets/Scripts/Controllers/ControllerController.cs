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
    }



    /// <summary>
    /// Creates a DefenderController for a Defender of a given type. Adds it to
    /// its respective list of controllers that the ControllerController
    /// updates each frame.
    /// </summary>
    /// <param name="defender">The Defender that needs a Controller.</param>
    public static void MakeDefenderController(Defender defender)
    {
        if (defender == null) return;

        switch (defender.TYPE)
        {
            case Defender.DefenderType.SQUIRREL:
                Assert.IsNotNull(defender as Squirrel);
                SquirrelController sc = new SquirrelController(defender as Squirrel);
                instance.defenderControllers.Add(sc);
                break;
            case Defender.DefenderType.BUTTERFLY:
                Assert.IsNotNull(defender as Butterfly);
                ButterflyController bc = new ButterflyController(defender as Butterfly);
                instance.defenderControllers.Add(bc);
                break;
            default:
                throw new System.Exception("Defender " + defender.NAME + " not supported.");
        }
    }

    /// <summary>
    /// Creates an EnemyController for an Enemy of a given type. Adds it to
    /// its respective list of controllers that the ControllerController
    /// updates each frame.
    /// </summary>
    /// <param name="enemy">The Enemy that needs a controller.</param>
    /// <param name="spawnTime">When the Enemy should spawn in the level. </param>
    /// <param name="spawnCoords">Where this Enemy should spawn.</param>
    public static void MakeEnemyController(Enemy enemy, float spawnTime, Vector2 spawnCoords)
    {
        if (enemy == null) return;

        switch (enemy.TYPE)
        {
            case Enemy.EnemyType.KUDZU:
                Kudzu kudzu = enemy as Kudzu;
                Assert.IsNotNull(kudzu, "Kudzu is null.");
                KudzuController kc = new KudzuController(kudzu, spawnTime, spawnCoords);
                instance.enemyControllers.Add(kc);
                break;
            default:
                throw new System.Exception("Enemy " + enemy.NAME + " not supported.");
        }
    }

    /// <summary>
    /// Creates a TreeController for a Tree of a given type. Adds it to
    /// its respective list of controllers that the ControllerController
    /// updates each frame.
    /// </summary>
    /// <param name="tree">The tree that needs a Controller.</param>
    public static void MakeTreeController(Tree tree)
    {
        if (tree == null) return;

        switch (tree.TYPE)
        {
            case Tree.TreeType.BASIC:
                BasicTree basicTree = tree as BasicTree;
                Assert.IsNotNull(basicTree, "BasicTree is null");
                BasicTreeController btc = new BasicTreeController(basicTree);
                instance.treeControllers.Add(btc);
                break;
            default:
                throw new System.Exception("Tree " + tree.NAME + " not supported.");
        }
    }

    /// <summary>
    /// Creates a HazardController for a Hazard of a given type. Adds it to
    /// its respective list of controllers that the ControllerController
    /// updates each frame.
    /// </summary>
    /// <param name="hazard">The Hazard that needs a controller.</param>
    public static void MakeHazardController(Hazard hazard)
    {
        if (hazard == null) return;
        switch (hazard.TYPE)
        {
            case Hazard.HazardType.BOMB_SPLAT:
                BombSplat bombSplat = hazard as BombSplat;
                Assert.IsNotNull(bombSplat, "BombSplat is null.");
                SlowZoneController szc = new SlowZoneController(bombSplat);
                instance.hazardControllers.Add(szc);
                break;
            default:
                throw new System.Exception("Hazard " + hazard.NAME + " not supported.");
        }
    }

    /// <summary>
    /// Creates a StructureController for a Structure of a given type. Adds it to
    /// its respective list of controllers that the ControllerController
    /// updates each frame.
    /// </summary>
    /// <param name="structure">The Structure that needs a controller.</param>
    public static void MakeStructureController(Structure structure)
    {
        if (structure == null) return;

        switch (structure.TYPE)
        {
            case Structure.StructureType.NEXUS:
                Nexus nexus = structure as Nexus;
                Assert.IsNotNull(nexus, "Nexus is null.");
                NexusController nxc = new NexusController(nexus);
                instance.structureControllers.Add(nxc);
                break;
            case Structure.StructureType.NEXUS_HOLE:
                NexusHole nexusHole = structure as NexusHole;
                Assert.IsNotNull(nexusHole, "NexusHole is null.");
                NexusHoleController nhc = new NexusHoleController(nexusHole);
                instance.structureControllers.Add(nhc);
                break;
            default:
                throw new System.Exception("Structure " + structure.NAME + " not supported.");
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
    }

    /// <summary>
    /// Informs the ControllerController of the most recent GameState
    /// so that it knows how to update its sub controllers.
    /// </summary>
    /// <param name="state">The most recent game state. </param>
    public static void InformOfGameState(GameState state) { instance.gameState = state; }
}
