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
    private List<PlaceableObjectController> defenderControllers;

    /// <summary>
    /// List of active EnemyControllers.
    /// </summary>
    private List<PlaceableObjectController> enemyControllers;

    /// <summary>
    /// List of active TreeControllers.
    /// </summary>
    private List<PlaceableObjectController> treeControllers;

    /// <summary>
    /// List of active ProjectileControllers.
    /// </summary>
    private List<PlaceableObjectController> projectileControllers;

    /// <summary>
    /// List of active HazardControllers.
    /// </summary>
    private List<PlaceableObjectController> hazardControllers;

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

        instance.defenderControllers = new List<PlaceableObjectController>();
        instance.enemyControllers = new List<PlaceableObjectController>();
        instance.treeControllers = new List<PlaceableObjectController>();
        instance.projectileControllers = new List<PlaceableObjectController>();
        instance.hazardControllers = new List<PlaceableObjectController>();
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
    /// <param name="startPos">Where the Hazard starts.</param>
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
    /// Returns a set of all alive, active Enemy objects.
    /// </summary>
    /// <returns> a set of all alive, active Enemy objects.</returns>
    public static HashSet<Enemy> GetAllActiveEnemies()
    {
        HashSet<Enemy> enemies = new HashSet<Enemy>();
        foreach (PlaceableObjectController pc in instance.enemyControllers)
        {
            if (pc == null || !pc.ValidModel()) continue;
            Enemy model = pc.GetModel() as Enemy;
            if (model == null) continue;
            if (!model.Dead()) enemies.Add(model);
        }
        return enemies;
    }

    /// <summary>
    /// Returns a set of all alive, active Trees
    /// </summary>
    /// <returns>a set of all alive, active Tree objects</returns>
    public static HashSet<Tree> GetAllActiveTrees()
    {
        HashSet<Tree> trees = new HashSet<Tree>();
        foreach (PlaceableObjectController pc in instance.treeControllers)
        {
            if (pc == null || !pc.ValidModel()) continue;
            Tree model = pc.GetModel() as Tree;
            if (model == null) continue;
            if (!model.Dead()) trees.Add(model);
        }
        return trees;
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
    }

    /// <summary>
    /// Runs through the list of uncollected PlaceableObjectControllers to identify
    /// their most downcasted version. Adds it to the correct list.
    /// </summary>
    /// <param name="controllers">The list of PlaceableObjectControllers to filter.</param>
    private void FilterCollectedControllers(List<PlaceableObjectController> controllers)
    {

        List<PlaceableObjectController> defControllers = new List<PlaceableObjectController>();
        List<PlaceableObjectController> emyControllers = new List<PlaceableObjectController>();
        List<PlaceableObjectController> treControllers = new List<PlaceableObjectController>();
        List<PlaceableObjectController> proControllers = new List<PlaceableObjectController>();
        List<PlaceableObjectController> hazControllers = new List<PlaceableObjectController>();

        foreach (PlaceableObjectController controller in controllers)
        {
            List<PlaceableObjectController> extricatedControllers =
                controller.ExtricateControllers();

            foreach (PlaceableObjectController extricatedController in extricatedControllers)
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
            }
        }

        defenderControllers.AddRange(defControllers);
        enemyControllers.AddRange(emyControllers);
        treeControllers.AddRange(treControllers);
        hazardControllers.AddRange(hazControllers);
        projectileControllers.AddRange(proControllers);
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
        instance.FilterCollectedControllers(instance.enemyControllers);
        instance.enemyControllers.ForEach(ec => ec.UpdateModel());

        // Update DefenderControllers
        instance.FilterCollectedControllers(instance.defenderControllers);
        instance.defenderControllers.ForEach(dc => dc.UpdateModel());

        // Update TreeControllers
        instance.FilterCollectedControllers(instance.treeControllers);
        instance.treeControllers.ForEach(tc => tc.UpdateModel());

        // Update ProjectileControllers
        instance.FilterCollectedControllers(instance.projectileControllers);
        instance.projectileControllers.ForEach(pc => pc.UpdateModel());

        // Update HazardControllers
        instance.FilterCollectedControllers(instance.hazardControllers);
        instance.hazardControllers.ForEach(hc => hc.UpdateModel());
    }

    /// <summary>
    /// Informs the ControllerController of the most recent GameState
    /// so that it knows how to update its sub controllers.
    /// </summary>
    /// <param name="state">The most recent game state. </param>
    public static void InformOfGameState(GameState state) { instance.gameState = state; }
}
