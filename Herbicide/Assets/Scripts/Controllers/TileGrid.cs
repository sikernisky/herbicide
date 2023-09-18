using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using System;


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
    /// True if Trees should spawn on Floorings when Tiles spawn.
    /// </summary>
    [SerializeField]
    private bool autoGenerateTrees;

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
        instance.enemyLocations = new Dictionary<Enemy, Tile>();
        instance.edgeTiles = new HashSet<Tile>();
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
        data.GetSoilLayers().ForEach(l => SpawnLayer(l, layerWidth, layerHeight));

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
        Assert.IsFalse(layer.IsEnemyLayer(), "Enemy Layers should not be handled by TileGrids.");

        if (layer.IsTileLayer() || layer.IsFlooringLayer() || layer.IsEdgeLayer())
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
                    else if (layer.IsFlooringLayer()) instance.MakeFlooring(layerName, coords);
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
    /// Creates a Flooring of a certain type and adds it to the collection
    /// of Flooring in this TileGrid.
    /// </summary>
    /// <param name="type">The Flooring's type, in string form.</param>
    /// <param name="coords">The (X, Y) coordinates at which to make the Flooring.</param>
    private void MakeFlooring(string type, Vector2Int coords)
    {
        Tile tileToFloor = TileExistsAt(coords.x, coords.y);
        if (tileToFloor == null) return;

        float xWorldPos = CoordinateToPosition(coords.x);
        float yWorldPos = CoordinateToPosition(coords.y);

        switch (type.ToLower())
        {
            case "soil":
                if (FloorTile(tileToFloor, Flooring.FlooringType.SOIL) && autoGenerateTrees)
                {
                    //Place trees on Floored tiles.
                    Tree tree = TreeFactory.GetTreePrefab(
                        Tree.TreeType.BASIC).GetComponent<Tree>();
                    if (tree == null) break;
                    PlaceOnTile(tileToFloor, tree, GetNeighbors(tileToFloor));
                }
                break;
            default:
                throw new System.Exception("Reached default case in MakeFlooring().");
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
        tileTransform.localScale = Vector2.one * TILE_SIZE;
        tileTransform.SetParent(instance.transform);

        return tile;
    }

    /// <summary>
    /// Adds a Tile to this TileGrid's Tile map, if possible.
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
            ISlottable placingItem = PlacementController.GetObjectPlacing();
            PlaceableObject itemPlaceable = placingItem as PlaceableObject;
            IUsable itemUsable = placingItem as IUsable;
            ISurface[] tileNeighbors = GetNeighbors(tile);
            if (PlaceOnTile(tile, itemPlaceable, tileNeighbors))
            {
                //Stop the placement event.
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

        //Tile was hovered over. Put hover logic here.
        if (PlacementController.Placing())
        {
            ISlottable placingSlottable = PlacementController.GetObjectPlacing();
            PlaceableObject placingPlaceable = placingSlottable as PlaceableObject;
            if (tile.GhostPlace(placingPlaceable)) PlacementController.StartGhostPlacing(tile);
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
    /// Returns the GameObject prefab representing a Flooring type.
    /// </summary>
    /// <param name="type">the type of the Flooring</param>
    /// <returns>the prefab representing the type</returns>
    private GameObject FlooringTypeToPrefab(Flooring.FlooringType type)
    {
        if (flooringPrefabs == null || (int)type < 0 || (int)type >= flooringPrefabs.Count)
            return new GameObject("FLOORINGPREFABS NULL");
        return flooringPrefabs[(int)type];
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
        return (int)pos / (int)TILE_SIZE;
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
    /// Returns true if a Tile exists at some (X, Y) position. This is
    /// different than the private method TileExistsAt(), which returns
    /// a Tile at the given position.
    /// </summary>
    /// <param name="x">the X-coordinate</param>
    /// <param name="y">the Y-coordinate</param>
    /// <returns>true if a Tile exists at some (X, Y) position; otherwise,
    /// false.</returns>
    public static bool TileExistsAtPosition(int x, int y)
    {
        if (!instance.ValidCoordinates(x, y)) return false;
        return instance.TileExistsAt(x, y) != null;
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

        if (direction == Direction.NORTH)
        {
            neighborX = originX;
            neighborY = originY + 1;
            Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
            if (neighbor != null) return neighbor;
        }

        if (direction == Direction.EAST)
        {
            neighborX = originX + 1;
            neighborY = originY;
            Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
            if (neighbor != null) return neighbor;
        }

        if (direction == Direction.SOUTH)
        {
            neighborX = originX;
            neighborY = originY - 1;
            Tile neighbor = instance.TileExistsAt(neighborX, neighborY);
            if (neighbor != null) return neighbor;
        }

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
    /// <param name="neighbors">The Tile's neighbors' PlaceableObject.</param>
    /// <returns>true if the PlaceableObject was placed; otherwise, false.</returns>
    public static bool PlaceOnTile(Tile target, PlaceableObject candidate, ISurface[] neighbors)
    {
        //Safety checks
        if (target == null || candidate == null || neighbors == null) return false;
        Assert.IsNotNull(target as ISurface);

        //TODO: IMPLEMENT
        bool result = target.Place(candidate, instance.GetNeighbors(target));
        return result;
    }

    /// <summary>
    /// Returns true if the clearing of a PlaceableObject off a Tile will be
    /// successful. If so, clears it. Otherwise, does nothing and returns false.
    /// </summary>
    /// <param name="target">The Tile to place on.</param>
    /// <param name="neighbors">The Tile's neighbors' PlaceableObjects.</param>
    /// <returns>true if the PlaceableObject was placed; otherwise, false.</returns>
    public static bool ClearTile(Tile target, ISurface[] neighbors)
    {
        //Safety checks
        if (target == null || neighbors == null) return false;
        Assert.IsNotNull(target as ISurface);

        //TODO: IMPLEMENT
        bool result = target.Remove(instance.GetNeighbors(target));
        return result;
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

        t.PaintTile(color);
    }

    /// <summary>
    /// Returns true if the flooring of a Floorable on this Tile will be
    /// successful. If so, floors it. Otherwise, does nothing and returns false.
    /// </summary>
    /// <param name="candidate">The Floorable to floor.</param>
    /// <param name="target">The Tile to place on.</param>
    /// <returns>true if the Floorable was floored; otherwise, false.</returns>
    public static bool FloorTile(Tile target, Flooring.FlooringType candidate)
    {
        if (target == null) return false;

        if (target.Floor(instance.FlooringTypeToPrefab(candidate),
            instance.GetNeighbors(target)))
        {
            //The floor was successful, so update neighbors of the target.
            foreach (ISurface surface in target.GetSurfaceNeighbors())
            {
                Tile neighbor = surface as Tile;
                if (neighbor == null) continue;
                neighbor.UpdateSurfaceNeighbors(instance.GetNeighbors(neighbor));
            }
            return true;
        }
        return false;
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
    /// <param name="enemy">the Enemy to check.</param>
    /// <returns>true if some world coordinates lie within the range of a Tile
    /// that is walkable by some enemy; otherwise, false.</returns>
    public static bool OnWalkableTile(Vector2 worldCoords, Enemy enemy)
    {
        Assert.IsNotNull(enemy);
        if (!OnTile(worldCoords)) return false;

        int coordX = PositionToCoordinate(worldCoords.x);
        int coordY = PositionToCoordinate(worldCoords.y);
        Tile t = instance.TileExistsAt(coordX, coordY);
        Assert.IsNotNull(t);

        //TODO: Eventually, incorporate Enemy checking.
        return t.WALKABLE;
    }

    /// <summary>
    /// Returns all ITargetables on this TileGrid.
    /// </summary>
    /// <returns>A list of all ITargetables on this TileGrid.</returns>
    public static List<ITargetable> GetAllTargetableObjects()
    {
        List<ITargetable> targetables = new List<ITargetable>();

        foreach (KeyValuePair<Vector2Int, Tile> pair in instance.tileMap)
        {
            Tile t = pair.Value;
            PlaceableObject placeableObject = t.GetPlaceableObject();
            ITargetable targetable = placeableObject as ITargetable;
            if (targetable != null && !targetable.Dead()) targetables.Add(targetable);
        }
        return targetables;
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
            instance.tileMap[pair.Key].PaintTile(Color.white);
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

        if (instance.IsDebugging()) goalTile.PaintTile(PATHFINDING_RED);
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
                List<Tile> path = instance.ReconstructPath(cameFrom, current);
                if (path.Count > 1)
                {
                    Tile nextTile = path[1];
                    startTile.PaintTile(Color.white);
                    return new Vector3(nextTile.GetX(), nextTile.GetY(), 1);
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

        Debug.Log("No path found.");

        //No path found, so we return the starting position.
        return startPos;
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
            if (IsDebugging()) current.PaintTile(PATHFINDING_BLUE);
            current = cameFrom[current];
            Assert.IsNotNull(current);
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    //----------------------END PATHFINDING------------------------//
}
