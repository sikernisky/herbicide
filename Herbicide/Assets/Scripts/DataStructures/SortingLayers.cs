/// <summary>
/// Sorting layers for a Model's SpriteRenderer.
/// </summary>
public enum SortingLayers
{
    DEFAULT, // Below everything, not setup.
    BASE_TILE, // Non-interesting tiles, like grass.
    FLOORING, // Soil/carpeting that goes on tiles.
    HAZARDS, // Mobs that are below other mobs but still above the ground. 
    GROUNDMOBS, // Mobs that sit on the ground, like Kudzus and Trees.
    PICKEDUPITEMS, // Mobs that other mobs have picked up.
    AIRMOBS, // Mobs that fly in the air, like Butterflies.
    PROJECTILES, // Bullets and other stuff like that.
    COLLECTABLES, // Things the player needs to collect.
    EMANATIONS // Spontaneous, temporary animations.
}

