/// \file
/// \brief The basic wrapper for the VisionLib.

/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

/// <summary>
///  The VLAbstractApplicationWrapper is a wrapper for the AbstractApplication.
///  The AbstractApplication represents the tracking context and it stores the
///  global ActionPipe and DataSet.
/// </summary>
public class VLAbstractApplicationWrapper: IDisposable
{
    private IntPtr handle;
    private bool disposed = false;
    private bool owner = false;

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlNew_AbstractApplicationWrapper();
    /// <summary>
    ///  Constructor of VLAbstractApplicationWrapper.
    /// </summary>
    public VLAbstractApplicationWrapper()
    {
        this.handle = vlNew_AbstractApplicationWrapper();
        this.owner = true;
    }

    ~VLAbstractApplicationWrapper()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    /// <summary>
    ///  Returns the handle to the native object.
    /// </summary>
    /// <returns>
    ///  Handle to native object.
    /// </returns>
    public IntPtr GetHandle()
    {
        return this.handle;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_AbstractApplicationWrapper(
        IntPtr abstractApplicationWrapper);
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
            vlDelete_AbstractApplicationWrapper(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>
    ///  Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLAbstractApplicationWrapper"/>. The <see cref="Dispose"/>
    ///  method leaves the <see cref="VLAbstractApplicationWrapper"/> in an
    ///  unusable state. After calling <see cref="Dispose"/>, you must release
    ///  all references to the <see cref="VLAbstractApplicationWrapper"/> so
    ///  the garbage collector can reclaim the memory that the
    ///  <see cref="VLAbstractApplicationWrapper"/> was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_FoundBlockedFeatures(
        IntPtr abstractApplicationWrapper);
    /// <summary>
    ///  Returns whether any features are used, which are not licensed.
    /// </summary>
    /// <remarks>
    ///  The 'blocked features' status will be re-evaluated from time to time.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if unlicensed features are used;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool FoundBlockedFeatures()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_FoundBlockedFeatures(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_ActivateFoundBlockedFeatures(
        IntPtr abstractApplicationWrapper);
    /// <summary>
    ///  Simulates the usage of blocked features. This can be used for testing
    ///  unlicensed behaviour on machines with a valid license.
    /// </summary>
    /// <remarks>
    ///  The 'blocked features' status will be re-evaluated from time to time.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if blocked features were activated successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool ActivateFoundBlockedFeatures()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_ActivateFoundBlockedFeatures(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_GetHostID(
        IntPtr abstractApplicationWrapper,
        StringBuilder hostID,
        System.UInt32 maxSize);
    /// <summary>
    ///  Returns the host ID of the current machine as string.
    /// <remarks>
    ///  The host ID is necessary for generating a license file.
    /// </remarks>
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if host ID was gotten;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="hostID">Output host ID string.</param>
    public bool GetHostID(out string hostID)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        StringBuilder sb = new StringBuilder(512);
        if (!vlAbstractApplicationWrapper_GetHostID(this.handle, sb,
            Convert.ToUInt32(sb.Capacity + 1)))
        {
            hostID = "";
            return false;
        }

        hostID = sb.ToString();

        return true;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_SetLicenseFilePath(
        IntPtr abstractApplicationWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string path);
    /// <summary>
    ///  Sets the path of the license file in the system.
    /// <remarks>
    ///   Calling of this function is mandatory for starting the tracking configuration.
    /// </remarks>
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if a valid license file could be found;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="path">The absolute location of the file.</param>
    public bool SetLicenseFilePath(string path)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_SetLicenseFilePath(
            this.handle, path);
    }



    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_AutoLoadPlugins(
        IntPtr abstractApplicationWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string pluginPath);
    /// <summary>
    ///  Loads all VisionLib plugins from a specific directory.
    /// </summary>
    /// <remarks>
    ///  Many VisionLib features are implemented as plugins, which need to be
    ///  loaded first.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if plugins were loaded successfully;
    ///  <c>false</c> otherwise.</returns>
    /// <param name="pluginPath">
    ///  Plugin directory. Can be an empty string in which case the plugins will
    ///  be loaded from the directory stored inside the PM_PLUGIN_PATH
    ///  environment variable. If this environment variable is not defined,
    ///  then the plugins will be loaded from the directory of the current
    ///  executable.
    /// </param>
    public bool AutoLoadPlugins(string pluginPath)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_AutoLoadPlugins(
            this.handle, pluginPath);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_SetResourcePath(
        IntPtr abstractApplicationWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string path);
    /// <summary>
    ///  Sets the resource path to the given given path.
    /// </summary>
    /// <remarks>
    ///  The resource directory contains tracking pipelines and other support
    ///  files.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if resource path was set successfully;
    ///  <c>false</c> otherwise.</returns>
    /// <param name="path">Path to the resource directory</param>
    public bool SetResourcePath(string path)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_SetResourcePath(
            this.handle, path);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_GetTrackerType(
        IntPtr abstractApplicationWrapper,
        StringBuilder trackerType,
        System.UInt32 maxSize);
    /// <summary>
    ///  Returns the type of the loaded tracking pipeline.
    /// </summary>
    /// <remarks>
    ///  This only works for tracking configurations loaded from a vl-file or
    ///  vl-string.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if tracker type was retrieved successful
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="trackerType">Output host ID string.</param>
    public bool GetTrackerType(out string trackerType)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        StringBuilder sb = new StringBuilder(512);
        if (!vlAbstractApplicationWrapper_GetTrackerType(this.handle, sb,
            Convert.ToUInt32(sb.Capacity + 1)))
        {
            trackerType = "";
            return false;
        }

        trackerType = sb.ToString();

        return true;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_ClearProject(
        IntPtr abstractApplicationWrapper);
    /// <summary>
    ///  Clears the ActionPipe and DataSet.
    /// </summary>
    /// <remarks>
    ///  Please notice, that all wrapper objects for DataBase objects and
    ///  Actions will be left in an invalid state after calling this function.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if project data was cleared successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool ClearProject()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_ClearProject(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_LoadProjectData(
        IntPtr abstractApplicationWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string filename);
    /// <summary>
    ///  Loads the specified tracking configuration XML file.
    /// </summary>
    /// <remarks>
    ///  Please notice, that all wrapper objects for DataBase objects and
    ///  Actions will be left in an invalid state after calling this function.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the tracking configuration was loaded successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="filename">Filename of the tracking configuration</param>
    /// <example>
    ///  <code>
    ///    VLAbstractApplicationWrapper aap = new VLAbstractApplicationWrapper();
    ///    string trackingFile = Path.Combine(Application.streamingAssetsPath, "tracking.pm");
    ///    aap.LoadProjectData(trackingFile);
    ///  </code>
    /// </example>
    public bool LoadProjectData(string filename)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_LoadProjectData(
            this.handle, filename);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_LoadProjectDataFromString(
        IntPtr abstractApplicationWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string str,
        [MarshalAs(UnmanagedType.LPStr)] string fakeFilename);
    /// <summary>
    ///  Loads the given string as tracking configuration.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Please notice, that all wrapper objects for DataBase objects and
    ///   Actions will be left in an invalid state after calling this function.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the tracking configuration was loaded successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="str">String with the tracking configuration</param>
    /// <param name="fakeFilename">
    ///  Filename which will be used to determine the type of the tracking
    ///  configuration (vl / PM) and for resolving relative file paths and the
    ///  type of the tracking configuration.
    /// </param>
    public bool LoadProjectDataFromString(string str, string fakeFilename)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_LoadProjectDataFromString(
            this.handle, str, fakeFilename);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlAbstractApplicationWrapper_GetActionPipe(
        IntPtr abstractApplicationWrapper);
    /// <summary>
    ///  Returns the main ActionPipe of the AbstractApplication.
    /// </summary>
    /// <remarks>
    ///  You must call the <see cref="VLActionPipeWrapper.Dispose"/> method of
    ///  the returned object after you are done using it. Failure to do so will
    ///  result in leaked memory.
    /// </remarks>
    /// <returns>
    ///  <c>VLActionPipeWrapper</c>, if ActionPipe was acquired successfully;
    ///  <c>null</c> otherwise.
    /// </returns>
    public VLActionPipeWrapper GetActionPipe()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        IntPtr actionPipeHandle = vlAbstractApplicationWrapper_GetActionPipe(
            this.handle);

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
    private static extern IntPtr vlAbstractApplicationWrapper_GetDataSet(
        IntPtr abstractApplicationWrapper);
    /// <summary>
    ///  Returns the main DataSet of the AbstractApplication.
    /// </summary>
    /// <remarks>
    ///  You must call the <see cref="VLDataSetWrapper.Dispose"/> method of
    ///  the returned object after you are done using it. Failure to do so will
    ///  result in leaked memory.
    /// </remarks>
    /// <returns>
    ///  <c>VLDataSetWrapper</c>, if DataSet was acquired successfully;
    ///  <c>null</c> otherwise.
    /// </returns>
    public VLDataSetWrapper GetDataSet()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        IntPtr dataSetHandle = vlAbstractApplicationWrapper_GetDataSet(
            this.handle);

        if (dataSetHandle != IntPtr.Zero)
        {
            return new VLDataSetWrapper(dataSetHandle, true);
        }
        else
        {
            return null;
        }
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_InitActions(
        IntPtr abstractApplicationWrapper);
    /// <summary>
    ///   Initializes all Actions.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the Actions were initialized successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool InitActions()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_InitActions(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_ApplyActions(
        IntPtr abstractApplicationWrapper);
    /// <summary>
    ///  Sequentially executes all Actions.
    /// </summary>
    /// <remarks>
    ///  The execution order is depth first and the Actions will run one by one.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the Actions were executed successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool ApplyActions()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_ApplyActions(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_RegisterThread(
        IntPtr abstractApplicationWrapper);
    /// <summary>
    ///  Registers the current thread for the AbstractApplication.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the thread was registered correctly;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool RegisterThread()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_RegisterThread(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_GetDeviceInfo(
        IntPtr abstractApplicationWrapper,
        StringBuilder deviceInfoString,
        System.UInt32 maxSize);
    /// <summary>
    ///  Retrieves the device info object from the AbstractApplication.
    /// </summary>
    /// <returns>
    ///  <c>VLDeviceInfo</c>, if the device info was acquired successfully;
    ///  <c>null</c> otherwise.
    /// </returns>
    public VLDeviceInfo GetDeviceInfo()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        string deviceInfo;
        StringBuilder sb = new StringBuilder(65536);
        if (!vlAbstractApplicationWrapper_GetDeviceInfo(this.handle, sb,
            Convert.ToUInt32(sb.Capacity + 1)))
        {
            Debug.LogError("No valid device info returned...");
            return null;
        }

        deviceInfo = sb.ToString();
        Debug.Log("[vlUnitySDK] DeviceInfoJson: " + deviceInfo);

        VLDeviceInfo devInfo =
            VLJsonUtility.FromJson<VLDeviceInfo>(deviceInfo);

        return devInfo;
    }


    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlAbstractApplicationWrapper_AddCameraCalibrationDB(
        IntPtr abstractApplicationWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string uri);
    /// <summary>
    ///  Adds a custom camera calibration database file.
    /// </summary>
    /// <remarks>
    ///  The calibration database must be added before loading a tracking
    ///  configuration.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the camera calibration database URI was added
    ///  successfully; <c>false</c> otherwise. <c>false</c> will also be
    ///  returned, if the URI was added already.
    /// </returns>
    /// <param name="uri">URI to the camera calibration database file</param>
    public bool AddCameraCalibrationDB(string uri)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLAbstractApplicationWrapper");
        }

        return vlAbstractApplicationWrapper_AddCameraCalibrationDB(this.handle,uri);
    }
}
/**@}*/
