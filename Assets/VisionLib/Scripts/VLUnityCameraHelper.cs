/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;

/// <summary>
///  Static class with helper functions and constants for doing camera
///  transformations.
/// </summary>
public static class VLUnityCameraHelper
{
    /// <summary>
    ///  Transformation matrix with a 0 degree rotation around the z-axis
    ///  (identity matrix).
    /// </summary>
    public static readonly Matrix4x4 rotationZ0 = Matrix4x4.identity;
    /// <summary>
    ///  Transformation matrix with a 90 degree rotation around the z-axis.
    /// </summary>
    public static readonly Matrix4x4 rotationZ90 = Matrix4x4.TRS(Vector3.zero,
        Quaternion.AngleAxis(90.0f, Vector3.forward), Vector3.one);
    /// <summary>
    ///  Transformation matrix with a 180 degree rotation around the z-axis.
    /// </summary>
    public static readonly Matrix4x4 rotationZ180 = Matrix4x4.TRS(Vector3.zero,
        Quaternion.AngleAxis(180.0f, Vector3.forward), Vector3.one);
    /// <summary>
    ///  Transformation matrix with a 270 degree rotation around the z-axis.
    /// </summary>
    public static readonly Matrix4x4 rotationZ270 = Matrix4x4.TRS(Vector3.zero,
        Quaternion.AngleAxis(270.0f, Vector3.forward), Vector3.one);

    public static readonly Matrix4x4 flipZ = Matrix4x4.Scale(
        new Vector3(1, 1, -1));

    public static readonly Matrix4x4 flipYZ = Matrix4x4.Scale(
        new Vector3(1, -1, -1));

    /// <summary>
    ///  Extracts the rotation from a 4x4 transformation matrix as Quaternion.
    /// </summary>
    /// <returns>
    ///  Quaternion with rotation extracted from given matrix.
    /// </returns>
    /// <param name="m">
    ///  4x4 transformation matrix.
    /// </param>
    public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
        // Source: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.html
        Quaternion q = new Quaternion();
        float trace = m[0, 0] + m[1, 1] + m[2, 2];
        if (trace > 0.0f)
        {
            float s = 0.5f / Mathf.Sqrt(trace + 1.0f);
            q.w = 0.25f / s;
            q.x = (m[2, 1] - m[1, 2]) * s;
            q.y = (m[0, 2] - m[2, 0]) * s;
            q.z = (m[1, 0] - m[0, 1]) * s;
        }
        else
        {
            if (m[0, 0] > m[1, 1] && m[0, 0] > m[2, 2])
            {
                float s = 2.0f * Mathf.Sqrt(1.0f + m[0, 0] - m[1, 1] - m[2, 2]);
                q.w = (m[2, 1] - m[1, 2]) / s;
                q.x = 0.25f * s;
                q.y = (m[0, 1] + m[1, 0]) / s;
                q.z = (m[0, 2] + m[2, 0]) / s;
            }
            else if (m[1, 1] > m[2, 2])
            {
                float s = 2.0f * Mathf.Sqrt(1.0f + m[1, 1] - m[0, 0] - m[2, 2]);
                q.w = (m[0, 2] - m[2, 0]) / s;
                q.x = (m[0, 1] + m[1, 0]) / s;
                q.y = 0.25f * s;
                q.z = (m[1, 2] + m[2, 1]) / s;
            }
            else
            {
                float s = 2.0f * Mathf.Sqrt(1.0f + m[2, 2] - m[0, 0] - m[1, 1]);
                q.w = (m[1, 0] - m[0, 1]) / s;
                q.x = (m[0, 2] + m[2, 0]) / s;
                q.y = (m[1, 2] + m[2, 1]) / s;
                q.z = 0.25f * s;
            }
        }
        return q;
    }

    /// <summary>
    ///  Computes the pose in VisionLib coordinates from a Unity Camera object.
    /// </summary>
    /// <param name="camera">
    ///  Camera object which should be used for computing the pose.
    /// </param>
    /// <param name="offset">
    ///  Transformation which will be applied to the transformation of the
    ///  camera before computing the pose. This is useful in case the screen
    ///  orientation was changed. Unity will then automatically rotate the
    ///  scene, but the camera image will not be rotated. Therefore you need to
    ///  reverse the automatic rotation from Unity. You might want to use the
    ///  rotationZ0, rotationZ90, rotationZ180 and rotationZ270 constants as
    ///  values for this parameter.
    /// </param>
    /// <param name="t">
    ///  Translation in VisionLib coordinates.
    /// </param>
    /// <param name="q">
    ///  Rotation in VisionLib coordinates.
    /// </param>
    public static void CameraToVLPose(Camera camera, Matrix4x4 offset,
        out Vector4 t, out Quaternion q)
    {
        Matrix4x4 worldToCameraMatrix = offset * camera.worldToCameraMatrix;

        // Convert from left-handed to right-handed model-view matrix
        worldToCameraMatrix[0, 2] = -worldToCameraMatrix[0, 2];
        worldToCameraMatrix[1, 2] = -worldToCameraMatrix[1, 2];
        worldToCameraMatrix[2, 2] = -worldToCameraMatrix[2, 2];

        // Convert from OpenGL coordinates into VisionLib coordinates
        worldToCameraMatrix = VLUnityCameraHelper.flipYZ * worldToCameraMatrix;

        t = worldToCameraMatrix.GetColumn(3);
        q = QuaternionFromMatrix(worldToCameraMatrix);
    }

    /// <summary>
    ///  Computes the position and rotation of a Unity Camera object from a
    ///  VisionLib pose.
    /// </summary>
    /// <param name="t">
    ///  Translation in VisionLib coordinates.
    /// </param>
    /// <param name="q">
    ///  Rotation in VisionLib coordinates.
    /// </param>
    /// <param name="position">
    ///  Position in Unity coordinates.
    /// </param>
    /// <param name="orientation">
    ///  Rotation in Unity coordinates.
    /// </param>
    public static void VLPoseToCamera(Vector3 t, Quaternion q,
        out Vector3 position, out Quaternion orientation)
    {
        position = -(Quaternion.Inverse(q) * t);
        // Negate the z-component in order to convert the right-handed
        // translation into a left-handed translation
        position.z = -position.z;

        // Rotate 180 degree around the x-axis in order to convert
        // the rotation from VisionLib coordinates
        // (x: right, y: down, z: inside) to right-handed
        // coordinates (x: right, y: up: z: outside)
        //Quaternion rotX180 = Quaternion.AngleAxis(180, Vector3.right);
        //q = q * rotX180;
        q = new Quaternion(q.w, q.z, -q.y, -q.x);

        // Negate the x- and z-component in order to convert the
        // right-handed rotation into a left-handed rotation
        // (negating the y- and w-component would have the same
        // effect)
        q.x = -q.x;
        q.z = -q.z;

        // Invert the rotation, because we want the rotation of the camera in
        // the world and not the rotation of the world around the camera
        orientation = Quaternion.Inverse(q);
    }

#if (UNITY_WSA_10_0 && !UNITY_EDITOR)
    // On UWP the camera image arrives in landscape-left mode. We therefore
    // assume, that the landscape-left screen orientation corresponds to a
    // rotation of 0 degrees.
    public static readonly ScreenOrientation ScreenOrientation0 =
        ScreenOrientation.LandscapeLeft;
    public static readonly ScreenOrientation ScreenOrientation180 =
        ScreenOrientation.LandscapeRight;
    public static readonly ScreenOrientation ScreenOrientation90CW =
        ScreenOrientation.Portrait;
    public static readonly ScreenOrientation ScreenOrientation90CCW =
        ScreenOrientation.PortraitUpsideDown;
#else
    // On other platforms the image arrives in portrait mode or
    // (on desktop PCs) Unity tells us, that the current screen orientation
    // always is in portrait mode. We therefore assume, that the portrait
    // screen orientation corresponds to a rotation of 0 degrees.
    public static readonly ScreenOrientation ScreenOrientation0 =
        ScreenOrientation.Portrait;
    public static readonly ScreenOrientation ScreenOrientation180 =
        ScreenOrientation.PortraitUpsideDown;
    public static readonly ScreenOrientation ScreenOrientation90CW =
        ScreenOrientation.LandscapeRight;
    public static readonly ScreenOrientation ScreenOrientation90CCW =
        ScreenOrientation.LandscapeLeft;
#endif
}

/**@}*/