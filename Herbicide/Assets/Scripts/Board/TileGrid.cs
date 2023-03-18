using System.Collections;
using System.Collections.Generic;
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
    /// Tile radius of the starting Soil patch, not including center Tile
    /// </summary>
    [SerializeField]
    private int soilRadius;

    /// <summary>
    /// Width and height of a Tile in the TileGrid
    /// </summary>
    private const float TILE_SIZE = 1f;

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
    public static readonly Color32 PATHFINDING_RED = new Color32(255, 0, 0, 255);

    /// <summary>
    /// Blue color for debugging and pathfinding.
    /// </summary>
    public static readonly Color32 PATHFINDING_BLUE = new Color32(0, 0, 255, 255);


    /// <summary>
    /// The placed IPlaceables in the grid, indexed by coordinate
    /// </summary>
    private IPlaceable[,] placedObjects;

    /// <summary>
    /// THE TileGrid instance
    /// </summary>
    private static TileGrid instance;

    /// <summary>
    /// Tiles in the TileGrid, indexed by their coordinates
    /// </summary>
    private Tile[,] tiles;

    /// <summary>
    /// Unique types of Tiles in the TileGrid.
    /// </summary>
    public enum TileType
    {
        GRASS
    }

    /// <summary>
    /// Unique types of Floorings in the TileGrid.
    /// </summary>
    public enum FlooringType
    {
        SOIL
    }

    /// <summary>
    /// Sprite assets for GrassTiles.<br></br>
    /// 
    /// index 0 --> dark Grass Tile<br></br>
    /// index 1 --> lite Grass Tile
    /// </summary>
    [SerializeField]
    public Sprite[] grassSprites;

    /// <summary>
    /// Detects any player input on a Tile and handles it based on the type
    /// of input.
    /// </summary>
    /// <param name="levelController">the LevelController singleton</param>
    public static void CheckTileInputEvents(LevelController levelController)
    {
        if (levelController == null) return;

        instance.CheckTileMouseDown();
        instance.CheckTileMouseUp();
        instance.CheckTileMouseEnter();
        instance.CheckTileMouseExit();
    }

    /// <summary>
    /// Updates the PlacementController to act upon any active placement events.
    /// </summary>
    /// <param name="levelController">the LevelController singleton</param>
    public static void CheckGridPlacementEvents(LevelController levelController)
    {
        if (levelController == null) return;

        instance.CheckPlacementEvent();
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

        //Initialize tiles and placed objects arrays.
        instance.tiles = new Tile[instance.grassDiameter, instance.grassDiameter];
        instance.placedObjects = new IPlaceable[instance.grassDiameter, instance.grassDiameter];
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

        //Make the tiles and flooring
        instance.MakeGrass();
        instance.MakeSoil();


        //Give tiles their neighbors
        for (int x = 0; x < instance.grassDiameter; x++)
        {
            for (int y = 0; y < instance.grassDiameter; y++)
            {
                if (instance.tiles[x, y] != null)
                {
                    Tile t = instance.tiles[x, y];
                    t.UpdateNeighbors(instance.GetNeighbors(t));
                }
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
        Vector2Int centerCoords = GetCenterCoordinates(grassDiameter);
        float centerXPos = CoordinateToPosition(centerCoords.x);
        float centerYPos = CoordinateToPosition(centerCoords.y);
        return new Vector2(centerXPos, centerYPos);
    }

    /// <summary>
    /// Instantiates the Grass Tiles that should be in the TileGrid.
    /// </summary>
    private void MakeGrass()
    {
        //Safety Checks
        if (IsGenerated()) return;
        Assert.IsTrue(grassDiameter % 2 != 0, "Diameter of TileGrid must be odd.");
        if (tiles == null) tiles = new Tile[grassDiameter, grassDiameter];

        for (int x = 0; x < grassDiameter; x++)
        {
            for (int y = 0; y < grassDiameter; y++)
            {
                float xWorldPos = CoordinateToPosition(x);
                float yWorldPos = CoordinateToPosition(y);
                if (InCircle(grassDiameter, xWorldPos, yWorldPos))
                {
                    Tile t = MakeTile(xWorldPos, yWorldPos, TileType.GRASS);
                    int spriteIndex = DifferentParities(x, y) ? 1 : 0;
                    t.Define(x, y, GetTileSprite(TileType.GRASS, spriteIndex));
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
        Vector2Int centerCoords = GetCenterCoordinates(grassDiameter);
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
                if (InCircle((soilRadius * 2) + 1, xWorldPos, yWorldPos))
                {
                    Tile t = TileExistsAt(new Vector2Int(x, y));
                    if (t == null) continue;
                    FloorTile(t, FlooringType.SOIL);
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
    private Tile MakeTile(float xPos, float yPos, TileType type)
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
    /// Adds a Tile to this TileGrid if possible.
    /// </summary>
    /// <param name="t">the Tile to add</param>
    private void AddTile(Tile t)
    {
        //Safety checks
        if (t == null || tiles == null) return;
        if (!ValidCoordinates(t.GetX(), t.GetY())) return;
        Assert.IsNull(tiles[t.GetX(), t.GetY()]);
        tiles[t.GetX(), t.GetY()] = t;
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
        if (xPos < 0 || yPos < 0) return false;

        Vector2Int centerCoords = GetCenterCoordinates(grassDiameter);
        float centerX = CoordinateToPosition(centerCoords.x);
        float centerY = CoordinateToPosition(centerCoords.y);

        float distanceX = centerX - xPos;
        float distanceY = centerY - yPos;
        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        return distance <= (diameter / 2) * TILE_SIZE;
    }

    /// <summary>
    /// Returns a Vector2Int holding the coordinates of a center
    /// tile. This is determined based off some diameter and does not
    /// care if Tiles are spawned yet.
    /// </summary>
    /// <returns>A Vector2Int holding the coordinates of the grid's center</returns>
    private Vector2Int GetCenterCoordinates(int diameter)
    {
        return new Vector2Int((int)((diameter - 1) / 2),
            (int)((diameter - 1) / 2));
    }

    /// <summary>
    /// Returns true if a Tile exists at some coordinates; otherwise, returns
    /// false.
    /// </summary>
    /// <param name="coordinates">The Tile coordinates.</param>
    /// <returns>the Tile at some coordinates; null if no Tile exists there.
    /// </returns>
    private Tile TileExistsAt(Vector2Int coordinates)
    {
        //Safety checks
        if (tiles == null) return null;
        if (!ValidCoordinates(coordinates.x, coordinates.y)) return null;

        return tiles[coordinates.x, coordinates.y];
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
                InventoryController.StopPlacingFromSlot();
            }
            //else use on tile.
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
    }

    /// <summary>
    /// Checks for placement events. Updates the PlacementController to
    /// act accordingly if there is an avtive event.
    /// </summary>
    private void CheckPlacementEvent()
    {
        PlacementController.CheckPlacementEvents(instance);
    }

    /// <summary>
    /// Checks if the player's mouse left some Tile. If so, triggers
    /// the mouse exit event for that Tile.
    /// </summary>
    private void CheckTileMouseExit()
    {
        Tile tile = InputController.TileDehovered();
        if (tile == null) return;

        //Tile was dehovered over. Put dehover logic here.
    }

    /// <summary>
    /// Returns the GameObject prefab representing a Tile type.
    /// </summary>
    /// <param name="type">the type of the Tile</param>
    /// <returns>the prefab representing the type</returns>
    private GameObject TileTypeToPrefab(TileType type)
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
        //Handle coordinate logic
        if (x < 0 || y < 0) return false;

        //Handle world position logic
        if (!InCircle(grassDiameter, CoordinateToPosition(x),
            CoordinateToPosition(y))) return false;

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

        Vector2Int neighboringCoords;
        int originX = origin.GetX();
        int originY = origin.GetY();

        if (direction == Direction.NORTH)
        {
            neighboringCoords = new Vector2Int(originX, originY + 1);
            Tile neighbor = instance.TileExistsAt(neighboringCoords);
            if (neighbor != null) return neighbor;
        }

        if (direction == Direction.EAST)
        {
            neighboringCoords = new Vector2Int(originX + 1, originY);
            Tile neighbor = instance.TileExistsAt(neighboringCoords);
            if (neighbor != null) return neighbor;
        }

        if (direction == Direction.SOUTH)
        {
            neighboringCoords = new Vector2Int(originX, originY - 1);
            Tile neighbor = instance.TileExistsAt(neighboringCoords);
            if (neighbor != null) return neighbor;
        }

        if (direction == Direction.WEST)
        {
            neighboringCoords = new Vector2Int(originX - 1, originY);
            Tile neighbor = instance.TileExistsAt(neighboringCoords);
            if (neighbor != null) return neighbor;
        }

        return null;
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
        if (result) instance.TrackPlaceable(candidate, target.GetX(), target.GetY());
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
        if (result) instance.UnTrackPlaceable(target.GetX(), target.GetY());
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
        Tile t = instance.TileExistsAt(new Vector2Int(x, y));
        if (t == null) return;

        t.PaintTile(color);
    }

    /// <summary>
    /// Adds an IPlaceable to the TileGrid's array of IPlaceables. 
    /// </summary>
    /// <param name="x">the X-Coordinate of the surface holding the item</param>
    /// <param name="y">the Y-Coordinate of the surface holding the item</param>

    private void TrackPlaceable(IPlaceable item, int x, int y)
    {

        if (item == null) return;
        if (!TileExistsAt(new Vector2Int(x, y))) return;
        if (placedObjects == null) return;
        Assert.IsNull(placedObjects[x, y]);

        placedObjects[x, y] = item;
    }

    /// <summary>
    /// Remocves an IPlaceable to the TileGrid's array of IPlaceables. 
    /// </summary>
    /// <param name="item">the item to remove</param>
    /// <param name="x">the X-Coordinate of the surface holding the item</param>
    /// <param name="y">the Y-Coordinate of the surface holding the item</param>

    private void UnTrackPlaceable(int x, int y)
    {
        if (!TileExistsAt(new Vector2Int(x, y))) return;
        if (placedObjects == null) return;
        Assert.IsNotNull(placedObjects[x, y]);

        placedObjects[x, y] = null;
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
            foreach (ISurface surface in target.GetNeighbors())
            {
                Tile neighbor = surface as Tile;
                if (neighbor == null) continue;
                neighbor.UpdateNeighbors(instance.GetNeighbors(neighbor));
            }
            return true;
        }
        return false;
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
    /// Returns the correct Sprite asset for a Tile given its type and an index.
    /// </summary>
    /// <param name="type">the type of Tile</param>
    /// <param name="index">the Sprite index of the Tile </param>
    /// <returns>the correct Sprite asset for a Tile given its type and an index.
    /// </returns>
    private Sprite GetTileSprite(TileType type, int index)
    {
        if (index < 0) return null;

        if (type == TileType.GRASS)
        {
            if (index >= grassSprites.Length) return null;
            return grassSprites[index];
        }

        return null;
    }
}
