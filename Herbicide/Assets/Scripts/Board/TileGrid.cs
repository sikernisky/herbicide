using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


/// <summary>
/// Represents the board of Tile objects. The TileGrid is circular and is
/// constructed off an even diameter. For example, if the diameter is six,
/// the board looks like:<br></br>
/// 
///     [][]
///   [][][][]
/// [][][][][][]
///   [][][][]
///     [][]
/// <br></br>
/// Main responsibilies of the TileGrid singleton include:<br></br>
/// 
/// (1) Instantiating and defining Tiles<br></br>
/// (2) Getting information about Tiles<br></br>
/// (3) Passing click events to Tiles<br></br>
/// (4) Flooring and placing on Tiles<br></br>
/// (5) Spawning the initial soil patch<br></br>
/// </summary>
public class TileGrid : MonoBehaviour
{
    /// <summary>
    /// Odd diameter of the TileGrid
    /// </summary>
    [SerializeField]
    private int grassDiameter;

    /// <summary>
    /// Amount of water, in tiles, to cushion around the grid and bridge from
    /// all four sides.
    /// </summary>
    [SerializeField]
    private int waterCushion;

    /// <summary>
    /// Length of the bridge extending from the circular TileGrid, in tiles. 
    /// </summary>
    [SerializeField]
    private int bridgeLength;

    /// <summary>
    /// Positive width of the bridge extending from the circular TileGrid, in tiles.
    /// </summary>
    [SerializeField]
    private int bridgeWidth;

    /// <summary>
    /// Tile radius of the starting Soil patch, not including center Tile
    /// </summary>
    [SerializeField]
    private int soilRadius;

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
    public static readonly Color32 PATHFINDING_RED = new Color32(255, 0, 0, 255);

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
    /// Unique types of Floorings in the TileGrid.
    /// </summary>
    public enum FlooringType
    {
        SOIL
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
        instance.enemyLocations = new Dictionary<Enemy, Tile>();
        instance.edgeTiles = new HashSet<Tile>();
        instance.SetCenterCoordinates();
    }

    /// <summary>
    /// Returns the correct position of the Camera after generating
    /// the TileGrid and the Tiles within it.
    /// </summary>
    /// <param name="levelController">the LevelController spawning the grid.</param>
    /// <returns>the position for the Camera, that if set to, represents the center
    /// of the TileGrid.</returns>
    public static Vector2 SpawnGrid(LevelController levelController)
    {
        //Safety checks
        if (levelController == null) return Vector2.zero;
        if (instance.IsGenerated()) return Vector2.zero;

        //Clear all collections
        instance.tileMap.Clear();
        instance.edgeTiles.Clear();

        //Make the tiles and flooring
        instance.MakeGrass();
        instance.MakeSoil();
        instance.MakeShore();
        instance.MakeWater();


        //Give tiles their neighbors
        for (int x = 0; x < instance.grassDiameter; x++)
        {
            for (int y = 0; y < instance.grassDiameter; y++)
            {
                Tile t = instance.TileExistsAt(x, y);
                if (t != null) t.UpdateSurfaceNeighbors(instance.GetNeighbors(t));
            }
        }

        //Finishing touches
        instance.SetGenerated();
        return instance.GetCameraPositionOnGrid();
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
        float centerXPos = CoordinateToPosition(centerCoords.x);
        float centerYPos = CoordinateToPosition(centerCoords.y);
        return new Vector2(centerXPos, centerYPos);
    }

    /// <summary>
    /// Instantiates the Grass Tiles that should be in the TileGrid. This includes
    /// the bridge that extends from the circular center.
    /// </summary>
    private void MakeGrass()
    {
        if (IsGenerated()) return;
        Assert.IsTrue(grassDiameter % 2 != 0, "Diameter of TileGrid must be odd.");
        Assert.IsTrue(grassDiameter >= 7, "Grass circle must have diameter >= 7.");

        int centerCoord = (grassDiameter - 1) / 2;

        for (int x = 0; x < grassDiameter; x++)
        {
            for (int y = 0; y < grassDiameter; y++)
            {
                float xWorldPos = CoordinateToPosition(x);
                float yWorldPos = CoordinateToPosition(y);
                if (TileExistsAt(x, y) == null)
                {
                    if (InCircle(grassDiameter, x, y))
                    {
                        Tile t = MakeTile(xWorldPos, yWorldPos, Tile.TileType.GRASS);
                        GrassTile gt = t as GrassTile;
                        Assert.IsNotNull(gt);
                        bool isLight = DifferentParities(x, y) ? true : false;
                        gt.Define(x, y, isLight);
                        gt.UnmarkEdge();
                        AddTile(t);
                    }
                }
            }
        }

        //LEFT SIDE
        for (int x = -bridgeLength; x < 0; x++)
        {
            for (int y = centerCoord - bridgeWidth; y <= centerCoord + bridgeWidth; y++)
            {
                float xWorldPos = CoordinateToPosition(x);
                float yWorldPos = CoordinateToPosition(y);
                if (TileExistsAt(x, y) == null)
                {
                    Tile t = MakeTile(xWorldPos, yWorldPos, Tile.TileType.GRASS);
                    GrassTile gt = t as GrassTile;
                    Assert.IsNotNull(gt);
                    bool isLight = DifferentParities(x, y) ? true : false;
                    gt.Define(x, y, isLight);
                    AddTile(t);
                }
            }
        }

        //RIGHT SIDE
        for (int x = grassDiameter; x < grassDiameter + bridgeLength; x++)
        {
            for (int y = centerCoord - bridgeWidth; y <= centerCoord + bridgeWidth; y++)
            {
                float xWorldPos = CoordinateToPosition(x);
                float yWorldPos = CoordinateToPosition(y);
                if (TileExistsAt(x, y) == null)
                {
                    Tile t = MakeTile(xWorldPos, yWorldPos, Tile.TileType.GRASS);
                    GrassTile gt = t as GrassTile;
                    Assert.IsNotNull(gt);
                    bool isLight = DifferentParities(x, y) ? true : false;
                    gt.Define(x, y, isLight);
                    AddTile(t);
                }
            }
        }



    }

    /// <summary>
    /// Instantiates the starting Soil Flooring that should be in the TileGrid.
    /// </summary>
    private void MakeSoil()
    {
        //Safety Checks
        if (IsGenerated()) return;
        Assert.IsTrue(grassDiameter % 2 != 0, "Diameter of TileGrid must be odd.");

        //Spawn around the center of the grass patch
        Vector2Int centerCoords = GetCenterCoordinates();
        int startX = centerCoords.x - soilRadius;
        int endX = centerCoords.x + soilRadius;
        int startY = centerCoords.y - soilRadius;
        int endY = centerCoords.y + soilRadius;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                float xWorldPos = CoordinateToPosition(x);
                float yWorldPos = CoordinateToPosition(y);
                Tile t = TileExistsAt(x, y);
                if (t == null) continue;
                if (FloorTile(t, FlooringType.SOIL) && autoGenerateTrees)
                {
                    //Place trees on Floored tiles.
                    Tree tree = TreeFactory.GetTreePrefab(
                        Tree.TreeType.BASIC).GetComponent<Tree>();
                    if (tree == null) continue;
                    PlaceOnTile(t, tree, GetNeighbors(t));
                }
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
    /// Generates ShoreTiles around the top and bottom edges of the
    /// TileGrid.
    /// </summary>
    private void MakeShore()
    {
        HashSet<Tile> tilesToAdd = new HashSet<Tile>();
        foreach (KeyValuePair<Vector2Int, Tile> pair in tileMap)
        {
            Tile t = pair.Value;
            if (t.GetTileType() == Tile.TileType.GRASS)
            {
                int x = t.GetX();
                float xWorldPos = CoordinateToPosition(x);
                Tile above = TileExistsAt(t.GetX(), t.GetY() + 1);
                if (above == null)
                {
                    //Make ShoreTile
                    int y = t.GetY() + 1;
                    float yWorldPos = CoordinateToPosition(y);
                    Tile topShore = MakeTile(xWorldPos, yWorldPos, Tile.TileType.SHORE);
                    ShoreTile shoreTile = topShore as ShoreTile;
                    shoreTile.Define(x, y, true);
                    tilesToAdd.Add(shoreTile);

                    //Mark GrassTile below as edge
                    (t as GrassTile).MarkEdge();
                    edgeTiles.Add(t);
                }
                Tile below = TileExistsAt(t.GetX(), t.GetY() - 1);
                if (below == null)
                {
                    //Make ShoreTile
                    int y = t.GetY() - 1;
                    float yWorldPos = CoordinateToPosition(y);
                    Tile topShore = MakeTile(xWorldPos, yWorldPos, Tile.TileType.SHORE);
                    ShoreTile shoreTile = topShore as ShoreTile;
                    shoreTile.Define(x, y, false);
                    tilesToAdd.Add(shoreTile);

                    //Mark GrassTile above as edge
                    (t as GrassTile).MarkEdge();
                    edgeTiles.Add(t);
                }
            }
        }

        foreach (Tile t in tilesToAdd)
        {
            AddTile(t);
        }
    }

    /// <summary>
    /// Generates WaterTiles everywhere possible.
    /// </summary>
    private void MakeWater()
    {
        int waterStartX = -bridgeLength - waterCushion;
        int waterEndX = grassDiameter + bridgeLength + waterCushion;
        int waterStartY = -waterCushion;
        int waterEndY = waterCushion + grassDiameter;

        for (int x = waterStartX; x < waterEndX; x++)
        {
            for (int y = waterStartY; y < waterEndY; y++)
            {
                float xWorldPos = CoordinateToPosition(x);
                float yWorldPos = CoordinateToPosition(y);
                if (TileExistsAt(x, y) == null)
                {
                    Tile t = MakeTile(xWorldPos, yWorldPos, Tile.TileType.WATER);
                    t.Define(x, y);
                    AddTile(t);
                }
            }
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
    /// Returns true if some (X, Y) coordinates are within a
    /// a circluar area.<br></br>
    /// 
    /// The center of this circular area is positioned at the
    /// world position of the grass patch's center Tile.
    /// </summary>
    /// <param name="diameter">The diameter of the circle.</param>
    /// <param name="xPos">The X-Coordinate to check.</param>
    /// <param name="yPos">The Y-Coordinate to check.</param>
    /// <returns>true if the coordinates are within a circular area;
    /// otherwise, false. </returns>
    private bool InGrassCircle(float xPos, float yPos)
    {
        if (xPos < 0 || yPos < 0) return false;

        int centerCoord = (grassDiameter - 1) / 2;
        float centerX = CoordinateToPosition(centerCoord);
        float centerY = CoordinateToPosition(centerCoord);

        float distanceX = centerX - xPos;
        float distanceY = centerY - yPos;
        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        return distance <= (grassDiameter / 2) * TILE_SIZE;
    }


    /// <summary>
    /// Returns true if some (X, Y) coordinates are within a
    /// a circluar area.<br></br>
    /// 
    /// The center of this circular area is positioned at the
    /// world position of the grass patch's center Tile.
    /// </summary>
    /// <param name="diameter">The diameter of the circle.</param>
    /// <param name="xPos">The X-Coordinate to check.</param>
    /// <param name="yPos">The Y-Coordinate to check.</param>
    /// <returns>true if the coordinates are within a circular area;
    /// otherwise, false. </returns>
    private bool InCircle(int diameter, float xPos, float yPos)
    {
        Vector2Int centerTile = GetCenterCoordinates();

        float centerX = centerTile.x * TILE_SIZE;
        float centerY = centerTile.y * TILE_SIZE;
        float radius = diameter * TILE_SIZE / 2;
        float distanceToCenter =
            Mathf.Sqrt(Mathf.Pow(xPos - centerX, 2) + Mathf.Pow(yPos - centerY, 2));

        return distanceToCenter <= radius;
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
    /// Sets the center coordinates to be the center of the grass circle.
    /// </summary>
    private void SetCenterCoordinates()
    {
        int centerCoord = (grassDiameter - 1) / 2;
        center = new Vector2Int(centerCoord, centerCoord);
    }


    /// <summary>
    /// Returns true if a Tile exists at some coordinates; otherwise, returns
    /// false.
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
            IPlaceable itemPlaceable = placingItem as IPlaceable;
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
            IPlaceable placingPlaceable = placingSlottable as IPlaceable;
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
    private GameObject FlooringTypeToPrefab(FlooringType type)
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
    /// Returns true if the placing of an IPlaceable on a Tile will be
    /// successful. If so, places it. Otherwise, does nothing and returns false.
    /// </summary>
    /// <param name="candidate">The IPlaceable to place.</param>
    /// <param name="target">The Tile to place on.</param>
    /// <param name="neighbors">The Tile's neighbors' IPlaceables.</param>
    /// <returns>true if the IPlaceable was placed; otherwise, false.</returns>
    public static bool PlaceOnTile(Tile target, IPlaceable candidate, ISurface[] neighbors)
    {
        //Safety checks
        if (target == null || candidate == null || neighbors == null) return false;
        Assert.IsNotNull(target as ISurface);

        //TODO: IMPLEMENT
        bool result = target.Place(candidate, instance.GetNeighbors(target));
        return result;
    }

    /// <summary>
    /// Returns true if the clearing of an IPlaceable off a Tile will be
    /// successful. If so, clears it. Otherwise, does nothing and returns false.
    /// </summary>
    /// <param name="target">The Tile to place on.</param>
    /// <param name="neighbors">The Tile's neighbors' IPlaceables.</param>
    /// <returns>true if the IPlaceable was placed; otherwise, false.</returns>
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
    public static bool FloorTile(Tile target, FlooringType candidate)
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
        return t != null && t.WALKABLE;
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
    /// Returns true if two integers are of different parities.
    /// </summary>
    /// <param name="x">the first integer</param>
    /// <param name="y">the second integer</param>
    /// <returns>true if two integers are of different parities.</returns>
    private bool DifferentParities(int x, int y)
    {
        return (x % 2 == 0 && y % 2 != 0) ||
            (x % 2 != 0 && y % 2 == 0);
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

    //---------------------BEGIN PATHFINDING-----------------------//

    /// <summary>
    /// Returns the world position of the next Tile in the path towards
    /// a given goal Tile. This method calls an A* pathfinding algorithm
    /// to calculate, retrace, and extract the next Tile from that path.
    /// </summary>
    /// <param name="start">The world position of the starting Tile.</param>
    /// <param name="goal">The world position of the Tile to pathfind towards.</param>
    /// <returns>the world position of the next Tile in a path towards a target Tile.</returns>
    public static Vector3 GetNextPositionInPath(Vector2 start, Vector2 goal)
    {
        int startingX = PositionToCoordinate(start.x);
        int startingY = PositionToCoordinate(start.y);
        int endingX = PositionToCoordinate(goal.x);
        int endingY = PositionToCoordinate(goal.y);

        Tile startingTile = instance.TileExistsAt(startingX, startingY);
        Tile endingTile = instance.TileExistsAt(endingX, endingY);

        // Call AStarPathfind
        List<Tile> path = instance.AStarPathfind(startingTile, endingTile);

        // If there's no path, return the current position
        if (path == null || path.Count == 0)
        {
            return new Vector3(start.x, start.y, 1);
        }


        // Otherwise, return the position of the next tile in the path
        Tile nextTile = path[0];
        return new Vector3(CoordinateToPosition(nextTile.GetX()), CoordinateToPosition(nextTile.GetY()), 0);
    }

    /// <summary>
    /// Returns a list of Tiles representing the shortest path from one Tile to another.
    /// Generates this path via A* pathfinding. The returned list "makes progress", meaning
    /// it starts with the next Tile in the path.
    /// </summary>
    /// <param name="startTile">The starting Tile.</param>
    /// <param name="targetTile">The ending Tile.</param>
    /// <returns>a list of Tiles representing the path from one Tile to another.</returns>
    public List<Tile> AStarPathfind(Tile startTile, Tile goalTile)
    {
        List<Tile> openList = new List<Tile> { startTile };
        HashSet<Tile> closedSet = new HashSet<Tile>();

        while (openList.Count > 0)
        {
            Tile currentTile = openList.OrderBy(t => t.GetTotalPathfindingCost()).First();

            // If the current tile is a walkable neighbor of the goalTile, return the path
            if (GetNeighbors(goalTile).Any(neighborSurface =>
            {
                Tile neighborTile = neighborSurface as Tile;
                return neighborTile != null && neighborTile.IsWalkable() && neighborTile == currentTile;
            }))
            {
                List<Tile> path = new List<Tile>();
                while (currentTile != startTile)
                {
                    path.Add(currentTile);
                    currentTile = currentTile.GetPathfindingParent();
                }
                path.Reverse();
                return path;
            }

            openList.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (ISurface neighborSurface in GetNeighbors(currentTile))
            {
                Tile neighborTile = neighborSurface as Tile;

                if (neighborTile == null || !neighborTile.IsWalkable() || closedSet.Contains(neighborTile))
                {
                    continue;
                }

                int tentativeMovementCost = currentTile.GetMovementCost() + currentTile.GetManhattanDistance(neighborTile);

                if (tentativeMovementCost < neighborTile.GetMovementCost() || !openList.Contains(neighborTile))
                {
                    neighborTile.SetMovementCost(tentativeMovementCost);
                    neighborTile.SetHeuristicCost(neighborTile.GetManhattanDistance(goalTile));
                    neighborTile.SetPathfindingParent(currentTile);

                    if (!openList.Contains(neighborTile))
                    {
                        openList.Add(neighborTile);
                    }
                }
            }
        }

        // If we're here, it means there's no path from startTile to a walkable tile next to the goalTile
        return null;
    }



    //----------------------END PATHFINDING------------------------//
}
