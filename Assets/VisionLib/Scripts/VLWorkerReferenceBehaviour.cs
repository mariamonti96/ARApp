/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;

/// <summary>
///  Base class for MonoBehaviour, which need access to the
///  <see cref="VLWorker"/> and <see cref="VLWorkerBehaviour"/> objects.
/// </summary>
public abstract class VLWorkerReferenceBehaviour : MonoBehaviour
{
    /// <summary>
    ///  Reference to used VLWorkerBehaviour.
    /// </summary>
    /// <remarks>
    ///  If this is not defined, then it will get set automatically.
    /// </remarks>
    [Tooltip("Reference to used VLWorkerBehaviour. If this is not defined, then it will get set automatically.")]
    public VLWorkerBehaviour workerBehaviour;

    protected VLWorker worker;

    /// <summary>
    ///  Initializes the <see cref="workerBehaviour"/> and <see cref="worker"/>
    ///  member variables.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, on success; <c>false</c> otherwise.
    /// </returns>
    protected bool InitWorkerReference()
    {
        // Worker already found?
        if (this.worker != null)
        {
            return true;
        }

        // VLWorkerBeahaviour specified explicitly?
        if (this.workerBehaviour != null)
        {
            this.worker = this.workerBehaviour.GetWorker();
            return (this.worker != null);
        }

        // Look for it at the same GameObject first
        this.workerBehaviour =
            GetComponent<VLWorkerBehaviour>();
        if (this.workerBehaviour != null)
        {
            this.worker = this.workerBehaviour.GetWorker();
            return (this.worker != null);
        }

        // Try to find it anywhere in the scene
        this.workerBehaviour = FindObjectOfType<VLWorkerBehaviour>();
        if (this.workerBehaviour != null)
        {
            this.worker = this.workerBehaviour.GetWorker();
            return (this.worker != null);
        }

        return false;
    }
}

/**@}*/