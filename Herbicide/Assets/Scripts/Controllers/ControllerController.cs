using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for Enemy and Defender controllers.
/// </summary>
public class ControllerController : MonoBehaviour
{
    /// <summary>
    /// List of active DefenderControllers.
    /// </summary>
    private List<DefenderController> defenderControllers;

    /// <summary>
    /// List of active EnemyControllers.
    /// </summary>
    private List<EnemyController> enemyControllers;

    /// <summary>
    /// List of active TreeControllers.
    /// </summary>
    private List<TreeController> treeControllers;

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

        instance.defenderControllers = new List<DefenderController>();
        instance.enemyControllers = new List<EnemyController>();
        instance.treeControllers = new List<TreeController>();
    }

    /// <summary>
    /// Adds a Defender to the queue of Defenders who need a DefenderController.
    /// </summary>
    /// <param name="defender">The Defender to add to the queue.</param>
    public static void MakeDefenderController(Defender defender)
    {
        if (defender == null) return;

        switch (defender.TYPE)
        {
            case Defender.DefenderType.SQUIRREL:
                Assert.IsNotNull(defender as Squirrel);
                SquirrelController sc = new SquirrelController(defender);
                instance.defenderControllers.Add(sc);
                break;
            default:
                throw new System.Exception("Defender " + defender + " not supported.");
        }
    }

    /// <summary>
    /// Adds an Enemy to the queue of Enemies who need an EnemyController.
    /// </summary>
    /// <param name="enemy">The Enemy to add to the queue.</param>
    /// <param name="spawnTime">when the Enemy should spawn in the level. </param>
    /// <param name="spawnCoords">where this Enemy should spawn</param>
    public static void MakeEnemyController(Enemy enemy, float spawnTime, Vector2 spawnCoords)
    {
        if (enemy == null) return;

        MovingEnemy movingEnemy = enemy as MovingEnemy;
        if (movingEnemy != null)
        {
            MovingEnemyController mec = new MovingEnemyController(movingEnemy, spawnTime, spawnCoords);
            instance.enemyControllers.Add(mec);
        }
    }

    /// <summary>
    /// Adds a non-Defender Placeable Object to the queue of 
    /// PlaceableObjects that need a DefenderController.
    /// </summary>
    /// <param name="defender">The non-Defender Placeable Object to add to
    /// the queue.</param>
    public static void MakeTreeController(Tree tree)
    {
        if (tree == null) return;

        TreeController sc = new TreeController(tree);
        instance.treeControllers.Add(sc);
    }

    /// <summary>
    /// Returns a set of all alive, active Enemy objects.
    /// </summary>
    /// <returns> a set of all alive, active Enemy objects.</returns>
    public static HashSet<Enemy> GetAllActiveEnemies()
    {
        HashSet<Enemy> enemies = new HashSet<Enemy>();
        foreach (EnemyController ec in instance.enemyControllers)
        {
            if (ec != null && ec.EnemyAlive()) enemies.Add(ec.GetEnemy());
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
        foreach (TreeController tc in instance.treeControllers)
        {
            if (tc != null && tc.TreeAlive()) trees.Add(tc.GetTree());

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
    }

    /// <summary>
    /// Updates all Controllers managed by the ControllerController.
    /// </summary>
    /// <param name="targets">All potential targets</param>
    /// <param name="dt">Current game time</param>
    public static void UpdateAllControllers(List<ITargetable> targets, float dt)
    {
        //Copy targets
        List<ITargetable> targetsCopy = new List<ITargetable>(targets);

        //General updates
        instance.TryRemoveControllers();
        instance.InformControllersOfGameState();

        //Update EnemyControllers
        instance.enemyControllers.ForEach(ec => ec.UpdateEnemy(targetsCopy, dt));

        //Update DefenderControllers
        targets.AddRange(GetAllActiveEnemies());
        instance.defenderControllers.ForEach(dc => dc.UpdateDefender(targets));

        //Update TreeControllers
        instance.treeControllers.ForEach(tc => tc.UpdateTree());
    }

    /// <summary>
    /// Informs the ControllerController of the most recent GameState
    /// so that it knows how to update its sub controllers.
    /// </summary>
    /// <param name="state">The most recent game state. </param>
    public static void InformOfGameState(GameState state)
    {
        instance.gameState = state;
    }
}
