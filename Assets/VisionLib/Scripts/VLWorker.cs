/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/// <summary>
///  The VLWorker is a wrapper for an Worker object. The Worker object manages
///  the tracking thread.
/// </summary>
public class VLWorker: IDisposable
{
    // NOTICE: Make sure, that no exceptions escape from delegates, which are
    // called from unmanaged code
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void BoolCallback(bool data, IntPtr clientData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void StringCallback(
        [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr clientData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void JsonStringCallback(
        [MarshalAs(UnmanagedType.LPStr)] string errorJson,
        [MarshalAs(UnmanagedType.LPStr)] string dataJson,
        IntPtr clientData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void ImageWrapperCallback(IntPtr handle, IntPtr clientData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void ExtrinsicDataWrapperCallback(IntPtr handle,
        IntPtr clientData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void IntrinsicDataWrapperCallback(IntPtr handle,
        IntPtr clientData);

    private IntPtr handle;
    private bool disposed = false;
    private bool owner;

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlNew_Worker(IntPtr aap);
    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlNew_SyncWorker(IntPtr aap);
    /// <summary>
    ///  Constructor of VLWorker.
    /// </summary>
    public VLWorker(VLAbstractApplicationWrapper aap, bool synchronous=false)
    {
        if (!synchronous)
        {
            this.handle = vlNew_Worker(aap.GetHandle());
        }
        else
        {
            this.handle = vlNew_SyncWorker(aap.GetHandle());
        }
        this.owner = true;
    }

    ~VLWorker()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    public bool GetDisposed()
    {
        return this.disposed;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_Worker(IntPtr worker);
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
            vlDelete_Worker(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLWorker"/>. The <see cref="Dispose"/> method leaves
    ///  the <see cref="VLWorker"/> in an unusable state. After calling
    ///  <see cref="Dispose"/>, you must release all references to the
    ///  <see cref="VLWorker"/> so the garbage collector can reclaim the
    ///  memory that the <see cref="VLWorker"/> was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_Start(IntPtr worker);
    /// <summary>
    ///  Starts the tracking thread.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the thread was started successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool Start()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_Start(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_Stop(IntPtr worker);
    /// <summary>
    ///  Stops the tracking thread.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the thread was stopped successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool Stop()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_Stop(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RunOnceSync(IntPtr worker);
    /// <summary>
    ///  Processes the enqueued commands and the tracking once.
    /// </summary>
    /// <remarks>
    ///  This function only works, if the Worker was created as synchronous
    ///  instance. The target number of FPS will get ignored. After calling
    ///  this function you should call VLWorker.ProcessCallbacks and
    ///  VLWorker.PollEvents to invoke callbacks and registered listeners.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, on success;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool RunOnceSync()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RunOnceSync(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlWorker_GetImageSync(IntPtr worker);
    /// <summary>
    ///  Returns a pointer to the camera image.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This function only works, if the Worker was created as synchronous
    ///   instance.
    ///  </para>
    ///  <para>
    ///   NOTICE: This functions is experimental and might get removed in
    ///   future.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>VLImageWrapper</c>, on success;
    ///  <c>null</c>, otherwise.
    /// </returns>
    public VLImageWrapper GetImageSync()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        IntPtr imageHandle = vlWorker_GetImageSync(this.handle);
        if (imageHandle != IntPtr.Zero)
        {
            return new VLImageWrapper(imageHandle, false);
        }
        else
        {
            return null;
        }
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_IsRunning(IntPtr worker);
    /// <summary>
    ///  Returns whether the thread is currently running or not.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if the thread is running;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool IsRunning()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_IsRunning(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_PushCommand(IntPtr worker,
        [MarshalAs(UnmanagedType.LPStr)] string name,
        [MarshalAs(UnmanagedType.LPStr)] string parameter,
        [MarshalAs(UnmanagedType.FunctionPtr)] BoolCallback callback,
        IntPtr clientData);
    /// <summary>
    ///  Enqueues a command for the tracking thread as string.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The command gets processed asynchronously by the tracking thread and
    ///   a callback will called once after the processing has finished.
    ///  </para>
    ///  <para>
    ///   The following tracking independent commands exist:
    ///   * createLineTracker &lt;trackingFile&gt;: Creates a line tracker.
    ///   * runTracking: Starts the tracking.
    ///   * pauseTracking: Stops the tracking.
    ///   * setTargetFPS &lt;fps&gt;: Sets the target number of frames per
    ///     seconds of the tracking thread.
    ///  </para>
    ///  <para>
    ///   The following commands exist for the line tracker:
    ///   * (Deprecated) reset: Resets the tracking pose to the initial pose.
    ///   * resetSoft: Resets the tracking.
    ///   * resetHard: Resets the tracking and all keyframes.
    ///   * setInitPose: Set the initial pose. The parameter must have the
    ///     following structure: t_{x} t_{y} t_{z} q_{x} q_{y} q_{z} q_{w}.
    ///  </para>
    /// </remarks>
    /// <param name="name">
    ///  Name of the command.
    /// </param>
    /// <param name="parameter">
    ///  Parameter for the command. Value will be ignored, if no parameters are
    ///  expected by the command.
    /// </param>
    /// <param name="callback">
    ///  Callback, which will be called inside <see cref="ProcessCallbacks"/>
    ///  after the command was processed.
    /// </param>
    /// <param name="clientData">
    ///  The callback function will be called with the given pointer value as
    ///  parameter.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the command was enqueue successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool PushCommand(string name, string parameter,
        BoolCallback callback, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_PushCommand(
            this.handle, name, parameter, callback, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_PushJsonCommand(IntPtr worker,
        [MarshalAs(UnmanagedType.LPStr)] string jsonString,
        [MarshalAs(UnmanagedType.FunctionPtr)] JsonStringCallback callback,
        IntPtr clientData);
    /// <summary>
    ///  Enqueues a command for the tracking thread using a JSON string.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The command gets processed asynchronously by the tracking thread and
    ///   a callback will called once after the processing has finished.
    ///  </para>
    ///  <para>
    ///   You need to make sure, that the JSON string has the expected format.
    ///   Therefore you might want to use the <see cref="VLWorker.PushCommand"/>
    ///   functions instead. This function will ensure that the command will be
    ///   pushed as expected.
    ///  </para>
    /// </remarks>
    /// <param name="jsonString">
    ///  The command with all necessary data as JSON string.
    /// </param>
    /// <param name="callback">
    ///  Callback, which will be called inside <see cref="ProcessCallbacks"/>
    ///  after the command was processed.
    /// </param>
    /// <param name="clientData">
    ///  The callback function will be called with the given pointer value.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the command was enqueue successfully;
    ///  <c>false</c> otherwise (usually some JSON syntax error).
    /// </returns>
    public bool PushJsonCommand(string jsonString, JsonStringCallback callback,
        IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_PushJsonCommand(
            this.handle, jsonString, callback, clientData);
    }

    /// <summary>
    ///  Enqueues a command for the tracking thread.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The command gets processed asynchronously by the tracking thread and
    ///   a callback will called once after the processing has finished.
    ///  </para>
    ///  <para>
    ///   The different commands are defined inside the
    ///   <see cref="VLWorkerCommands"/> namespace.
    ///  </para>
    /// </remarks>
    /// <param name="cmd">
    ///  The command object.
    /// </param>
    /// <param name="callback">
    ///  Callback, which will be called inside <see cref="ProcessCallbacks"/>
    ///  after the command was processed.
    /// </param>
    /// <param name="clientData">
    ///  The callback function will be called with the given pointer value.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the command was enqueue successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool PushCommand(VLWorkerCommands.CommandBase cmd,
        JsonStringCallback callback, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_PushJsonCommand(this.handle,
            VLJsonUtility.ToJson(cmd), callback, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_ProcessCallbacks(IntPtr worker);
    /// <summary>
    ///  Executes all enqueued callbacks.
    /// </summary>
    /// <remarks>
    ///  Callbacks aren't called immediately from the tracking thread in
    ///  order to avoid synchronisation problems. Instead this method should
    ///  be called regularly from the main thread.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the command was enqueue successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="PushCommand"/>
    public bool ProcessCallbacks()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_ProcessCallbacks(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_AddImageListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] ImageWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Registers a listener for image events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which will be notified during the event processing,
    ///  if an image event occurred.
    /// </param>
    /// <param name="clientData">
    ///  The listener function will be called with the given pointer value as
    ///  parameter.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was registered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool AddImageListener(ImageWrapperCallback listener,
        IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_AddImageListener(this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RemoveImageListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] ImageWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregisters a listener from image events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which should be unregistered.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value used as parameter during the registration of the
    ///  listener.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was unregistered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool RemoveImageListener(ImageWrapperCallback listener,
        IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RemoveImageListener(this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_AddExtrinsicDataListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] ExtrinsicDataWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Registers a listener for extrinsic data events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which will be notified during the event processing,
    ///  if an extrinsic data event occurred.
    /// </param>
    /// <param name="clientData">
    ///  The listener function will be called with the given pointer value as
    ///  parameter.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was registered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLExtrinsicDataWrapper"/>
    public bool AddExtrinsicDataListener(ExtrinsicDataWrapperCallback listener,
        IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_AddExtrinsicDataListener(
            this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RemoveExtrinsicDataListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] ExtrinsicDataWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregisters a listener from extrinsic data events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which should be unregistered.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value used as parameter during the registration of the
    ///  listener.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was unregistered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool RemoveExtrinsicDataListener(
        ExtrinsicDataWrapperCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RemoveExtrinsicDataListener(
            this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_AddIntrinsicDataListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] IntrinsicDataWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Registers a listener for intrinsic data events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which will be notified during the event processing,
    ///  if an intrinsic data event occurred.
    /// </param>
    /// <param name="clientData">
    ///  The listener function will be called with the given pointer value as
    ///  argument.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was registered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLIntrinsicDataWrapper"/>
    public bool AddIntrinsicDataListener(IntrinsicDataWrapperCallback listener,
        IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_AddIntrinsicDataListener(
            this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RemoveIntrinsicDataListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] IntrinsicDataWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregisters a listener from intrinsic data events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which should be unregistered.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value used as parameter during the registration of the
    ///  listener.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was unregistered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool RemoveIntrinsicDataListener(
        IntrinsicDataWrapperCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RemoveIntrinsicDataListener(
            this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_AddTrackingStateListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Registers a listener for tracking state events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which will be notified during the event processing,
    ///  if an tracking state event occurred.
    /// </param>
    /// <param name="clientData">
    ///  The listener function will be called with the given pointer value as
    ///  parameter.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was registered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool AddTrackingStateListener(StringCallback listener,
        IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_AddTrackingStateListener(
            this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RemoveTrackingStateListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregisters a listener from tracking state events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which should be unregistered.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value used as parameter during the registration of the
    ///  listener.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was unregistered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool RemoveTrackingStateListener(StringCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RemoveTrackingStateListener(
            this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_AddPerformanceInfoListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Registers a listener for performance information events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which will be notified during the event processing,
    ///  if a performance info state event occurred.
    /// </param>
    /// <param name="clientData">
    ///  The listener function will be called with the given pointer value as
    ///  parameter.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was registered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    public bool AddPerformanceInfoListener(StringCallback listener,
        IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_AddPerformanceInfoListener(
            this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RemovePerformanceInfoListener(IntPtr worker,
        [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregisters a listener from performance info events.
    /// </summary>
    /// <param name="listener">
    ///  Listener which should be unregistered.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value used as parameter during the registration of the
    ///  listener.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was unregistered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool RemovePerformanceInfoListener(StringCallback listener,
        IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RemovePerformanceInfoListener(
            this.handle, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_AddNamedImageListener(IntPtr worker,
        [MarshalAs(UnmanagedType.LPStr)] string key,
        [MarshalAs(UnmanagedType.FunctionPtr)] ImageWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Registers a listener for named image events.
    /// </summary>
    /// <param name="key">
    ///  Key of the requested image.
    /// </param>
    /// <param name="listener">
    ///  Listener which will be notified during the event processing,
    ///  if a named image event occurred.
    /// </param>
    /// <param name="clientData">
    ///  The listener function will be called with the given pointer value as
    ///  parameter.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was registered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool AddNamedImageListener(string key,
        ImageWrapperCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_AddNamedImageListener(
            this.handle, key, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RemoveNamedImageListener(IntPtr worker,
        [MarshalAs(UnmanagedType.LPStr)] string key,
        [MarshalAs(UnmanagedType.FunctionPtr)] ImageWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregisters a listener from named image events.
    /// </summary>
    /// <param name="key">
    ///  Key of the requested image.
    /// </param>
    /// <param name="listener">
    ///  Listener which should be unregistered.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value used as parameter during the registration of the
    ///  listener.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was unregistered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool RemoveNamedImageListener(string key,
        ImageWrapperCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RemoveNamedImageListener(
            this.handle, key, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_AddNamedExtrinsicDataListener(
        IntPtr worker,
        [MarshalAs(UnmanagedType.LPStr)] string key,
        [MarshalAs(UnmanagedType.FunctionPtr)] ExtrinsicDataWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Registers a listener for named extrinsic data events.
    /// </summary>
    /// <param name="key">
    ///  Key of the requested extrinsic data.
    /// </param>
    /// <param name="listener">
    ///  Listener which will be notified during the event processing,
    ///  if a named extrinsic data event occurred.
    /// </param>
    /// <param name="clientData">
    ///  The listener function will be called with the given pointer value as
    ///  parameter.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was registered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool AddNamedExtrinsicDataListener(string key,
        ExtrinsicDataWrapperCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_AddNamedExtrinsicDataListener(
            this.handle, key, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RemoveNamedExtrinsicDataListener(IntPtr worker,
        [MarshalAs(UnmanagedType.LPStr)] string key,
        [MarshalAs(UnmanagedType.FunctionPtr)] ExtrinsicDataWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregisters a listener from named extrinsic data events.
    /// </summary>
    /// <param name="key">
    ///  Key of the requested extrinsic data.
    /// </param>
    /// <param name="listener">
    ///  Listener which should be unregistered.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value used as parameter during the registration of the
    ///  listener.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was unregistered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool RemoveNamedExtrinsicDataListener(string key,
        ExtrinsicDataWrapperCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RemoveNamedExtrinsicDataListener(
            this.handle, key, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_AddNamedIntrinsicDataListener(
        IntPtr worker,
        [MarshalAs(UnmanagedType.LPStr)] string key,
        [MarshalAs(UnmanagedType.FunctionPtr)] IntrinsicDataWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Registers a listener for named intrinsic data events.
    /// </summary>
    /// <param name="key">
    ///  Key of the requested intrinsic data.
    /// </param>
    /// <param name="listener">
    ///  Listener which will be notified during the event processing,
    ///  if a named intrinsic data event occurred.
    /// </param>
    /// <param name="clientData">
    ///  The listener function will be called with the given pointer value as
    ///  parameter.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was registered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool AddNamedIntrinsicDataListener(string key,
        IntrinsicDataWrapperCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_AddNamedIntrinsicDataListener(
            this.handle, key, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_RemoveNamedIntrinsicDataListener(IntPtr worker,
        [MarshalAs(UnmanagedType.LPStr)] string key,
        [MarshalAs(UnmanagedType.FunctionPtr)] IntrinsicDataWrapperCallback listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregisters a listener from named intrinsic data events.
    /// </summary>
    /// <param name="key">
    ///  Key of the requested intrinsic data.
    /// </param>
    /// <param name="listener">
    ///  Listener which should be unregistered.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value used as parameter during the registration of the
    ///  listener.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the listener was unregistered successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="VLImageWrapper"/>
    public bool RemoveNamedIntrinsicDataListener(string key,
        IntrinsicDataWrapperCallback listener, IntPtr clientData)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_RemoveNamedIntrinsicDataListener(
            this.handle, key, listener, clientData);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_PollEvents(IntPtr worker);
    /// <summary>
    ///  Calls the registered listeners for the enqueued events.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Listeners aren't called immediately from the tracking thread in
    ///   order to avoid synchronisation problems. Instead this method should
    ///   be called regularly from the main thread.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if the events where processed successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="AddImageListener"/>
    /// <seealso cref="AddExtrinsicDataListener"/>
    /// <seealso cref="AddIntrinsicDataListener"/>
    public bool PollEvents()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_PollEvents(this.handle);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern bool vlWorker_WaitEvents(IntPtr worker,
        System.UInt32 timeout);
    /// <summary>
    ///  Waits for enqueued events and calls the registered listeners.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Listeners aren't called immediately from the tracking thread in
    ///   order to avoid synchronisation problems. Instead this method should
    ///   be called regularly from the main thread.
    ///  </para>
    /// </remarks>
    /// <param name="timeout">
    ///  Number of milliseconds before stopping to wait. Under normal
    ///  circumstances this shouldn't happen, but in case something went wrong,
    ///  we don't want to wait indefinitely.
    /// </param>
    /// <returns>
    ///  <c>true</c>, if the events where processed successfully;
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="AddImageListener"/>
    /// <seealso cref="AddExtrinsicDataListener"/>
    /// <seealso cref="AddIntrinsicDataListener"/>
    public bool WaitEvents(uint timeout)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLWorker");
        }

        return vlWorker_WaitEvents(this.handle, timeout);
    }

    // [DllImport (VLUnitySdk.dllName)]
    // private static extern bool vlWorker_Lock(IntPtr worker);
    // public bool Lock()
    // {
    //     if (this.disposed)
    //     {
    //         throw new ObjectDisposedException("VLWorker");
    //     }

    //     return vlWorker_Lock(this.handle);
    // }

    // [DllImport (VLUnitySdk.dllName)]
    // private static extern bool vlWorker_Unlock(IntPtr worker);
    // public bool Unlock()
    // {
    //     if (this.disposed)
    //     {
    //         throw new ObjectDisposedException("VLWorker");
    //     }

    //     return vlWorker_Unlock(this.handle);
    // }
}

/**@}*/