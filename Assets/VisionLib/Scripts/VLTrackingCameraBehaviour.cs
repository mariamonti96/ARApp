/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;
using System.Collections;

/// <summary>
///  Camera used for rendering the augmentation.
/// </summary>
[AddComponentMenu("VisionLib/VL Tracking Camera Behaviour")]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Transform))]
public class VLTrackingCameraBehaviour : MonoBehaviour
{
    /// <summary>
    ///  Layer with the camera image background.
    /// </summary>
    /// <remarks>
    ///  This layer will not be rendered by the tracking camera.
    /// </remarks>
    public int backgroundLayer = 8;

    private Camera trackingCamera;

    private Matrix4x4 rotCamera = VLUnityCameraHelper.rotationZ0;
    private ScreenOrientation screenOrientation = ScreenOrientation.Portrait;

    private float[] modelViewMatrixArray = new float[16];
    private Matrix4x4 modelViewMatrix = new Matrix4x4();
    private float[] projectionMatrixArray = new float[16];
    private Matrix4x4 projectionMatrix = new Matrix4x4();

    private void OnExtrinsicData(VLExtrinsicDataWrapper extrinsicData)
    {
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
        this.trackingCamera = this.GetComponent<Camera>();

        // Don't clear the background image
        this.trackingCamera.clearFlags = CameraClearFlags.Depth;

        // Render after the background camera
        this.trackingCamera.depth = 2;

        // Don't render the background image
        int mask = 1 << this.backgroundLayer;
        this.trackingCamera.cullingMask &= ~mask;
    }

    private void OnEnable()
    {
        this.OnOrientationChange(
            VLDetectScreenChangeBehaviour.GetOrientation(this.gameObject));
        VLDetectScreenChangeBehaviour.OnOrientationChange += OnOrientationChange;

        VLWorkerBehaviour.OnExtrinsicData += OnExtrinsicData;
        VLWorkerBehaviour.OnIntrinsicData += OnIntrinsicData;
    }

    private void OnDisable()
    {
        VLWorkerBehaviour.OnIntrinsicData -= OnIntrinsicData;
        VLWorkerBehaviour.OnExtrinsicData -= OnExtrinsicData;
        VLDetectScreenChangeBehaviour.OnOrientationChange -= OnOrientationChange;
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {

    }

    private void Update()
    {
    }
}

/**@}*/