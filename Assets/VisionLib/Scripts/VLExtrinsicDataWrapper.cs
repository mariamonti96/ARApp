/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
///  The VLExtrinsicDataWrapper is a wrapper for an ExtrinsicData object.
///  ExtrinsicData objects represent the extrinsic camera parameters
///  (position and orientation).
/// </summary>
/// <seealso cref="VLDataSetWrapper.GetExtrinsicData"/>
public class VLExtrinsicDataWrapper: IDisposable
{
    private IntPtr handle;
    private bool disposed = false;
    private bool owner;

    /// <summary>
    ///  Constructor of VLExtrinsicDataWrapper.
    /// </summary>
    /// <remarks>
    ///  Don't call this constructor directly. Use the
    ///  <see cref="VLAbstractApplicationWrapper.GetDataSet"/> and
    ///  <see cref="VLDataSetWrapper.GetIntrinsicData"/> methods instead.
    /// </remarks>
    /// <param name="handle">
    ///  Handle to the native object.
    /// </param>
    /// <param name="owner">
    ///  <c>true</c>, if the VLExtrinsicDataWrapper is the owner of the native
    ///  object; <c>false</c>, otherwise.
    /// </param>
    public VLExtrinsicDataWrapper(IntPtr handle, bool owner)
    {
        this.handle = handle;
        this.owner = owner;
    }

    ~VLExtrinsicDataWrapper()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_ExtrinsicDataWrapper(
        IntPtr extrinsicDataWrapper);
    private void Dispose(bool disposing)
    {
        // Prevent multiple calls to Dispose
        if (this.disposed)
        {
            return;
        }

        // Was dispose called explicitly by the user?
        if (disposing)
        {
            // Dispose managed resources (those that implement IDisposable)
        }

        // Clean up unmanaged resources
        if (this.owner)
        {
            vlDelete_ExtrinsicDataWrapper(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>
    ///  Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLExtrinsicDataWrapper"/>. The  <see cref="Dispose"/> method
    ///  leaves the <see cref="VLExtrinsicDataWrapper"/> in an unusable state.
    ///  After calling <see cref="Dispose"/>, you must release all references to
    ///  the <see cref="VLExtrinsicDataWrapper"/> so the garbage collector can
    ///  reclaim the memory that the <see cref="VLExtrinsicDataWrapper"/> was
    ///  occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlExtrinsicDataWrapper_GetValid(
        IntPtr extrinsicDataWrapper);
    /// <summary>
    ///  Returns whether the current tracking pose is valid (the tracking was
    ///  successful).
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the current tracking pose is valid;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool GetValid()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        return vlExtrinsicDataWrapper_GetValid(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlExtrinsicDataWrapper_GetModelViewMatrix(
        IntPtr extrinsicDataWrapper, IntPtr matrix, System.UInt32 matrixElementCount);
    /// <summary>
    ///  Returns the current camera pose as model-view matrix.
    /// </summary>
    /// <remarks>
    ///  The returned matrix assumes a right-handed coordinate system and is
    ///  stored in the following order (column-major order):
    ///  \f[
    ///   \begin{bmatrix}
    ///    0 & 4 &  8 & 12\\
    ///    1 & 5 &  9 & 13\\
    ///    2 & 6 & 10 & 14\\
    ///    3 & 7 & 11 & 15\\
    ///   \end{bmatrix}
    ///  \f]
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the model view matrix was gotten successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="matrix">
    ///  Float array with 16 elements for storing the model-view matrix.
    /// </param>
    public bool GetModelViewMatrix(float[] matrix)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        bool result = false;
        GCHandle matrixHandle = GCHandle.Alloc(matrix, GCHandleType.Pinned);
        try
        {
            result = vlExtrinsicDataWrapper_GetModelViewMatrix(
                this.handle,
                matrixHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(matrix.Length)
            );
        }
        finally
        {
            matrixHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlExtrinsicDataWrapper_GetT(
        IntPtr extrinsicDataWrapper, IntPtr t, System.UInt32 elementCount);
    /// <summary>
    ///  Returns the translation \f$t\f$ from the world coordinate system to
    ///  the camera coordinate system.
    /// </summary>
    /// <remarks>
    ///  Please notice, that \f$(R,t)\f$ represents the transformation of a
    ///  3D point \f$P_w\f$ from world coordinates into a 3D point \f$P_c\f$
    ///  in camera coordinates: \f$P_c = RP_w + t\f$.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the translation was acquired successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="t">
    ///  Float array with 3 elements \f$(x,y,z)\f$ for storing the translation.
    /// </param>
    public bool GetT(float[] t)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        bool result = false;
        GCHandle vectorHandle = GCHandle.Alloc(t, GCHandleType.Pinned);
        try
        {
            result = vlExtrinsicDataWrapper_GetT(
                this.handle,
                vectorHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(t.Length)
            );
        }
        finally
        {
            vectorHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlExtrinsicDataWrapper_SetT(
        IntPtr extrinsicDataWrapper, IntPtr t, System.UInt32 elementCount);
    /// <summary>
    ///  Sets the translation \f$t\f$ from the world coordinate system to
    ///  the camera coordinate system.
    /// </summary>
    /// <remarks>
    ///  Please notice, that \f$(R,t)\f$ represents the transformation of a
    ///  3D point \f$P_w\f$ from world coordinates into a 3D point \f$P_c\f$
    ///  in camera coordinates: \f$P_c = RP_w + t\f$.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the translation was set successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="t">
    ///  Float array with 3 elements \f$(x,y,z)\f$, which contain the
    ///  translation.
    /// </param>
    public bool SetT(float[] t)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        bool result = false;
        GCHandle vectorHandle = GCHandle.Alloc(t, GCHandleType.Pinned);
        try
        {
            result = vlExtrinsicDataWrapper_SetT(
                this.handle,
                vectorHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(t.Length)
            );
        }
        finally
        {
            vectorHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlExtrinsicDataWrapper_GetR(
        IntPtr extrinsicDataWrapper, IntPtr q, System.UInt32 elementCount);
    /// <summary>
    ///  Returns the rotation \f$R\f$ from the world coordinate system to the
    ///  camera coordinate system.
    /// </summary>
    /// <remarks>
    ///  Please notice, that \f$(R,t)\f$ represents the transformation of a
    ///  3D point \f$P_w\f$ from world coordinates into a 3D point \f$P_c\f$
    ///  in camera coordinates: \f$P_c = RP_w + t\f$.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the rotation was acquired successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="q">
    ///  Float array with 4 elements \f$(x,y,z,w)\f$ for storing the rotation
    ///  as quaternion.
    /// </param>
    public bool GetR(float[] q)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        bool result = false;
        GCHandle quaternionHandle = GCHandle.Alloc(q, GCHandleType.Pinned);
        try
        {
            result = vlExtrinsicDataWrapper_GetR(
                this.handle,
                quaternionHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(q.Length)
            );
        }
        finally
        {
            quaternionHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlExtrinsicDataWrapper_SetR(
        IntPtr extrinsicDataWrapper, IntPtr q, System.UInt32 elementCount);
    /// <summary>
    ///  Sets the rotation \f$R\f$ from the world coordinate system to the
    ///  camera coordinate system.
    /// </summary>
    /// <remarks>
    ///  Please notice, that \f$(R,t)\f$ represents the transformation of a
    ///  3D point \f$P_w\f$ from world coordinates into a 3D point \f$P_c\f$
    ///  in camera coordinates: \f$P_c = RP_w + t\f$.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the rotation was set successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="q">
    ///  Float array with 4 elements \f$(x,y,z,w)\f$, which contains the
    ///  rotation as quaternion.
    /// </param>
    public bool SetR(float[] q)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        bool result = false;
        GCHandle quaternionHandle = GCHandle.Alloc(q, GCHandleType.Pinned);
        try
        {
            result = vlExtrinsicDataWrapper_SetR(
                this.handle,
                quaternionHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(q.Length)
            );
        }
        finally
        {
            quaternionHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlExtrinsicDataWrapper_GetCamPosWorld(
        IntPtr extrinsicDataWrapper, IntPtr t, System.UInt32 elementCount);
    /// <summary>
    ///  Returns the position \f$P_{cam}\f$ of the camera in world coordinates.
    /// </summary>
    /// <remarks>
    ///  Internally the position \f$P_{cam}\f$ will be computed from the
    ///  transformation \f$(R,t)\f$ which transforms a 3D point from world
    ///  coordinates into camera coordinates (\f$P_{cam} = -R^{-1}t\f$).
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the position was acquired successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="pos">
    ///  Float array with 3 elements f$(x,y,z)\f$ for storing the position.
    /// </param>
    public bool GetCamPosWorld(float[] pos)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        bool result = false;
        GCHandle vectorHandle = GCHandle.Alloc(pos, GCHandleType.Pinned);
        try
        {
            result = vlExtrinsicDataWrapper_GetCamPosWorld(
                this.handle,
                vectorHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(pos.Length)
            );
        }
        finally
        {
            vectorHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlExtrinsicDataWrapper_SetCamPosWorld(
        IntPtr extrinsicDataWrapper, IntPtr t, System.UInt32 elementCount);
    /// <summary>
    ///  Sets the position \f$P_{cam}\f$ of the camera in world coordinates.
    /// </summary>
    /// <remarks>
    ///  Internally this will be stored as a transformation \f$(R,t)\f$ of a 3D
    ///  point from world coordinates into camera coordinates
    ///  (\f$t = -RP_{cam}\f$).
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the position was acquired successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="pos">
    ///  Float array with 3 elements \f$(x,y,z)\f$, which contains the
    ///  position.
    /// </param>
    public bool SetCamPosWorld(float[] pos)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        bool result = false;
        GCHandle vectorHandle = GCHandle.Alloc(pos, GCHandleType.Pinned);
        try
        {
            result = vlExtrinsicDataWrapper_SetCamPosWorld(
                this.handle,
                vectorHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(pos.Length)
            );
        }
        finally
        {
            vectorHandle.Free();
        }

        return result;
    }

    public bool SetFromCamera(Camera camera)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLExtrinsicDataWrapper");
        }

        // TODO(mbuchner): Implement more of this in native code

        bool result = true;

        Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
        // Convert from left-handed to right-handed model-view matrix
        worldToCameraMatrix[0, 2] = -worldToCameraMatrix[0, 2];
        worldToCameraMatrix[1, 2] = -worldToCameraMatrix[1, 2];
        worldToCameraMatrix[2, 2] = -worldToCameraMatrix[2, 2];
        // Convert from OpenGL coordinates into VisionLib coordinates
        worldToCameraMatrix = VLUnityCameraHelper.flipYZ * worldToCameraMatrix;

        // Position

        Vector4 t = worldToCameraMatrix.GetColumn(3);
        float[] tData = new float[3];
        tData[0] = t.x;
        tData[1] = t.y;
        tData[2] = t.z;
        if (!this.SetT(tData))
        {
            result = false;
        }

        // Orientation

        Quaternion q = VLUnityCameraHelper.QuaternionFromMatrix(
            worldToCameraMatrix);
        float[] qData = new float[4];
        qData[0] = q.x;
        qData[1] = q.y;
        qData[2] = q.z;
        qData[3] = q.w;
        if (!this.SetR(qData))
        {
            result = false;
        }

        return result;
    }
}

/**@}*/