using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Represents a visible slot that hosts and displays a chosen Synergy.
/// </summary>
public class SynergySlot : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Ordered list of the tier Image components.
    /// </summary>
    [SerializeField]
    private List<Image> pillars;

    /// <summary>
    /// The Synergy this Prefab represents.
    /// </summary>
    [SerializeField]
    private SynergyController.Synergy synergy;

    /// <summary>
    /// Color of a pillar when illuminated.
    /// </summary>
    private readonly Color32 LIT_COLOR = new Color32(255, 255, 255, 255);

    /// <summary>
    /// Color of a pillar when extinguished.
    /// </summary>
    private readonly Color32 OFF_COLOR = new Color32(0, 0, 0, 255);

    /// <summary>
    /// Speed of the SynergySlot's movement lerp.
    /// </summary>
    private readonly float LERP_SPEED = 6f;

    /// <summary>
    /// The position the SynergySlot sits in.
    /// </summary>
    private Vector3 basePos;

    /// <summary>
    /// The position the SynergySlot lerps towards.
    /// </summary>
    private Vector3 lerpedPosition;

    /// <summary>
    /// The progress of the SynergySlot's lerp.
    /// </summary>
    private float lerpProgress = 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Main update loop for the SynergySlot. Lights up / turns off pillars 
    /// based on the synergy's current tier.
    /// </summary>
    /// <param name="tierNum">The current tier of the synergy in this slot.</param>
    /// <param name="hovering">true if this SynergySlot is being hoevered.</param>
    /// <param name="lerpCurve">the curve for this SynergySlot's lerp.</param>

    public void UpdateSynergySlot(int tierNum, bool hovering, AnimationCurve lerpCurve)
    {
        Assert.IsTrue(tierNum <= pillars.Count, "There are only " + pillars.Count
            + " tiers but you are trying to light up tier " + tierNum + ".");

        for (int i = 0; i < pillars.Count; i++)
        {
            if (i < tierNum) pillars[i].color = LIT_COLOR;
            else pillars[i].color = OFF_COLOR;
        }

        // Lerp Logic
        if (hovering) lerpProgress += Time.deltaTime * LERP_SPEED;
        else lerpProgress -= Time.deltaTime * LERP_SPEED;

        lerpProgress = Mathf.Clamp(lerpProgress, 0f, 1f);
        float curveValue = lerpCurve.Evaluate(lerpProgress);
        GetComponent<RectTransform>().position = Vector3.Lerp(basePos, lerpedPosition, curveValue);
    }

    /// <summary>
    /// Sets up the SynergySlot.
    /// </summary>
    public void SetupSynergySlot()
    {
        basePos = GetComponent<RectTransform>().position;
        lerpedPosition = new Vector3(basePos.x - 100, basePos.y, basePos.z);
    }

    /// <summary>
    /// Returns this SynergySlot's synergy.
    /// </summary>
    /// <returns>this SynergySlot's synergy.</returns>
    public SynergyController.Synergy GetSynergy() => synergy;

    #endregion
}
