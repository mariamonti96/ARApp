/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
///  The VLActionWrapper is a wrapper for an Action. All processing steps in
///  the VisionLib are encapsulated inside Action classes.
/// </summary>
/// <seealso cref="VLActionPipeWrapper.FindAction"/>
public class VLActionWrapper: IDisposable
{
    private IntPtr handle;
    private bool disposed = false;
    private bool owner;

    /// <summary>
    ///  Constructor of VLActionWrapper.
    /// </summary>
    /// <remarks>
    ///  Don't call this constructor directly. Use the
    ///  <see cref="VLAbstractApplicationWrapper.GetActionPipe"/> and
    ///  <see cref="VLActionPipeWrapper.FindAction"/> methods instead.
    /// </remarks>
    /// <param name="handle">
    ///  Handle to the native object.
    /// </param>
    /// <param name="owner">
    ///  <c>true</c>, if the VLActionWrapper is the owner of the native object;
    ///  <c>false</c>, otherwise.
    /// </param>
    public VLActionWrapper(IntPtr handle, bool owner)
    {
        this.handle = handle;
        this.owner = owner;
    }

    ~VLActionWrapper()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_ActionWrapper(IntPtr actionWrapper);
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
            vlDelete_ActionWrapper(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>
    ///  Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLActionWrapper"/>. The <see cref="Dispose"/> method
    ///  leaves the <see cref="VLActionWrapper"/> in an unusable state.
    ///  After calling <see cref="Dispose"/>, you must release all references
    ///  to the <see cref="VLActionWrapper"/> so the garbage collector
    ///  can reclaim the memory that the <see cref="VLActionWrapper"/>
    ///  was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionWrapper_IsEnabled(IntPtr actionWrapper);
    /// <summary>
    ///  Returns the enabled state of the action.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the action is enabled;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool IsEnabled()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionWrapper");
        }

        return vlActionWrapper_IsEnabled(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionWrapper_SetEnabled(IntPtr actionWrapper,
        bool enabled);
    /// <summary>
    ///  Sets the enabled state of the action.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if enabled state was set successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="enabled">Enabled state.</param>
    public bool SetEnabled(bool enabled)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionWrapper");
        }

        return vlActionWrapper_SetEnabled(this.handle, enabled);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionWrapper_GetAttribute(
        IntPtr actionWrapper,
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
            throw new ObjectDisposedException("VLActionWrapper");
        }

        StringBuilder sb = new StringBuilder(512);
        if (!vlActionWrapper_GetAttribute(this.handle, attributeName, sb,
            Convert.ToUInt32(sb.Capacity + 1)))
        {
            value = "";
            return false;
        }

        value = sb.ToString();

        return true;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionWrapper_SetAttribute(
        IntPtr actionWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string attributeName,
        [MarshalAs(UnmanagedType.LPStr)] string value);
    /// <summary>
    ///  Sets the value of an attribute as string.
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
            throw new ObjectDisposedException("VLActionWrapper");
        }

        return vlActionWrapper_SetAttribute(this.handle, attributeName, value);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionWrapper_Init(
        IntPtr actionWrapper,
        IntPtr dataSetWrapper);
    /// <summary>
    ///  Initializes the Action.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the Action was initialized successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="dataSet">
    ///  DataSet to be used for initializing the Action. You can use the
    ///  <see cref="VLAbstractApplicationWrapper.GetDataSet"/> function to
    ///  retrieve the default DataSet.
    /// </param>
    public bool Init(VLDataSetWrapper dataSet)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionWrapper");
        }

        return vlActionWrapper_Init(this.handle, dataSet.GetHandle());
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionWrapper_Apply(
        IntPtr actionWrapper,
        IntPtr dataSetWrapper);
    /// <summary>
    ///  Executes the Action.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the Action was executed successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="dataSet">
    ///  DataSet used for the execution of the Action. You can use the
    ///  <see cref="VLAbstractApplicationWrapper.GetDataSet"/> function to
    ///  retrieve the default DataSet.
    /// </param>
    public bool Apply(VLDataSetWrapper dataSet)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionWrapper");
        }

        return vlActionWrapper_Apply(this.handle, dataSet.GetHandle());
    }
}

/**@}*/