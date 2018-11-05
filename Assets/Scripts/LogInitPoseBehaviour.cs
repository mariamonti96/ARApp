using UnityEngine;
using System;

/// <summary>
///  Logs the current transformation of the Camera component in VisionLib
///  coordinates.
/// </summary>
/// <remarks>
///  <para>
///   This is just an example to show you how to convert a Camera
///   transformation from Unity coordinates into VisionLib coordinates using
///   the VLUnityCameraHelper class.
///  </para>
/// </remarks>
[RequireComponent(typeof(Camera))]
public class LogInitPoseBehaviour : MonoBehaviour
{
    /// <summary>
    ///  Pretty-print the JSON output.
    /// </summary>
    public bool prettyPrint = true;

    private Camera cam;
    private Matrix4x4 rotCamera = VLUnityCameraHelper.rotationZ0;

    /// <summary>
    ///  Helper class for converting the initial pose into JSON.
    /// </summary>
    [Serializable]
    public class InitPose
    {
        public string type;
        public float[] t;
        public float[] q;
        public InitPose(float tx, float ty, float tz,
            float qx, float qy, float qz, float qw)
        {
            this.type = "visionlib";
            this.t = new float[3] { tx, ty, tz };
            this.q = new float[4] { qx, qy, qz, qw };
        }
    }

    /// <summary>
    ///  Returns the current transformation of the Camera component in
    ///  VisionLib coordinates as JSON string.
    /// </summary>
    /// <returns>
    ///  JSON string with initial pose in VisionLib coordinates.
    /// </returns>
    public string GetInitPoseJson()
    {
        // Get the VisionLib transformation from the camera component
        Vector4 t;
        Quaternion q;
        VLUnityCameraHelper.CameraToVLPose(
            this.cam, this.rotCamera, out t, out q);

        // Convert the transformation into JSON
        InitPose param = new InitPose(t.x, t.y, t.z, q.x, q.y, q.z, q.w);
        return VLJsonUtility.ToJson(param, this.prettyPrint);
    }

    /// <summary>
    ///  Logs the current transformation of the Camera component in VisionLib
    ///  coordinates.
    /// </summary>
    public void DoLog()
    {
        string initPoseJson = this.GetInitPoseJson();
        Debug.Log(initPoseJson);
    }

    private void OnOrientationChange(ScreenOrientation orientation)
    {
        if (orientation == ScreenOrientation.Portrait)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ0;
        }
        else if (orientation == ScreenOrientation.PortraitUpsideDown)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ180;
        }
        else if (orientation == ScreenOrientation.LandscapeLeft)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ270;
        }
        else if (orientation == ScreenOrientation.LandscapeRight)
        {
            this.rotCamera = VLUnityCameraHelper.rotationZ90;
        }
    }

    private void Awake()
    {
        this.cam = this.GetComponent<Camera>();
    }

    private void OnEnable()
    {
        this.OnOrientationChange(
            VLDetectScreenChangeBehaviour.GetOrientation(this.gameObject));
        VLDetectScreenChangeBehaviour.OnOrientationChange += OnOrientationChange;
    }

    private void OnDisable()
    {
        VLDetectScreenChangeBehaviour.OnOrientationChange -= OnOrientationChange;
    }
}