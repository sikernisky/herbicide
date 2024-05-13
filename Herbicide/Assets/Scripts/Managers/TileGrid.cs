using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using UnityEngine.UIElements;
using System.IO.Compression;
using UnityEditor.Timeline;


/// <summary>
/// Represents the board of Tile objects. This board is spawned using
/// data from a Tiled2D JSON file.
/// 
/// Main responsibilies of the TileGrid singleton include:<br></br>
/// 
/// (1) Instantiating and defining Tiles<br></br>
/// (2) Getting information about Tiles<br></br>
/// (3) Passing click events to Tiles<br></br>
/// (4) Flooring and placing on Tiles<br></br>
/// (5) Pathfinding between Tiles
/// </summary>
public class TileGrid : MonoBehaviour
{
    /// <summary>
    /// Width and height of a Tile in the TileGrid
    /// </summary>
    public const float TILE_SIZE = 1f;

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
    /// Red color for debugging and pathfinding.
    /// </summary>
    public static readonly Color32 PATHFINDING_RED = new Color32(255, 0, 0, 60);

    /// <summary>
    /// Blue color for debugging and pathfinding.
    /// </summary>
    public static readonly Color32 PATHFINDING_BLUE = new Color32(0, 0, 255, 255);

    /// <summary>
    /// Red color for hovering over a Tile while attempting to place.
    /// </summary>
    public static readonly Color32 HIGHLGHT_RED = new Color32(255, 60, 60, 255);

    /// <summary>
    /// Blue color for hovering over a Tile while attempting to place.
    /// </summary>
    public static readonly Color32 HIGHLGHT_BLUE = new Color32(60, 255, 60, 255);

    /// <summary>
    /// THE TileGrid instance
    /// </summary>
    private static TileGrid instance;

    /// <summary>
    /// The centermost Tile in this TileGrid.
    /// </summary>
    private Vector2Int center;

    /// <summary>
    /// Tiles in the TileGrid, mapped to by their coordinates
    /// </summary>
    private Dictionary<Vector2Int, Tile> tileMap;

    /// <summary>
    /// Tiles in the TileGrid.
    /// </summary>
    private List<Tile> tiles;

    /// <summary>
    /// All Floorings in this TileGrid.
    /// </summary>
    private HashSet<Flooring> floorings;

    /// <summary>
    /// All GrassTiles in this TileGrid.
    /// </summary>
    private HashSet<Tile> grassTiles;

    /// <summary>
    /// Tiles occupied by enemies, mapped to by the Enemies that
    /// occupy them.
    /// </summary>
    private Dictionary<Enemy, Tile> enemyLocations;

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
    /// true if debug mode is on.
    /// </summary>
    private bool debugOn;

    /// <summary>
    /// Tiles that host NexusHoles.
    /// </summary>
    private Dictionary<Tile, NexusHole> nexusHoleHosts;

    /// <summary>
    /// Main update loop for the TileGrid; updates its Tiles.
    /// </summary>
    public static void UpdateTiles()
    {
        // TEMPORARY: We only need to update Grass Tiles.
        foreach (Tile t in instance.grassTiles)
        {
            t.UpdateTile();
        }
    }

    /// <summary>
    /// Updates Tiles in this TileGrid with boolean values of whether an
    /// Enemy sits on it. Also updates the TileGrid's mapping of Enemies
    /// to Tiles.
    /// </summary>
    /// <param name="activeEnemies">All Enemies on the TileGrid.</param>
    public static void TrackEnemyTilePositions(HashSet<Enemy> activeEnemies)
    {
        if (instance.enemyLocations == null) return;
        foreach (Enemy e in activeEnemies)
        {
            //Enemy is null, remove it.
            if (e == null && instance.enemyLocations.ContainsKey(e))
            {
                instance.enemyLocations.Remove(e);
            }

            //Enemy exists in the mapping, see if we need to update it.
            else if (instance.enemyLocations.ContainsKey(e))
            {
                int coordX = PositionToCoordinate(e.GetPosition().x);
                int coordY = PositionToCoordinate(e.GetPosition().y);
                Tile t = instance.TileExistsAt(coordX, coordY);
                if (t != null) instance.enemyLocations[e] = t;
                else Debug.Log("Enemy exists on a nonexistant Tile??");
            }

            //Enemy does not exist in the mapping, add it.
            else
            {
                int coordX = PositionToCoordinate(e.GetPosition().x);
                int coordY = PositionToCoordinate(e.GetPosition().y);
                Tile t = instance.TileExistsAt(coordX, coordY);
                if (t != null) instance.enemyLocations.Add(e, t);
                else Debug.Log("Enemy exists on a nonexistant Tile??");
            }
        }

        //Tell Tiles if an Enemy is on them.
        foreach (KeyValuePair<Vector2Int, Tile> kvp in instance.tileMap)
        {
            if (instance.enemyLocations.ContainsValue(kvp.Value)) kvp.Value.SetOccupiedByEnemy(true);
            else kvp.Value.SetOccupiedByEnemy(false);
        }
    }


    /// <summary>
    /// Detects any player input on a Tile and handles it based on the type
    /// of input.
    /// </summary>
    public static void CheckTileInputEvents()
    {
        instance.CheckTileMouseDown();
        instance.CheckTileMouseUp();
        instance.CheckTileMouseEnter();
        instance.CheckTileMouseExit();
    }

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
        instance.enemyLocations = new Dictionary<Enemy, Tile>();
        instance.edgeTiles = new HashSet<Tile>();
        instance.nexusHoleHosts = new Dictionary<Tile, NexusHole>();
    }

    /// <summary>
    /// Returns the correct position of the Camera after generating
    /// the TileGrid and the Tiles within it.
    /// </summary>
    /// <param name="levelController">the LevelController spawning the grid.</param>
    /// <param name="data">the TiledData object containing grid layer information.</param>
    /// <returns>the position for the Camera, that if set to, represents the center
    /// of the TileGrid.</returns>
    public static Vector2 SpawnGrid(LevelController levelController, TiledData data)
    {
        //Safety checks
        if (levelController == null) return Vector2.zero;
        if (instance.IsGenerated()) return Vector2.zero;
        if (data == null) return Vector2.zero;

        //Clear all collections
        instance.tileMap.Clear();
        instance.edgeTiles.Clear();

        //Set the center
        instance.SetCenterCoordinates(data.GetLayerDimensions());

        //Gather Tile IDs
        instance.FindGIDs(data);

        //Spawn our layers: Tiles first, Edges second, Flooring last.
        int layerWidth = data.GetLayerDimensions().x;
        int layerHeight = data.GetLayerDimensions().y;
        data.GetWaterLayers().ForEach(l => SpawnLayer(l, layerWidth, layerHeight));
        data.GetGrassLayers().ForEach(l => SpawnLayer(l, layerWidth, layerHeight));
        data.GetShoreLayers().ForEach(l => SpawnLayer(l, layerWidth, layerHeight));
        data.GetObjectLayers().ForEach(l => SpawnObjectLayer(l, data.GetMapHeight()));

        // For pathfinding, give tiles their neighbors
        instance.SetNeighbors(layerWidth, layerHeight);

        //Finishing touches
        instance.SetGenerated();
        return instance.GetCameraPositionOnGrid();
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
                    mob.SetSpawnPos(new Vector3(obToSpawn.GetSpawnCoordinates(mapHeight).x, obToSpawn.GetSpawnCoordinates(mapHeight).y, 1));
                    Tile targetTile = instance.TileExistsAt(obToSpawn.GetSpawnCoordinates(mapHeight).x, obToSpawn.GetSpawnCoordinates(mapHeight).y);
                    PlaceOnTile(targetTile, mob);
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
                        // ENEMY SPAWN LOGIC HERE.
                    }
                }
                EnemyManager.PopulateWithEnemies(enemySpawnMarkers, mapHeight);
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
                    FloorTile(tileOn, flooring);
                }
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
    /// Returns the Camera's position at the center of the TileGrid. The center
    /// of the TileGrid is at the world position of the grass tiles' center
    /// tile.
    /// </summary>
    /// <returns>the position for the Camera, that if set to, represents the center
    /// of the TileGrid.</returns>
    private Vector2 GetCameraPositionOnGrid()
    {
        Vector2Int centerCoords = GetCenterCoordinates();
        float centerXPos = CoordinateToPosition(centerCoords.x) - TILE_SIZE / 2;
        float centerYPos = CoordinateToPosition(centerCoords.y) - TILE_SIZE / 2;

        if (centerCoords.x % 2 == 0) centerXPos -= TILE_SIZE / 2;
        if (centerCoords.y % 2 == 0) centerYPos -= TILE_SIZE / 2;

        return new Vector2(centerXPos, centerYPos);
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
                GrassTile grass = MakeTile(xWorldPos, yWorldPos, Tile.TileType.GRASS) as GrassTile;
                Assert.IsNotNull(grass);
                grass.Define(coords.x, coords.y, tiledId);
                AddTile(grass);
                break;
            case "water":
                WaterTile water = MakeTile(xWorldPos, yWorldPos, Tile.TileType.WATER) as WaterTile;
                water.Define(coords.x, coords.y);
                AddTile(water);
                break;
            default:
                throw new System.Exception("Reached default case in MakeTile().");
        }
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
                ShoreTile shore = MakeTile(xWorldPos, yWorldPos, Tile.TileType.SHORE) as ShoreTile;
                Assert.IsNotNull(shore);
                shore.Define(coords.x, coords.y, tiledId);
                AddTile(shore);
                break;
            default:
                throw new System.Exception("Reached default case in MakeEdge().");
        }
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
                if (t != null) t.UpdateSurfaceNeighbors(GetNeighbors(t));
            }
        }
    }


    /// <summary>
    /// Returns the closest edge Tile to a given coordinate.
    /// </summary>
    /// <param name="x">The X-coordinate from which to find the
    /// closest edge Tile.</param>
    /// <param name="y">The Y-coordinate from which to find the
    /// closest edge Tile.</param>
    /// <returns>The (X, Y) coordinates of the closest edge Tile to
    /// a given coordinate.</returns>
    public static Vector2Int NearestEdgeCoordinate(int x, int y)
    {
        float closestPosition = float.MaxValue;
        Tile closestTile = null;
        foreach (Tile t in instance.edgeTiles)
        {
            float distance = t.GetManhattanDistance(x, y);
            if (distance < closestPosition)
            {
                closestTile = t;
                closestPosition = distance;
            }
        }
        if (closestTile == null) return Vector2Int.zero;
        return new Vector2Int(closestTile.GetX(), closestTile.GetY());
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
            case "nexus":
                return NexusFactory.GetNexusPrefab();
            case "nexushole":
                return NexusHoleFactory.GetNexusHolePrefab();
            case "basictree":
                return BasicTreeFactory.GetBasicTreePrefab();
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
    /// Returns a Tile that should be in the TileGrid. Instantiates that
    /// Tile.
    /// </summary>
    /// <param name="xPos">The X-world position of the Tile.</param>
    /// <param name="yPos">The Y-world position of the Tile.</param>
    /// <param name="type">The TileType of the Tile prefab.</param>
    /// <returns>The instantiated Tile that should be in the TileGrid.
    /// </returns>
    private Tile MakeTile(float xPos, float yPos, Tile.TileType type)
    {
        GameObject prefab = TileTypeToPrefab(type);
        Tile tile = Instantiate(prefab).GetComponent<Tile>();
        Transform tileTransform = tile.transform;
        tile.name = type.ToString();
        tileTransform.position = new Vector3(xPos, yPos, 1);
        tileTransform.localScale = Vector2.one * TILE_SIZE * 1.0001f; // This multiplier prevents tiny gaps
        tileTransform.SetParent(instance.transform);

        return tile;
    }

    /// <summary>
    /// Adds a Tile to this TileGrid's collections.
    /// </summary>
    /// <param name="t">the Tile to add</param>
    private void AddTile(Tile t)
    {
        //Safety checks
        if (t == null || tileMap == null) return;
        if (!ValidCoordinates(t.GetX(), t.GetY())) return;
        Assert.IsNull(TileExistsAt(t.GetX(), t.GetY()));
        Vector2Int key = new Vector2Int(t.GetX(), t.GetY());
        tileMap.Add(key, t);
        tiles.Add(t);
        if (t.GetTileType() == Tile.TileType.GRASS) grassTiles.Add(t);
    }

    /// <summary>
    /// Returns the Tile-coordinates of the center-most tile in the TileGrid.
    /// </summary>
    /// <returns>the Tile-coordinates of the center-most tile in the 
    /// TileGrid.</returns>
    private Vector2Int GetCenterCoordinates()
    {
        return center;
    }

    /// <summary>
    /// Sets the (X, Y) coordinates of the center-most tile in the TileGrid.
    /// Left and top-most tiles get priority in the case of even numbers.
    /// </summary>
    /// <param name="gridData">The (X, Y) dimensions of the TileGrid.</param>
    private void SetCenterCoordinates(Vector2Int dims)
    {
        instance.center = new Vector2Int(Mathf.FloorToInt(dims.x) / 2,
                         Mathf.FloorToInt(dims.y) / 2);
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
        if (!ValidCoordinates(x, y)) return null;
        Vector2Int coordinates = new Vector2Int(x, y);
        if (!tileMap.ContainsKey(coordinates)) return null;

        return tileMap[coordinates];
    }

    /// <summary>
    /// Checks if the player clicked down on some Tile. If so, triggers
    /// the mouse down event for that Tile.
    /// </summary>
    private void CheckTileMouseDown()
    {
        Tile tile = InputController.TileClickedDown();
        if (tile == null) return;

        //Tile was clicked down. Put click logic here.
        //if (tile.Occupied()) Debug.Log("yes");
    }

    /// <summary>
    /// Checks if the player clicked up on some Tile. If so, triggers
    /// the mouse up event for that Tile. The event checks for the possibilities,
    /// in order:<br><br>
    /// 
    /// (1) Is the player trying to place something on this Tile?
    /// (2) Is the player trying to use something on this Tile?
    /// </summary>
    private void CheckTileMouseUp()
    {
        Tile tile = InputController.TileClickedUp();
        if (tile == null) return;

        //(1) and (2): Placing and Using
        if (PlacementController.Placing())
        {
            Model placingItem = PlacementController.GetObjectPlacing();
            PlaceableObject itemPlaceable = placingItem as PlaceableObject;
            if (PlaceOnTile(tile, itemPlaceable))
            {
                //Stop the placement event.
                PlacementController.StopGhostPlacing();
                PlacementController.StopPlacingObject(true);
            }
            //TODO: implement (2) in a future sprint.
        }
    }

    /// <summary>
    /// Checks if the player's mouse entered some Tile. If so, triggers
    /// the mouse enter event for that Tile.
    /// </summary>
    private void CheckTileMouseEnter()
    {
        Tile tile = InputController.TileHovered();
        if (tile == null) return;

        // Tile was hovered over. Put hover logic here.
        if (PlacementController.Placing())
        {
            // Ghost place
            Model placingSlottable = PlacementController.GetObjectPlacing();
            PlaceableObject placingPlaceable = placingSlottable as PlaceableObject;
            if (tile.GhostPlace(placingPlaceable))
            {
                PlacementController.StartGhostPlacing(tile);

                // Show attack range if Mob
                Mob mob = placingSlottable as Mob;
                if (mob != null)
                {
                    int ar = Mathf.FloorToInt(mob.BASE_ATTACK_RANGE);
                    if (ar == float.MaxValue || ar <= 0) return;
                    int mobX = tile.GetX();
                    int mobY = tile.GetY();

                    for (int x = mobX - ar; x <= mobX + ar; x++)
                    {
                        for (int y = mobY - ar; y <= mobY + ar; y++)
                        {
                            Tile rangeTile = TileExistsAt(x, y);
                            if (rangeTile == null) continue;

                            // Calculate the distance from the mob to this tile
                            float distance = Mathf.Sqrt(Mathf.Pow(x - mobX, 2) + Mathf.Pow(y - mobY, 2));

                            // Check if the tile is within the circular attack range
                            if (distance <= ar && rangeTile.WALKABLE)
                            {
                                rangeTile.SetHighlighterColor(new Color32(50, 255, 0, 150));
                            }
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Checks if the player's mouse left some Tile. If so, triggers
    /// the mouse exit event for that Tile.
    /// </summary>
    private void CheckTileMouseExit()
    {
        Tile tile = InputController.TileDehovered();
        if (tile == null) return;

        //Tile was dehovered, put logic below
        PlacementController.StopGhostPlacing();

        UnhighlightTiles();
    }

    /// <summary>
    /// Sets every Tile's highlighter layer to be white and invisible.
    /// </summary>
    private void UnhighlightTiles()
    {
        foreach (KeyValuePair<Vector2Int, Tile> kvp in instance.tileMap)
        {
            Tile t = kvp.Value;
            if (t == null) continue;
            t.SetHighlighterColor(new Color32(255, 255, 255, 0));
        }
    }

    /// <summary>
    /// Returns a read-only list of all Tiles in the TileGrid.
    /// </summary>
    /// <returns>a read-only list of all Tiles in the TileGrid.</returns>
    public static IReadOnlyList<Tile> GetAllTiles()
    {
        return instance.tiles.AsReadOnly();
    }

    /// <summary>
    /// Returns the GameObject prefab representing a Tile type.
    /// </summary>
    /// <param name="type">the type of the Tile</param>
    /// <returns>the prefab representing the type</returns>
    private GameObject TileTypeToPrefab(Tile.TileType type)
    {
        if (tilePrefabs == null || (int)type < 0 || (int)type >= tilePrefabs.Count)
            return new GameObject("TILEPREFABS NULL");
        return tilePrefabs[(int)type];
    }

    /// <summary>
    /// Returns a world position that corresponds to a Tile's
    /// coordinate.
    /// </summary>
    /// <param name="coord">the coordinate to convert.</param>
    /// <returns>the world position representation of a Tile coordinate.
    /// </returns>
    public static float CoordinateToPosition(int coord)
    {
        return coord * TILE_SIZE;
    }

    /// <summary>
    /// Returns a Tile coordinate that corresponds to a Tile's
    /// world position.
    /// </summary>
    /// <param name="coord">the world position to convert.</param>
    /// <returns>the Tile coordinate representation of a world position.
    /// </returns>
    public static int PositionToCoordinate(float pos)
    {
        // return (int)pos / (int)TILE_SIZE;
        return (int)(MathF.Round(pos) / MathF.Round(TILE_SIZE));
    }

    /// <summary>
    /// Returns true if some (X, Y) coordinates could possibly exist in
    /// the TileGrid.<br></br>
    /// 
    /// Does not consider whether a Tile at those coordinates has been made
    /// yet.
    /// </summary>
    /// <param name="x">the X-coordinate</param>
    /// <param name="y">the Y-coordinate</param>
    /// <returns>true if some (X, Y) coordinates could possibly exist in
    /// the TileGrid.</returns>
    private bool ValidCoordinates(int x, int y)
    {
        return true;
    }

    /// <summary>
    /// Returns true if the TileGrid is completely spawned.
    /// </summary>
    /// <returns>true if the TileGrid is generated; otherwise, false.
    /// </returns>
    public bool IsGenerated()
    {
        return generated;
    }

    /// <summary>
    /// Informs the TileGrid that it has been completely generated.
    /// </summary>
    private void SetGenerated()
    {
        generated = true;
    }

    /// <summary>
    /// Returns a Tile's neighboring ISurface in a given direction; returns null
    /// if that neighbor does not exist.
    /// </summary>
    /// <param name="origin">the tile of which to get a neighboring Tile</param>
    /// <param name="direction">the direction of the neighbor</param>
    /// <returns>a Tile's neighboring ISurface in a given direction, or null if it
    /// doesn't exist.</returns>
    private static ISurface GetNeighbor(Tile origin, Direction direction)
    {
        //Safety check
        if (origin == null) return null;

        int neighborX;
        int neighborY;
        int originX = origin.GetX();
        int originY = origin.GetY();

        // if (direction == Direction.NORTHWEST)
        // {
        //     neighborX = originX - 1;
        //     neighborY = originY + 1;
        //     Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
        //     if (neighbor != null) return neighbor;
        // }

        if (direction == Direction.NORTH)
        {
            neighborX = originX;
            neighborY = originY + 1;
            Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
            if (neighbor != null) return neighbor;
        }

        // if (direction == Direction.NORTHEAST)
        // {
        //     neighborX = originX + 1;
        //     neighborY = originY + 1;
        //     Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
        //     if (neighbor != null) return neighbor;
        // }

        if (direction == Direction.EAST)
        {
            neighborX = originX + 1;
            neighborY = originY;
            Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
            if (neighbor != null) return neighbor;
        }

        // if (direction == Direction.SOUTHEAST)
        // {
        //     neighborX = originX + 1;
        //     neighborY = originY - 1;
        //     Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
        //     if (neighbor != null) return neighbor;
        // }

        if (direction == Direction.SOUTH)
        {
            neighborX = originX;
            neighborY = originY - 1;
            Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
            if (neighbor != null) return neighbor;
        }

        // if (direction == Direction.SOUTHWEST)
        // {
        //     neighborX = originX - 1;
        //     neighborY = originY - 1;
        //     Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
        //     if (neighbor != null) return neighbor;
        // }

        if (direction == Direction.WEST)
        {
            neighborX = originX - 1;
            neighborY = originY;
            Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
            if (neighbor != null) return neighbor;
        }

        return null;
    }


    /// <summary>
    /// Returns true if the placing of a PlaceableObject on a Tile will be
    /// successful. If so, places it. Otherwise, does nothing and returns false.
    /// </summary>
    /// <param name="candidate">The PlaceableObject to place.</param>
    /// <param name="target">The Tile to place on.</param>
    /// <param name="existing">true if the PlaceableObject to place has already been instantiated. </param>
    /// <returns>true if the PlaceableObject was placed; otherwise, false.</returns>
    public static bool PlaceOnTile(Tile target, PlaceableObject candidate, bool existing = false)
    {
        // Safety checks
        if (target == null || candidate == null) return false;
        ISurface[] neighbors = instance.GetNeighbors(target);
        if (neighbors == null) return false;
        Assert.IsNotNull(target as ISurface);


        // If we can't place on the Tile, return.
        int targetX = target.GetX();
        int targetY = target.GetY();
        Vector2Int size = candidate.SIZE;
        for (int x = targetX; x < size.x + targetX; x++)
        {
            for (int y = targetY; y < size.y + targetY; y++)
            {
                Tile tileAtExpansion = instance.tileMap[new Vector2Int(x, y)];
                if (!tileAtExpansion.CanPlace(candidate, instance.GetNeighbors(tileAtExpansion)))
                {
                    return false;
                }
            }
        }


        // We can place.
        if (!existing)
        {
            Assert.IsNotNull(candidate);

            Defender placedDefender = candidate.GetComponent<Defender>();
            if (placedDefender != null) ControllerController.MakeController(placedDefender);

            Hazard placedSlowZone = candidate.GetComponent<Hazard>();
            if (placedSlowZone != null) ControllerController.MakeController(placedSlowZone);

            Structure placedStructure = candidate.GetComponent<Structure>();
            if (placedStructure != null) ControllerController.MakeController(placedStructure);

            Tree placedTree = candidate.GetComponent<Tree>();
            if (placedTree != null) ControllerController.MakeController(placedTree);
        }



        // Place the candidate.
        target.Place(candidate, instance.GetNeighbors(target));

        NexusHole nexusHoleCandidate = candidate as NexusHole;
        if (candidate as NexusHole != null) instance.nexusHoleHosts.Add(target, nexusHoleCandidate);

        // If the candidate is bigger than 1x1, set its expanded tiles to Occupied.
        if (!candidate.OCCUPIER) return true;
        for (int x = targetX; x < size.x + targetX; x++)
        {
            for (int y = targetY; y < size.y + targetY; y++)
            {
                Tile tileAtExpansion = instance.tileMap[new Vector2Int(x, y)];
                tileAtExpansion.SetOccupant(candidate);
            }
        }

        return true;
    }

    /// <summary>
    /// Returns true if the placing of a PlaceableObject on a Tile will be
    /// successful. If so, places it. Otherwise, does nothing and returns false.
    /// This method takes the coordinates of the Tile.
    /// </summary>
    /// <param name="candidate">The PlaceableObject to place.</param>
    /// <param name="targetCoords">The coordinates of the Tile to place on.</param>
    /// <param name="existing">true if the PlaceableObject to place has already been instantiated. </param>
    /// <returns>true if the PlaceableObject was placed; otherwise, false.</returns>
    public static bool PlaceOnTile(Vector2Int targetCoords, PlaceableObject candidate, bool existing = false)
    {
        //Safety checks
        int xCoord = PositionToCoordinate(targetCoords.x);
        int yCoord = PositionToCoordinate(targetCoords.y);
        Tile target = instance.TileExistsAt(xCoord, yCoord);

        return PlaceOnTile(target, candidate, existing);
    }

    /// <summary>
    /// Paints a tile at (x, y) a color.
    /// </summary>
    /// <param name="x">The X-Coordinate of the Tile.</param>
    /// <param name="y">The Y-Coordinate of the Tile.</param>
    /// <param name="color">The color to paint with.</param>
    public static void PaintTile(int x, int y, Color32 color)
    {
        Tile t = instance.TileExistsAt(x, y);
        if (t == null) return;

        t.SetColor(color);
    }

    /// <summary>
    /// Returns true if the flooring of a Floorable on this Tile will be
    /// successful. If so, floors it. Otherwise, does nothing and returns false.
    /// </summary>
    /// <param name="candidate">The Flooring to floor with.</param>
    /// <param name="target">The Tile to place on.</param>
    /// <returns>true if the Floorable was floored; otherwise, false.</returns>
    public static bool FloorTile(Tile target, Flooring candidate)
    {
        if (target == null) return false;

        if (target.CanFloor(candidate, instance.GetNeighbors(target)))
        {
            Assert.IsNotNull(candidate);


            // Flooring 
            target.Floor(candidate, instance.GetNeighbors(target));
            foreach (ISurface surface in target.GetSurfaceNeighbors())
            {
                Tile neighbor = surface as Tile;
                if (neighbor == null) continue;
                neighbor.UpdateSurfaceNeighbors(instance.GetNeighbors(neighbor));
            }

            // Give Flooring a controller
            ControllerController.MakeController(candidate);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true if the flooring of a Floorable on this Tile will be
    /// successful. If so, floors it. Otherwise, does nothing and returns false.
    /// </summary>
    /// <param name="candidate">The Flooring to floor with.</param>
    /// <param name="targetCoords">The coordinates of the Tile to place on.</param>
    /// <returns>true if the Floorable was floored; otherwise, false.</returns>
    public static bool FloorTile(Vector2Int targetCoords, Flooring candidate)
    {
        int xCoord = PositionToCoordinate(targetCoords.x);
        int yCoord = PositionToCoordinate(targetCoords.y);
        Tile tile = instance.TileExistsAt(xCoord, yCoord);
        return FloorTile(tile, candidate);
    }

    /// <summary>
    /// Returns true if some world coordinates lie within the range of a Tile
    /// in the TileGrid.
    /// </summary>
    /// <param name="worldCoords">the coordinates to check.</param>
    /// <returns>true if some world coordinates lie within the range of a Tile
    /// in the TileGrid; otherwise, false.</returns>
    public static bool OnTile(Vector2 worldCoords)
    {
        int coordX = PositionToCoordinate(worldCoords.x);
        int coordY = PositionToCoordinate(worldCoords.y);

        Tile t = instance.TileExistsAt(coordX, coordY);
        return t != null;
    }

    /// <summary>
    /// Returns true if some world coordinates lie within the range of a Tile
    /// in the TileGrid that is walkable for some enemy.
    /// </summary>
    /// <param name="worldCoords">the coordinates to check.</param>
    /// <returns>true if some world coordinates lie within the range of a Tile
    /// that is walkable by some enemy; otherwise, false.</returns>
    public static bool OnWalkableTile(Vector2 worldCoords)
    {
        if (!OnTile(worldCoords)) return false;

        int coordX = PositionToCoordinate(worldCoords.x);
        int coordY = PositionToCoordinate(worldCoords.y);
        //Debug.Log("world coords: " + worldCoords + ", converted:" + coordX + " " + coordY);
        Tile t = instance.TileExistsAt(coordX, coordY);
        Assert.IsNotNull(t);

        //TODO: Eventually, incorporate Enemy checking.
        return t.WALKABLE;
    }

    /// <summary>
    /// Returns true if some world coordinates represent the position
    /// of a NexusHole.
    /// </summary>
    /// <param name="worldCoords">the coordinates to check.</param>
    /// <returns>true if some world coordinates represent the position
    /// of a NexusHole; otherwise, false. /// .</returns>
    public static bool IsNexusHole(Vector2 worldCoords)
    {
        if (!OnTile(worldCoords)) return false;

        int coordX = PositionToCoordinate(worldCoords.x);
        int coordY = PositionToCoordinate(worldCoords.y);
        Tile t = instance.TileExistsAt(coordX, coordY);
        Assert.IsNotNull(t);

        return instance.nexusHoleHosts.ContainsKey(t);
    }

    /// <summary>
    /// Returns true if some world coordinates represent the position
    /// of a NexusHole.
    /// </summary>
    /// <param name="worldCoords">the coordinates to check.</param>
    /// <returns>true if some world coordinates represent the position
    /// of a NexusHole; otherwise, false. /// .</returns>
    public static bool IsNexusHole(Vector3? worldCoords)
    {
        Assert.IsTrue(worldCoords.HasValue);
        Vector2 convertedCoords = new Vector2();
        convertedCoords.x = worldCoords.Value.x;
        convertedCoords.y = worldCoords.Value.y;
        return IsNexusHole(convertedCoords);
    }

    /// <summary>
    /// Returns an array of a Tile's four neighboring Surfaces. For any Tile that 
    /// does not exist, returns null instead.
    /// </summary>
    /// <param name="target">the Tile of which to get neighbors</param>
    /// <returns>a Tile's four neighboring Surfaces</returns>
    private ISurface[] GetNeighbors(Tile target)
    {
        ISurface[] targetNeighbors = new ISurface[4];
        targetNeighbors[0] = GetNeighbor(target, Direction.NORTH);
        targetNeighbors[1] = GetNeighbor(target, Direction.EAST);
        targetNeighbors[2] = GetNeighbor(target, Direction.SOUTH);
        targetNeighbors[3] = GetNeighbor(target, Direction.WEST);
        return targetNeighbors;
    }

    /// <summary>
    /// Sets the debug mode to be off or on.
    /// </summary>
    /// <param name="levelController">The LevelController instance.</param>
    /// <param name="isOn">true if the debug mode should be on; false if not.</param>
    public static void SetDebug(LevelController levelController, bool isOn)
    {
        Assert.IsNotNull(levelController);
        instance.debugOn = isOn;
        if (!isOn) instance.UnpaintAllTiles();
    }

    /// <summary>
    /// Unpaints all Tiles in this TileGrid. This is done by painting them
    /// white.
    /// </summary>
    private void UnpaintAllTiles()
    {
        Assert.IsNotNull(instance.tileMap);

        foreach (KeyValuePair<Vector2Int, Tile> pair in instance.tileMap)
        {
            instance.tileMap[pair.Key].SetColor(Color.white);
        }
    }

    /// <summary>
    /// Returns true if debug mode is on.
    /// </summary>
    /// <returns>true if debug mode is on; otherwise, false. </returns>
    private bool IsDebugging()
    {
        return debugOn;
    }

    //---------------------BEGIN PATHFINDING-----------------------//


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
    public static Vector3 NextTilePosTowardsGoal(Vector3 startPos, Vector3 goalPos)
    {

        //From given positions, find the corresponding Tiles.
        int xStartCoord = PositionToCoordinate(startPos.x);
        int yStartCoord = PositionToCoordinate(startPos.y);
        int xGoalCoord = PositionToCoordinate(goalPos.x);
        int yGoalCoord = PositionToCoordinate(goalPos.y);
        Tile startTile = instance.TileExistsAt(xStartCoord, yStartCoord);
        Tile goalTile = instance.TileExistsAt(xGoalCoord, yGoalCoord);

        if (instance.IsDebugging()) goalTile.SetColor(PATHFINDING_RED);
        if (startTile == null || goalTile == null) return startPos;

        //Initialize data structures.
        List<Tile> openList = new List<Tile> { startTile };
        HashSet<Tile> closedList = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float> { { startTile, 0 } };
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float>();
        fScore.Add(startTile, instance.ManhattanDistance(startTile, goalTile));

        //Iterative A* Algorithm.
        while (openList.Count > 0)
        {
            Tile current = openList.OrderBy(tile => fScore[tile]).First();

            if (current == goalTile)
            {
                startTile.SetColor(Color.white);
                return new Vector3(goalTile.GetX(), goalTile.GetY(), 1);
            }

            foreach (Tile neighbor in instance.GetNeighbors(current))
            {
                if (neighbor == goalTile)
                {
                    List<Tile> path = instance.ReconstructPath(cameFrom, current);
                    if (path.Count > 1)
                    {
                        foreach (Tile tileToPaint in path)
                        {
                            if (instance.IsDebugging())
                            {
                                if (!tileToPaint.IsWalkable()) tileToPaint.SetColor(Color.blue);
                                else tileToPaint.SetColor(Color.red);
                            }
                        }
                        Tile nextTile = path[1];
                        if (!nextTile.IsWalkable()) continue;
                        startTile.SetColor(Color.white);
                        return new Vector3(nextTile.GetX(), nextTile.GetY(), 1);
                    }
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Tile neighbor in instance.GetNeighbors(current))
            {
                if (neighbor == null) continue;
                if (closedList.Contains(neighbor)) continue;

                if (neighbor != goalTile && !neighbor.IsWalkable())
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

        //Debug.Log("No path found.");

        //No path found, so we return the lowest position possible.
        return new Vector3(float.MinValue, float.MinValue, float.MinValue);
    }

    /// <summary>
    /// Returns the length of the shortest path from a starting Tile to a goal Tile
    /// using the A* pathfinding algorithm. If no path exists, returns -1.
    /// Manhattan distance is used as the heuristic function.
    /// </summary>
    /// <param name="cache">The PathfindingCache making this call.</param>
    /// <param name="startPos">The starting position of the path.</param>
    /// <param name="goalPos">The ending position of the path.</param>
    /// <returns>The length of the path or -1 if no path exists.</returns>
    public static int PathLengthTowardsGoal(PathfindingCache cache, Vector3 startPos, Vector3 goalPos)
    {
        // From given positions, find the corresponding Tiles.
        int xStartCoord = PositionToCoordinate(startPos.x);
        int yStartCoord = PositionToCoordinate(startPos.y);
        int xGoalCoord = PositionToCoordinate(goalPos.x);
        int yGoalCoord = PositionToCoordinate(goalPos.y);
        Tile startTile = instance.TileExistsAt(xStartCoord, yStartCoord);
        Tile goalTile = instance.TileExistsAt(xGoalCoord, yGoalCoord);

        if (startTile == null || goalTile == null) return -1;

        // Initialize data structures.
        List<Tile> openList = new List<Tile> { startTile };
        HashSet<Tile> closedList = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float> { { startTile, 0 } };
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float> { { startTile, instance.ManhattanDistance(startTile, goalTile) } };

        // Iterative A* Algorithm.
        while (openList.Count > 0)
        {
            Tile current = openList.OrderBy(tile => fScore[tile]).First();

            if (current == goalTile)
            {
                List<Tile> path = instance.ReconstructPath(cameFrom, current);
                return path.Count; // Return the length of the path
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Tile neighbor in instance.GetNeighbors(current))
            {
                if (closedList.Contains(neighbor)) continue;

                float tentativeGScore = gScore[current] + 1;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + instance.ManhattanDistance(neighbor, goalTile);
            }
        }

        // No path found, return -1.
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

        return Math.Abs(a.GetX() - b.GetX()) + Math.Abs(a.GetY() - b.GetY());
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
            if (IsDebugging()) current.SetColor(PATHFINDING_BLUE);
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
    public static bool CanReach(PathfindingCache cache, Vector3 startPos, Vector3 goalPos)
    {
        Assert.IsNotNull(cache);
        Vector3 minVector = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        return NextTilePosTowardsGoal(startPos, goalPos) != minVector;
    }
    //----------------------END PATHFINDING------------------------//
}
