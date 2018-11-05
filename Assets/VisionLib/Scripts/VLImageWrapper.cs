/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
///  The VLImageWrapper is a wrapper for an Image object.
/// </summary>
/// <seealso cref="VLDataSetWrapper.GetImage"/>
public class VLImageWrapper: IDisposable
{
    private IntPtr handle;
    private bool disposed = false;
    private bool owner;

    /// <summary>
    ///  Constructor of VLImageWrapper.
    /// </summary>
    /// <remarks>
    ///  Don't call this constructor directly. Use the
    ///  <see cref="VLAbstractApplicationWrapper.GetDataSet"/> and
    ///  <see cref="VLDataSetWrapper.GetImage"/> methods instead.
    /// </remarks>
    /// <param name="handle">
    ///  Handle to the native object.
    /// </param>
    /// <param name="owner">
    ///  <c>true</c>, if the VLImageWrapper is the owner of the native object;
    ///  <c>false</c>, otherwise.
    /// </param>
    public VLImageWrapper(IntPtr handle, bool owner)
    {
        this.handle = handle;
        this.owner = owner;
    }

    ~VLImageWrapper()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_ImageWrapper(
        IntPtr imageWrapper);
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
            vlDelete_ImageWrapper(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLImageWrapper"/>. The <see cref="Dispose"/> method leaves
    ///  the <see cref="VLImageWrapper"/> in an unusable state. After calling
    ///  <see cref="Dispose"/>, you must release all references to the
    ///  <see cref="VLImageWrapper"/> so the garbage collector can reclaim the
    ///  memory that the <see cref="VLImageWrapper"/> was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.UInt32 vlImageWrapper_GetFormat(
        IntPtr imageWrapper);
    /// <summary>
    ///  Returns an enumeration with the internal type of the image.
    /// </summary>
    /// <returns>
    ///  <see cref="VLUnitySdk.ImageFormat"/> enumeration with the internal type
    ///  of the image.
    /// </returns>
    public VLUnitySdk.ImageFormat GetFormat()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLImageWrapper");
        }

        return (VLUnitySdk.ImageFormat)vlImageWrapper_GetFormat(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.UInt32 vlImageWrapper_GetBytesPerPixel(
        IntPtr imageWrapper);
    /// <summary>
    ///  Returns the number of bytes per pixel.
    /// </summary>
    /// <returns>
    ///  The number of bytes per pixel.
    /// </returns>
    public int GetBytesPerPixel()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLImageWrapper");
        }

        return Convert.ToInt32(vlImageWrapper_GetBytesPerPixel(this.handle));
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.UInt32 vlImageWrapper_GetWidth(
        IntPtr imageWrapper);
    /// <summary>
    ///  Returns the width of the image.
    /// </summary>
    /// <returns>
    ///  The width in pixels.
    /// </returns>
    public int GetWidth()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLImageWrapper");
        }

        return Convert.ToInt32(vlImageWrapper_GetWidth(this.handle));
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern System.UInt32 vlImageWrapper_GetHeight(
        IntPtr imageWrapper);
    /// <summary>
    ///  Returns the height of the image.
    /// </summary>
    /// <returns>
    ///  The height in pixels.
    /// </returns>
    public int GetHeight()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLImageWrapper");
        }

        return Convert.ToInt32(vlImageWrapper_GetHeight(this.handle));
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlImageWrapper_CopyToBuffer(
        IntPtr imageWrapper, IntPtr buffer, System.UInt32 bufferSize);
    /// <summary>
    ///  Copies the VisionLib image into the given byte array.
    /// </summary>
    /// <remarks>
    ///  Please make sure, that the byte array is large enough for storing the
    ///  whole image date (width * height * bytesPerPixel). The number of bytes
    ///  per pixel an be acquired using the <see cref="GetBytesPerPixel"/>
    ///  function.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the data was copied to the byte array successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="buffer">
    ///  Byte array for storing the raw image data.
    /// </param>
    public bool CopyToBuffer(byte[] buffer)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLImageWrapper");
        }

        bool result = false;
        GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            result = vlImageWrapper_CopyToBuffer(
                this.handle,
                bufferHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(buffer.Length)
            );
        }
        finally
        {
            bufferHandle.Free();
        }

        return result;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlImageWrapper_CopyFromBuffer(
        IntPtr imageWrapper, IntPtr buffer, System.UInt32 width,
        System.UInt32 height);
    /// <summary>
    ///  Copies the given byte array into the VisionLib image.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The VisionLib image will be resized according to the width and height
    ///   parameter.
    ///  </para>
    ///  <para>
    ///   Please make sure, that the data stored in the byte array has the same
    ///   format as the image. The image format can be acquired using the
    ///   <see cref="GetFormat"/> function.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the data was copied into the image successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="buffer">Byte array with the image data.</param>
    /// <param name="width">New width of the image.</param>
    /// <param name="height">New height of the image.</param>
    public bool CopyFromBuffer(byte[] buffer, int width, int height)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLImageWrapper");
        }

        if (this.handle == IntPtr.Zero)
        {
            return false;
        }

        if (buffer.Length < width * height * 3)
        {
            return false;
        }

        bool result = false;
        GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            result = vlImageWrapper_CopyFromBuffer(
                this.handle,
                bufferHandle.AddrOfPinnedObject(),
                Convert.ToUInt32(width),
                Convert.ToUInt32(height)
            );
        }
        finally
        {
            bufferHandle.Free();
        }

        return result;
    }
}

/**@}*/