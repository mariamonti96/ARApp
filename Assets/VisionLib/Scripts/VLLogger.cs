/** @addtogroup vlUnitySDK
 *  @{
 */

using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
///  ...
/// </summary>
public class VLLogger: IDisposable
{
    [MonoPInvokeCallback(typeof(VLUnitySdk.LogDelegate))]
    private static void DispatchLogCallback(string message, IntPtr clientData)
    {
        try
        {
            VLLogger logger = (VLLogger)GCHandle.FromIntPtr(clientData).Target;
            logger.LogHandler(message);
        }
        catch (Exception e) // Catch all exceptions, because this is a callback
                            // invoked from native code
        {
            Debug.LogError("[vlUnitySDK] " + e.GetType().Name + ": " +
                e.Message);
        }
    }
    private static VLUnitySdk.LogDelegate dispatchLogCallbackDelegate =
        new VLUnitySdk.LogDelegate(DispatchLogCallback);

    private GCHandle gcHandle;
    private bool disposed = false;

    public VLLogger()
    {
        this.gcHandle = GCHandle.Alloc(this);

        if (!VLUnitySdk.AddLogListener(dispatchLogCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to add log listener");
        }
    }

    ~VLLogger()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

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
        if (!VLUnitySdk.RemoveLogListener(dispatchLogCallbackDelegate,
            GCHandle.ToIntPtr(this.gcHandle)))
        {
            Debug.LogWarning("[vlUnitySDK] Failed to remove log listener");
        }

        // Release the handle to the current object
        this.gcHandle.Free();

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLLogger"/>. The <see cref="Dispose"/> method leaves
    ///  the <see cref="VLLogger"/> in an unusable state. After calling
    ///  <see cref="Dispose"/>, you must release all references to the
    ///  <see cref="VLLogger"/> so the garbage collector can reclaim the
    ///  memory that the <see cref="VLLogger"/> was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    private void LogHandler(string message)
    {
        Debug.Log("[VisionLib] " + message);
    }

    public void SetLogBufferSize(int maxEntries)
    {
        if (maxEntries < 0)
        {
            Debug.LogWarning("[vlUnitySDK] LogBufferSize must be zero or larger");
            return;
        }
        VLUnitySdk.SetLogBufferSize(Convert.ToUInt32(maxEntries));
    }

    public void EnableLogBuffer()
    {
        VLUnitySdk.EnableLogBuffer();
    }

    public void DisableLogBuffer()
    {
        VLUnitySdk.DisableLogBuffer();
    }

    public bool FlushLogBuffer()
    {
        return VLUnitySdk.FlushLogBuffer();
    }

    public VLUnitySdk.LogLevel GetLogLevel()
    {
        return VLUnitySdk.GetLogLevel();
    }

    public bool SetLogLevel(VLUnitySdk.LogLevel level)
    {
        return VLUnitySdk.SetLogLevel(level);
    }

    public bool Log(string message, VLUnitySdk.LogLevel level)
    {
        return VLUnitySdk.Log(message, level);
    }
}

/**@}*/