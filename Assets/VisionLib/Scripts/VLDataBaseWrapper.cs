/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
///  The VLDataBaseWrapper is a wrapper for a DataBase object. DataBase objects
///  represent the data which can be exchanged between Actions.
/// </summary>
/// <seealso cref="VLDataSetWrapper"/>
public class VLDataBaseWrapper: IDisposable
{
    private IntPtr handle;
    private bool disposed = false;
    private bool owner;

    /// <summary>
    ///  Constructor of VLDataBaseWrapper.
    /// </summary>
    /// <remarks>
    ///  Don't call this constructor directly. Use the
    ///  <see cref="VLAbstractApplicationWrapper.GetDataSet"/> and
    ///  <see cref="VLDataSetWrapper.GetDataBase"/> methods instead.
    /// </remarks>
    /// <param name="handle">
    ///  Handle to the native object.
    /// </param>
    /// <param name="owner">
    ///  <c>true</c>, if the VLDataBaseWrapper is the owner of the native
    ///  object; <c>false</c>, otherwise.
    /// </param>
    public VLDataBaseWrapper(IntPtr handle, bool owner)
    {
        this.handle = handle;
        this.owner = owner;
    }

    ~VLDataBaseWrapper()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_DataBaseWrapper(IntPtr actionWrapper);
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
            vlDelete_DataBaseWrapper(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>
    ///  Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLDataBaseWrapper"/>. The <see cref="Dispose"/> method
    ///  leaves the <see cref="VLDataBaseWrapper"/> in an unusable state.
    ///  After calling <see cref="Dispose"/>, you must release all references
    ///  to the <see cref="VLDataBaseWrapper"/> so the garbage collector
    ///  can reclaim the memory that the <see cref="VLDataBaseWrapper"/>
    ///  was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlDataBaseWrapper_GetAttribute(
        IntPtr dataBaseWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string attributeName,
        StringBuilder value,
        System.UInt32 maxSize);
    /// <summary>
    ///  Returns the value of an attribute as string.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the attribute value was acquired successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="value">Output attribute value.</param>
    public bool GetAttribute(string attributeName, out string value)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLDataBaseWrapper");
        }

        StringBuilder sb = new StringBuilder(512);
        if (!vlDataBaseWrapper_GetAttribute(this.handle, attributeName, sb,
            Convert.ToUInt32(sb.Capacity + 1)))
        {
            value = "";
            return false;
        }

        value = sb.ToString();

        return true;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlDataBaseWrapper_SetAttribute(
        IntPtr dataBaseWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string attributeName,
        [MarshalAs(UnmanagedType.LPStr)] string value);
    /// <summary>
    ///  Set the value of an attribute as string.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if attribute value was changed successfully;
    ///  <c>false</c>, otherwise.
    /// </returns>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="value">Value to be set</param>
    public bool SetAttribute(string attributeName, string value)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLDataBaseWrapper");
        }

        return vlDataBaseWrapper_SetAttribute(this.handle, attributeName, value);
    }
}

/**@}*/