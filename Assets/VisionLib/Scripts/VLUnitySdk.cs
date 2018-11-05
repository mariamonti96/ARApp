/** @defgroup vlUnitySDK vlUnitySDK API
 *  @brief The reference of the UnitySDK functions.
 * 
 * The unity plugin is created using the C# language and reflects the C-API. 
 * Thus a lot of functions might are familiar. Anyway, due to the nature of Unity and its C# adaption. 
 * Various callbacks and are different since they are put into behaviours.
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Static class for storing the global functions of the vlUnitySDK.
/// </summary>
public static class VLUnitySdk
{
#if UNITY_EDITOR
    // Use the plugin for the editor platform
    #if UNITY_EDITOR_WIN
        public const string dllName = "vlSDK";
    #elif UNITY_EDITOR_OSX
        public const string dllName = "libvlSDK";
    #elif UNITY_EDITOR_LINUX
        public const string dllName = "vlSDK";
    #else
        #warning Unsupported editor platform
        public const string dllName = "vlSDK";
    #endif

    // The plugin is in an architecture specific sub-directory before the
    // deployment
    #if UNITY_EDITOR_32
        public const string subDir = "x86";
    #elif UNITY_EDITOR_64
        public const string subDir = "x86_64";
    #else
        #warning Unsupported editor architecture
        public const string subDir = "x86_64";
    #endif
#else
    // Use the plugin for the build target
    #if UNITY_STANDALONE_WIN
        public const string dllName = "vlSDK";
    #elif UNITY_STANDALONE_OSX
        public const string dllName = "libvlSDK";
    #elif UNITY_IOS
        public const string dllName = "__Internal";
    #elif UNITY_ANDROID
        public const string dllName = "vlSDK";
        // Ensures that the AssetManager is not garbage collected
        private static AndroidJavaObject assetManager;
    #elif UNITY_WSA_10_0
        public const string dllName = "vlSDK";
    #else
        #warning Unsupported build target
        public const string dllName = "vlSDK";
    #endif

    // During the deployment Unity will copy the plugin from the architecture
    // specific sub-directory into the plugin directory
    public const string subDir = "";
#endif

    /// <summary>
    ///  Log levels.
    /// </summary>
    public enum LogLevel : int
    {
        /// <summary>Log level.</summary>
        Log = 0,
        /// <summary>Fatal level.</summary>
        Fatal = 1,
        /// <summary>Warning level.</summary>
        Warning = 2,
        /// <summary>Notice level.</summary>
        Notice = 3,
        /// <summary>Info level.</summary>
        Info = 4,
        /// <summary>Debug level.</summary>
        Debug = 5
    };

    /// <summary>
    ///  Internal image formats.
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>Unsupported image format</summary>
        Undefined = 0,
        /// <summary>Grey value image.</summary>
        Grey = 1,
        /// <summary>Image with a red, green and blue channel.</summary>
        RGB = 2,
        /// <summary>Image with a red, green, blue and alpha channel.</summary>
        RGBA = 3
    };

    // NOTICE: Make sure, that no exceptions escape from delegates, which are
    // called from unmanaged code
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void LogDelegate(
        [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr clientData);

    static VLUnitySdk()
    {
        // Add the plugin directory to the PATH environment variable, so that
        // the operating system can find the DLLs

        string pluginPath = Application.dataPath +
            Path.DirectorySeparatorChar + "Plugins";

        // The plugins are in architecture specific sub-directories before the
        // deployment
#if UNITY_EDITOR
        pluginPath += Path.DirectorySeparatorChar + VLUnitySdk.subDir;
#endif

#if !UNITY_WSA_10_0 // Environment not supported for WSA apps
        string oldPath = Environment.GetEnvironmentVariable("PATH",
            EnvironmentVariableTarget.Process);
        if (!oldPath.Contains(pluginPath))
        {
            // Update the PATH variable (only for the current process)
            Environment.SetEnvironmentVariable("PATH",
                pluginPath + Path.PathSeparator + oldPath,
                EnvironmentVariableTarget.Process);
        }
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Instantiate an AssetManager and pass it to VisionLib
        VLUnitySdk.assetManager = activity.Call<AndroidJavaObject>("getAssets");
        AndroidJavaClass platformClass = new AndroidJavaClass("org.instantreality.mini.Platform");
        platformClass.SetStatic<AndroidJavaObject>("context", activity);
        platformClass.CallStatic("init", assetManager);

        // Request Camera permissions
        AndroidJavaClass permission = new AndroidJavaClass("android.Manifest$permission");
        AndroidJavaClass permissionRequester = new AndroidJavaClass("org.instantreality.mini.PermissionRequester");
        permissionRequester.CallStatic("addPermissionToRequest", permission.GetStatic<string>("CAMERA"));
        permissionRequester.CallStatic("addPermissionToRequest", permission.GetStatic<string>("WRITE_EXTERNAL_STORAGE"));
        permissionRequester.CallStatic("requestPermissions", activity);
#endif
    }

    [DllImport (dllName)]
    private static extern System.UInt32 vlGetVersionMajor();
    /// <summary>
    ///  Returns the major version number of the VisionLib plugin.
    /// </summary>
    /// <returns>The major version number.</returns>
    public static uint GetMajorVersion()
    {
        return vlGetVersionMajor();
    }

    [DllImport (dllName)]
    private static extern System.UInt32 vlGetVersionMinor();
    /// <summary>
    ///  Returns the minor version number of the VisionLib plugin.
    /// </summary>
    /// <returns>The minor version number.</returns>
    public static uint GetMinorVersion()
    {
        return vlGetVersionMinor();
    }

    [DllImport (dllName)]
    private static extern System.UInt32 vlGetVersionRevision();
    /// <summary>
    ///  Returns the revision version number of the VisionLib plugin.
    /// </summary>
    /// <returns>The revision version number.</returns>
    public static uint GetRevisionVersion()
    {
        return vlGetVersionRevision();
    }

    [DllImport (dllName)]
    private static extern bool vlGetVersionString(
        StringBuilder version,
        System.UInt32 maxSize);
    /// <summary>
    ///  Returns the current version of the VisionLib plugin as a string.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if version was gotten, <c>false</c> otherwise.
    /// </returns>
    /// <param name="version">Output version string.</param>
    public static bool GetVersionString(out string version)
    {
        StringBuilder sb = new StringBuilder(128);
        if (!vlGetVersionString(sb, Convert.ToUInt32(sb.Capacity + 1)))
        {
            version = "";
            return false;
        }

        version = sb.ToString();

        return true;
    }

    [DllImport (dllName)]
    private static extern bool vlGetVersionHashString(
        StringBuilder hashString,
        System.UInt32 maxSize);
    /// <summary>
    ///  Returns the current version hash of the VisionLib plugin as a string.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if version hash was gotten, <c>false</c> otherwise.
    /// </returns>
    /// <param name="hashString">Output version hash string.</param>
    public static bool GetVersionHashString(out string hashString)
    {
        StringBuilder sb = new StringBuilder(128);
        if (!vlGetVersionHashString(sb, Convert.ToUInt32(sb.Capacity + 1)))
        {
            hashString = "";
            return false;
        }

        hashString = sb.ToString();

        return true;
    }
    
    [DllImport (dllName)]
    private static extern bool vlGetVersionTimestampString(
        StringBuilder versionTimestamp,
        System.UInt32 maxSize);
    /// <summary>
    ///  Returns the current build time of the VisionLib plugin as a string.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if version timestamp was gotten, <c>false</c> otherwise.
    /// </returns>
    /// <param name="versionTimestamp">Output version timestamp.</param>
    public static bool GetVersionTimestampString(out string versionTimestamp)
    {
        StringBuilder sb = new StringBuilder(128);
        if (!vlGetVersionTimestampString(sb, Convert.ToUInt32(sb.Capacity + 1)))
        {
            versionTimestamp = "";
            return false;
        }

        versionTimestamp = sb.ToString();

        return true;
    }

    [DllImport (dllName)]
    private static extern bool vlAddLogListener(
       [MarshalAs(UnmanagedType.FunctionPtr)] LogDelegate listener,
       IntPtr clientData);
    /// <summary>
    ///  Registers a log listener.
    /// </summary>
    /// <remarks>
    ///  Please make sure, that no exceptions escape from the log listener as
    ///  this would collide with the exception handling of the unmanaged code.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if log listener was added successfully,
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="listener">
    ///  Listener which will receive all VisionLib log messages.
    /// </param>
    /// <param name="clientData">
    ///  The listener will be notified with the given pointer value.
    /// </param>
    public static bool AddLogListener(LogDelegate listener, IntPtr clientData)
    {
        return vlAddLogListener(listener, clientData);
    }

    [DllImport (dllName)]
    private static extern bool vlRemoveLogListener(
        [MarshalAs(UnmanagedType.FunctionPtr)] LogDelegate listener,
        IntPtr clientData);
    /// <summary>
    ///  Unregister the given log listener.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if log listener was removed successfully,
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="listener">
    ///  Listener, which should be removed.
    /// </param>
    /// <param name="clientData">
    ///  Pointer value which was used during the registration.
    /// </param>
    public static bool RemoveLogListener(LogDelegate listener,
        IntPtr clientData)
    {
        return vlRemoveLogListener(listener, clientData);
    }

    [DllImport (dllName)]
    private static extern bool vlClearLogListeners();
    /// <summary>
    ///  Removes all log listeners.
    /// </summary>
    /// <returns>
    ///  <c>true</c>, if all log listener were removed successfully,
    ///  <c>false</c> otherwise.
    /// </returns>
    public static bool ClearLogListeners()
    {
        return vlClearLogListeners();
    }

    [DllImport (dllName)]
    private static extern void vlEnableLogBuffer();
    /// <summary>
    ///  Enables log buffering.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If log buffering is enabled, then log messages will not get dispatched
    ///   immediately. Instead they will get buffered and one must continuously
    ///   call the vlFlushLogBuffer function in order to dispatch the buffered log
    ///   messages to the registered listeners.
    ///  </para>
    ///  <para>
    ///   This has the advantage, that registered listeners will not get notified
    ///   from some arbitrary thread, which would require proper thread
    ///   synchronization.
    ///  </para>
    ///  <para>
    ///   By default log buffering is disabled.
    ///  </para>
    /// </remarks>
    public static void EnableLogBuffer()
    {
        vlEnableLogBuffer();
    }

    [DllImport (dllName)]
    private static extern void vlDisableLogBuffer();
    /// <summary>
    ///  Disables log buffering.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If log buffering is disabled, then log messages will get dispatched
    ///   immediately. This might happen from a different thread. Therefore one
    ///   must make sure, that registered log listeners are thread-safe.
    ///  </para>
    ///  <para>
    ///   With disabled log buffering, calling the vlFlushLogBuffer function is
    ///   not necessary.
    ///  </para>
    ///  <para>
    ///   By default log buffering is disabled.
    ///  </para>
    /// </remarks>
    public static void DisableLogBuffer()
    {
        vlDisableLogBuffer();
    }

    [DllImport (dllName)]
    private static extern void vlSetLogBufferSize(System.UInt32 maxEntries);
    /// <summary>
    ///  Sets the maximum number of log messages in the log buffer.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If log buffering is enabled, then log messages will get buffered. In
    ///   order to not allocate too much memory, the size of the log buffer is
    ///   limited to a certain number of entries. If there are too many log
    ///   messages in the buffer, then the oldest message will get removed.
    ///  </para>
    ///  <para>
    ///   By default the maximum number of buffer entries is 32.
    ///  </para>
    /// </remarks>
    public static void SetLogBufferSize(uint maxEntries)
    {
        vlSetLogBufferSize(maxEntries);
    }

    [DllImport (dllName)]
    private static extern bool vlFlushLogBuffer();
    /// <summary>
    ///  Notifies registered log listeners of all buffered log messages.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If log buffering is enabled, then log messages will not get dispatched
    ///   immediately. Instead they will get buffered and one must continuously
    ///   call the vlFlushLogBuffer function in order to dispatch the buffered log
    ///   messages to the registered listeners.
    ///  </para>
    ///  <para>
    ///   Failing to call vlFlushLogBuffer with enabled log buffering will have
    ///   the effect, that registered log listeners will not get notified of any
    ///   log messages and old log messages will be lost forever.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, on success <c>false</c> otherwise.
    /// </returns>
    public static bool FlushLogBuffer()
    {
        return vlFlushLogBuffer();
    }

    [DllImport (dllName)]
    private static extern System.Int32 vlGetLogLevel();
    /// <summary>
    ///  Gets the current log level.
    /// </summary>
    /// <returns>
    ///  The log level (0: log, 1: fatal, 2: warning, 3: notice, 4: info,
    ///  5: debug).
    /// </returns>
    public static LogLevel GetLogLevel()
    {
        return (LogLevel)vlGetLogLevel();
    }

    [DllImport (dllName)]
    private static extern bool vlSetLogLevel(System.Int32 level);
    /// <summary>
    ///  Sets the log level.
    /// </summary>
    /// <remarks>
    ///  It is recommended to set the log level during development to 2,
    ///  otherwise there will be too many messages. Only for debugging purposes
    ///  it might be useful to increase the log level. Before deploying your
    ///  application you should set the log level to 0 or 1.
    /// </remarks>
    /// <returns>
    ///  <c>true</c>, if log level was set successfully,
    ///  <c>false</c> otherwise.
    /// </returns>
    /// <param name="level">New log level (<see cref="GetLogLevel"/> for a
    /// description of the different levels)</param>
    public static bool SetLogLevel(LogLevel level)
    {
        return vlSetLogLevel((int)level);
    }

    [DllImport (dllName)]
    private static extern bool vlLog(
        [MarshalAs(UnmanagedType.LPStr)] string message,
        System.Int32 level);
    /// <summary>
    /// Logs the given message as VisionLib log.
    /// </summary>
    /// <param name="message">Message, which should be logged.</param>
    /// <param name="level">Log level (<see cref="GetLogLevel"/> for a
    /// description of the different levels)</param>
    public static bool Log(string message, LogLevel level)
    {
        return vlLog(message, (int)level);
    }

    [DllImport (dllName)]
    private static extern void vlSetScreenOrientation(
        System.Int32 orientation);
    public static void SetScreenOrientation(int orientation)
    {
        vlSetScreenOrientation(orientation);
    }
}

/**@}*/