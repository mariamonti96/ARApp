/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;

/// <summary>
///  The VLDebugCameraBehaviour can be used to visualize arbitrary tracking
///  poses from the tracking system.
/// </summary>
/// <remarks>
///  <para>
///   Right now you must know the internal name of the camera pose. Therefore
///   this behaviour is primarily a debugging tool for vlUnitySDK developers.
///  </para>
///  <para>
///   This behaviour can either be used by replacing the
///   VLTrackingCameraBehaviour of the VLCamera GameObject with the
///   VLDebugCameraBehaviour or by using the VLDebugCamera prefab and disabling
///   the Camera and the VLTrackingCamera behaviour of the VLCamera object.
///  </para>
/// </remarks>
[AddComponentMenu("VisionLib/VL Debug Camera Behaviour")]
public class VLDebugCameraBehaviour : VLWorkerReferenceBehaviour
{
    /// <summary>
    ///  Used extrinsic data key.
    /// </summary>
    public string extrinsicDataId = "VisualizationExtrinsicData";

    /// <summary>
    ///  Used intrinsic data key.
    /// </summary>>
    public string intrinsicDataId = "IntrinsicData";

    /// <summary>
    ///  Layer with the camera image background.
    /// </summary>
    /// <remarks>
    ///  This layer will not be rendered by the tracking camera.
    /// </remarks>
    public int backgroundLayer = 8;

    /// <summary>
    ///  Target camera which will be moved.
    /// </summary>
    /// <remarks>
    ///  If this is not defined, then the Camera behaviour attached to the
    ///  current GameObject will be used automatically.
    /// </remarks>
    public Camera trackingCamera;

    private Matrix4x4 rotCamera = VLUnityCameraHelper.rotationZ0;
    private ScreenOrientation screenOrientation = ScreenOrientation.Portrait;

    private float[] modelViewMatrixArray = new float[16];
    private Matrix4x4 modelViewMatrix = new Matrix4x4();
    private float[] projectionMatrixArray = new float[16];
    private Matrix4x4 projectionMatrix = new Matrix4x4();

    private string internalExtrinsicDataId;
    private bool subscribedToExtrinsicData;

    private string internalIntrinsicDataId;
    private bool subscribedToIntrinsicData;

    private GCHandle gcHandle;

    private bool ExtrinsicDataKeyChanged()
    {
        return this.extrinsicDataId != this.internalExtrinsicDataId;
    }

    private string GetExtrinsicDataKey()
    {
        return this.extrinsicDataId;
    }

    private string GetInternalExtrinsicDataKey()
    {
        return this.internalExtrinsicDataId;
    }

    private bool IntrinsicDataKeyChanged()
    {
        return this.intrinsicDataId != this.internalIntrinsicDataId;
    }

    private string GetIntrinsicDataKey()
    {
        return this.intrinsicDataId;
    }

    private string GetInternalIntrinsicDataKey()
    {
        return this.internalIntrinsicDataId;
    }

    // Dispatch extrinsic data event to object instance
    [MonoPInvokeCallback(typeof(VLWorker.ExtrinsicDataWrapperCallback))]
    private static void DispatchNamedExtrinsicDataEvent(IntPtr handle,
        IntPtr clientData)
    {
        try
        {
            VLExtrinsicDataWrapper extrinsicData = new VLExtrinsicDataWrapper(
                handle, false);
            GCHandle gcHandle = GCHandle.FromIntPtr(clientData);
            VLDebugCameraBehaviour debugCameraBehaviour =
                (VLDebugCameraBehaviour)gcHandle.Target;
            debugCameraBehaviour.OnExtrinsicData(extrinsicData);
            extrinsicData.Dispose();
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.ExtrinsicDataWrapperCallback dispatchNamedExtrinsicDataEventDelegate =
        new VLWorker.ExtrinsicDataWrapperCallback(DispatchNamedExtrinsicDataEvent);

    // Dispatch intrinsic data event to object instance
    [MonoPInvokeCallback(typeof(VLWorker.IntrinsicDataWrapperCallback))]
    private static void DispatchNamedIntrinsicDataEvent(IntPtr handle,
        IntPtr clientData)
    {
        try
        {
            VLIntrinsicDataWrapper intrinsicData = new VLIntrinsicDataWrapper(
                handle, false);
            GCHandle gcHandle = GCHandle.FromIntPtr(clientData);
            VLDebugCameraBehaviour debugCameraBehaviour =
                (VLDebugCameraBehaviour)gcHandle.Target;
            debugCameraBehaviour.OnIntrinsicData(intrinsicData);
            intrinsicData.Dispose();
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.IntrinsicDataWrapperCallback dispatchNamedIntrinsicDataEventDelegate =
        new VLWorker.IntrinsicDataWrapperCallback(DispatchNamedIntrinsicDataEvent);

    private void OnExtrinsicData(VLExtrinsicDataWrapper extrinsicData)
    {
        if ( this.trackingCamera == null)
        {
            return;
        }

        if (!extrinsicData.GetModelViewMatrix(this.modelViewMatrixArray))
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
        this.trackingCamera.worldToCameraMatrix =
            this.rotCamera * this.modelViewMatrix;
        */

        // Compute the right-handed world to camera matrix
        Matrix4x4 worldToCameraMatrix = this.rotCamera * this.modelViewMatrix;

        // Compute the left-handed world to camera matrix
        worldToCameraMatrix = VLUnityCameraHelper.flipZ * worldToCameraMatrix * VLUnityCameraHelper.flipZ;

        // Compute the left-handed camera to world matrix
        Matrix4x4 cameraToWorldMatrix = worldToCameraMatrix.inverse;

        // Extract the rotation and translation from the computed matrix
        this.trackingCamera.transform.rotation =
            Quaternion.LookRotation(
                cameraToWorldMatrix.GetColumn(2),
                cameraToWorldMatrix.GetColumn(1));
        this.trackingCamera.transform.position =
            cameraToWorldMatrix.GetColumn(3);

        // TODO(mbuchner): Compute camera orientation in native code
    }

    private void OnIntrinsicData(VLIntrinsicDataWrapper intrinsicData)
    {
        if ( this.trackingCamera == null)
        {
            return;
        }

        // Apply the intrinsic camera parameters
        if (intrinsicData.GetProjectionMatrix(
            this.trackingCamera.nearClipPlane,
            this.trackingCamera.farClipPlane,
            Screen.width, Screen.height, this.screenOrientation, 0,
            projectionMatrixArray))
        {
            for (int i=0; i < 16; ++i)
            {
                projectionMatrix[i % 4, i / 4] =
                    projectionMatrixArray[i];
            }
            this.trackingCamera.projectionMatrix = projectionMatrix;
        }
    }

    private void OnOrientationChange(ScreenOrientation orientation)
    {
        this.screenOrientation = orientation;
        if (orientation == VLUnityCameraHelper.ScreenOrientation0)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ0;
        }
        else if (orientation == VLUnityCameraHelper.ScreenOrientation180)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ180;
        }
        else if (orientation == VLUnityCameraHelper.ScreenOrientation90CCW)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ90;
        }
        else if (orientation == VLUnityCameraHelper.ScreenOrientation90CW)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ270;
        }
    }

    private void Awake()
    {
        if (this.trackingCamera == null)
        {
            this.trackingCamera = this.GetComponent<Camera>();
        }

        if (this.trackingCamera != null)
        {
            // Don't clear the background image
            this.trackingCamera.clearFlags = CameraClearFlags.Depth;

            // Render after the background camera
            this.trackingCamera.depth = 2;

            // Don't render the background image
            int mask = 1 << this.backgroundLayer;
            this.trackingCamera.cullingMask &= ~mask;
        }
        else
        {
            Debug.LogWarning("[vlUnitySDK] VLDebugCameraBehaviour GameObject must have a Camera behaviour attached or 'trackingCamera' must be set");
        }
    }

    private void SubscribeToExtrinsicData()
    {
        if (this.worker != null)
        {
            if (this.worker.AddNamedExtrinsicDataListener(
                this.GetExtrinsicDataKey(),
                dispatchNamedExtrinsicDataEventDelegate,
                GCHandle.ToIntPtr(this.gcHandle)))
            {
                this.internalExtrinsicDataId = this.extrinsicDataId;
                this.subscribedToExtrinsicData = true;
            }
            else
            {
                Debug.LogWarning("[vlUnitySDK] Failed to add named extrinsic data listener");
            }
        }
    }

    private void UnsubscribeFromExtrinsicData()
    {
        if (this.worker != null &&
            !this.worker.GetDisposed() &&
            this.subscribedToExtrinsicData)
        {
            string key = this.GetInternalExtrinsicDataKey();
            if (!this.worker.RemoveNamedExtrinsicDataListener(key,
                dispatchNamedExtrinsicDataEventDelegate,
                GCHandle.ToIntPtr(this.gcHandle)))
            {
                Debug.LogWarning("[vlUnitySDK] Failed to remove named extrinsic data listener");
            }
            this.subscribedToExtrinsicData = false;
        }
    }

    private void SubscribeToIntrinsicData()
    {
        if (this.worker != null)
        {
            if (this.worker.AddNamedIntrinsicDataListener(
                this.GetIntrinsicDataKey(),
                dispatchNamedIntrinsicDataEventDelegate,
                GCHandle.ToIntPtr(this.gcHandle)))
            {
                this.internalIntrinsicDataId = this.intrinsicDataId;
                this.subscribedToIntrinsicData = true;
            }
            else
            {
                Debug.LogWarning("[vlUnitySDK] Failed to add named intrinsic data listener");
            }
        }
    }

    private void UnsubscribeFromIntrinsicData()
    {
        if (this.worker != null &&
            !this.worker.GetDisposed() &&
            this.subscribedToIntrinsicData)
        {
            string key = this.GetInternalIntrinsicDataKey();
            if (!this.worker.RemoveNamedIntrinsicDataListener(key,
                dispatchNamedIntrinsicDataEventDelegate,
                GCHandle.ToIntPtr(this.gcHandle)))
            {
                Debug.LogWarning("[vlUnitySDK] Failed to remove named intrinsic data listener");
            }
            this.subscribedToIntrinsicData = false;
        }
    }

    private void OnEnable()
    {
        // Get a handle to the current object and make sure, that the object
        // doesn't get deleted by the garbage collector. We then use this
        // handle as client data for the native callbacks. This allows us to
        // retrieve the current address of the actual object during the
        // callback execution. GCHandleType.Pinned is not necessary, because we
        // are accessing the address only through the handle object, which gets
        // stored in a global handle table.
        this.gcHandle = GCHandle.Alloc(this);

        this.OnOrientationChange(
            VLDetectScreenChangeBehaviour.GetOrientation(this.gameObject));
        VLDetectScreenChangeBehaviour.OnOrientationChange += OnOrientationChange;

        // Get the worker from the object with the WorkerBehaviour component
        this.InitWorkerReference();

        this.SubscribeToExtrinsicData();
        this.SubscribeToIntrinsicData();
    }

    private void OnDisable()
    {
        this.UnsubscribeFromIntrinsicData();
        this.UnsubscribeFromExtrinsicData();

        VLDetectScreenChangeBehaviour.OnOrientationChange -= OnOrientationChange;

        // Release the handle to the current object
        this.gcHandle.Free();
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
    }

    private void Update()
    {
        if (this.InitWorkerReference())
        {
            if (this.ExtrinsicDataKeyChanged())
            {
                this.UnsubscribeFromExtrinsicData();
                this.SubscribeToExtrinsicData();
            }
            if (this.IntrinsicDataKeyChanged())
            {
                this.UnsubscribeFromIntrinsicData();
                this.SubscribeToIntrinsicData();
            }
        }
    }
}

/**@}*/