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
    /// kudzu1-0.5,knotwood1-1.5<br></br>
    /// 
    /// This means that a Kudzu will spawn at stage 1 at 0.5 seconds into that stage,
    /// not the entire level.
    ///  </summary>
    /// <param name="markers">All Enemy spawn markers.</param>
    /// <param name="mapHeight">The height, in tiles, of the map. </param>
    public static void PopulateWithEnemies(Dictionary<Vector2Int, string> markers, int mapHeight)
    {
        if (markers == null) return;
        Assert.IsFalse(Populated(), "Already populated.");

        // Store a dictionary where the key is the stage and the value is the latest spawn time for that stage.
        Dictionary<int, float> latestStageTimes = new Dictionary<int, float>();

        // Parse the data.
        foreach (KeyValuePair<Vector2Int, string> marker in markers)
        {
            Vector2Int spawnLocation = marker.Key;
            string unparsedData = marker.Value;
            List<(string, int, float)> parsedData = instance.ParseMarkerData(unparsedData);
            parsedData = parsedData.OrderBy(x => x.Item2).ThenBy(x => x.Item3).ToList();

            foreach (var enemyData in parsedData)
            {
                string enemyName = enemyData.Item1;
                GameObject enemy = instance.GetEnemyPrefabFromString(enemyName);
                Assert.IsNotNull(enemy);
                Enemy enemyComp = enemy.GetComponent<Enemy>();
                enemy.gameObject.SetActive(false);
                enemyComp.GetCollider().enabled = false;
                float spawnX = TileGrid.CoordinateToPosition(marker.Key.x);
                float spawnY = TileGrid.CoordinateToPosition(marker.Key.y);
                Vector3 spawnWorldPos = new Vector3(spawnX, spawnY, 1);
                int spawnStage = enemyData.Item2;
                float spawnTime = enemyData.Item3;
                instance.spawnTimes.Add(spawnTime);
                enemyComp.SetStage(spawnStage);
                enemyComp.SetSpawnTime(spawnTime);
                enemyComp.SetSpawnPos(spawnWorldPos);
                ControllerController.MakeModelController(enemyComp);

                // Store the latest spawn time for this stage.
                if (latestStageTimes.ContainsKey(spawnStage))
                {
                    if (spawnTime > latestStageTimes[spawnStage]) latestStageTimes[spawnStage] = spawnTime;
                }
                else latestStageTimes.Add(spawnStage, spawnTime);

            }
        }

        StageController.SetStageData(latestStageTimes);

        instance.spawnTimes.Sort();
        instance.populated = true;
    }

    /// <summary>
    /// Returns a list of tuples of Enemy names, their stages, and spawn times.
    /// </summary>
    /// <param name="input">A string of comma separated names followed
    /// by their stage and spawn times. </param>
    /// <returns>a list of tuples of Enemy names, their stages, and spawn times.</returns>
    private List<(string, int, float)> ParseMarkerData(string input)
    {
        Assert.IsNotNull(input);

        string[] sliced = input.Split(',');
        List<(string, int, float)> result = new List<(string, int, float)>();
        string pattern = @"([a-zA-Z]+)(\d+)-(\d+(\.\d+)?)";

        foreach (string item in sliced)
        {
            Match match = Regex.Match(item, pattern);
            if (!match.Success) continue;
            string enemyName = match.Groups[1].Value;
            int stage = int.Parse(match.Groups[2].Value);
            float spawnTime = float.Parse(match.Groups[3].Value);
            result.Add((enemyName, stage, spawnTime));
        }

        return result;
    }

    /// <summary>
    /// Returns the number of Enemies that have yet to be spawned in this level. 
    /// </summary>
    /// <returns>the number of Enemies that have yet to be spawned 
    /// in this level. </returns>
    /// <param name="gameTime">Current game time. </param>
    public static int EnemiesRemaining(float gameTime)
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
