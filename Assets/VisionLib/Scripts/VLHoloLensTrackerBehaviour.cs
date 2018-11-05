/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using UnityEngine;

#if (UNITY_EDITOR || UNITY_WSA_10_0)
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
#else // UNITY_2017_2_OR_NEWER
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
#endif // UNITY_2017_2_OR_NEWER
#endif // (UNITY_EDITOR || UNITY_WSA_10_0)

/// <summary>
///  Manages the model-based initialization of the HoloLens tracking.
/// </summary>
/// <remarks>
///  Right now this behaviour only works correctly, if there also is an
///  enabled VLHoloLensInitCameraBehaviour somewhere in the scene.
/// </remarks>
[AddComponentMenu("VisionLib/VL HoloLens Tracker Behaviour")]
public class VLHoloLensTrackerBehaviour : VLWorkerReferenceBehaviour
{
    /// <summary>
    ///  The GameObject representing the HoloLens camera.
    /// </summary>
    ///  If this is not defined, then the VLHoloLensTrackerBehaviour tries to
    ///  find the camera automatically. It will first try to find a camera
    ///  component on the current GameObject. Then it will use the main camera.
    ///  If this also fails, it will use any camera available in the current
    ///  scene.
    /// </remarks>
    [Tooltip("The GameObject representing the HoloLens camera. If this is not defined, then it will get set automatically.")]
    public GameObject holoLensCamera;

    /// <summary>
    ///  GameObject with the AR content attached to it.
    /// </summary>
    /// <remarks>
    ///  Any existing transformation of the content GameObject will get
    ///  overwritten. If you need to transform the content, then please add
    ///  a child GameObject and apply the transformation to it instead.
    /// </remarks>
    [Tooltip("GameObject with the AR content attached to it")]
    public GameObject content;

    public float smoothTime = 1.0f / 10.0f;

#if (UNITY_EDITOR || UNITY_WSA_10_0)
    private static readonly string worldAnchorName =
        "VISVideoSourceWorldAnchor";
    private WorldAnchorStore anchorStore;
    private string[] anchorIDs = new string[0];
    private WorldAnchor worldAnchor;
    private GameObject worldAnchorGO;

    private float[] modelViewMatrixArray = new float[16];
    private Matrix4x4 modelViewMatrix = new Matrix4x4();

    private bool initMode = true;

    private Vector3 contentTargetPosition = Vector3.zero;
    private Vector3 contentTargetPositionVelocity = Vector3.zero;
    private Quaternion contentTargetRotation = Quaternion.identity;

    private int updateIgnoreCounter = 0; // TODO(mbuchner): Find a better
                                         // solution for ignoring the
                                         // OnExtrinsicData call

    private void OnTrackerInitializing()
    {
        // Reset the transformation, because the world anchor GameObject must
        // be centred at the origin to work correctly
        if (this.anchorStore != null)
        {
            ClearWorldAnchor();
            this.worldAnchorGO.transform.position = Vector3.zero;
            this.worldAnchorGO.transform.rotation = Quaternion.identity;
            SaveWorldAnchor();
        }
        else
        {
            this.worldAnchorGO.transform.position = Vector3.zero;
            this.worldAnchorGO.transform.rotation = Quaternion.identity;
            // The new anchor position will be saved during the callback from
            // WorldAnchorStore.GetAsync
        }

        this.contentTargetPosition = Vector3.zero;
        this.contentTargetPositionVelocity = Vector3.zero;
        this.contentTargetRotation = Quaternion.identity;
        this.updateIgnoreCounter = 0;

        this.ActivateInitMode();
    }

    private void OnExtrinsicData(VLExtrinsicDataWrapper extrinsicData)
    {
        if (this.updateIgnoreCounter > 0)
        {
            this.updateIgnoreCounter -= 1;
            return;
        }

        // State changed from invalid to valid?
        bool valid = extrinsicData.GetValid();
        if (valid && this.initMode)
        {
            this.DeactivateInitMode();
        }
        // State changed from valid to invalid?
        else if (!valid && !this.initMode)
        {
            // Don't go to initialization mode, because the HoloLens is able to
            // relocate itself
            //this.ActivateInitMode();
        }

        if (!extrinsicData.GetModelViewMatrix(this.modelViewMatrixArray))
        {
            return;
        }

        if (valid)
        {
            // Apply the extrinsic camera parameters
            for (int i=0; i < 16; ++i)
            {
                this.modelViewMatrix[i % 4, i / 4] =
                    this.modelViewMatrixArray[i];
            }

            // Compute the left-handed world to camera matrix
            this.modelViewMatrix = VLUnityCameraHelper.flipZ *
                VLUnityCameraHelper.flipYZ *
                this.modelViewMatrix *
                VLUnityCameraHelper.flipZ;

            Vector3 t = this.modelViewMatrix.GetColumn(3);
            Quaternion q = Quaternion.LookRotation(
                this.modelViewMatrix.GetColumn(2),
                this.modelViewMatrix.GetColumn(1));

            // Don't set the position directly. Interpolate smoothly in the
            // Update function instead
            this.contentTargetPosition = t;
            this.contentTargetRotation = q;
        }
    }

    private void OnTrackerReset(bool success)
    {
        this.ActivateInitMode();
        this.updateIgnoreCounter = 1; // Ignore the next OnExtrinsicData call,
                                      // because it might contain the previous
                                      // (valid) tracking pose
    }

    public void ActivateInitMode()
    {
        if (this.initMode)
        {
            return;
        }

        this.initMode = true;

        // Attach content to HoloLens camera
        if (this.content != null && this.holoLensCamera != null)
        {
            this.content.transform.parent = this.holoLensCamera.transform;
        }
    }

    private void DeactivateInitMode()
    {
        if (!this.initMode)
        {
            return;
        }

        this.initMode = false;

        // Detach content from HoloLens camera and attach to world anchor
        // GameObject
        if (this.content != null)
        {
            this.content.transform.parent = this.worldAnchorGO.transform;
            // When changing the parent node, Unity will keep the same
            // position and rotation in the world. Therefore we don't have to
            // convert the initial pose to world coordinates by ourself.
        }

        this.contentTargetPositionVelocity = Vector3.zero;
    }

    private void SaveWorldAnchor()
    {
        if (this.anchorStore == null)
        {
            Debug.LogError("[vlUnitySDK] SaveWorldAnchor: WorldAnchorStore not ready");
            return;
        }

        if (this.worldAnchor != null)
        {
            Debug.LogWarning("[vlUnitySDK] SaveWorldAnchor: Saving world anchor without clearing first");
        }

        this.worldAnchor = this.worldAnchorGO.AddComponent<WorldAnchor>();
        /*
        if (!this.worldAnchor.isLocated)
        {
            Debug.LogWarning("[vlUnitySDK] SaveWorldAnchor: Anchor is not located");
        }
        */

        if (!this.anchorStore.Save(
            VLHoloLensTrackerBehaviour.worldAnchorName,
            this.worldAnchor))
        {
            Debug.LogError("[vlUnitySDK] SaveWorldAnchor: Anchor save failed");
        }
    }

    private void ClearWorldAnchor()
    {
        if (this.worldAnchor != null)
        {
            DestroyImmediate(this.worldAnchor);
            this.worldAnchor = null;
        }

        if (this.anchorStore != null)
        {
            this.anchorStore.Delete(
                VLHoloLensTrackerBehaviour.worldAnchorName);
        }
    }

    // Set WorldAnchor to current object position
    public void UpdateWorldAnchor()
    {
        if (this.anchorStore == null)
        {
            Debug.LogError("[vlUnitySDK] UpdateWorldAnchor: WorldAnchorStore not ready");
            return;
        }

        if (this.content == null)
        {
            Debug.LogError("[vlUnitySDK] UpdateWorldAnchor: Content is null");
            return;
        }

        ClearWorldAnchor();
        this.worldAnchorGO.transform.position =
            this.content.transform.position;
        this.worldAnchorGO.transform.rotation =
            this.content.transform.rotation;
        SaveWorldAnchor();

        this.content.transform.localPosition = Vector3.zero;
        this.content.transform.localRotation = Quaternion.identity;

        this.contentTargetPosition = Vector3.zero;
        this.contentTargetRotation = Quaternion.identity;

        // Ignore the next update of visionLib's extrinsicData, since it is
        // related to the old anchor position
        this.updateIgnoreCounter = 1;
    }

    private bool InitCameraReference()
    {
        // HoloLens camera specified manually or previously found?
        if (this.holoLensCamera != null)
        {
            return true;
        }

        // Look for it at the same GameObject first
        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            this.holoLensCamera = camera.gameObject;
            return true;
        }

        // Use the main camera
        camera = Camera.main;
        if (camera != null)
        {
            this.holoLensCamera = camera.gameObject;
            return true;
        }

        // Try to find it anywhere in the scene
        camera = FindObjectOfType<Camera>();
        if (camera != null)
        {
            this.holoLensCamera = camera.gameObject;
            return true;
        }

        return false;
    }

    private void Awake()
    {
        // Create the GameObject, which represents the origin of the HoloLens
        // coordinate system in Unity
        this.worldAnchorGO = new GameObject("VLHoloLensWorldAnchor");

        if (this.content == null)
        {
            Debug.LogWarning("[vlUnitySDK] Content is null. Did you forget to set the 'content' property?");
        }
    }

    private void OnEnable()
    {
        VLWorkerBehaviour.OnTrackerInitializing += OnTrackerInitializing;
        VLWorkerBehaviour.OnExtrinsicData += OnExtrinsicData;
        VLWorkerBehaviour.OnTrackerResetSoft += OnTrackerReset;
        VLWorkerBehaviour.OnTrackerResetHard += OnTrackerReset;
        this.worldAnchorGO.SetActive(true);
    }

    private void OnDisable()
    {
        // GameObject not destroyed already?
        if (this.worldAnchorGO != null)
        {
            this.worldAnchorGO.SetActive(false);
        }
        VLWorkerBehaviour.OnTrackerResetHard -= OnTrackerReset;
        VLWorkerBehaviour.OnTrackerResetSoft -= OnTrackerReset;
        VLWorkerBehaviour.OnExtrinsicData -= OnExtrinsicData;
        VLWorkerBehaviour.OnTrackerInitializing -= OnTrackerInitializing;
    }

    private void OnGetWorldAnchorStoreCallback(WorldAnchorStore anchorStore)
    {
        this.anchorStore = anchorStore;
        ClearWorldAnchor();
        SaveWorldAnchor();
    }

    private void Start()
    {
        if (!this.InitWorkerReference())
        {
            Debug.LogWarning("[vlUnitySDK] Failed to get the VLWorker from VLWorkerBehaviour");
        }

        if (!this.InitCameraReference())
        {
            Debug.LogWarning("[vlUnitySDK] Could not find HoloLens camera");
        }

        WorldAnchorStore.GetAsync(OnGetWorldAnchorStoreCallback);

        this.initMode = false;
        this.ActivateInitMode();
    }

    private void OnDestroy()
    {
    }

    private void Update()
    {
        // Because the WorldAnchorManager, SharingWorldAnchorManager and
        // ImportExportAnchorManager of the MixedRealityToolkit globally erase
        // all anchors (see the following issue: https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/1488)
        // we must constantly check whether someone deleted our global anchor.
        // In that case we re-create the global anchor.
        if (this.anchorStore != null)
        {
            if (this.anchorStore.anchorCount != this.anchorIDs.Length)
            {
                this.anchorIDs = new string[this.anchorStore.anchorCount];
            }

            int anchorCount = this.anchorStore.GetAllIds(this.anchorIDs);
            if (Array.IndexOf(this.anchorIDs,
                    VLHoloLensTrackerBehaviour.worldAnchorName, 0, anchorCount) < 0)
            {
                Debug.LogWarning("[vlUnitySDK] Global anchor was deleted by some outside source. Creating new one.");
                ClearWorldAnchor();
                SaveWorldAnchor();
            }
        }

        if (this.content == null)
        {
            return;
        }

        if (!this.initMode)
        {
            // Interpolate the transformation of the content
            if (this.smoothTime > 0)
            {
                this.content.transform.localPosition = Vector3.SmoothDamp(
                    this.content.transform.localPosition,
                    this.contentTargetPosition,
                    ref this.contentTargetPositionVelocity,
                    this.smoothTime);

                float elapsedTime = Mathf.Min(Time.deltaTime, this.smoothTime);
                this.content.transform.localRotation = Quaternion.Slerp(
                    this.content.transform.localRotation,
                    this.contentTargetRotation,
                    0.5f * (elapsedTime / this.smoothTime));
            }
            else
            {
                this.content.transform.localPosition = this.contentTargetPosition;
                this.content.transform.localRotation = this.contentTargetRotation;
            }
        }
    }
#else // Empty dummy implementation
    private void Awake()
    {
    }

    private void Start()
    {
        Debug.LogWarning("[vlUnitySDK] The VLHoloLensTrackerBehaviour only works for Windows Store applications");
    }

    private void Update()
    {
    }
#endif // (UNITY_EDITOR || UNITY_WSA_10_0)
}

/**@}*/