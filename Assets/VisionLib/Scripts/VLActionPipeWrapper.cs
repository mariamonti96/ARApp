/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
/// <summary>
///  The VLActionPipeWrapper is a wrapper for an ActionPipe. ActionPipes
///  store collections of Actions.
/// </summary>
/// <remarks>
///  <para>
///   Since the ActionPipe inherits from the Action class it's also possible
///   that an ActionPipe contains other ActionsPipes (composite pattern).
///  </para>
///  <para>
///   The AbstractApplication stores a global ActionPipe and executes all
///   Actions within it when calling
///   <see cref="VLAbstractApplicationWrapper.ApplyActions"/>. ActionsPipes
///   usually forward this apply call to their sub-actions.
///  </para>
/// </remarks>
/// <seealso cref="VLActionWrapper"/>
/// <seealso cref="VLAbstractApplicationWrapper.GetActionPipe"/>
/// <seealso cref="FindActionPipe"/>
public class VLActionPipeWrapper: IDisposable
{
    private IntPtr handle;
    private bool disposed = false;
    private bool owner;

    /// <summary>
    ///  Constructor of VLActionPipeWrapper.
    /// </summary>
    /// <remarks>
    ///  Don't call this constructor directly. Use the
    ///  <see cref="VLAbstractApplicationWrapper.GetActionPipe"/> and / or
    ///  <see cref="VLActionPipeWrapper.FindActionPipe"/> methods instead.
    /// </remarks>
    /// <param name="handle">
    ///  Handle to the native object.
    /// </param>
    /// <param name="owner">
    ///  <c>true</c>, if the VLActionPipeWrapper is the owner of the native
    ///  object; <c>false</c>, otherwise.
    /// </param>
    public VLActionPipeWrapper(IntPtr handle, bool owner)
    {
        this.handle = handle;
        this.owner = owner;
    }

    ~VLActionPipeWrapper()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_ActionPipeWrapper(
        IntPtr actionPipeWrapper);
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
            vlDelete_ActionPipeWrapper(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>
    ///  Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLActionPipeWrapper"/>. The <see cref="Dispose"/> method
    ///  leaves the <see cref="VLActionPipeWrapper"/> in an unusable state.
    ///  After calling <see cref="Dispose"/>, you must release all references
    ///  to the <see cref="VLActionPipeWrapper"/> so the garbage
    ///  collector can reclaim the memory that the
    ///  <see cref="VLActionPipeWrapper"/> was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionPipeWrapper_IsEnabled(
        IntPtr actionPipeWrapper);
    /// <summary>
    ///  Returns the enabled state of the ActionPipe.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the ActionPipe is enabled;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool IsEnabled()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionPipeWrapper");
        }

        return vlActionPipeWrapper_IsEnabled(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionPipeWrapper_SetEnabled(
        IntPtr actionPipeWrapper, bool enabled);
    /// <summary>
    ///  Sets the enabled state of the ActionPipe.
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
            throw new ObjectDisposedException("VLActionPipeWrapper");
        }

        return vlActionPipeWrapper_SetEnabled(this.handle, enabled);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionPipeWrapper_GetAttribute(
        IntPtr actionPipeWrapper,
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
            throw new ObjectDisposedException("VLActionPipeWrapper");
        }

        StringBuilder sb = new StringBuilder(512);
        if (!vlActionPipeWrapper_GetAttribute(this.handle, attributeName, sb,
            Convert.ToUInt32(sb.Capacity + 1)))
        {
            value = "";
            return false;
        }

        value = sb.ToString();

        return true;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionPipeWrapper_SetAttribute(
        IntPtr actionPipeWrapper,
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
            throw new ObjectDisposedException("VLActionPipeWrapper");
        }

        return vlActionPipeWrapper_SetAttribute(this.handle, attributeName, value);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlActionPipeWrapper_FindAction(
        IntPtr actionPipeWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string key);
    /// <summary>
    ///  Find the Action with the given key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The Action will be searched recursively depth-first within all
    ///   sub-action and -actionpipes.
    ///  </para>
    ///  <para>
    ///   You must call the Dispose method of the returned object after you are
    ///   done using it. Failure to do so will result in leaked memory.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>VLActionWrapper</c>, if Action was found;
    ///  <c>null</c>, otherwise.
    /// </returns>
    /// <param name="key">Key of the Action to be found</param>
    public VLActionWrapper FindAction(string key)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionPipeWrapper");
        }

        IntPtr actionHandle = vlActionPipeWrapper_FindAction(this.handle, key);

        if (actionHandle != IntPtr.Zero)
        {
            return new VLActionWrapper(actionHandle, true);
        }
        else
        {
            return null;
        }
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlActionPipeWrapper_FindActionPipe(
        IntPtr actionPipeWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string key);
    /// <summary>
    ///  Find the ActionPipe with the given key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The ActionPipe will be searched recursively depth-first within all
    ///   sub-action and -actionpipes.
    ///  </para>
    ///  <para>
    ///   You must call the Dispose method of the returned object after you are
    ///   done using it. Failure to do so will result in leaked memory.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>VLActionPipeWrapper</c>, if ActionPipe was found;
    ///  <c>null</c>, otherwise.
    /// </returns>
    /// <param name="key">Key of the ActionPipe to be found</param>
    public VLActionPipeWrapper FindActionPipe(string key)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionPipeWrapper");
        }

        IntPtr actionPipeHandle = vlActionPipeWrapper_FindActionPipe(
            this.handle, key);

        if (actionPipeHandle != IntPtr.Zero)
        {
            return new VLActionPipeWrapper(actionPipeHandle, true);
        }
        else
        {
            return null;
        }
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionPipeWrapper_Init(
        IntPtr actionWrapper,
        IntPtr dataSetWrapper);
    /// <summary>
    ///  Initializes the ActionPipe.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the ActionPipe was initialized successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="dataSet">
    ///  DataSet to be used for initializing the ActionPipe. You can use the
    ///  <see cref="VLAbstractApplicationWrapper.GetDataSet"/> function to
    ///  retrieve the default DataSet.
    /// </param>
    public bool Init(VLDataSetWrapper dataSet)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionPipeWrapper");
        }

        return vlActionPipeWrapper_Init(this.handle, dataSet.GetHandle());
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlActionPipeWrapper_Apply(
        IntPtr actionWrapper,
        IntPtr dataSetWrapper);
    /// <summary>
    ///  Executes the ActionPipe.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the ActionPipe was executed successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="dataSet">
    ///  DataSet used for the execution of the ActionPipe. You can use the
    ///  <see cref="VLAbstractApplicationWrapper.GetDataSet"/> function to
    ///  retrieve the default DataSet.
    /// </param>
    public bool Apply(VLDataSetWrapper dataSet)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLActionPipeWrapper");
        }

        return vlActionPipeWrapper_Apply(this.handle, dataSet.GetHandle());
    }
}
 /**@}*/
