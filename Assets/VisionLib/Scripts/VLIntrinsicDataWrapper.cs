/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
///  The VLIntrinsicDataWrapper is a wrapper for an IntrinsicData object.
///  IntrinsicData objects represent the intrinsic camera parameters
///  (focal length, principal point, skew and distortion parameters).
/// </summary>
/// <seealso cref="VLDataSetWrapper.GetIntrinsicData"/>
public class VLIntrinsicDataWrapper: IDisposable
{
    private IntPtr handle;
    private bool disposed = false;
    private bool owner;

    /// <summary>
    ///  Constructor of VLIntrinsicDataWrapper.
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
    ///  <c>true</c>, if the VLIntrinsicDataWrapper is the owner of the native
    ///  object; <c>false</c>, otherwise.
    /// </param>
    public VLIntrinsicDataWrapper(IntPtr handle, bool owner)
    {
        this.handle = handle;
        this.owner = owner;
    }

    ~VLIntrinsicDataWrapper()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_IntrinsicDataWrapper(
        IntPtr intrinsicDataPerspectiveBaseWrapper);
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
            vlDelete_IntrinsicDataWrapper(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>
    ///  Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLIntrinsicDataWrapper"/>. The <see cref="Dispose"/> method
    ///  leaves the <see cref="VLIntrinsicDataWrapper"/> in an unusable state.
    ///  After calling <see cref="Dispose"/>, you must release all references
    ///  to the <see cref="VLIntrinsicDataWrapper"/> so the garbage collector
    ///  can reclaim the memory that the <see cref="VLIntrinsicDataWrapper"/>
    ///  was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.UInt32 vlIntrinsicDataWrapper_GetWidth(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns the width of the intrinsic camera calibration.
    /// </summary>
    /// <returns>
    ///  The width in pixels.
    /// </returns>
    public int GetWidth()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return Convert.ToInt32(vlIntrinsicDataWrapper_GetWidth(this.handle));
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.UInt32 vlIntrinsicDataWrapper_GetHeight(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns the height of the intrinsic camera calibration.
    /// </summary>
    /// <returns>
    ///  The height in pixels.
    /// </returns>
    public int GetHeight()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return Convert.ToInt32(vlIntrinsicDataWrapper_GetHeight(this.handle));
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.Double vlIntrinsicDataWrapper_GetFxNorm(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns the normalized focal length of the intrinsic camera
    ///  calibration in x direction.
    /// </summary>
    /// <remarks>
    ///  The focal length in x direction was normalized through a division by
    ///  the width of the camera calibration.
    /// </remarks>
    /// <returns>
    ///  Normalized focal length in x direction.
    /// </returns>
    public double GetFxNorm()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return vlIntrinsicDataWrapper_GetFxNorm(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.Double vlIntrinsicDataWrapper_GetFyNorm(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns the normalized focal length of the intrinsic camera
    ///  calibration in y direction.
    /// </summary>
    /// <remarks>
    ///  The focal length in y direction was normalized through a division by
    ///  the height of the camera calibration.
    /// </remarks>
    /// <returns>
    ///  Normalized focal length in y direction.
    /// </returns>
    public double GetFyNorm()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return vlIntrinsicDataWrapper_GetFyNorm(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.Double vlIntrinsicDataWrapper_GetSkewNorm(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns the normalized skew of the intrinsic camera calibration.
    /// </summary>
    /// <remarks>
    ///  The skew was normalized through a division by the width of the
    ///  camera calibration.
    /// </remarks>
    /// <returns>
    ///  Normalized skew.
    /// </returns>
    public double GetSkewNorm()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return vlIntrinsicDataWrapper_GetSkewNorm(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.Double vlIntrinsicDataWrapper_GetCxNorm(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns the normalized x-component of the principal point.
    /// </summary>
    /// <remarks>
    ///  The x-component was normalized through a division by the width of the
    ///  camera calibration.
    /// </remarks>
    /// <returns>
    ///  Normalized x-component of the principal point.
    /// </returns>
    public double GetCxNorm()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return vlIntrinsicDataWrapper_GetCxNorm(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.Double vlIntrinsicDataWrapper_GetCyNorm(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns the normalized y-component of the principal point.
    /// </summary>
    /// <remarks>
    ///  The y-component was normalized through a division by the height of the
    ///  camera calibration.
    /// </remarks>
    /// <returns>
    ///  Normalized y-component of the principal point.
    /// </returns>
    public double GetCyNorm()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return vlIntrinsicDataWrapper_GetCyNorm(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlIntrinsicDataWrapper_GetCalibrated(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns whether the intrinsic parameters are valid.
    /// </summary>
    /// <remarks>
    /// A intrinsic camera calibration used for tracking should always be valid.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the intrinsic calibration is valid;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool GetCalibrated()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return vlIntrinsicDataWrapper_GetCalibrated(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.Double vlIntrinsicDataWrapper_GetCalibrationError(
        IntPtr intrinsicDataWrapper);
    /// <summary>
    ///  Returns the calibration error.
    /// </summary>
    /// <remarks>
    ///  The reprojection error in pixel. This is interesting for evaluating
    ///  the quality of a camera calibration.
    /// </remarks>
    /// <returns>
    ///  NormalizedThe reprojection error in pixel.
    /// </returns>
    public double GetCalibrationError()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        return vlIntrinsicDataWrapper_GetCalibrationError(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlIntrinsicDataWrapper_GetRadialDistortion(
        IntPtr intrinsicDataWrapper, IntPtr k, System.UInt32 elementCount);
    /// <summary>
    ///  Retrieves the radial distortion parameters.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, on success; <c>false</c> otherwise.
    /// </returns>
    /// <param name="k">
    ///  Double array with 5 elements for storing the distortion parameters.
    /// </param>
    public bool GetRadialDistortion(double[] k)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        bool result = false;
        GCHandle arrayHandle = GCHandle.Alloc(k, GCHandleType.Pinned);
        try
        {
            result = vlIntrinsicDataWrapper_GetRadialDistortion(
                this.handle,
                arrayHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(k.Length)
            );
        }
        finally
        {
            arrayHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlIntrinsicDataWrapper_SetRadialDistortion(
        IntPtr extrinsicDataWrapper, IntPtr k, System.UInt32 elementCount);
    /// <summary>
    ///  Sets the radial distortion parameters.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, on success; <c>false</c> otherwise.
    /// </returns>
    /// <param name="t">
    ///  Double array with 5 elements, which contains the distortion
    ///  parameters.
    /// </param>
    public bool SetRadialDistortion(double[] k)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        bool result = false;
        GCHandle arrayHandle = GCHandle.Alloc(k, GCHandleType.Pinned);
        try
        {
            result = vlIntrinsicDataWrapper_SetRadialDistortion(
                this.handle,
                arrayHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(k.Length)
            );
        }
        finally
        {
            arrayHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlIntrinsicDataWrapper_GetProjectionMatrix(
        IntPtr intrinsicDataPerspectiveBaseWrapper, float nearFact,
        float farFact, System.UInt32 screenWidth, System.UInt32 screenHeight,
        System.UInt32 screenOrientation, System.UInt32 mode, IntPtr matrix,
        System.UInt32 matrixElementCount);
    /// <summary>
    ///  Computed the projection matrix from the intrinsic camera parameters.
    /// </summary>
    /// <remarks>
    ///  The returned matrix is stored in the following order
    ///  (column-major order):
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
    ///  <c>true</c>, if the projection matrix was gotten successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="nearFact">
    ///  Value for the near clipping plane.
    /// </param>
    /// <param name="farFact">
    ///  Value for the far clipping plane.
    /// </param>
    /// <param name="screenWidth">
    ///  Width of the screen.
    /// </param>
    /// <param name="screenHeight">
    ///  Height of the screen.
    /// </param>
    /// <param name="screenOrientation">
    ///  Orientation of the screen. We assume, that the camera calibration was
    ///  done with images in portrait mode. Therefore usually
    ///  ScreenOrientation.Portrait should be used for screen which can't be
    ///  rotated.
    /// </param>
    /// <param name="mode">
    ///  The mode defines how to handle mismatching aspect ratios. Right now
    ///  the mode value is ignored, but later we will support different modes
    ///  like 'cover' (scale the projection surface up until it covers the
    ///  whole screen) and 'contain' (scale the projection surface down until
    ///  it is completely contained inside the screen).
    /// </param>
    /// <param name="matrix">
    ///  Float array with 16 elements for storing the projection matrix.
    /// </param>
    public bool GetProjectionMatrix(float nearFact,
                                    float farFact,
                                    int screenWidth,
                                    int screenHeight,
                                    ScreenOrientation screenOrientation,
                                    int mode,
                                    float[] matrix)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLIntrinsicDataWrapper");
        }

        bool result = false;
        GCHandle matrixHandle = GCHandle.Alloc(matrix, GCHandleType.Pinned);
        try
        {
            uint orientation;
            if (screenOrientation == VLUnityCameraHelper.ScreenOrientation0)
            {
                orientation = 0;
            }
            else if (screenOrientation == VLUnityCameraHelper.ScreenOrientation180)
            {
                orientation = 1;
            }
            else if (screenOrientation == VLUnityCameraHelper.ScreenOrientation90CCW)
            {
                orientation = 2;
            }
            else if (screenOrientation == VLUnityCameraHelper.ScreenOrientation90CW)
            {
                orientation = 3;
            } else
            {
                Debug.LogWarning("[vlUnitySDK] Unsupported screen orientation");
                orientation = 0;
            }

            result = vlIntrinsicDataWrapper_GetProjectionMatrix(
                this.handle,
                nearFact,
                farFact,
                Convert.ToUInt32(screenWidth),
                Convert.ToUInt32(screenHeight),
                orientation,
                Convert.ToUInt32(mode),
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
}

/**@}*/