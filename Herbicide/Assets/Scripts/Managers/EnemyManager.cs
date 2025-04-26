using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System.Text.RegularExpressions;

/// <summary>
/// Manages what and how many Enemies spawn in a level.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the EnemyManager singleton
    /// </summary>
    private static EnemyManager instance;

    /// <summary>
    /// Sorted spawn times of all Enemies this level.
    /// </summary>
    private List<float> spawnTimes;

    /// <summary>
    /// true if the EnemyManager has been populated.
    /// </summary>
    private bool populated;

    #endregion

    #region Methods

    /// <summary>
    /// Gives the EnemyManager a dictionary of Enemy spawn marker locations
    /// and their unparsed spawn data. This method decodes the data and instantiates
    /// Enemies to its specification.<br></br>
    /// 
    /// In Tiled, an unparsed spawn data string looks like this:<br></br>
    ///  
    /// kudzu0-0.5, knotwood1-1.5<br></br>
    /// 
    /// This means that a Kudzu will spawn 0.5 seconds after the first stage starts,
    /// and a Knotwood will spawn 1.5 seconds after the second stage starts.<br></br>
    /// 
    /// These values are clamped depending on how many stages are in the level.
    ///  </summary>
    /// <param name="markers">All Enemy spawn markers.</param>
    public static void PopulateWithEnemies(Dictionary<Vector2Int, string> markers)
    {
        if (markers == null) return;
        Assert.IsFalse(Populated(), "Already populated.");

        // Store a dictionary where the key is the stage and the value is the latest spawn time for that stage.

        // Parse the data.
        foreach (KeyValuePair<Vector2Int, string> marker in markers)
        {
            Vector2Int spawnLocation = marker.Key;
            string unparsedData = marker.Value;
            //List<(string, StageOfDay, float)> parsedData = instance.ParseMarkerDataOld(unparsedData);
            List<(string, float)> parsedDataNew = instance.ParseMarkerDataNew(unparsedData);
            //parsedData = parsedData.OrderBy(x => x.Item2).ThenBy(x => x.Item3).ToList();
            parsedDataNew = parsedDataNew.OrderBy(x => x.Item2).ToList();

            foreach(var enemyData in parsedDataNew)
            {
                GameObject enemyPrefab = instance.GetEnemyPrefabFromString(enemyData.Item1);
                enemyPrefab.SetActive(false);
                Enemy enemyComp = enemyPrefab.GetComponent<Enemy>();
                enemyComp.SetSpawnTime(enemyData.Item2);
                enemyComp.SetSpawnWorldPosition(instance.GetSpawnWorldPositionFromCoordinatePosition(spawnLocation));
                ControllerManager.MakeModelController(enemyComp);
            }

            /* foreach (var enemyData in parsedData)
             {
                 string enemyName = enemyData.Item1;
                 GameObject enemy = instance.GetEnemyPrefabFromString(enemyName);
                 Assert.IsNotNull(enemy);
                 Enemy enemyComp = enemy.GetComponent<Enemy>();
                 enemy.gameObject.SetActive(false);
                 float spawnX = TileGrid.CoordinateToPosition(marker.Key.x);
                 float spawnY = TileGrid.CoordinateToPosition(marker.Key.y);
                 Vector3 spawnWorldPos = new Vector3(spawnX, spawnY, 1);
                 StageOfDay spawnStage = enemyData.Item2;
                 float spawnTime = enemyData.Item3;
                 instance.spawnTimes.Add(spawnTime);
                 enemyComp.SetStage(spawnStage);
                 enemyComp.SetSpawnTime(spawnTime);
                 enemyComp.SetSpawnWorldPosition(spawnWorldPos);

                 Spurge spurgeComp = enemyComp as Spurge;
                 if(spurgeComp != null)
                 {
                     // Spurge in front of enemy to be spawned.
                     GameObject enemySpurgeMinionInFront = EnemyFactory.GetEnemyPrefab(ModelType.SPURGE_MINION);
                     Assert.IsNotNull(enemySpurgeMinionInFront);
                     SpurgeMinion spurgeMinionCompInFront = enemySpurgeMinionInFront.GetComponent<SpurgeMinion>();
                     Assert.IsNotNull(spurgeMinionCompInFront);
                     spurgeMinionCompInFront.gameObject.SetActive(false);
                     spurgeMinionCompInFront.SetSpawnWorldPosition(spawnWorldPos);
                     spurgeMinionCompInFront.SetStage(spawnStage);
                     spurgeMinionCompInFront.SetSpawnTime(spawnTime - 1.0f);

                     // Spurge behind enemy to be spawned.
                     GameObject enemySpurgeMinionBehind = EnemyFactory.GetEnemyPrefab(ModelType.SPURGE_MINION);
                     Assert.IsNotNull(enemySpurgeMinionBehind);
                     SpurgeMinion spurgeMinionCompBehind = enemySpurgeMinionBehind.GetComponent<SpurgeMinion>();
                     Assert.IsNotNull(spurgeMinionCompBehind);
                     spurgeMinionCompBehind.gameObject.SetActive(false);
                     spurgeMinionCompBehind.SetSpawnWorldPosition(spawnWorldPos);
                     spurgeMinionCompBehind.SetStage(spawnStage);
                     spurgeMinionCompBehind.SetSpawnTime(spawnTime + 1.0f);

                     // Add the SpurgeMinions to the Spurge.
                     spurgeComp.AddMinion(spurgeMinionCompInFront);
                     spurgeComp.AddMinion(spurgeMinionCompBehind);

                     // Make the controllers for the SpurgeMinions.
                     ControllerManager.MakeModelController(spurgeMinionCompInFront);
                     ControllerManager.MakeModelController(spurgeMinionCompBehind);
                 }*/

            /*
                            // Store the latest spawn time for this stage.
                            if (latestStageTimes.ContainsKey(spawnStage))
                            {
                                if (spawnTime > latestStageTimes[spawnStage]) latestStageTimes[spawnStage] = spawnTime;
                            }
                            else latestStageTimes.Add(spawnStage, spawnTime);*/

            //}
        }

        //StageController.SetStageData(latestStageTimes);

        instance.spawnTimes.Sort();
        instance.populated = true;
    }

    /// <summary>
    /// Returns the world position of the spawn location from the coordinate position.
    /// </summary>
    /// <param name="coordinatePosition">the coordinate position of the spawn location.</param>
    /// <returns>the world position of the spawn location from the coordinate position.</returns>
    private Vector2 GetSpawnWorldPositionFromCoordinatePosition(Vector2Int coordinatePosition)
    {
        float worldX = TileGrid.CoordinateToPosition(coordinatePosition.x);
        float worldY = TileGrid.CoordinateToPosition(coordinatePosition.y);
        return new Vector2(worldX, worldY);
    }

    /// <summary>
    /// Returns a list of tuples of Enemy names and spawn times.
    /// </summary>
    /// <param name="input">A string of comma-separated names followed by their spawn times.</param>
    /// <returns>A list of tuples of Enemy names and spawn times.</returns>
    private List<(string, float)> ParseMarkerDataNew(string input)
    {
        Assert.IsNotNull(input);

        string[] sliced = input.Split(',');
        List<(string, float)> result = new List<(string, float)>();
        string pattern = @"([a-zA-Z]+)-(\d+(\.\d+)?)";

        foreach (string item in sliced)
        {
            Match match = Regex.Match(item.Trim(), pattern);
            if (!match.Success) continue;

            string enemyName = match.Groups[1].Value;
            float spawnTime = float.Parse(match.Groups[2].Value);

            result.Add((enemyName, spawnTime));
        }

        return result;
    }


    /// <summary>
    /// Returns the number of Enemies that have yet to be spawned in this level. 
    /// </summary>
    /// <returns>the number of Enemies that have yet to be spawned 
    /// in this level. </returns>
    /// <param name="gameTime">Current game time. </param>
    public static int NumEnemiesThatRemainToBeSpawned(float gameTime)
    {
        Assert.IsNotNull(instance.spawnTimes);

        int enemiesToGo = 0;
        foreach (float spawnTime in instance.spawnTimes)
        {
            if (spawnTime > gameTime) enemiesToGo++;
        }
        return enemiesToGo;
    }

    /// <summary>
    /// Returns true if the EnemyManager has been populated.
    /// </summary>
    /// <returns>true if the EnemyManager has been populated;
    /// otherwise, false. </returns>
    public static bool Populated() => instance.populated;

    /// <summary>
    /// Finds and sets the EnemyManager singleton.
    /// </summary>
    /// <param name="levelController"></param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        EnemyManager[] enemyManagers = FindObjectsOfType<EnemyManager>();
        Assert.IsNotNull(enemyManagers, "Array of InputControllers is null.");
        Assert.AreEqual(1, enemyManagers.Length);
        instance = enemyManagers[0];
        instance.spawnTimes = new List<float>();
    }

    /// <summary>
    /// Returns a GameObject with an Enemy component attached that matches
    /// the string passed into this method. If the string does not match
    /// an Enemy type, throws an Exception.
    /// </summary>
    /// <param name="enemyName">The name of the Enemy.</param>
    /// <returns>A GameObject with an Enemy component that matches the string
    /// passed into this method.</returns>
    private GameObject GetEnemyPrefabFromString(string enemyName)
    {
        Assert.IsNotNull(enemyName);

        switch (enemyName.ToLower())
        {
            case "knotwood":
                return EnemyFactory.GetEnemyPrefab(ModelType.KNOTWOOD);
            case "kudzu":
                return EnemyFactory.GetEnemyPrefab(ModelType.KUDZU);
            case "spurge":
                return EnemyFactory.GetEnemyPrefab(ModelType.SPURGE);
            default:
                break;
        }

        throw new System.NotSupportedException(enemyName + " not supported.");
    }

    #endregion
}
