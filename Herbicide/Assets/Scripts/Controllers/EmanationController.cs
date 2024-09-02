using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls an Emanation. <br></br>
/// 
/// The EmanationController is responsible for manipulating its Emanation
/// and bringing it to life. This includes moving it playing animations,
/// and more. <br></br>
/// 
/// The EmanationController does not follow the Animation infrastructure
/// used by all other models. This is because it only needs one track and
/// never changes states. Therefore, using that framework would require
/// implementing unused code. </summary>
public class EmanationController
{
    #region Fields

    /// <summary>
    /// Different types of Emanations. 
    /// </summary>
    public enum EmanationType
    {
        BEAR_CHOMP,
        QUILL_PIERCE,
        BLACKBERRY_EXPLOSION
    }

    /// <summary>
    /// Type of Emanation this controller is animating.
    /// </summary>
    private readonly EmanationType TYPE;

    /// <summary>
    /// The number of cycles the EmanationController must complete
    /// before removing its Emanation model.
    /// </summary>
    private int requiredCycles;

    /// <summary>
    /// The number of animation cycles completed so far.
    /// </summary>
    private int cyclesCompleted;

    /// <summary>
    /// Number of seconds it takes to complete one cycle of
    /// the emanation.
    /// </summary>
    private float cycleTime;

    /// <summary>
    /// Animation track to play.
    /// </summary>
    private Sprite[] emanationTrack;

    /// <summary>
    /// Current frame of the Emanation's animation track.
    /// </summary>
    private int frame;

    /// <summary>
    /// Keeps track of animation time.
    /// </summary>
    private float counter;

    /// <summary>
    /// The GameObject used to display the Emanation.
    /// </summary>
    private GameObject emanationDummy;

    /// <summary>
    /// The SpriteRenderer component used to display the Emanation.
    /// </summary>
    private SpriteRenderer emanationRenderer;

    /// <summary>
    /// Where the Emanation spawns.
    /// </summary>
    private Vector3 spawnPos;

    /// <summary>
    /// true if this Emanation will be destroyed at the end of the frame;
    /// otherwise, false.
    /// </summary>
    private bool scheduledForDestruction;

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new EmanationController.
    /// </summary>
    /// <param name="emanationType">The Emanation to play.</param>
    /// <param name="numCycles">The number of cycles the EmanationController must
    /// run before removing its Emanation.</param>
    /// <param name="target">Where the Emanation should be.</param> 
    /// <param name="rotation">The rotation of the Emanation.</param>
    public EmanationController(EmanationType emanationType, int numCycles, Vector3 target, Quaternion rotation)
    {
        Assert.IsTrue(numCycles > 0);

        emanationTrack = EmanationFactory.GetEmanationTrack(emanationType);
        TYPE = emanationType;
        cycleTime = EmanationFactory.GetEmanationCycleTime(emanationType);
        requiredCycles = numCycles;
        spawnPos = target;

        GameObject emanation = new GameObject(TYPE.ToString());
        emanationDummy = emanation;
        emanationRenderer = emanation.AddComponent<SpriteRenderer>();
        emanationRenderer.sortingLayerName = SortingLayers.EMANATIONS.ToString().ToLower();
        emanationDummy.transform.position = spawnPos;
        emanationDummy.transform.rotation = rotation;
    }

    /// <summary>
    /// Main update loop for the EmanationController.
    /// </summary>
    public void UpdateEmanation()
    {
        if (ScheduledForDestruction()) return;
        if (ShouldRemoveModel())
        {
            DestroyAndRemoveModel();
            return;
        }
        if (DoneAnimating()) return;

        // Debug.Log("here");


        counter += Time.deltaTime;
        emanationRenderer.sprite = emanationTrack[frame];
        float stepTime = cycleTime / emanationTrack.Length;
        if (counter - stepTime > 0)
        {
            if (frame + 1 >= emanationTrack.Length)
            {
                frame = 0;
                cyclesCompleted++;
            }
            else frame++;
            counter = 0;
        }

    }

    /// <summary>
    /// Returns true if the ControllerController should remove this EmanationController.
    /// This should happen when the EmanationController completes its required number
    /// of animation cycles.
    /// </summary>
    /// <returns>true if the ControllerController should remove this EmanationController;
    /// otherwise, false. </returns>
    public bool ShouldRemoveController() => emanationDummy == null;

    /// <summary>
    /// Returns true if this controller should destroy its Emanation.
    /// </summary>
    /// <returns>true if this controller should destroy its Emanation; otherwise,
    /// false. </returns>
    private bool ShouldRemoveModel() => DoneAnimating();

    /// <summary>
    /// Returns true if the EmanationController has completed its required number
    /// of animation cycles. 
    /// </summary>
    /// <returns>true if the EmanationController has completed its required number
    /// of animation cycles; otherwise, false. </returns>
    private bool DoneAnimating() => cyclesCompleted >= requiredCycles;

    /// <summary>
    /// Destroys and detatches the Model from this Controller.
    /// </summary>
    protected virtual void DestroyAndRemoveModel()
    {
        if (emanationDummy == null || System.Object.Equals(null, emanationDummy)) return;

        DestroyModel();
        emanationDummy = null;
    }

    /// <summary>
    /// Schedules the Emanation for destruction and destroys it at the
    /// end of the current frame.
    /// </summary>
    public void DestroyModel()
    {
        if (ScheduledForDestruction()) return;
        scheduledForDestruction = true;
        GameObject.Destroy(emanationDummy);
    }

    /// <summary>
    /// Returns true if the Emanation is scheduled for destruction.
    /// </summary>
    /// <returns>true if the Emanation is scheduled for destruction;
    /// otherwise, false.</returns>
    protected bool ScheduledForDestruction() => scheduledForDestruction;

    #endregion
}
