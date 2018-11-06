using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  This behaviour automatically loads a tracking configuration on start up.
/// </summary>
/// <remarks>
///  This is just an example to show how this could be implemented. Please feel
///  free to write your own start script.
/// </remarks>
public class AutoStart : MonoBehaviour
{
    /// <summary>
    ///  Reference to used VLWorkerBehaviour.
    /// </summary>
    /// <remarks>
    ///  If this is not defined, then the first found VLWorkerBehaviour will be
    ///  used automatically.
    /// </remarks>
    public VLWorkerBehaviour workerBehaviour;

    /// <summary>
    ///  Optional GameObject which will get enabled after starting the tracker.
    /// </summary>
    public GameObject visualAid;

    /// <summary>
    ///  Tracking configuration file to be loaded automatically on start up.
    /// </summary>
    public string filename = "";

    private bool trackingStarted = false;

    private void StartTracking()
    {
        // VLWorkerBeahaviour not specified explicitly?
        if (this.workerBehaviour == null)
        {
            // Try to get the VLWorkerBehaviour from the current GameObject
            this.workerBehaviour = GetComponent<VLWorkerBehaviour>();

            // If the current GameObject doesn't have a VLWorkerBehaviour
            // attached, just use the first VLWorkerBehaviour from anywhere in
            // the scene
            if (this.workerBehaviour == null)
            {
                this.workerBehaviour = FindObjectOfType<VLWorkerBehaviour>();

                // Failed to get VLWorkerBehaviour?
                if (this.workerBehaviour == null)
                {
                    return;
                }
            }
        }

        this.workerBehaviour.StartTracking(filename);

        if (this.visualAid != null)
        {
            this.visualAid.SetActive(true);
        }

        this.trackingStarted = true;
    }

    void Start ()
    {
        StartTracking();
    }

    void Update ()
    {
        if (!this.trackingStarted)
        {
            StartTracking();
        }
    }
}
