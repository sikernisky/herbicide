using UnityEditor;
using UnityEngine;

/// <summary>
/// A utility class that automatically assigns sliced sprites to an AnimationSet. 
/// </summary>
public class AnimationImporter : EditorWindow
{
    #region Fields

    /// <summary>
    /// The sprite sheet to slice and load.
    /// </summary>
    private Texture2D spriteSheet;

    /// <summary>
    /// The AnimationSet to assign the sliced sprites to.
    /// </summary>
    private EnemyAnimationSet enemyAnimationSet;

    #endregion

    #region Methods

    /// <summary>
    /// Shows the Animation Importer window.
    /// </summary>
    [MenuItem("Window/Animation Importer")]
    public static void ShowWindow() => GetWindow<AnimationImporter>("Animation Importer");

    /// <summary>
    /// Called when the window is opened.
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Animation Importer", EditorStyles.boldLabel);

        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", spriteSheet, typeof(Texture2D), false);
        enemyAnimationSet = (EnemyAnimationSet)EditorGUILayout.ObjectField("Unit Animations", enemyAnimationSet, typeof(EnemyAnimationSet), false);

        if (GUILayout.Button("Slice and Load"))
        {
            //SliceAndLoadSpriteSheet();
        }
    }

    #endregion
}
