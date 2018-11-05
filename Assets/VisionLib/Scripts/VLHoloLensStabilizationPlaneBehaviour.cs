/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;

#if (UNITY_EDITOR || UNITY_WSA_10_0)
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#else // UNITY_2017_2_OR_NEWER
using UnityEngine.VR.WSA;
#endif // UNITY_2017_2_OR_NEWER
#endif // (UNITY_EDITOR || UNITY_WSA_10_0)

/// <summary>
///  Positions stabilization plane over VLHoloLensTrackerBehaviour content.
/// </summary>
/// <remarks>
///  <para>
///   The stabilization plane is a method for reducing holographic
///   turbulence. See
///   https://developer.microsoft.com/en-us/windows/mixed-reality/case_study_-_using_the_stabilization_plane_to_reduce_holographic_turbulence
///   for further details.
///  </para>
///  <para>
///   This will only work well, if the origin of the content is actually
///   the part the user should focus on.
///  </para>
///  <para>
///   For more advanced stabilization plane features, please consider
///   disabling this behaviour and using the functionalities of the
///   MixedRealityToolkit for Unity
///   (https://github.com/Microsoft/MixedRealityToolkit-Unity) instead.
///  </para>
/// </remarks>
[AddComponentMenu("VisionLib/VL HoloLens Stabilization Plane Behaviour")]
public class VLHoloLensStabilizationPlaneBehaviour : MonoBehaviour
{
    /// <summary>
    ///  Reference to used VLHoloLensTrackerBehaviour.
    /// </summary>
    /// <remarks>
    ///  If this is not defined, then it will get set automatically.
    /// </remarks>
    public VLHoloLensTrackerBehaviour holoLensTrackerBehaviour;

#if (UNITY_EDITOR || UNITY_WSA_10_0)
    private void Awake()
    {
    }

    private void Start()
    {
        // Automatically find VLHoloLensTrackerBehaviour, if it wasn't
        // specified explicitly
        if (this.holoLensTrackerBehaviour == null)
        {
            // Try to find it on the same GameObject
            this.holoLensTrackerBehaviour =
                GetComponent<VLHoloLensTrackerBehaviour>();
            if (this.holoLensTrackerBehaviour == null)
            {
                // Try to find it anywhere in the scene
                this.holoLensTrackerBehaviour =
                    FindObjectOfType<VLHoloLensTrackerBehaviour>();
                if (this.holoLensTrackerBehaviour == null)
                {
                    Debug.LogWarning("[vlUnitySDK] No GameObject with VLHoloLensTrackerBehaviour found");
                }
            }
        }
    }

    private void Update()
    {
    }

    private void LateUpdate()
    {
        GameObject content = (this.holoLensTrackerBehaviour != null ?
            this.holoLensTrackerBehaviour.content : null);
        Transform contentTransform = (content != null ?
            content.transform : null);
        if (contentTransform == null)
        {
            return;
        }

        HolographicSettings.SetFocusPointForFrame(
            contentTransform.position,
            -Camera.main.transform.forward);
    }
#else // Empty dummy implementation
    private void Awake()
    {
    }

    private void Start()
    {
        Debug.LogWarning("[vlUnitySDK] The VLHoloLensStabilizationPlaneBehaviour only works for Windows Store applications");
    }

    private void Update()
    {
    }

    public void LateUpdate()
    {
    }
#endif // (UNITY_EDITOR || UNITY_WSA_10_0)
}

/**@}*/