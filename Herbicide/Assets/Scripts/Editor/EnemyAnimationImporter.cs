using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

/// <summary>
/// EnemyAnimationImporter is a utility class that automatically
/// assigns sliced sprites to an EnemyAnimationSet. 
/// </summary>
public class EnemyAnimationImporter : EditorWindow
{
    /// <summary>
    /// All different types of SpriteSheets. 
    /// </summary>
    private enum SpriteSheetType { Movement, Attack }

    /// <summary>
    /// The type of SpriteSheet we are slicing.
    /// </summary>
    private SpriteSheetType selectedSpriteSheetType;

    /// <summary>
    /// The sprite sheet we are slicing.
    /// </summary>
    private Texture2D spriteSheet;

    /// <summary>
    /// The destination scriptable object for the sliced sprites.
    /// </summary>
    private EnemyAnimationSet destinationAnimationSet;


    /// <summary>
    /// Shows the Enemy Sprite Sheet Slicer window.
    /// </summary>
    [MenuItem("Window/Enemy Sprite Sheet Slicer")]
    public static void ShowWindow()
    {
        GetWindow<EnemyAnimationImporter>("Enemy Sprite Sheet Slicer");
    }

    /// <summary>
    /// Called when the window is opened.
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Custom Animation Importer", EditorStyles.boldLabel);

        selectedSpriteSheetType = (SpriteSheetType)EditorGUILayout.EnumPopup("Sprite Sheet Type", selectedSpriteSheetType);
        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", spriteSheet, typeof(Texture2D), false);
        destinationAnimationSet = (EnemyAnimationSet)EditorGUILayout.ObjectField("Enemy Animation Set", destinationAnimationSet, typeof(EnemyAnimationSet), false);

        if (GUILayout.Button("Slice and Load"))
        {
            if (selectedSpriteSheetType == SpriteSheetType.Movement)
            {
                SliceAndLoadMovementSpriteSheet();
            }
            else if (selectedSpriteSheetType == SpriteSheetType.Attack)
            {
                SliceAndLoadAttackSpriteSheet();
            }
        }
    }

    /// <summary>
    /// Slices the movement animation sprite sheet and loads it into the
    /// destination scriptable object.
    /// </summary>
    private void SliceAndLoadMovementSpriteSheet()
    {
        // if (spriteSheet == null || destinationAnimationSet == null)
        // {
        //     Debug.LogError("Please assign a sprite sheet and a scriptable object.");
        //     return;
        // }

        // string path = AssetDatabase.GetAssetPath(spriteSheet);
        // TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        // if (textureImporter == null)
        // {
        //     Debug.LogError("Could not get TextureImporter for the sprite sheet.");
        //     return;
        // }

        // textureImporter.isReadable = true;
        // textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        // textureImporter.spritePixelsPerUnit = 16;
        // textureImporter.filterMode = FilterMode.Point;

        // int columns = 12; // Total number of animations
        // int rows = 6; // Frames per animation
        // int frameWidth = 16;
        // int frameHeight = 32;

        // SpriteMetaData[] newData = new SpriteMetaData[columns * rows];
        // for (int i = 0; i < columns; i++)
        // {
        //     for (int j = 0; j < rows; j++)
        //     {
        //         SpriteMetaData metaData = new SpriteMetaData
        //         {
        //             name = $"hihi{i}_frame_{j}",
        //             rect = new Rect(i * frameWidth, spriteSheet.height - ((j + 1) * frameHeight), frameWidth, frameHeight),
        //             alignment = (int)SpriteAlignment.Custom,
        //             pivot = new Vector2(0.5f, 0.25f)
        //         };
        //         newData[i * rows + j] = metaData;
        //     }
        // }

        // textureImporter.spritesheet = newData;
        // AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        // Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.name).ToArray();

        // destinationAnimationSet.movementAnimationNorthHealthy = sprites.Take(6).ToArray();
        // destinationAnimationSet.movementAnimationEastHealthy = sprites.Skip(6).Take(6).ToArray();
        // destinationAnimationSet.movementAnimationSouthHealthy = sprites.Skip(12).Take(6).ToArray();
        // destinationAnimationSet.movementAnimationWestHealthy = sprites.Skip(18).Take(6).ToArray();

        // destinationAnimationSet.movementAnimationNorthDamaged = sprites.Skip(24).Take(6).ToArray();
        // destinationAnimationSet.movementAnimationEastDamaged = sprites.Skip(30).Take(6).ToArray();
        // destinationAnimationSet.movementAnimationSouthDamaged = sprites.Skip(36).Take(6).ToArray();
        // destinationAnimationSet.movementAnimationWestDamaged = sprites.Skip(42).Take(6).ToArray();

        // destinationAnimationSet.movementAnimationNorthCritical = sprites.Skip(48).Take(6).ToArray();
        // destinationAnimationSet.movementAnimationEastCritical = sprites.Skip(54).Take(6).ToArray();
        // destinationAnimationSet.movementAnimationSouthCritical = sprites.Skip(60).Take(6).ToArray();
        // destinationAnimationSet.movementAnimationWestCritical = sprites.Skip(66).Take(6).ToArray();

        // foreach (Sprite s in sprites)
        // {
        //     Debug.Log(s.name);
        // }

        // EditorUtility.SetDirty(destinationAnimationSet);
        // AssetDatabase.SaveAssets();

        // Debug.Log("Sprite sheet sliced and loaded into the scriptable object.");

        if (spriteSheet == null || destinationAnimationSet == null)
        {
            Debug.LogError("Please assign a sprite sheet and a scriptable object.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(spriteSheet);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter == null)
        {
            Debug.LogError("Could not get TextureImporter for the sprite sheet.");
            return;
        }

        textureImporter.isReadable = true;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spritePixelsPerUnit = 16;
        textureImporter.filterMode = FilterMode.Point;

        int columns = 12; // Total number of animations
        int rows = 6; // Frames per animation
        int frameWidth = 16;
        int frameHeight = 32;

        string[] directions = { "south", "north", "east", "west" };
        string[] states = { "healthy", "damaged", "critical" };

        SpriteMetaData[] newData = new SpriteMetaData[columns * rows];
        int index = 0;

        for (int i = 0; i < columns; i++)
        {
            int directionIndex = i / 3;
            int stateIndex = i % 3;
            string direction = directions[directionIndex];
            string state = states[stateIndex];

            for (int j = 0; j < rows; j++)
            {
                SpriteMetaData metaData = new SpriteMetaData
                {
                    name = $"{direction}_{state}_frame_{j}",
                    rect = new Rect(i * frameWidth, spriteSheet.height - ((j + 1) * frameHeight), frameWidth, frameHeight),
                    alignment = (int)SpriteAlignment.Custom,
                    pivot = new Vector2(0.5f, 0.25f)
                };
                newData[index] = metaData;
                index++;
            }
        }

        textureImporter.spritesheet = newData;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.name).ToArray();

        destinationAnimationSet.movementAnimationSouthHealthy = sprites.Where(s => s.name.Contains("south_healthy")).ToArray();
        destinationAnimationSet.movementAnimationSouthDamaged = sprites.Where(s => s.name.Contains("south_damaged")).ToArray();
        destinationAnimationSet.movementAnimationSouthCritical = sprites.Where(s => s.name.Contains("south_critical")).ToArray();

        destinationAnimationSet.movementAnimationNorthHealthy = sprites.Where(s => s.name.Contains("north_healthy")).ToArray();
        destinationAnimationSet.movementAnimationNorthDamaged = sprites.Where(s => s.name.Contains("north_damaged")).ToArray();
        destinationAnimationSet.movementAnimationNorthCritical = sprites.Where(s => s.name.Contains("north_critical")).ToArray();

        destinationAnimationSet.movementAnimationEastHealthy = sprites.Where(s => s.name.Contains("east_healthy")).ToArray();
        destinationAnimationSet.movementAnimationEastDamaged = sprites.Where(s => s.name.Contains("east_damaged")).ToArray();
        destinationAnimationSet.movementAnimationEastCritical = sprites.Where(s => s.name.Contains("east_critical")).ToArray();

        destinationAnimationSet.movementAnimationWestHealthy = sprites.Where(s => s.name.Contains("west_healthy")).ToArray();
        destinationAnimationSet.movementAnimationWestDamaged = sprites.Where(s => s.name.Contains("west_damaged")).ToArray();
        destinationAnimationSet.movementAnimationWestCritical = sprites.Where(s => s.name.Contains("west_critical")).ToArray();

        foreach (Sprite s in sprites)
        {
            Debug.Log(s.name);
        }

        EditorUtility.SetDirty(destinationAnimationSet);
        AssetDatabase.SaveAssets();

        Debug.Log("Sprite sheet sliced and loaded into the scriptable object.");

    }

    /// <summary>
    /// Slices the attack animation sprite sheet and loads it into the
    /// destination scriptable object.
    /// </summary>
    private void SliceAndLoadAttackSpriteSheet()
    {
        if (spriteSheet == null || destinationAnimationSet == null)
        {
            Debug.LogError("Please assign a sprite sheet and a scriptable object.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(spriteSheet);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter == null)
        {
            Debug.LogError("Could not get TextureImporter for the sprite sheet.");
            return;
        }

        textureImporter.isReadable = true;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spritePixelsPerUnit = 16;
        textureImporter.filterMode = FilterMode.Point;

        // North and South (16x32)
        int northSouthFrameWidth = 16;
        int northSouthFrameHeight = 32;

        // East and West (32x32)
        int eastWestFrameWidth = 32;
        int eastWestFrameHeight = 32;

        string[] directions = { "south", "north", "east", "west" };
        string[] states = { "healthy", "damaged", "critical" };

        SpriteMetaData[] newData = new SpriteMetaData[12];
        int index = 0;

        // South and North animations (swapped order)
        for (int i = 0; i < 2; i++) // First two columns
        {
            for (int j = 0; j < 3; j++) // Three frames for each health state
            {
                string direction = directions[i];
                string state = states[j];
                SpriteMetaData metaData = new SpriteMetaData
                {
                    name = $"{direction}_{state}_frame_{0}",
                    rect = new Rect(i * northSouthFrameWidth, spriteSheet.height - ((j + 1) * northSouthFrameHeight), northSouthFrameWidth, northSouthFrameHeight),
                    alignment = (int)SpriteAlignment.Custom,
                    pivot = new Vector2(0.5f, 0.25f)
                };
                newData[index++] = metaData;
            }
        }

        // East and West animations
        for (int i = 0; i < 2; i++) // Last two columns
        {
            for (int j = 0; j < 3; j++)
            {
                string direction = directions[i + 2];
                string state = states[j];
                Vector2 pivot = (i == 0) ? new Vector2(0.25f, 0.25f) : new Vector2(0.75f, 0.25f);

                SpriteMetaData metaData = new SpriteMetaData
                {
                    name = $"{direction}_{state}_frame_{0}",
                    rect = new Rect(2 * northSouthFrameWidth + i * eastWestFrameWidth, spriteSheet.height - ((j + 1) * eastWestFrameHeight), eastWestFrameWidth, eastWestFrameHeight),
                    alignment = (int)SpriteAlignment.Custom,
                    pivot = pivot
                };
                newData[index++] = metaData;
            }
        }

        textureImporter.spritesheet = newData;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.name).ToArray();

        destinationAnimationSet.attackAnimationSouthHealthy = sprites.Where(s => s.name.Contains("south_healthy")).ToArray();
        destinationAnimationSet.attackAnimationSouthDamaged = sprites.Where(s => s.name.Contains("south_damaged")).ToArray();
        destinationAnimationSet.attackAnimationSouthCritical = sprites.Where(s => s.name.Contains("south_critical")).ToArray();

        destinationAnimationSet.attackAnimationNorthHealthy = sprites.Where(s => s.name.Contains("north_healthy")).ToArray();
        destinationAnimationSet.attackAnimationNorthDamaged = sprites.Where(s => s.name.Contains("north_damaged")).ToArray();
        destinationAnimationSet.attackAnimationNorthCritical = sprites.Where(s => s.name.Contains("north_critical")).ToArray();

        destinationAnimationSet.attackAnimationEastHealthy = sprites.Where(s => s.name.Contains("east_healthy")).ToArray();
        destinationAnimationSet.attackAnimationEastDamaged = sprites.Where(s => s.name.Contains("east_damaged")).ToArray();
        destinationAnimationSet.attackAnimationEastCritical = sprites.Where(s => s.name.Contains("east_critical")).ToArray();

        destinationAnimationSet.attackAnimationWestHealthy = sprites.Where(s => s.name.Contains("west_healthy")).ToArray();
        destinationAnimationSet.attackAnimationWestDamaged = sprites.Where(s => s.name.Contains("west_damaged")).ToArray();
        destinationAnimationSet.attackAnimationWestCritical = sprites.Where(s => s.name.Contains("west_critical")).ToArray();

        foreach (Sprite s in sprites)
        {
            Debug.Log(s.name);
        }

        EditorUtility.SetDirty(destinationAnimationSet);
        AssetDatabase.SaveAssets();

        Debug.Log("Attack sprite sheet sliced and loaded into the scriptable object.");
    }
}
