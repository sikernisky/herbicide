using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a collectable currency.
/// </summary>
public abstract class Currency : Collectable
{
    /// <summary>
    /// Value of this currency.
    /// </summary>
    public virtual int VALUE => 1;

    /// <summary>
    /// Seconds to complete a bob cycle.
    /// </summary>
    protected virtual float BOB_SPEED => 3f;

    /// <summary>
    /// How "tall" this Currency bobs.
    /// </summary>
    protected virtual float BOB_HEIGHT => .15f;

    /// <summary>
    /// Starting position of this Currency when it bobs up and down.
    /// </summary>
    private Vector3 bobStartPosition;

    /// <summary>
    /// true if this Currency's bobbing start position has been set. 
    /// </summary>
    private bool startPositionSet;

    /// <summary>
    /// Time offset to create varying bobbing animations.
    /// </summary>
    private float timeOffset;

    /// <summary>
    /// Reference to this Currency's SpriteRenderer component.
    /// </summary>
    [SerializeField]
    private SpriteRenderer currencyRenderer;

    /// <summary>
    /// Collects this currency, updating the player's balance and playing
    /// relevant animations and sounds. Last, destroys the currency.
    /// </summary>
    public override void OnCollect()
    {
        SoundController.PlaySoundEffect("collectCurrency");
    }

    /// <summary>
    /// Updates this Currency.
    /// </summary>
    public void UpdateCurrency()
    {
        Bob();
    }

    /// <summary>
    /// Performs actions when this Currency spawns in game. Sets the
    /// starting position for the Bob animation.
    /// </summary>
    public void OnSpawn()
    {
        bobStartPosition = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2);
        startPositionSet = true;
    }

    /// <summary>
    /// Bobs this Currency up and down.
    /// </summary>
    protected virtual void Bob()
    {
        if (!startPositionSet) return;

        float t = (Mathf.Cos((Time.time + timeOffset) * BOB_SPEED) + 1) * 0.5f;
        float newY = Mathf.Lerp(-BOB_HEIGHT, BOB_HEIGHT, t);
        Vector3 newPosition = new Vector3(
            bobStartPosition.x,
            bobStartPosition.y + newY,
            bobStartPosition.z);
        transform.position = newPosition;
    }
}
