/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using AOT;
using VLWorkerCommands;

[AddComponentMenu("VisionLib/VL Worker Behaviour")]
public class VLWorkerBehaviour : MonoBehaviour
{
    public VLLicenseFile licenseFile;
    public string calibrationDataBaseURI;

    private bool trackingRunning = false;

    /// <summary>
    ///  Directory from which the tracking configuration files will be loaded.
    /// </summary>
    /// <remarks>
    ///  If this is null or an empty string, then the tracking configuration
    ///  files will be loaded from the <c>/StreamingAssets/VisionLib/</c>
    ///  directory. This has the advantage, that they can be found on all
    ///  platforms. Otherwise you need to make sure, that the baseDir is
    ///  accessible for the current platform.
    /// </remarks>
    public string baseDir;

    /// <summary>
    ///  Delegate for <see cref="OnTrackerInitializing"/> events.
    /// </summary>
    public delegate void TrackerInitializingAction();
    /// <summary>
    ///  Event which will be emitted once after calling the StartTracking
    ///  function.
    /// </summary>
    public static event TrackerInitializingAction OnTrackerInitializing;

    /// <summary>
    ///  Delegate for <see cref="OnTrackerInitialized"/> events.
    /// </summary>
    /// <param name="success">
    ///  <c>true</c>, if the tracking configuration was loaded
    ///  successfully; <c>false</c>, otherwise.
    /// </param>
    public delegate void TrackerInitializedAction(bool success);
    /// <summary>
    ///  Event which will be emitted after the tracking configuration was
    ///  loaded.
    /// </summary>
    public static event TrackerInitializedAction OnTrackerInitialized;

    /// <summary>
    ///  Delegate for <see cref="OnTrackerInitializedWithIssues"/> events.
    /// </summary>
    /// <param name="errors">
    ///  <c>null</c>, if the no warning was emitted
    ///  <c>VLTrackingIssues</c>, as a list of VLTrackingIssue.
    /// </param>
    /// <param name="warnings">
    ///  <c>null</c>, if the no warning was emitted
    ///  <c>VLTrackingIssues</c>, as a list of VLTrackingIssue.
    /// </param>
    public delegate void TrackerInitializedWithIssuesAction(VLTrackingIssues errors,VLTrackingIssues warnings);
    /// <summary>
    ///  Event which will be emitted after the tracking configuration was
    ///  loaded.
    /// </summary>
    public static event TrackerInitializedWithIssuesAction OnTrackerInitializedWithIssues;

    /// <summary>
    ///  Delegate for <see cref="OnTrackerStopped"/> events.
    /// </summary>
    /// <param name="success">
    ///  <c>true</c>, on success; <c>false</c>, otherwise.
    /// </param>
    public delegate void TrackerStoppedAction(bool success);
    /// <summary>
    ///  Event which will be emitted after the tracking was stopped.
    /// </summary>
    public static event TrackerStoppedAction OnTrackerStopped;

    /// <summary>
    ///  Delegate for <see cref="OnTrackerRunning"/> events.
    /// </summary>
    /// <param name="success">
    ///  <c>true</c>, on success; <c>false</c>, otherwise.
    /// </param>
    public delegate void TrackerRunningAction(bool success);
    /// <summary>
    ///  Event which will be emitted once after the tracking was stopped or
    ///  paused and is now running again.
    /// </summary>
    public static event TrackerRunningAction OnTrackerRunning;

    /// <summary>
    ///  Delegate for <see cref="OnTrackerRanOnce"/> events.
    /// </summary>
    /// <param name="success">
    ///  <c>true</c>, on success; <c>false</c>, otherwise.
    /// </param>
    public delegate void TrackerRanOnceAction(bool success);
    /// <summary>
    ///  Event which will be emitted once after the tracking was explicitly ran
    ///  once.
    /// </summary>
    public static event TrackerRanOnceAction OnTrackerRanOnce;

    /// <summary>
    ///  Delegate for <see cref="OnTrackerPaused"/> events.
    /// </summary>
    /// <param name="success">
    ///  <c>true</c>, on success; <c>false</c>, otherwise.
    /// </param>
    public delegate void TrackerPausedAction(bool success);
    /// <summary>
    ///  Event which will be emitted after the tracking was paused.
    /// </summary>
    public static event TrackerPausedAction OnTrackerPaused;

    /// <summary>
    ///  Delegate for <see cref="TrackedResetAction"/> events.
    /// </summary>
    /// <param name="hard">Not used</param>
    public delegate void TrackerResetAction(bool hard);
    /// <summary>
    ///  (DEPRECATED) Event which will be emitted after a reset was executed.
    /// </summary>
    public static event TrackerResetAction OnTrackerReset;

    /// <summary>
    ///  Delegate for <see cref="OnTrackerResetSoft"/> events.
    /// </summary>
    /// <param name="success">
    ///  <c>true</c>, on success; <c>false</c>, otherwise.
    /// </param>
    public delegate void TrackerResetSoftAction(bool success);
    /// <summary>
    ///  Event which will be emitted after a soft reset was executed.
    /// </summary>
    public static event TrackerResetSoftAction OnTrackerResetSoft;

    /// <summary>
    ///  Delegate for <see cref="TrackerResetHardAction"/> events.
    /// </summary>
    /// <param name="success">
    ///  <c>true</c>, on success; <c>false</c>, otherwise.
    /// </param>
    public delegate void TrackerResetHardAction(bool success);
    /// <summary>
    ///  Event which will be emitted after a hard reset was executed.
    /// </summary>
    public static event TrackerResetHardAction OnTrackerResetHard;

    /// <summary>
    ///  Delegate for <see cref="WriteInitDataAction"/> events.
    /// </summary>
    /// <param name="success">
    ///  <c>true</c>, on success; <c>false</c>, otherwise.
    /// </param>
    public delegate void WriteInitDataAction(bool success);
    /// <summary>
    ///  Event which will be emitted after a the initialization were written
    ///  to disk.
    /// </summary>
    public static event WriteInitDataAction OnWriteInitData;

	/// <summary>
	///  Delegate for <see cref="ReadInitDataAction"/> events.
	/// </summary>
	/// <param name="success">
	///  <c>true</c>, on success; <c>false</c>, otherwise.
	/// </param>
	public delegate void ReadInitDataAction(bool success);
    /// <summary>
    ///  Event which will be emitted after a the initialization data has been loaded
    ///  from an uri.
    /// </summary>
    public static event ReadInitDataAction OnReadInitData;

	/// <summary>
	///  Delegate for <see cref="ResetInitDataAction"/> events.
	/// </summary>
	/// <param name="success">
	///  <c>true</c>, on success; <c>false</c>, otherwise.
	/// </param>
	public delegate void ResetInitDataAction(bool success);
    /// <summary>
    ///  Event which will be emitted after a the initialization data has been reset
    ///  from an uri.
    /// </summary>
    public static event ResetInitDataAction OnResetInitData;

    public delegate void CalibrationDataAction(VLCameraCalibration result);
    public static event CalibrationDataAction OnCameraCalibrationData;


        /// <summary>
    ///  Delegate for <see cref="OnGetModelProperties"/> events.
    /// </summary>
    /// <param name="properties">
    ///  <c>VLModelProperties</c> as an Array of actual Model Properties
    /// </param>
    public delegate void GetModelPropertiesAction(VLModelProperties[] properties);
    /// <summary>
    ///  Event which will be emitted when the model properties have been rquested by GetModelStateProperties. 
    /// </summary>
    public static event GetModelPropertiesAction OnGetModelProperties;

    /// <summary>
    ///  Delegate for <see cref="OnTrackingState"/> events.
    /// </summary>
    /// <param name="state">
    ///  <c>100</c>, if object is tracked right now; <c>0</c>, otherwise.
    ///  NOTICE: This is still work in progress and more states will be added
    ///  later.
    /// </param>
    /// <param name="objectID">
    ///  <c>objectID</c>, may refer to a different object.
    /// </param>
    public delegate void TrackingStateAction(int state, string objectID);
    /// <summary>
    ///  Event with the current tracking state. This Event will be emitted for
    ///  each tracking frame.
    /// </summary>
    public static event TrackingStateAction OnTrackingState;

    /// <summary>
    ///  Delegate for <see cref="OnTrackingStates"/> events.
    /// </summary>
    /// <param name="state">
    ///  <see cref="VLTrackingState"/> with information about the currently
    ///  tracked objects.
    /// </param>
    public delegate void TrackingStatesAction(VLTrackingState state);
    /// <summary>
    ///  Event with the current tracking state of all tracked objects. This
    ///  Event will be emitted for each tracking frame.
    /// </summary>
    public static event TrackingStatesAction OnTrackingStates;

    /// <summary>
    ///  Delegate for <see cref="OnPerformanceInfo"/> events.
    /// </summary>
    /// <param name="state">
    ///  <see cref="VLPerformanceInfo"/> with information about the performance.
    /// </param>
    public delegate void PerformanceInfoAction(VLPerformanceInfo state);
    /// <summary>
    ///  Event with the current tracking performance. This Event will be
    ///  emitted for each tracking frame.
    /// </summary>
    public static event PerformanceInfoAction OnPerformanceInfo;

    public delegate void ImageAction(VLImageWrapper image);
    public static event ImageAction OnImage;

    public delegate void ExtrinsicDataAction(VLExtrinsicDataWrapper extrinsicData);
    public static event ExtrinsicDataAction OnExtrinsicData;

    public delegate void IntrinsicDataAction(VLIntrinsicDataWrapper intrinsicData);
    public static event IntrinsicDataAction OnIntrinsicData;

    /// <summary>
    ///  Target number of frames per second for the tracking thread.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The tracking will run as fast as possible, if the value is zero or
    ///   less.
    ///  </para>
    ///  <para>
    ///   Higher values will result in a smoother tracking experience, but the
    ///   battery will be drained faster.
    ///  </para>
    /// </remarks>
    public int targetFPS = 30;
    private int lastTargetFPS = -1;

    /// <summary>
    ///  Whether to wait for tracking events.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If <c>true</c>, the Update member function will wait until there is
    ///   at least one tracking event. This will limit the speed of the Unity
    ///   update cycler to the speed of the tracking, but the tracking will feel
    ///   more smooth, because the camera image will be shown with less delay.
    ///  </para>
    ///  <para>
    ///   If <c>false</c>, the speed of the tracking and the Unity update cycle
    ///   are largely separate. Due to the out of sync update rates, the camera
    ///   might be shown with a slight delay.
    ///  </para>
    /// </remarks>
    public bool waitForEvents = false;

    private VLDeviceInfo deviceInfo = null;

    /// <summary>
    ///  VisionLib log level.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Available log levels:
    ///   * 0: Log
    ///   * 1: Fatal
    ///   * 2: Warning
    ///   * 3: Notice
    ///   * 4: Info
    ///   * 5: Debug
    ///  </para>
    ///  <para>
    ///   Log level N will disable all log messages with a level > N.
    ///  </para>
    /// </remarks>
    public VLUnitySdk.LogLevel logLevel = VLUnitySdk.LogLevel.Warning;
    private VLUnitySdk.LogLevel lastLogLevel = VLUnitySdk.LogLevel.Warning;

    private VLAbstractApplicationWrapper aap = null;
    private VLLogger logger = null;
    private VLWorker worker = null;
    private GCHandle gcHandle;
    private string resourceDir;

    /// <summary>
    ///  Returns the owned VLWorker object.
    /// </summary>
    /// <returns>
    ///  VLWorker object or null, if the VLWorker wasn't initialized yet.
    /// </returns>
    public VLWorker GetWorker()
    {
        return this.worker;
    }

    private static string RemoveQueryString(string uri)
    {
        // Return the sub-string before the first '?'
        int queryStringStart = uri.IndexOf('?', 0);
        if (queryStringStart >= 0)
        {
            uri = uri.Substring(0, queryStringStart);
        }
        return uri;
    }

    /// <summary>
    /// Adds the camera calibration DataBase using the URI. It will not be loaded at this point but only the addability checked.
    /// The loading of the actual database happens when starting the tracking pipe!
    /// 
    /// </summary>
    /// <returns><c>true</c>, if camera calibration DB was added, <c>false</c> otherwise.</returns>
    /// <param name="uri">URI pointing to the camera calibration to be merged.</param>
    public bool AddCameraCalibrationDB(string uri)
    {
        return this.aap.AddCameraCalibrationDB(uri);
    }

    /// <summary>
    ///  Start the tracking using a vl-file.
    /// </summary>
    /// <remarks>
    ///  The type of the tracker will be derived from the vl-file.
    /// </remarks>
    public void StartTracking(string filename)
    {
        if (this.worker == null)
        {
            return;
        }

        // Add the camera calibration database from a given URI
        if (this.calibrationDataBaseURI != ""){
            this.AddCameraCalibrationDB(this.calibrationDataBaseURI);
        }

        // Use the old file format?
        string extension = Path.GetExtension(
            RemoveQueryString(filename)).ToLower();
        if (extension != ".vl")
        {
            // Append the system to the file name (the new file format
            // automatically detects the current system)
#if UNITY_EDITOR
    #if UNITY_EDITOR_WIN
            filename += "Windows.pm";
    #elif UNITY_EDITOR_OSX
            filename += "MacOS.pm";
    #elif UNITY_EDITOR_LINUX
            filename += "Linux.pm";
    #else
            #warning Tracker not implemented for unsupported editor platform
            filename += "Unknown.pm";
    #endif
#else
    #if UNITY_STANDALONE_WIN
            filename += "Windows.pm";
    #elif UNITY_STANDALONE_OSX
            filename += "MacOS.pm";
    #elif UNITY_STANDALONE_LINUX
            filename += "Linux.pm";
    #elif UNITY_IOS
            filename += "iOS.pm";
    #elif UNITY_ANDROID
            filename += "Android.pm";
    #elif UNITY_WSA_10_0
            filename += "Windows.pm";
    #else
            #warning Tracker not implemented for unsupported build target
            filename += "Unknown.pm";
    #endif
#endif
        }

        this.aap.SetResourcePath(this.resourceDir);
        string trackingFile = "";
        if (filename.StartsWith("http://") || filename.StartsWith("https://")) 
        {
            trackingFile = filename;
        }
        else
        {
            trackingFile = Path.Combine(this.baseDir, filename);
        }

        if (OnTrackerInitializing != null)
        {
            OnTrackerInitializing();
        }

        this.worker.Start();

        this.worker.PushCommand(
            new CreateTrackerCmd(trackingFile),
            dispatchCreateTrackerCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));

        this.worker.PushCommand(
            new SetTargetFpsCmd(this.targetFPS),
            null,
            IntPtr.Zero);
    }

    /// <summary>
    ///  Stop the tracking (releases all tracking resources).
    /// </summary>
    public void StopTracking()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.Stop();

        this.trackingRunning = false;
        if (OnTrackerStopped != null)
        {
            OnTrackerStopped(true);
        }
    }

    /// <summary>
    ///  Pause the tracking.
    /// </summary>
    public void PauseTracking()
    {
        this.trackingRunning = false;
        PauseTrackingInternal();
    }

    /// <summary>
    ///  Pause the tracking internal. 
    ///  Does not modify the `trackingRunning` variable.
    /// </summary>
    private void PauseTrackingInternal()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new PauseTrackingCmd(),
            dispatchPauseTrackingCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    /// <summary>
    ///  Resume the tracking.
    /// </summary>
    public void ResumeTracking()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new RunTrackingCmd(),
            dispatchRunTrackingCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    /// <summary>
    ///  Runs the tracking once while the tracking is paused.
    /// </summary>
    public void RunTrackingOnce()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new RunTrackingOnceCmd(),
            dispatchRunTrackingOnceCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    /// <summary>
    ///  Resets the tracking pose to the initial pose.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   NOTICE: Deprecated. Use <see cref="ResetTrackingSoft"/> or
    ///   <see cref="ResetTrackingHard"/> instead.
    ///  </para>
    /// </remarks>
    public void ResetTracking()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new VLModelTrackerCommands.ResetTrackingCmd(),
            dispatchResetTrackingCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    /// <summary>
    ///  Reset the tracking.
    /// </summary>
    public void ResetTrackingSoft()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new VLModelTrackerCommands.ResetSoftCmd(),
            dispatchResetTrackingSoftCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    /// <summary>
    ///  Reset the tracking and all keyframes.
    /// </summary>
    public void ResetTrackingHard()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new VLModelTrackerCommands.ResetHardCmd(),
            dispatchResetTrackingHardCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    /// <summary>
    ///  Write the captured initialization data as file to default location
    ///  with default name.
    /// </summary>
    /// <remarks>
    ///  A default name will be used for the file ("InitData_timestamp.binz").
    ///  The file will be written to different locations depending on the
    ///  platform:
    ///  * Windows: Current users home directory
    ///  * MacOS: Current users document directory
    ///  * iOS / Android: The current applications document directory
    /// </remarks>
    public void WriteInitData()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new VLModelTrackerCommands.WriteInitDataCmd(),
            dispatchWriteInitDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    /// <summary>
    ///  Write the captured initialization data as file to custom location
    ///  with custom name.
    /// </summary>
    /// <remarks>
    ///  In order to avoid having to use a different file path for each
    ///  platform, the "local_storage_dir" scheme can be used as file prefix.
    ///  This scheme points to different locations depending on the platform:
    ///  * Windows: Current users home directory
    ///  * MacOS: Current users document directory
    ///  * iOS / Android: The current applications document directory
    /// </remarks>
    /// <param name="filePrefix">
    ///  Will be used as filename and path. A time stamp and the file
    ///  extension will be appended automatically. A plausible value could be
    ///  for example "local_storage_dir:MyInitData_".
    /// </param>
    public void WriteInitData(string filePrefix)
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new VLModelTrackerCommands.WriteInitDataWithPrefixCmd(filePrefix),
            dispatchWriteInitDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }
    
    /// <summary>
    ///  Loads the captured initialization data as file from a custom location.
    /// </summary>
    /// <remarks>
    ///  In order to load init data at best use a static uri. A common way is for each
    ///  platform, is using  "local_storage_dir" scheme which can be used as file prefix.
    ///  This scheme points to different locations depending on the platform:
    ///  * Windows: Current users home directory
    ///  * MacOS: Current users document directory
    ///  * iOS / Android: The current applications document directory
    /// </remarks>
    /// <param name="uri">
    ///  Will be used as filename and path. A time stamp and the file
    ///  extension will be appended automatically. A plausible value could be
    ///  for example "local_storage_dir:MyInitData_".
    /// </param>
    public void ReadInitData(string uri)
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new VLModelTrackerCommands.ReadInitDataWithPrefixCmd(uri),
            dispatchReadInitDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    /// <summary>
    ///  Reset the offline initialization data.
    /// </summary>
    /// <remarks>
    ///  In order to reset the initialization data loaded at the beginning this fuction can be called.
    ///  The init data learned on the fly, will still be maintained and can be reset by issuing a hard reset.
    /// </remarks>
    public void ResetInitData()
    {
        if (this.worker == null)
        {
            return;
        }

        this.worker.PushCommand(
            new VLModelTrackerCommands.ResetInitDataCmd(),
            dispatchResetInitDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    public bool SetCameraCalibrationState(string camCalibState)
    {
        if (this.worker == null)
        {
            return false;
        }
            bool res = true;
            this.worker.PushCommand(
            new VLCameraCalibrationCommands.CameraCalibrationCmd(camCalibState),
            dispatchCalibrateDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
            return res;

    }

    public bool SetModelProperty(string name,string property,bool state){
        if (this.worker == null)
        {
            return false;
        }
        return this.worker.PushCommand(
            new VLWorkerCommands.SetModelBoolPropertyCmd(name,property,state),
            null,
            IntPtr.Zero);
    }

    public bool GetModelProperties(){
        if (this.worker == null)
        {
            return false;
        }
        return this.worker.PushCommand(
            new VLWorkerCommands.GetModelPropertiesCmd(),
            dispatchGetModelPropertiesCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
    }

    public bool WriteCameraCalibration(string uri)
    {
        if (this.worker == null)
        {
            return false;
        }
        bool res = true;
        this.worker.PushCommand(
            new VLCameraCalibrationCommands.WriteCameraCalibrationCmd(uri),
            dispatchCalibrateDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle));
        return res;

    }

    /// <summary>
    ///  Set <see cref="waitForEvents"/> to the given value.
    /// </summary>
    /// <remarks>
    ///  See <see cref="waitForEvents"/> for further information.
    /// </remarks>
    public void SetWaitForEvents(bool wait)
    {
        this.waitForEvents = wait;
    }

    /// <summary>
    /// Returns the device info, when the worker object has been initialized.
    /// You  can call this function in ordet to get usefull system information in before starting the tracking pipe
    /// You might use this structure for retreiving the available cameras in the system.
    /// </summary>
    /// <returns>The device info object or null.</returns>
    public VLDeviceInfo GetDeviceInfo()
    {
        return deviceInfo;
    }

    private static VLWorkerBehaviour GetInstance(IntPtr clientData)
    {
        return (VLWorkerBehaviour)GCHandle.FromIntPtr(clientData).Target;
    }

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchCreateTrackerCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).CreateTrackerHandler(errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchCreateTrackerCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchCreateTrackerCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchPauseTrackingCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).PauseTrackingHandler(errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchPauseTrackingCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchPauseTrackingCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchRunTrackingCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).RunTrackingHandler(errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchRunTrackingCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchRunTrackingCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchRunTrackingOnceCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).RunTrackingOnceHandler(
                errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchRunTrackingOnceCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchRunTrackingOnceCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchResetTrackingCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).ResetTrackingHandler(errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchResetTrackingCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchResetTrackingCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchResetTrackingSoftCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).ResetTrackingSoftHandler(
                errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchResetTrackingSoftCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchResetTrackingSoftCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchResetTrackingHardCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).ResetTrackingHardHandler(
                errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchResetTrackingHardCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchResetTrackingHardCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchWriteInitDataCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).WriteInitDataHandler(
                errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchWriteInitDataCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchWriteInitDataCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchReadInitDataCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).ReadInitDataHandler(
                errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchReadInitDataCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchReadInitDataCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchResetInitDataCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).ResetInitDataHandler(
                errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchResetInitDataCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchResetInitDataCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchCalibrateDataCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).CameraCalibrationDataHandler(
            errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
        // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
            e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchCalibrateDataCallbackDelegate =
            new VLWorker.JsonStringCallback(DispatchCalibrateDataCallback);

    [MonoPInvokeCallback(typeof(VLWorker.JsonStringCallback))]
    private static void DispatchGetModelPropertiesCallback(
        string errorJson, string resultJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).GetModelPropertiesHandler(errorJson, resultJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.JsonStringCallback dispatchGetModelPropertiesCallbackDelegate =
        new VLWorker.JsonStringCallback(DispatchGetModelPropertiesCallback);


    [MonoPInvokeCallback(typeof(VLWorker.ImageWrapperCallback))]
    private static void DispatchImageCallback(IntPtr handle, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).ImageHandler(handle);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.ImageWrapperCallback dispatchImageCallbackDelegate =
        new VLWorker.ImageWrapperCallback(DispatchImageCallback);

    [MonoPInvokeCallback(typeof(VLWorker.ExtrinsicDataWrapperCallback))]
    private static void DispatchExtrinsicDataCallback(
        IntPtr handle, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).ExtrinsicDataHandler(handle);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.ExtrinsicDataWrapperCallback dispatchExtrinsicDataCallbackDelegate =
        new VLWorker.ExtrinsicDataWrapperCallback(DispatchExtrinsicDataCallback);

    [MonoPInvokeCallback(typeof(VLWorker.IntrinsicDataWrapperCallback))]
    private static void DispatchIntrinsicDataCallback(
        IntPtr handle, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).IntrinsicDataHandler(handle);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLWorker.IntrinsicDataWrapperCallback dispatchIntrinsicDataCallbackDelegate =
        new VLWorker.IntrinsicDataWrapperCallback(DispatchIntrinsicDataCallback);

    [MonoPInvokeCallback(typeof(VLWorker.StringCallback))]
    private static void DispatchTrackingStateCallback(
        string trackingStateJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).TrackingStateHandler(trackingStateJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // called from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message + "\n" +
                trackingStateJson);
        }
    }
    private static VLWorker.StringCallback dispatchTrackingStateCallbackDelegate =
        new VLWorker.StringCallback(DispatchTrackingStateCallback);

    [MonoPInvokeCallback(typeof(VLWorker.StringCallback))]
    private static void DispatchPerformanceInfoCallback(
        string performanceInfoJson, IntPtr clientData)
    {
        try
        {
            GetInstance(clientData).PerformanceInfoHandler(performanceInfoJson);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // called from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message + "\n" +
                performanceInfoJson);
        }
    }
    private static VLWorker.StringCallback dispatchPerformanceInfoCallbackDelegate =
        new VLWorker.StringCallback(DispatchPerformanceInfoCallback);

    private void CreateTrackerHandler(string errorJson, string resultJson)
    {
        bool hasError = (errorJson != null);
        if (OnTrackerInitializedWithIssues != null)
        {
            VLTrackingIssues errorIssues=null;
            VLTrackingIssues warningIssues=null;
            if (errorJson != null)
            {
                errorIssues =
                    VLJsonUtility.FromJson<VLTrackingIssues>(errorJson);
            }
            if (resultJson != null)
            {
                warningIssues =
                    VLJsonUtility.FromJson<VLTrackingIssues>(resultJson);
            }

            OnTrackerInitializedWithIssues(errorIssues,warningIssues);
        }

        if (OnTrackerInitialized != null)
        {
            OnTrackerInitialized(errorJson == null);
        }

        // Push the RunTracking command after calling the OnTrackerInitialized
        // event in order to give the user the chance to push commands which
        // will then be executed before the tracking is running.
            // only run if no error has occured...
            if (!hasError){
                this.worker.PushCommand(
                    new RunTrackingCmd(),
                    DispatchRunTrackingCallback,
                    GCHandle.ToIntPtr(this.gcHandle));
            } else {
                StopTracking();
            }
    }

    private void PauseTrackingHandler(string errorJson, string resultJson)
    {
        if (OnTrackerPaused != null)
        {
            OnTrackerPaused(errorJson == null);
        }
    }

    private void RunTrackingHandler(string errorJson, string resultJson)
    {
        this.trackingRunning = errorJson == null;
        if (OnTrackerRunning != null)
        {
            OnTrackerRunning(this.trackingRunning);
        }
    }

    private void RunTrackingOnceHandler(string errorJson, string resultJson)
    {
        if (OnTrackerRanOnce != null)
        {
            OnTrackerRanOnce(errorJson == null);
        }
    }

    private void ResetTrackingHandler(string errorJson, string resultJson)
    {
        if (OnTrackerReset != null)
        {
            OnTrackerReset(false);
        }
    }

    private void ResetTrackingSoftHandler(string errorJson, string resultJson)
    {
        if (OnTrackerResetSoft != null)
        {
            OnTrackerResetSoft(errorJson == null);
        }
    }

    private void ResetTrackingHardHandler(string errorJson, string resultJson)
    {
        if (OnTrackerResetHard != null)
        {
            OnTrackerResetHard(errorJson == null);
        }
    }

    private void WriteInitDataHandler(string errorJson, string resultJson)
    {
        if (OnWriteInitData != null)
        {
            OnWriteInitData(errorJson == null);
        }
    }

    private void ReadInitDataHandler(string errorJson, string resultJson)
    {
        if (OnReadInitData != null)
        {
            OnReadInitData(errorJson == null);
        }
    }

    private void ResetInitDataHandler(string errorJson, string resultJson)
    {
        if (OnResetInitData != null)
        {
            OnResetInitData(errorJson == null);
        }
    }

    private void CameraCalibrationDataHandler(string errorJson, string resultJson)
    {
        if (OnCameraCalibrationData != null)
        {
            VLCameraCalibrationAnswer calib =
                VLJsonUtility.FromJson<VLCameraCalibrationAnswer>(resultJson);
            if (calib != null &&
                calib.calibration != null &&
                calib.stateChange.command == "getResults")
            {
                OnCameraCalibrationData(calib.calibration);
            }
        }
    }

    private void GetModelPropertiesHandler(string errorJson, string resultJson)
    {
        bool hasError = (errorJson != null);
        if (OnGetModelProperties != null)
        {
            VLModelPropertiesStructure modelProperties=null;
            if (resultJson != null)
            {
                modelProperties =
                    VLJsonUtility.FromJson<VLModelPropertiesStructure>(resultJson);
                OnGetModelProperties(modelProperties.info);
            } else {
                OnGetModelProperties(null);
            }

        }

    }


    private void ImageHandler(IntPtr handle)
    {
        if (OnImage != null)
        {
            VLImageWrapper image = new VLImageWrapper(handle, false);
            OnImage(image);
            image.Dispose();
        }
    }

    private void ExtrinsicDataHandler(IntPtr handle)
    {
        VLExtrinsicDataWrapper extrinsicData =
            new VLExtrinsicDataWrapper(handle, false);

        if (OnExtrinsicData != null)
        {
            OnExtrinsicData(extrinsicData);
        }

        if (OnTrackingState != null)
        {
            if (extrinsicData.GetValid())
            {
                OnTrackingState(100, "0");
            }
            else
            {
                OnTrackingState(0, "0");
            }
        }

        extrinsicData.Dispose();
    }

    private void IntrinsicDataHandler(IntPtr handle)
    {
        if (OnIntrinsicData != null)
        {
            VLIntrinsicDataWrapper intrinsicData =
                new VLIntrinsicDataWrapper(handle, false);
            OnIntrinsicData(intrinsicData);
            intrinsicData.Dispose();
        }
    }

    private void TrackingStateHandler(string trackingStateJson)
    {
        VLTrackingState state =
            VLJsonUtility.FromJson<VLTrackingState>(trackingStateJson);
        if (state != null && OnTrackingStates != null)
        {
            OnTrackingStates(state);
        }
    }

    private void PerformanceInfoHandler(string performanceInfoJson)
    {
        VLPerformanceInfo performanceInfo =
            VLJsonUtility.FromJson<VLPerformanceInfo>(performanceInfoJson);
        if (OnPerformanceInfo != null)
        {
            OnPerformanceInfo(performanceInfo);
        }
    }

    private void Awake()
    {
        // Get a handle to the current object and make sure, that the object
        // doesn't get deleted by the garbage collector. We then use this
        // handle as client data for the native callbacks. This allows us to
        // retrieve the current address of the actual object during the
        // callback execution. GCHandleType.Pinned is not necessary, because we
        // are accessing the address only through the handle object, which gets
        // stored in a global handle table.
        this.gcHandle = GCHandle.Alloc(this);

        // Print the version of the vlUnitySDK. If this works, then we can be
        // quite certain, that other things also work.

        string version;
        string versionTimestamp;
        string versionHash;
        if (VLUnitySdk.GetVersionString(out version) &&
            VLUnitySdk.GetVersionTimestampString(out versionTimestamp) &&
            VLUnitySdk.GetVersionHashString(out versionHash))
        {
            Debug.Log("[vlUnitySDK] v" + version + " (" +
                versionTimestamp + ", " + versionHash + ")");
        }
        else
        {
            Debug.LogWarning("[vlUnitySDK] Failed to get version strings");
        }

        // Construct the path to the directory with the tracking configurations,
        // if it wasn't specified explicitly
        if (String.IsNullOrEmpty(this.baseDir))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            this.baseDir = "file:///android_asset/VisionLib";
#else
            this.baseDir = Path.Combine(
                Application.streamingAssetsPath, "VisionLib");
#endif
        }

        // Construct the path to the Resources folder (the Resource folder is
        // only used by certain platforms)
#if UNITY_ANDROID && !UNITY_EDITOR
        this.resourceDir = "file:///android_asset/VisionLib/Resources";
#else
        this.resourceDir = Path.Combine(
            Application.streamingAssetsPath, "VisionLib/Resources");
#endif

        // Create an AbstractApplication, which will manage the ActionPipe and
        // DataSet

        this.aap = new VLAbstractApplicationWrapper();

        this.deviceInfo = this.aap.GetDeviceInfo();

// Set the path to the license file
#if UNITY_ANDROID && !UNITY_EDITOR
        string absoluteLicenseFilePath = Path.Combine(
            "file:///android_asset/", this.licenseFile.path);
#else
        string absoluteLicenseFilePath = Path.Combine(
            Application.streamingAssetsPath, this.licenseFile.path);
#endif
        
        this.aap.SetLicenseFilePath(absoluteLicenseFilePath);

        // Add a log listener, which will write all VisionLib logs to the
        // Unity console

        this.logger = new VLLogger();
#if UNITY_2017_1_OR_NEWER
        // Unity 2017 with Mono .NET 4.6 as scripting runtime version can't
        // properly handle callbacks from external threads. Until this is
        // fixed, we need to buffer the log messages and fetch them from the
        // main thread inside the update function.
        this.logger.EnableLogBuffer();
#else
        this.logger.DisableLogBuffer();
#endif
        this.logger.SetLogLevel(this.logLevel);

        // Print the host ID. This ID is needed for generating a license file.

        string hostID;
        if (this.aap.GetHostID(out hostID))
        {
            Debug.Log("[vlUnitySDK] HostID=" + hostID);
        }
        else
        {
            Debug.LogWarning("[vlUnitySDK] Failed to get host ID");
        }

        // Many VisionLib features are implemented as plugins, which we need
        // to load first

        string pluginPath = Application.dataPath +
            Path.DirectorySeparatorChar + "Plugins";

        // The plugins are in architecture specific sub-directories before the
        // deployment
#if UNITY_EDITOR
        pluginPath += Path.DirectorySeparatorChar + VLUnitySdk.subDir;
#endif
        this.aap.AutoLoadPlugins(pluginPath);

        // Create worker instance and register listeners for it

        this.worker = new VLWorker(this.aap);

        if (!this.worker.AddImageListener(
            dispatchImageCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to add image listener");
        }
        if (!this.worker.AddExtrinsicDataListener(
            dispatchExtrinsicDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to add extrinsic data listener");
        }
        if (!this.worker.AddIntrinsicDataListener(
            dispatchIntrinsicDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to add intrinsic data listener");
        }
        if (!this.worker.AddTrackingStateListener(
            dispatchTrackingStateCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to add tracking state listener");
        }
        if (!this.worker.AddPerformanceInfoListener(
            dispatchPerformanceInfoCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to add performance info listener");
        }
        
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        // Explicitly remove the listeners, so we know if everything went well
        if (!this.worker.RemovePerformanceInfoListener(
            dispatchPerformanceInfoCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to remove performance info listener");
        }
        if (!this.worker.RemoveTrackingStateListener(
            dispatchTrackingStateCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to remove tracking state listener");
        }
        if (!this.worker.RemoveIntrinsicDataListener(
            dispatchIntrinsicDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to remove intrinsic data listener");
        }
        if (!this.worker.RemoveExtrinsicDataListener(
            dispatchExtrinsicDataCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to remove extrinsic data listener");
        }
        if (!this.worker.RemoveImageListener(
            dispatchImageCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to remove image listener");
        }

        // Release the worker reference (this is necessary, because it
        // references native resources)
        this.worker.Dispose();
        this.worker = null;

        // Release the log listener, because we will add a new one during the
        // next call to Awake
        this.logger.Dispose();
        this.logger = null;

        // Release the AbstractApplication reference (this is necessary,
        // because it references native resources)
        this.aap.Dispose();
        this.aap = null;

        // Release the handle to the current object
        this.gcHandle.Free();
    }

    private void OnApplicationQuit()
    {
    }

    private void Update()
    {
        // if (!this.aap.ActivateFoundBlockedFeatures())
        // {
        //     Debug.LogError("[vlUnitySDK] Could not activate 'found blocked features'");
        // }

        // Log level changed?
        if (this.lastLogLevel != this.logLevel)
        {
            this.logger.SetLogLevel(this.logLevel);
            this.lastLogLevel = this.logLevel;
        }

        // Target FPS changed?
        if (this.lastTargetFPS != this.targetFPS)
        {
            this.lastTargetFPS = this.targetFPS;
            this.worker.PushCommand(
                new SetTargetFpsCmd(this.targetFPS),
                null,
                IntPtr.Zero);
        }

        this.worker.ProcessCallbacks();
        if (this.waitForEvents)
        {
            this.worker.WaitEvents(1000);
        }
        else
        {
            this.worker.PollEvents();
        }

#if UNITY_2017_1_OR_NEWER
        this.logger.FlushLogBuffer();
#endif
    }

#if UNITY_WSA_10_0
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseTrackingInternal();
        }
        else if (this.trackingRunning)
        {
            ResumeTracking();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PauseTrackingInternal();
        }
        else if (this.trackingRunning)
        {
            ResumeTracking();
        }
    }
#endif
}

/**@}*/
