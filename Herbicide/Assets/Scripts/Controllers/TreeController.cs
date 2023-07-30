using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for Trees.
/// </summary>
public class TreeController
{
    /// <summary>
    /// The Tree controlled by this TreeController.
    /// </summary>
    private Tree tree;

    /// <summary>
    /// Number of Trees created in the level so far.
    /// </summary>
    private static int NUM_TREES;

    /// <summary>
    /// Unique ID of this TreeController.
    /// </summary>
    private int id;

    /// <summary>
    /// The current state of the game.
    /// </summary>
    private GameState gameState;


    /// <summary>
    /// Makes a new TreeController for a Tree.
    /// </summary>
    /// <param name="tree">The Tree controlled by this TreeController.</param>
    public TreeController(Tree tree)
    {
        Assert.IsNotNull(tree);

        tree.ResetStats();
        this.tree = tree;
        this.id = NUM_TREES;
        NUM_TREES++;
    }

    /// <summary>
    /// Updates the Tree controlled by this TreeController.
    /// </summary>
    public void UpdateTree()
    {
        if (!ValidTree()) return;

        if (GetTree().GetHealth() <= 0) KillTree();
        UpdateDamageFlash();
    }

    /// <summary>
    /// Plays this TreeController's Tree's damage flash effect
    /// if necessary.
    /// </summary>
    private void UpdateDamageFlash()
    {
        if (GetTree().GetDamageFlashingTime() > 0)
        {
            const float intensity = .4f;
            float treeFlashTime = GetTree().DAMAGE_FLASH_TIME;

            float newDamageFlashingTime = GetTree().GetDamageFlashingTime() - Time.deltaTime;
            GetTree().SetDamageFlashingTime(Mathf.Clamp(newDamageFlashingTime, 0, treeFlashTime));
            float score = Mathf.Lerp(
                intensity,
                1f,
                Mathf.Abs(GetTree().GetDamageFlashingTime() - treeFlashTime / 2f) * (intensity * 10f)
            );
            byte greenBlueComponent = (byte)(score * 255);
            Color32 color = new Color32(255, greenBlueComponent, greenBlueComponent, 255);
            GetTree().SetColor(color);
        }
    }

    /// <summary>
    /// Destroys the Tree controlled by this TreeController.
    /// </summary>
    private void KillTree()
    {
        if (!ValidTree()) return;

        GetTree().Die();
        GameObject.Destroy(GetTree().gameObject);
    }

    /// <summary>
    /// Returns the Tree controlled by this TreeController.
    /// </summary>
    /// <returns>the Tree controlled by this TreeController.</returns>
    public Tree GetTree()
    {
        return tree;
    }

    /// <summary>
    /// Returns true if this TreeController should be removed.
    /// </summary>
    /// <returns>true if this TreeController should be removed; otherwise,
    /// false.</returns>
    public bool ShouldRemoveController()
    {
        return !ValidTree();
    }

    /// <summary>
    /// Returns true if this TreeController controls a valid Tree. If not,
    /// it should be removed.
    /// </summary>
    /// <returns>true if this TreeController controls a valid Tree;
    /// otherwise, false.</returns>
    private bool ValidTree()
    {
        return GetTree() != null;
    }

    /// <summary>
    /// Returns true if this TreeController controls a Tree with a positive
    /// health value.
    /// </summary>
    /// <returns>true if this TreeController controls a Tree with a positive
    /// health value; otherwise, false. </returns>
    public bool TreeAlive()
    {
        return ValidTree() && !GetTree().Dead();
    }

    /// <summary>
    /// Informs this TreeController of the most recent GameState so that
    /// it knows how to update its Tree.
    /// </summary>
    /// <param name="state">The most recent GameState.</param>
    public void InformOfGameState(GameState state)
    {
        gameState = state;
    }
}
