using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Manages the board of Tile objects. Responsible for spawning
/// the current map from Tiled JSON, pathfinding, and tracking
/// where Models are on the board.
/// </summary>
public class TileGrid : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The width, in Tiles, of the TileGrid.
    /// </summary>
    public int gridWidth;

    /// <summary>
    /// The height, in Tiles, of the TileGrid.
    /// </summary>
    public int gridHeight;

    /// <summary>
    /// All Tile prefabs, indexed by their type
    /// </summary>
    [SerializeField]
    private List<GameObject> tilePrefabs;

    /// <summary>
    /// All Flooring prefabs, indexed by their type
    /// </summary>
    [SerializeField]
    private List<GameObject> flooringPrefabs;

    /// <summary>
    /// true if the TileGrid spawned its Tiles
    /// </summary>
    private bool generated;

    /// <summary>
    /// THE TileGrid instance
    /// </summary>
    private static TileGrid instance;

    /// <summary>
    /// Tiles in the TileGrid, mapped to by their coordinates
    /// </summary>
    private Dictionary<Vector2Int, Tile> tileMap;

    /// <summary>
    /// Tiles in the TileGrid.
    /// </summary>
    private List<Tile> tiles;

    /// <summary>
    /// All GrassTiles in this TileGrid.
    /// </summary>
    private HashSet<Tile> grassTiles;

    /// <summary>
    /// Tiles on the edge of the TileGrid.
    /// </summary>
    private HashSet<Tile> edgeTiles;

    /// <summary>
    /// All first Global Tile IDs that exist among all tilesets used in
    /// this TileGrid.
    /// </summary>
    private List<int> firstGIDs;

    /// <summary>
    /// Tiles and all PlaceableObjects on them.
    /// </summary>
    private Dictionary<Tile, HashSet<PlaceableObject>> tileObjectMap;

    /// <summary>
    /// Total number of enemy spawn markers.
    /// </summary>
    private int numEnemySpawnMarkers;

    /// <summary>
    /// The waypoints in the TileGrid.
    /// </summary>
    private List<Waypoint> waypoints;

    /// <summary>
    /// A mappping of path IDs to the waypoints that are part of them.
    /// The key is the path ID, and the value is a list of sorted waypoints.<br></br>
    /// 
    /// Example: <br></br>
    /// 
    /// 0 -> [Waypoint0]<br></br>
    /// 1 -> [Waypoint0, Waypoint1]<br></br>
    /// 2 -> [Waypoint0, Waypoint1, Waypoint2]
    /// </summary>
    private Dictionary<int, List<Waypoint>> pathWaypointMap;

    /// <summary>
    /// All positions of shore tiles in the TileGrid.
    /// </summary>
    private List<Vector3> shorePositions;

    /// <summary>
    /// All positions of GoalHoles in the TileGrid.
    /// </summary>
    private List<Vector3> goalHolePositions;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the `instance` field of the TileGrid class. If already set,
    /// does nothing. Also instantiates the 2D arrays to hold the Tiles
    /// and items placed on them.
    /// </summary>
    /// <param name="levelController">the LevelController making this
    /// singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        //Safety checks.
        if (instance != null) return;
        if (levelController == null) return;

        //Set singleton.
        TileGrid[] grid = FindObjectsOfType<TileGrid>();
        Assert.IsTrue(grid != null && grid.Length == 1, "Too many / not " +
            "enough TileGrids found.");
        instance = grid[0];

        //Initialize tile map. Set the center.
        instance.tileMap = new Dictionary<Vector2Int, Tile>();
        instance.tiles = new List<Tile>();
        instance.grassTiles = new HashSet<Tile>();
        instance.edgeTiles = new HashSet<Tile>();
        instance.tileObjectMap = new Dictionary<Tile, HashSet<PlaceableObject>>();
        instance.shorePositions = new List<Vector3>();
        instance.waypoints = new List<Waypoint>();
        instance.pathWaypointMap = new Dictionary<int, List<Waypoint>>();
        instance.goalHolePositions = new List<Vector3>();
    }


    /// <summary>
    /// Returns the Camera's position at the center of the grid. This is calculated by taking
    /// the top leftmost shore and the bottom rightmost shore and averaging their positions.
    /// </summary>
    /// <returns>the position for the Camera, that if set to, represents the center
    /// of the grid.</returns>
    private Vector3 GetCameraPositionAtCenterOfGrid()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Vector3 pos in shorePositions)
        {
            minX = Mathf.Min(minX, pos.x * BoardConstants.TileSize);
            maxX = Mathf.Max(maxX, pos.x * BoardConstants.TileSize);
            minY = Mathf.Min(minY, pos.y * BoardConstants.TileSize);
            maxY = Mathf.Max(maxY, pos.y * BoardConstants.TileSize);
        }

        float centerXPos = (minX + maxX) / 2f;
        float centerYPos = (minY + maxY) / 2f;
        return new Vector3(centerXPos + BoardConstants.TileSize/2f, centerYPos - BoardConstants.TileSize/2f, ViewConstants.CameraZPosition);
    }

    /// <summary>
    /// Returns a GameObject with an Structure component attached that matches
    /// the string passed into this method. If the string does not match
    /// a Structure type, throws an Exception.
    /// </summary>
    /// <param name="structureName">The name of the Structure.</param>
    /// <returns>A GameObject with a Structure component that matches the string
    /// passed into this method.</returns>
    private GameObject GetStructurePrefabFromString(string structureName)
    {
        Assert.IsNotNull(structureName);

        switch (structureName.ToLower())
        {
            case "spawnhole":
                return HoleFactory.GetHolePrefab(ModelType.SPAWN_HOLE);
            case "goalhole":
                return HoleFactory.GetHolePrefab(ModelType.GOAL_HOLE);
            case "basictree":
                return TreeFactory.GetTreePrefab(ModelType.BASIC_TREE);
            case "speedtree":
                return TreeFactory.GetTreePrefab(ModelType.SPEED_TREE);
            case "stonewall":
                return WallFactory.GetWallPrefab(ModelType.STONE_WALL);
            default:
                break;
        }

        throw new System.NotSupportedException(structureName + " not supported.");
    }

    /// <summary>
    /// Returns a GameObject with an Flooring component attached that matches
    /// the string passed into this method. If the string does not match
    /// a Flooring type, throws an Exception.
    /// </summary>
    /// <param name="flooringName">The name of the Flooring.</param>
    /// <returns>A GameObject with a Flooring component that matches the string
    /// passed into this method.</returns>
    private GameObject GetFlooringPrefabFromString(string flooringName)
    {
        Assert.IsNotNull(flooringName);

        switch (flooringName.ToLower())
        {
            case "soilflooring":
                return FlooringFactory.GetFlooringPrefab(ModelType.SOIL_FLOORING);
            default:
                break;
        }

        throw new System.NotSupportedException(flooringName + " not supported.");
    }

    /// <summary>
    /// Adds a Tile to this TileGrid's collections.
    /// </summary>
    /// <param name="t">the Tile to add</param>
    private void AddTile(Tile t)
    {
        //Safety checks
        if (t == null || tileMap == null) return;
        Assert.IsNull(TileExistsAt(t.Coordinates.x, t.Coordinates.y));
        Vector2Int key = new Vector2Int(t.Coordinates.x, t.Coordinates.y);
        tileMap.Add(key, t);
        tileObjectMap.Add(t, new HashSet<PlaceableObject>());
        tiles.Add(t);
        if (t.TYPE == ModelType.GRASS_TILE) grassTiles.Add(t);
    }

    /// <summary>
    /// Returns the Tile at some coordinates; if it doesn't exist, returns
    /// null.
    /// </summary>
    /// <param name="x">The X-Coordinate.</param>
    /// <param name="y">The Y-Coordinate.</param>
    /// <returns>the Tile at some coordinates; null if no Tile exists there.
    /// </returns>
    private Tile TileExistsAt(int x, int y)
    {
        //Safety checks
        if (tileMap == null) return null;
        Vector2Int coordinates = new Vector2Int(x, y);
        if (!tileMap.ContainsKey(coordinates)) return null;

        return tileMap[coordinates];
    }

    /// <summary>
    /// Returns the Tile at some world position; if it doesn't exist, returns
    /// null.
    /// </summary>
    /// <param name="worldPosition">The world position to check.</param>
    /// <returns>the Tile at some world position; null if no Tile exists there.
    /// </returns>
    private Tile TileExistsAt(Vector2 worldPosition)
    {
        int x = PositionToCoordinate(worldPosition.x);
        int y = PositionToCoordinate(worldPosition.y);
        return TileExistsAt(x, y);
    }

    /// <summary>
    /// Returns a world position that corresponds to a Tile's
    /// coordinate.
    /// </summary>
    /// <param name="coord">the coordinate to convert.</param>
    /// <returns>the world position representation of a Tile coordinate.
    /// </returns>
    public static float CoordinateToPosition(int coord) => coord * BoardConstants.TileSize + BoardConstants.TileSize / 2;

    /// <summary>
    /// Returns a Tile coordinate that corresponds to a Tile's
    /// world position.
    /// </summary>
    /// <param name="coord">the world position to convert.</param>
    /// <returns>the Tile coordinate representation of a world position.
    /// </returns>
    public static int PositionToCoordinate(float pos) => (int)MathF.Floor(pos / BoardConstants.TileSize);

    /// <summary>
    /// Returns true if some world coordinates represent the position
    /// of a GoalHole.
    /// </summary>
    /// <param name="worldPosition">the coordinates to check.</param>
    /// <returns>true if some world coordinates represent the position
    /// of a GoalHole; otherwise, false. /// .</returns>
    public static bool IsGoalHole(Vector2 worldPosition)
    {
        if (!instance.TileExistsAt(worldPosition)) return false;

        int coordX = PositionToCoordinate(worldPosition.x);
        int coordY = PositionToCoordinate(worldPosition.y);
        Tile t = instance.TileExistsAt(coordX, coordY);
        Assert.IsNotNull(t);

        if (!instance.tileObjectMap.ContainsKey(t)) return false;
        HashSet<PlaceableObject> useables = instance.tileObjectMap[t];
        return useables.Any(useable => useable is GoalHole);
    }

    /// <summary>
    /// Returns true if some world coordinates represent the position
    /// of a GoalHole.
    /// </summary>
    /// <param name="worldCoords">the coordinates to check.</param>
    /// <returns>true if some world coordinates represent the position
    /// of a GoalHole; otherwise, false. /// .</returns>
    public static bool IsGoalHole(Vector3? worldCoords)
    {
        Assert.IsTrue(worldCoords.HasValue);
        Vector2 convertedCoords = new Vector2();
        convertedCoords.x = worldCoords.Value.x;
        convertedCoords.y = worldCoords.Value.y;
        return IsGoalHole(convertedCoords);
    }

    /// <summary>
    /// Returns true if a given Model Type exists on the surface at some coordinate position.
    /// </summary>
    /// <param name="coordinates">the coordinate position to check.</param>
    /// <param name="modelType">the ModelType to check for.</param>
    /// <returns>true if a given Model Type exists on the surface at some coordinate
    /// position; otherwise, false.</returns>
    public static bool ModelTypeExistsOnSurfaceAt(Vector2Int coordinates, ModelType modelType)
    {
        Tile t = instance.TileExistsAt(coordinates);
        if (t == null) return false;
        return t.HasModel(modelType);
    }

    /// <summary>
    /// Returns true if a given class Type exists on the surface at some coordinate position.
    /// </summary>
    /// <param name="coordinates">the coordinate position to check.</param>
    /// <typeparam name="T">the class Type to check for.</typeparam>
    /// <returns>true if a given class Type exists on the surface at some coordinate
    /// position; otherwise, false.</returns>
    public static bool ClassTypeExistsOnSurfaceAt<T>(Vector2Int coordinates)
    {
        Tile t = instance.TileExistsAt(coordinates);
        if (t == null) return false;
        return t.HasModel<T>();
    }

    /// <summary>
    /// Returns the coordinates of a neighbor in a given direction.
    /// </summary>
    /// <param name="baseCoordinates">the base coordinates.</param>
    /// <param name="direction">the direction to check.</param>
    /// <returns>the coordinates of a neighbor in a given direction.</returns>
    public static Vector2Int GetCoordinatesOfNeighborInDirection(Vector2Int baseCoordinates, Direction direction)
    {
        return direction switch
        {
            Direction.NORTH => new Vector2Int(baseCoordinates.x, baseCoordinates.y + 1),
            Direction.SOUTH => new Vector2Int(baseCoordinates.x, baseCoordinates.y - 1),
            Direction.WEST => new Vector2Int(baseCoordinates.x - 1, baseCoordinates.y),
            Direction.EAST => new Vector2Int(baseCoordinates.x + 1, baseCoordinates.y),
            _ => baseCoordinates
        };
    }

    #endregion

    #region Board Generation

    /// <summary>
    /// Returns true if the TileGrid is completely spawned.
    /// </summary>
    /// <returns>true if the TileGrid is generated; otherwise, false.
    /// </returns>
    public bool IsGenerated() => generated;

    /// <summary>
    /// Informs the TileGrid that it has been completely generated.
    /// </summary>
    private void SetGenerated() => generated = true;

    /// <summary>
    /// Returns the correct position of the Camera after generating
    /// the TileGrid and the Tiles within it.
    /// </summary>
    /// <param name="levelController">the LevelController spawning the grid.</param>
    /// <param name="data">the TiledData object containing grid layer information.</param>
    /// <returns>the position for the Camera, that if set to, represents the center
    /// of the TileGrid.</returns>
    public static Vector3 SpawnGrid(LevelController levelController, TiledData data)
    {
        //Safety checks
        if (levelController == null) return Vector2.zero;
        if (instance.IsGenerated()) return Vector2.zero;
        if (data == null) return Vector2.zero;

        //Clear all collections
        instance.tileMap.Clear();
        instance.edgeTiles.Clear();

        //Gather Tile IDs
        instance.FindGIDs(data);

        //Spawn our layers: Tiles first, Edges second, Flooring last.
        instance.gridWidth = data.GetLayerDimensions().x;
        instance.gridHeight = data.GetLayerDimensions().y;
        data.GetWaterLayers().ForEach(l => SpawnLayer(l, instance.gridWidth, instance.gridHeight));
        data.GetGrassLayers().ForEach(l => SpawnLayer(l, instance.gridWidth, instance.gridHeight));
        data.GetShoreLayers().ForEach(l => SpawnLayer(l, instance.gridWidth, instance.gridHeight));
        instance.SetNeighbors(instance.gridWidth, instance.gridHeight);

        data.GetObjectLayers().ForEach(l => SpawnObjectLayer(l, data.GetMapHeight()));

        // Assert we have everything we need
        int numSpawnHoles = 0;
        foreach (var pair in instance.tileObjectMap)
        {
            numSpawnHoles += pair.Value.Count(obj => obj is SpawnHole);
        }
        Assert.IsTrue(numSpawnHoles > 0, "No SpawnHoles found in TileGrid.");
        Assert.IsTrue(instance.numEnemySpawnMarkers > 0, "No enemy spawn markers found in TileGrid.");

        //Finishing touches
        instance.SetGenerated();
        return instance.GetCameraPositionAtCenterOfGrid();
    }

    /// <summary>
    /// Spawns a layer of Tiles or Flooring from a LayerData object.
    /// </summary>
    /// <param name="layer">The LayerData object to spawn.</param>
    /// <param name="layerWidth">The width, in Tiles, of the layer to spawn.</param>
    /// <param name="layerHeight">The height, in Tiles, of the layer to spawn.</param>
    private static void SpawnLayer(LayerData layer, int layerWidth, int layerHeight)
    {
        Assert.IsNotNull(layer, "LayerData `layer` is null");
        Assert.IsTrue(layerHeight > 0, "Layer height must be positive.");
        Assert.IsTrue(layerWidth > 0, "Layer width must be positive.");
        Assert.IsFalse(layer.IsObjectLayer(), "Cannot spawn object layers.");
        Assert.IsFalse(layer.IsEnemyLayer(), "Enemy Layers should not be handled by TileGrids.");
        Assert.IsFalse(layer.IsPlaceableLayer(), "Use SpawnPlaceableLayer().");


        if (layer.IsTileLayer() || layer.IsEdgeLayer())
        {
            int currRow = layerHeight - 1;
            int currCol = 0;
            string layerName = layer.GetLayerName();
            Vector2Int coords = new Vector2Int();
            foreach (int tileInt in layer.GetTileData())
            {
                if (tileInt > 0)
                {
                    coords.Set(currCol, currRow);
                    int localId = instance.FindLocalTileId(tileInt);
                    if (layer.IsTileLayer()) instance.MakeTile(layerName, coords, localId);
                    else if (layer.IsEdgeLayer()) instance.MakeEdge(layerName, coords, localId);
                }

                currCol = (currCol + 1) % layerWidth;
                currRow = currCol == 0 ? currRow - 1 : currRow;
            }
        }
        else
        {
            //Future layer types go here.
        }
    }

    /// <summary>
    /// Spawns a layer of Objects from a LayerData object.
    /// </summary>
    /// <param name="layer">The LayerData object to spawn.</param>
    /// <param name="mapHeight">The height of the LayerData layer.</param>
    private static void SpawnObjectLayer(LayerData layer, int mapHeight)
    {
        Assert.IsNotNull(layer, "LayerData `layer` is null");
        Assert.IsTrue(layer.IsObjectLayer(), "Cannot spawn tile layers.");

        string layerName = layer.GetLayerName();
        if (layerName.ToLower() == "enemies") return;
        switch (layerName.ToLower())
        {
            case "structures":
                List<ObjectData> structures = new List<ObjectData>();
                structures.AddRange(layer.GetStructureObjectData());

                foreach (ObjectData obToSpawn in structures)
                {
                    GameObject spawnedStructure = instance.GetStructurePrefabFromString(obToSpawn.GetStructureName());
                    Assert.IsNotNull(spawnedStructure);
                    Mob mob = spawnedStructure.GetComponent<Mob>();
                    Vector3 spawnPos = new Vector3(obToSpawn.GetSpawnCoordinates(mapHeight).x, obToSpawn.GetSpawnCoordinates(mapHeight).y, 1);
                    if(mob as GoalHole != null) instance.goalHolePositions.Add(spawnPos);
                    mob.SetSpawnWorldPosition(spawnPos);
                    Tile targetTile = instance.TileExistsAt(obToSpawn.GetSpawnCoordinates(mapHeight).x, obToSpawn.GetSpawnCoordinates(mapHeight).y);
                    ControllerManager.MakeModelController(mob);
                    instance.tileObjectMap[targetTile].Add(mob);
                    targetTile.Place(mob);
                }
                break;
            case "markers":
                List<ObjectData> markers = new List<ObjectData>();
                markers.AddRange(layer.GetMarkerObjectData());
                Dictionary<Vector2Int, string> enemySpawnMarkers = new Dictionary<Vector2Int, string>();
                foreach (ObjectData markToSpawn in markers)
                {
                    if (markToSpawn.GetMarkerName().ToLower() == "enemyspawn")
                    {
                        int x = markToSpawn.GetSpawnCoordinates(mapHeight).x;
                        int y = markToSpawn.GetSpawnCoordinates(mapHeight).y;
                        string unparsedData = markToSpawn.GetEnemySpawnData();
                        enemySpawnMarkers.Add(new Vector2Int(x, y), unparsedData);
                        instance.numEnemySpawnMarkers++;
                    }
                }
                EnemyManager.PopulateWithEnemies(enemySpawnMarkers);
                break;
            case "flooring":
                List<ObjectData> floorings = new List<ObjectData>();
                floorings.AddRange(layer.GetFlooringObjectData());
                foreach (ObjectData obToSpawn in floorings)
                {
                    GameObject spawnedFlooring = instance.GetFlooringPrefabFromString(obToSpawn.GetFlooringName());
                    Assert.IsNotNull(spawnedFlooring);
                    Flooring flooring = spawnedFlooring.GetComponent<Flooring>();
                    Assert.IsNotNull(flooring);
                    int flooringX = obToSpawn.GetSpawnCoordinates(mapHeight).x;
                    int flooringY = obToSpawn.GetSpawnCoordinates(mapHeight).y;
                    Tile tileOn = instance.TileExistsAt(flooringX, flooringY);
                    Assert.IsNotNull(tileOn);
                    tileOn.Place(flooring as IFixedSurface);
                }
                break;
            case "waypoints":
                List<ObjectData> waypoints = new List<ObjectData>();
                waypoints.AddRange(layer.GetWaypointObjectData());
                foreach(ObjectData waypoint in waypoints)
                {
                    int x = waypoint.GetSpawnCoordinates(mapHeight).x;
                    int y = waypoint.GetSpawnCoordinates(mapHeight).y;
                    Vector3 spawnPos = new Vector3(x + BoardConstants.TileSize/2f, y + BoardConstants.TileSize/2f, 1);
                    int xCoord = PositionToCoordinate(spawnPos.x);
                    int yCoord = PositionToCoordinate(spawnPos.y);
                    Vector2Int coords = new Vector2Int(xCoord, yCoord);
                    string pathIds = waypoint.GetPathIdData();
                    string orderInPaths = waypoint.GetOrderInPathData();
                    Waypoint wp = new Waypoint(spawnPos, coords, pathIds, orderInPaths);
                    instance.waypoints.Add(wp);
                }
                instance.PreprocessWaypoints(instance.waypoints);
                break;
        }
    }


    /// <summary>
    /// Finds all first Global Tile IDs from all tilesets used in this TileGrid
    /// and adds them to the `firstGIDs` field. If the field is already set,
    /// does nothing.
    /// </summary>
    /// <param name="data">The parsed Tiled JSON file.</param>
    private void FindGIDs(TiledData data)
    {
        Assert.IsNotNull(data, "TiledData `data` is null.");

        if (firstGIDs != null) return;
        firstGIDs = data.GetAllFirstGIDs();
    }

    /// <summary>
    /// Returns the local ID of a Tile in its tileset.
    /// </summary>
    /// <param name="globalTileId">the Tile's global Tile Id.</param>
    /// <returns>the local ID of the Tile.</returns>
    private int FindLocalTileId(int globalTileId)
    {
        //Access a list of first GIDs, sorted in ascending order
        //Find out, through an algorithm, which two `globalTileId` falls between
        //Subtract the first GID from `globalTileId`

        for (int i = 0; i < firstGIDs.Count; i++)
        {
            if (i == firstGIDs.Count - 1) return globalTileId - firstGIDs[i];
            bool greater = globalTileId >= firstGIDs[i];
            bool lesser = globalTileId < firstGIDs[i + 1];
            if (greater && lesser) return globalTileId - firstGIDs[i];
        }

        throw new System.ArgumentException("Invalid global tile Id.", "globalTileId");
    }

    /// <summary>
    /// Assigns all Tiles and Edges in this TileGrid their adjacent
    /// neighbors.
    /// </summary>
    /// <param name="mapWidth">The width, in tiles, of the map.</param>
    /// <param name="mapHeight">The height, in tiles, of the map.</param>
    private void SetNeighbors(int mapWidth, int mapHeight)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Tile t = instance.TileExistsAt(x, y);
                if (t != null) t.SetNeighbors(GetNeighbors(t));
            }
        }
    }

    /// <summary>
    /// Returns an array of a Tile's four neighboring Surfaces. For any Tile that 
    /// does not exist, returns null instead.
    /// </summary>
    /// <param name="target">the Tile of which to get neighbors</param>
    /// <returns>a Tile's four neighboring Surfaces</returns>
    private IFixedSurface[] GetNeighbors(Tile target)
    {
        IFixedSurface[] targetNeighbors = new IFixedSurface[4];
        targetNeighbors[0] = GetNeighbor(target, Direction.NORTH);
        targetNeighbors[1] = GetNeighbor(target, Direction.EAST);
        targetNeighbors[2] = GetNeighbor(target, Direction.SOUTH);
        targetNeighbors[3] = GetNeighbor(target, Direction.WEST);
        return targetNeighbors;
    }

    /// <summary>
    /// Returns a Tile's neighboring ISurface in a given direction; returns null
    /// if that neighbor does not exist.
    /// </summary>
    /// <param name="origin">The tile of which to get a neighboring Tile.</param>
    /// <param name="direction">The direction of the neighbor.</param>
    /// <returns>A Tile's neighboring ISurface in a given direction, or null if it
    /// doesn't exist.</returns>
    private static IFixedSurface GetNeighbor(Tile origin, Direction direction)
    {
        Assert.IsNotNull(origin, "Origin Tile is null.");
        (int x, int y) = GetNeighborCoordinates(origin.Coordinates.x, origin.Coordinates.y, direction);
        return instance.TileExistsAt(x, y);
    }

    /// <summary>
    /// Returns the coordinates of a Tile's neighbor in a given direction.
    /// </summary>
    /// <param name="x">The X-coordinate of the Tile.</param>
    /// <param name="y">The Y-coordinate of the Tile.</param>
    /// <param name="direction">The direction of the neighbor.</param>
    private static (int, int) GetNeighborCoordinates(int x, int y, Direction direction)
    {
        return direction switch
        {
            Direction.NORTH => (x, y + 1),
            Direction.EAST => (x + 1, y),
            Direction.SOUTH => (x, y - 1),
            Direction.WEST => (x - 1, y),
            _ => (x, y)
        };
    }

    /// <summary>
    /// Creates a Tile of a certain type and adds it to the collection
    /// of Tiles in this TileGrid.
    /// </summary>
    /// <param name="type">The Tile's type, in string form.</param>
    /// <param name="coords">The (X, Y) coordinates at which to make the Tile.</param>
    /// <param name="tiledId">The edge tile's local ID, in Tiled.</param>
    private void MakeTile(string type, Vector2Int coords, int tiledId)
    {
        Assert.IsNotNull(type);
        Tile existCheck = TileExistsAt(coords.x, coords.y);
        if (existCheck != null) return;

        float xWorldPos = CoordinateToPosition(coords.x);
        float yWorldPos = CoordinateToPosition(coords.y);

        switch (type.ToLower())
        {
            case "grass":
                GrassTile grass = MakeTile(xWorldPos, yWorldPos, ModelType.GRASS_TILE) as GrassTile;
                Assert.IsNotNull(grass);
                grass.DefineWithCoordinates(coords.x, coords.y);
                grass.SetSpriteUsingLocalTiledIndex(tiledId);
                AddTile(grass);
                break;
            case "water":
                WaterTile water = MakeTile(xWorldPos, yWorldPos, ModelType.WATER_TILE) as WaterTile;
                water.DefineWithCoordinates(coords.x, coords.y);
                water.SetSpriteUsingLocalTiledIndex(tiledId);
                AddTile(water);
                break;
            default:
                throw new System.Exception("Reached default case in MakeTile().");
        }
    }

    /// <summary>
    /// Returns a Tile that should be in the TileGrid. Instantiates that
    /// Tile.
    /// </summary>
    /// <param name="xPos">The X-world position of the Tile.</param>
    /// <param name="yPos">The Y-world position of the Tile.</param>
    /// <param name="type">The TileType of the Tile prefab.</param>
    /// <returns>The instantiated Tile that should be in the TileGrid.
    /// </returns>
    private Tile MakeTile(float xPos, float yPos, ModelType type)
    {
        GameObject prefab = TileFactory.GetTilePrefab(type);
        Tile tile = Instantiate(prefab).GetComponent<Tile>();
        Transform tileTransform = tile.transform;
        tile.name = type.ToString();
        tileTransform.position = new Vector3(xPos, yPos);
        tileTransform.localScale = Vector2.one * BoardConstants.TileSize * 1.0001f; // This multiplier prevents tiny gaps
        tileTransform.SetParent(instance.transform);

        return tile;
    }

    /// <summary>
    /// Creates an Edge Tile of a certain type and adds it to the collection
    /// of Tiles in this TileGrid.
    /// </summary>
    /// <param name="type">The Edge Tile's type, in string form.</param>
    /// <param name="coords">The (X, Y) coordinates at which to make the Edge Tile.</param>
    /// <param name="tiledId">The edge tile's local ID, in Tiled.</param>
    private void MakeEdge(string type, Vector2Int coords, int tiledId)
    {
        Assert.IsNotNull(type);
        Tile existCheck = TileExistsAt(coords.x, coords.y);
        if (existCheck != null) return;

        float xWorldPos = CoordinateToPosition(coords.x);
        float yWorldPos = CoordinateToPosition(coords.y);

        switch (type.ToLower())
        {
            case "shore":
                ShoreTile shore = MakeTile(xWorldPos, yWorldPos, ModelType.SHORE_TILE) as ShoreTile;
                Assert.IsNotNull(shore);
                shore.DefineWithCoordinates(coords.x, coords.y);
                shore.SetSpriteUsingLocalTiledIndex(tiledId);
                AddTile(shore);
                shorePositions.Add(new Vector3(xWorldPos, yWorldPos, 1));
                break;
            default:
                throw new System.Exception("Reached default case in MakeEdge().");
        }
    }

    /// <summary>
    /// Preprocesses waypoints to create a map of pathId to sorted waypoints.
    /// </summary>
    /// <param name="allWaypoints">List of all waypoints in the game.</param>
    private void PreprocessWaypoints(List<Waypoint> allWaypoints)
    {
        if (allWaypoints == null) throw new ArgumentNullException(nameof(allWaypoints));

        var result = new Dictionary<int, List<Waypoint>>();

        foreach (var waypoint in allWaypoints)
        {
            for (int i = 0; i < waypoint.PathIds.Length; i++)
            {
                int pathId = waypoint.PathIds[i];
                int order = waypoint.OrderInPaths[i];
                if (!result.ContainsKey(pathId)) result[pathId] = new List<Waypoint>();
                result[pathId].Add(waypoint);
            }
        }

        foreach (var key in result.Keys.ToList())
        {
            result[key] = result[key]
                .OrderBy(wp => wp.OrderInPaths[Array.IndexOf(wp.PathIds, key)])
                .ToList();
        }
        pathWaypointMap = result;
    }

    /// <summary>
    /// Returns a new list of sorted waypoints for a given path.
    /// </summary>
    /// <param name="pathId">The ID of the path.</param>
    /// <returns>a new list of sorted waypoints for a given path.</returns>
    public static List<Waypoint> GetSortedWaypointsInPath(int pathId)
    {
        Assert.IsTrue(instance.pathWaypointMap.ContainsKey(pathId), $"PathId {pathId} does not exist.");
        List<Waypoint> waypoints = instance.pathWaypointMap[pathId];
        return new List<Waypoint>(waypoints);
    }

    #endregion

    #region Waypoints & Pathfinding


    /// <summary>
    /// Returns the world position of the next waypoint for a given path and index.
    /// If the index is out of bounds for the path, returns the closest GoalHole
    /// world position.
    /// </summary>
    /// <param name="pathId">The ID of the path.</param>
    /// <param name="currentIndex">The current waypoint index along the path.</param>
    /// <returns>The world position of the next waypoint.</returns>
    public static Vector3 GetNextWaypointWorldPosition(int pathId, int currentIndex)
    {
        if(!instance.WaypointExists(pathId, currentIndex)) return instance.GetClosestGoalHolePositionToLastWaypointInPath(pathId, currentIndex);
        else return instance.GetNextWaypointFromPathIdAndCurrentIndex(pathId, currentIndex).WorldPosition;
    }

    /// <summary>
    /// Returns true if a waypoint exists for a given path and index.
    /// </summary>
    /// <param name="pathId">The ID of the path.</param>
    /// <param name="currentIndex">The current waypoint index along the path.</param>
    /// <returns>true if a waypoint exists for a given path and index; otherwise,
    /// false. </returns>
    private bool WaypointExists(int pathId, int currentIndex)
    {
        if (!pathWaypointMap.ContainsKey(pathId)) return false;
        var waypoints = pathWaypointMap[pathId];
        return currentIndex >= 0 && currentIndex < waypoints.Count;
    }

    /// <summary>
    /// Returns the world position of the closest GoalHole to the last waypoint in a path.
    /// </summary>
    /// <param name="pathId">The ID of the path.</param>
    /// <param name="currentIndex">The current waypoint index along the path.</param>
    /// <returns>the world position of the closest GoalHole to the last waypoint in a path.</returns>
    private Vector3 GetClosestGoalHolePositionToLastWaypointInPath(int pathId, int currentIndex)
    {
        Assert.IsTrue(pathWaypointMap.ContainsKey(pathId), "No path found for ID " + pathId);
        Assert.IsTrue(pathWaypointMap[pathId].Count > 0, "No waypoints found for path ID " + pathId);
        Assert.IsTrue(goalHolePositions.Count > 0, "No GoalHoles found in TileGrid.");

        Vector3 lastWaypointPos = GetNextWaypointFromPathIdAndCurrentIndex(pathId, currentIndex - 1).WorldPosition;
        Vector3 closestGoalHolePos = goalHolePositions[0];
        float minDistance = Vector3.Distance(lastWaypointPos, closestGoalHolePos);
        if (minDistance > 0)
        {
            foreach (Vector3 goalHolePos in goalHolePositions)
            {
                float distance = Vector3.Distance(lastWaypointPos, goalHolePos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestGoalHolePos = goalHolePos;
                }
            }
        }
        return closestGoalHolePos;
    }

    /// <summary>
    /// Returns the next waypoint for a given path and index.
    /// </summary>
    /// <param name="pathId">The ID of the path.</param>
    /// <param name="currentIndex">The current waypoint index along the path.</param>
    /// <returns>the next waypoint for a given path and index.</returns>
    private Waypoint GetNextWaypointFromPathIdAndCurrentIndex(int pathId, int currentIndex)
    {
        Assert.IsTrue(WaypointExists(pathId, currentIndex), $"Waypoint does not exist for pathId {pathId} and index {currentIndex}.");
        return waypoints[currentIndex];
    }

    /// <summary>
    /// Returns the position of the next Tile in the path from a starting
    /// Tile to a goal Tile. If no such path exists, returns the starting
    /// Tile's position. <br></br>
    /// 
    /// Uses the A* pathfinding algorithm to calculate this position.
    /// Manhattan distance is used as the heuristic function.
    /// </summary>
    /// <param name="startPos">The starting position of the path.</param>
    /// <param name="goalPos">The ending position of the path. </param>
    public static Vector3 NextTilePosTowardsGoalUsingAStar(Vector3 startPos, Vector3 goalPos)
    {
        //From given positions, find the corresponding Tiles.

        int xStartCoord = PositionToCoordinate(startPos.x);
        int yStartCoord = PositionToCoordinate(startPos.y);
        int xGoalCoord = PositionToCoordinate(goalPos.x);
        int yGoalCoord = PositionToCoordinate(goalPos.y);

        if (PathfindingCache.HasCachedNextPosition(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord))
        {
            var nextPos = PathfindingCache.GetCachedNextPosition(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord);
            return new Vector3(CoordinateToPosition(nextPos.Item1), CoordinateToPosition(nextPos.Item2), 1);
        }

        Tile startTile = instance.TileExistsAt(xStartCoord, yStartCoord);
        Tile goalTile = instance.TileExistsAt(xGoalCoord, yGoalCoord);

        if (startTile == null || goalTile == null) return startPos;
        if (startPos == goalPos) return startPos;

        //Initialize data structures.
        List<Tile> openList = new List<Tile> { startTile };
        HashSet<Tile> closedList = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float> { { startTile, 0 } };
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float>
        {
            { startTile, instance.ManhattanDistance(startTile, goalTile) }
        };

        //Iterative A* Algorithm.
        while (openList.Count > 0)
        {
            // Choose a random tile with the lowest fScore
            float minFScore = openList.Min(tile => fScore[tile]);
            List<Tile> minFScoreTiles = openList.Where(tile => fScore[tile] == minFScore).ToList();
            Tile current = minFScoreTiles[UnityEngine.Random.Range(0, minFScoreTiles.Count)];

            if (current == goalTile)
            {
                startTile.SetColor(Color.white);
                PathfindingCache.CacheNextPosition(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord, current.Coordinates.x, current.Coordinates.y);
                PathfindingCache.CacheReachability(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord, true);

                // Convert goal tile to world position
                float goalWorldX = CoordinateToPosition(goalTile.Coordinates.x);
                float goalWorldY = CoordinateToPosition(goalTile.Coordinates.y);
                return new Vector3(goalWorldX, goalWorldY, 1);
            }

            foreach (Tile neighbor in instance.GetNeighbors(current))
            {
                if (neighbor == goalTile)
                {
                    List<Tile> path = instance.ReconstructPath(cameFrom, current);
                    if (path.Count > 1)
                    {
                        Tile nextTile = path[1];
                        if (!nextTile.IsCurrentlyWalkable()) continue;
                        startTile.SetColor(Color.white);
                        float nextWorldX = CoordinateToPosition(nextTile.Coordinates.x);
                        float nextWorldY = CoordinateToPosition(nextTile.Coordinates.y);
                        return new Vector3(nextWorldX, nextWorldY, 1);
                    }
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Tile neighbor in instance.GetNeighbors(current))
            {
                if (neighbor == null) continue;
                if (closedList.Contains(neighbor)) continue;

                if (neighbor != goalTile && !neighbor.IsCurrentlyWalkable())
                {
                    closedList.Add(neighbor);
                    continue;
                }

                float tentativeGScore = gScore[current] + 1;
                if (!openList.Contains(neighbor)) openList.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor]) continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + instance.ManhattanDistance(neighbor, goalTile);
            }
        }

        // No path found
        PathfindingCache.CacheReachability(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord, false);
        return new Vector3(int.MinValue, int.MinValue, int.MinValue);
    }

    /// <summary>
    /// Returns the position of the next Tile in the path from a starting
    /// Tile to a goal Tile. If no such path exists, returns the starting
    /// Tile's position. The next tile may not be the opposite direction
    /// of the pathfinder<br></br>
    /// 
    /// Uses the A* pathfinding algorithm to calculate this Tile distance
    /// from the start to the goal.
    /// </summary>
    /// <param name="startPos">The starting position of the path.</param>
    /// <param name="goalPos">The ending position of the path. </param>
    /// <param name="currentDirection">The current direction of the pathfinder.</param>
    /// <returns>the position of the next Tile in the path from a starting position
    /// to a goal position.</returns>
    public static Vector3 NextTilePosTowardsGoalUsingPathfindingDistance(Vector3 startPos, Vector3 goalPos, Direction currentDirection)
    {
        int xStartCoord =  PositionToCoordinate(startPos.x);
        int yStartCoord = PositionToCoordinate(startPos.y);
        int xGoalCoord = PositionToCoordinate(goalPos.x);
        int yGoalCoord = PositionToCoordinate(goalPos.y);
        Tile startTile = instance.TileExistsAt(xStartCoord, yStartCoord);
        Tile goalTile = instance.TileExistsAt(xGoalCoord, yGoalCoord);

        if (startTile == null || goalTile == null) return startPos;

        ISurface[] neighbors = instance.GetNeighbors(startTile);
        Vector3 bestTilePosition = startPos;
        int bestDistance = int.MaxValue;

        foreach (var neighbor in neighbors)
        {
            if (neighbor is Tile neighborTile && neighborTile.IsCurrentlyWalkable())
            {
                Vector3 neighborPos = neighborTile.GetWorldPosition();
                int neighborX = PositionToCoordinate(neighborPos.x);
                int neighborY = PositionToCoordinate(neighborPos.y);
                int pathDistance = GetPathfindingTileDistance(neighborPos, goalPos);

                Direction? directionToNeighbor = null;
                if (neighborX > xStartCoord) directionToNeighbor = Direction.EAST;
                else if (neighborX < xStartCoord) directionToNeighbor = Direction.WEST;
                else if (neighborY > yStartCoord) directionToNeighbor = Direction.NORTH;
                else if (neighborY < yStartCoord) directionToNeighbor = Direction.SOUTH;

                Direction oppositeDirection = currentDirection switch
                {
                    Direction.NORTH => Direction.SOUTH,
                    Direction.SOUTH => Direction.NORTH,
                    Direction.EAST => Direction.WEST,
                    Direction.WEST => Direction.EAST,
                    _ => throw new InvalidOperationException("Unexpected direction")
                };

                if (directionToNeighbor != null && directionToNeighbor != oppositeDirection)
                {
                    if (pathDistance >= 0 && pathDistance < bestDistance)
                    {
                        bestDistance = pathDistance;
                        bestTilePosition = neighborPos;
                    }
                }
            }
        }
        return bestTilePosition;
    }

    /// <summary>
    /// Returns the tile distance between two positions using the pathfinding
    /// algorithm. This takes into account the actual path cost, which can vary
    /// based on obstacles and path winding.
    /// </summary>
    /// <param name="startPos">The starting position.</param>
    /// <param name="goalPos">The goal position.</param>
    /// <returns>
    /// The tile distance between the two positions, or -1 if no path exists.
    /// </returns>
    public static int GetPathfindingTileDistance(Vector3 startPos, Vector3 goalPos)
    {
        // Convert positions to coordinates
        int xStartCoord = PositionToCoordinate(startPos.x);
        int yStartCoord = PositionToCoordinate(startPos.y);
        int xGoalCoord = PositionToCoordinate(goalPos.x);
        int yGoalCoord = PositionToCoordinate(goalPos.y);

        if (PathfindingCache.HasCachedDistance(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord))
        {
            return PathfindingCache.GetCachedDistance(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord);
        }

        // Find the corresponding tiles
        Tile startTile = instance.TileExistsAt(xStartCoord, yStartCoord);
        Tile goalTile = instance.TileExistsAt(xGoalCoord, yGoalCoord);

        if (startTile == null || goalTile == null) return -1; // No valid path
        if (startTile == goalTile) return 0; // Same tile

        // Initialize data structures
        List<Tile> openList = new List<Tile> { startTile };
        HashSet<Tile> closedList = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, int> gScore = new Dictionary<Tile, int> { { startTile, 0 } };

        while (openList.Count > 0)
        {
            // Find the tile in the open list with the lowest gScore
            Tile current = openList.OrderBy(tile => gScore[tile]).First();

            if (current == goalTile)
            {
                // Path found, return the cost
                int distance = gScore[goalTile];
                PathfindingCache.CacheDistance(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord, distance);
                return gScore[goalTile];
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Tile neighbor in instance.GetNeighbors(current))
            {
                if (neighbor == null || closedList.Contains(neighbor)) continue;

                // Skip non-walkable tiles unless it's the goal
                if (neighbor != goalTile && !neighbor.IsCurrentlyWalkable())
                {
                    closedList.Add(neighbor);
                    continue;
                }

                int tentativeGScore = gScore[current] + 1; // Distance to the neighbor is always 1 tile

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                // Update path information
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
            }
        }

        PathfindingCache.CacheDistance(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord, -1);
        return -1;
    }

    /// <summary>
    /// Returns the Manhattan distance between two Tiles.
    /// </summary>
    /// <param name="a">The first Tile.</param>
    /// <param name="b">The second Tile.</param>
    /// <returns>the Manhattan distance between two Tiles.</returns>
    private float ManhattanDistance(Tile a, Tile b)
    {
        Assert.IsNotNull(a, "Tile `a` is null.");
        Assert.IsNotNull(b, "Tile `b` is null.");
        return Math.Abs(a.Coordinates.x - b.Coordinates.x) + Math.Abs(a.Coordinates.y - b.Coordinates.y);
    }

    /// <summary>
    /// Reconstructs the path from the starting tile to a specified
    /// tile using the "cameFrom" breadcrumbs.
    /// </summary>
    /// <param name="cameFrom">A dictionary that maps each tile to the
    /// tile that led to it during the A* search.</param>
    /// <param name="current">The end tile of the path,
    ///  typically the goal tile.</param>
    /// <returns>A list of tiles representing the path from the start
    /// to the specified end tile. The first tile in the list is the starting tile,
    /// and the last tile is the specified end tile.</returns>
    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        Assert.IsNotNull(cameFrom, "Dictionary `cameFrom` is null.");
        Assert.IsNotNull(current, "Tile `current` is null.");

        List<Tile> path = new List<Tile> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            Assert.IsNotNull(current);
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Returns true if a path exists from one position to another.
    /// </summary>
    /// <param name="cache">The PathfindingCache making this call.</param>
    /// <param name="startPos">The starting position.</param>
    /// <param name="goalPos">The ending position.</param>
    /// <returns>true if a path exists from start to goal; otherwise,
    /// false. </returns>
    public static bool CanReach(Vector3 startPos, Vector3 goalPos)
    {
        int xStartCoord = PositionToCoordinate(startPos.x);
        int yStartCoord = PositionToCoordinate(startPos.y);
        int xGoalCoord = PositionToCoordinate(goalPos.x);
        int yGoalCoord = PositionToCoordinate(goalPos.y);

        if (PathfindingCache.HasCachedReachability(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord))
        {
            return PathfindingCache.GetCachedReachability(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord);
        }

        bool isReachable = NextTilePosTowardsGoalUsingAStar(startPos, goalPos) != new Vector3(int.MinValue, int.MinValue, int.MinValue);
        PathfindingCache.CacheReachability(xStartCoord, yStartCoord, xGoalCoord, yGoalCoord, isReachable);
        return isReachable;
    }

    #endregion
}
