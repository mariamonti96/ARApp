/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;

/// <summary>
///  Camera used to define the initial pose.
/// </summary>
/// <remarks>
///  <para>
///   It's possible to change the camera position and orientation at
///   runtime. The new initial pose will then be used while the tracking is
///   lost.
///  </para>
///  <para>
///   If there is no VLInitCameraBehaviour in the scene or the
///   VLInitCameraBehaviour is disabled, then the initial pose from the
///   tracking configuration file will be used.
///  </para>
///  <para>
///   Please make sure, that there is only one active VLInitCameraBehaviour in
///   the scene. Otherwise both behaviours will try to set the initial pose,
///   which will lead to unexpected behaviour.
///  </para>
///  <para>
///   Right now this behaviour does not work with the HoloLens model-based
///   tracking. In that case please use the VLHoloLensInitCameraBehaviour
///   or VLHoloLensInitCamera prefab instead.
///  </para>
/// </remarks>
[AddComponentMenu("VisionLib/VL Init Camera Behaviour")]
public class VLInitCameraBehaviour : VLWorkerReferenceBehaviour
{
    /// <summary>
    ///  Reference to the Camera behaviour.
    /// </summary>
    /// <remarks>
    ///  If this is not defined, then the Camera component attached to the
    ///  current GameObject will be used automatically.
    /// </remarks>
    public Camera initCamera;

    /// <summary>
    ///  Layer with the camera image background.
    /// </summary>
    /// <remarks>
    ///  This layer will not be rendered.
    /// </remarks>
    public int backgroundLayer = 8;

    /// <summary>
    ///  Use the last valid camera pose as initialization pose.
    /// </summary>
    /// <remarks>
    ///  Since this might results in an awkward InitCamera transformation, it's
    ///  recommended to give the user the option to restore the original pose
    ///  using the <see cref="Reset"/> function.
    /// </remarks>
    [Tooltip("Use the last valid pose as initialization pose")]
    public bool useLastValidPose;

    /// <summary>
    ///  Overwrite camera transformation with values from tracking
    ///  configuration.
    /// </summary>
    /// <remarks>
    ///  The InitCamera can then be transformed afterwards, but will get
    ///  overwritten again after loading a new tracking configuration.
    /// </remarks>
    [Tooltip("Overwrite camera transformation with values from tracking configuration")]
    public bool overwriteOnLoad;

    private Matrix4x4 rotCamera = VLUnityCameraHelper.rotationZ0;
    private Matrix4x4 invRotCamera = VLUnityCameraHelper.rotationZ0;
    private ScreenOrientation screenOrientation = ScreenOrientation.Portrait;

    private float[] modelViewMatrixArray = new float[16];
    private Matrix4x4 modelViewMatrix = new Matrix4x4();
    private float[] projectionMatrixArray = new float[16];
    private Matrix4x4 projectionMatrix = new Matrix4x4();

    private GCHandle gcHandle;
    private bool settingInitPose;
    private bool ready;
    private bool reset;

    private Vector3 originalPosition;
    private Quaternion originalOrientation;

    /// <summary>
    ///  Restores the original transformation of the InitCamera.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This might be useful if the InitCamera was transformed in some
    ///   awkward way for some reason (e.g. because
    ///   <see cref="useLastValidPose"/> is set to <c>true</c> and the tracking
    ///   failed) and we quickly want to restore the original state.
    ///  </para>
    ///  <para>
    ///   If <see cref="overwriteOnLoad"/> is set to <c>false</c>, then this
    ///   will restore the transformation during the initialization of the
    ///   VLInitCameraBehaviour. If <see cref="overwriteOnLoad"/> is set to
    ///   <c>true</c>, then this will restore the transformation from the
    ///   tracking configuration.
    ///  </para>
    /// </remarks>
    public void Reset()
    {
        this.reset = true;
    }

    private static VLInitCameraBehaviour GetInstance(IntPtr clientData)
    {
        return (VLInitCameraBehaviour)GCHandle.FromIntPtr(clientData).Target;
    }

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchGetInitPoseCallback(string errorJson,
        string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).OnGetInitPoseCallback(
                errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchGetInitPoseCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchGetInitPoseCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchSetInitPoseCallback(string errorJson,
        string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).OnSetInitPoseCallback(
                errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchSetInitPoseCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchSetInitPoseCallback);

    private static VLModelTrackerCommands.SetInitPoseCmd.Param CameraToInitParam(
        Camera camera, Matrix4x4 offset)
    {
        Vector4 t;
        Quaternion q;
        VLUnityCameraHelper.CameraToVLPose(camera, offset, out t, out q);
        return new VLModelTrackerCommands.SetInitPoseCmd.Param(
            t.x, t.y, t.z, q.x, q.y, q.z, q.w);
    }

    private void OnOrientationChange(ScreenOrientation orientation)
    {
        this.screenOrientation = orientation;
        if (orientation == VLUnityCameraHelper.ScreenOrientation0)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ0;
            this.invRotCamera = VLUnityCameraHelper.rotationZ0;
        }
        else if (orientation == VLUnityCameraHelper.ScreenOrientation180)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ180;
            this.invRotCamera = VLUnityCameraHelper.rotationZ180;
        }
        else if (orientation == VLUnityCameraHelper.ScreenOrientation90CCW)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ270;
            this.invRotCamera = VLUnityCameraHelper.rotationZ90;
        }
        else if (orientation == VLUnityCameraHelper.ScreenOrientation90CW)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ90;
            this.invRotCamera = VLUnityCameraHelper.rotationZ270;
        }
    }

    private void OnTrackerInitializing()
    {
        this.settingInitPose = false;
        this.ready = false;
        this.reset = false;
    }

    private void OnTrackerInitialized(bool success)
    {
        if (!this.overwriteOnLoad)
        {
            if (this.initCamera)
            {
                this.SetInitPose();
                this.ready = true;
            }
        }
        else
        {
            this.GetInitPose();
        }
    }

    private void OnExtrinsicData(VLExtrinsicDataWrapper extrinsicData)
    {
        if (!this.useLastValidPose || !this.ready || this.initCamera == null)
        {
            return;
        }

        if (!extrinsicData.GetValid() ||
            !extrinsicData.GetModelViewMatrix(this.modelViewMatrixArray))
        {
            return;
        }

        // Apply the extrinsic camera parameters

        for (int i=0; i < 16; ++i)
        {
            this.modelViewMatrix[i % 4, i / 4] =
                this.modelViewMatrixArray[i];
        }
        /*
        // TODO(mbuchner): Why is this necessary? According to the Unity
        // documentation, Camera.worldToCameraMatrix should work with OpenGL
        // coordinates.
        this.modelViewMatrix[0, 2] = -this.modelViewMatrix[0, 2];
        this.modelViewMatrix[1, 2] = -this.modelViewMatrix[1, 2];
        this.modelViewMatrix[2, 2] = -this.modelViewMatrix[2, 2];

        // Reverse the rotation of the scene which Unity does
        // automatically
        this.initCamera.worldToCameraMatrix =
            this.invRotCamera * this.modelViewMatrix;
        */

        // Compute the right-handed world to camera matrix
        Matrix4x4 worldToCameraMatrix = this.invRotCamera * this.modelViewMatrix;

        // Compute the left-handed world to camera matrix
        worldToCameraMatrix = VLUnityCameraHelper.flipZ * worldToCameraMatrix * VLUnityCameraHelper.flipZ;

        // Compute the left-handed camera to world matrix
        Matrix4x4 cameraToWorldMatrix = worldToCameraMatrix.inverse;

        // Extract the rotation and translation from the computed matrix
        this.initCamera.transform.rotation =
            Quaternion.LookRotation(
                cameraToWorldMatrix.GetColumn(2),
                cameraToWorldMatrix.GetColumn(1));
        this.initCamera.transform.position =
            cameraToWorldMatrix.GetColumn(3);

        // TODO(mbuchner): Compute camera orientation in native code
    }

    private void OnIntrinsicData(VLIntrinsicDataWrapper intrinsicData)
    {
        if (this.initCamera == null)
        {
            return;
        }

        // Apply the intrinsic camera parameters
        if (intrinsicData.GetProjectionMatrix(
            this.initCamera.nearClipPlane,
            this.initCamera.farClipPlane,
            Screen.width, Screen.height, this.screenOrientation, 0,
            projectionMatrixArray))
        {
            for (int i=0; i < 16; ++i)
            {
                projectionMatrix[i % 4, i / 4] =
                    projectionMatrixArray[i];
            }
            this.initCamera.projectionMatrix = projectionMatrix;
        }
    }

    private void OnGetInitPoseCallback(string errorJson, string resultJson)
    {
        if (errorJson != null)
        {
            Debug.LogWarning("[vlUnitySDK] OnGetInitPoseCallback: Failed to get init pose");
            return;
        }

        VLModelTrackerCommands.GetInitPoseResult result =
            VLJsonUtility.FromJson<VLModelTrackerCommands.GetInitPoseResult>(
                resultJson);

        Vector3 position;
        Quaternion orientation;
        VLUnityCameraHelper.VLPoseToCamera(
            new Vector3(result.t[0], result.t[1], result.t[2]),
            new Quaternion(result.q[0], result.q[1], result.q[2], result.q[3]),
            out position, out orientation);

        if (this.initCamera != null)
        {
            this.initCamera.transform.position = position;
            this.initCamera.transform.rotation = orientation;
            this.originalPosition = position;
            this.originalOrientation = orientation;
            this.ready = true;
        }
        else
        {
            Debug.LogWarning("[vlUnitySDK] OnGetInitPoseCallback: initCamera is null");
        }
    }

    private void OnSetInitPoseCallback(string errorJson, string resultJson)
    {
        this.settingInitPose = false;
    }

    private void GetInitPose()
    {
        if (this.InitWorkerReference())
        {
            this.worker.PushCommand(
                new VLModelTrackerCommands.GetInitPoseCmd(),
                dispatchGetInitPoseCallbackDelegate,
                GCHandle.ToIntPtr(this.gcHandle));
        }
        else
        {
            Debug.LogWarning("[vlUnitySDK] GetInitPose: Failed to get the VLWorker from VLWorkerBehaviour");
        }
    }

    private void SetInitPose()
    {
        if (this.InitWorkerReference())
        {
            this.settingInitPose = true;
            VLModelTrackerCommands.SetInitPoseCmd.Param param =
                CameraToInitParam(this.initCamera, this.rotCamera);
            this.worker.PushCommand(
                new VLModelTrackerCommands.SetInitPoseCmd(param),
                dispatchSetInitPoseCallbackDelegate,
                GCHandle.ToIntPtr(this.gcHandle));
        }
        else
        {
            Debug.LogWarning("[vlUnitySDK] SetInitPose: Failed to get the VLWorker from VLWorkerBehaviour");
        }
    }

    private void Awake()
    {
        // Get a handle to the current object and make sure, that the object
        // doesn't get deleted by the garbage collector. We then use this
        // handle as client data for the native callbacks. This allows us to
        // retrieve the current address of the actual object during the
        // callback execution. GCHandleType.Pinned is not necessary, because we
        // are accessing the address only through the handle object, which gets
        // stored in a global handle table.
        this.gcHandle = GCHandle.Alloc(this);

        // Get the initCamera from the current GameObject, if it wasn't
        // specified explicitly
        if (this.initCamera == null)
        {
            this.initCamera = this.GetComponent<Camera>();
        }

        if (this.initCamera != null)
        {
            // Don't clear the background image
            this.initCamera.clearFlags = CameraClearFlags.Depth;

            // Render after the background camera
            this.initCamera.depth = 2;

            // Don't render the background image
            int mask = 1 << this.backgroundLayer;
            this.initCamera.cullingMask &= ~mask;

            // Store the original transformation so we can restore it later
            this.originalPosition = this.initCamera.transform.position;
            this.originalOrientation = this.initCamera.transform.rotation;
        }
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        this.gcHandle.Free();
    }

    private void OnEnable()
    {
        this.OnOrientationChange(
            VLDetectScreenChangeBehaviour.GetOrientation(this.gameObject));
        VLDetectScreenChangeBehaviour.OnOrientationChange += OnOrientationChange;

        VLWorkerBehaviour.OnExtrinsicData += OnExtrinsicData;
        VLWorkerBehaviour.OnIntrinsicData += OnIntrinsicData;
        VLWorkerBehaviour.OnTrackerInitializing += OnTrackerInitializing;
        VLWorkerBehaviour.OnTrackerInitialized += OnTrackerInitialized;
    }

    private void OnDisable()
    {
        VLWorkerBehaviour.OnTrackerInitialized -= OnTrackerInitialized;
        VLWorkerBehaviour.OnTrackerInitializing -= OnTrackerInitializing;
        VLWorkerBehaviour.OnIntrinsicData -= OnIntrinsicData;
        VLWorkerBehaviour.OnExtrinsicData -= OnExtrinsicData;

        VLDetectScreenChangeBehaviour.OnOrientationChange -= OnOrientationChange;
    }

    private void Update()
    {
        if (this.reset && this.ready)
        {
            this.initCamera.transform.position = this.originalPosition;
            this.initCamera.transform.rotation = this.originalOrientation;
            this.reset = false;
        }

        this.InitWorkerReference();
        if (!this.settingInitPose &&
            this.worker != null &&
            this.initCamera != null &&
            this.ready &&
            this.worker.IsRunning())
        {
            this.SetInitPose();
        }
    }
}

/**@}*/