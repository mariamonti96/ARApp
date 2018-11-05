using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  This behaviour allows you to select one of the connected cameras (if more
///  than one camera is connected) before loading the tracking configuration.
/// </summary>
/// <remarks>
///  <para>
///   This is just an example to show you how a camera selection could be
///   implemented. Please feel free to write you own camera selection.
///  </para>
///  <para>
///   NOTICE: The camera selection will only work correctly, if the vl-file
///   doesn't specify the "useImageSource" attribute.
///  </para>
/// </remarks>
public class StartTrackingWithCameraSelectionBehaviour : MonoBehaviour
{
    /// <summary>
    ///  Reference to used VLWorkerBehaviour.
    /// </summary>
    /// <remarks>
    ///  If this is not defined, then the first found VLWorkerBehaviour will be
    ///  used automatically.
    /// </remarks>
    public VLWorkerBehaviour workerBehaviour;

    /// <summary>
    ///  A unique ID used for the selection window.
    /// </summary>
    /// <remarks>
    ///  Set this to a unique value, if you also use the GUI.Window or
    ///  GUILayout.Window functions with the default ID.
    /// </remarks>
    public int windowID = 0;

    /// <summary>
    ///  Stores the device information.
    /// </summary>
    private VLDeviceInfo deviceInfo = null;

    /// <summary>
    ///  Stores to URI to the tracking configuration file.
    /// </summary>
    private string trackingConfigURI = null;

    /// <summary>
    ///  Whether to show the camera selection window.
    /// </summary>
    private bool windowOpen = false;

    /// <summary>
    ///  Rectangle for the camera selection window. The actual values will get
    ///  determined automatically at runtime.
    /// </summary>
    private Rect windowRect;

    /// <summary>
    ///  Used to scale the UI inside the OnGUI function.
    /// </summary>
    private GUIMatrixScaler guiScaler = new GUIMatrixScaler(640, 480);

    /// <summary>
    ///  Stores whether the initialization was successful and the behaviour is
    ///  ready to work.
    /// </summary>
    private bool initialized = false;

    /// <summary>
    ///  Display the camera selection screen and starts the tracking using a
    ///  vl-file after selecting a camera.
    /// </summary>
    /// <remarks>
    ///  NOTICE: The camera selection will only work correctly, if the vl-file
    ///  doesn't specify the "useImageSource" attribute.
    /// </remarks>
    public void StartTracking(string filename)
    {
        this.trackingConfigURI = filename;
        this.windowOpen = true;
    }

    /// <summary>
    ///  Acquires a reference to the VLWorkerBehavioiur and gathers the
    ///  VLDeviceInfo.
    /// </summary>
    private void Init()
    {
        // VLWorkerBeahaviour not specified explicitly?
        if (this.workerBehaviour == null)
        {
            // Try to get the VLWorkerBehaviour from the current GameObject
            this.workerBehaviour = GetComponent<VLWorkerBehaviour>();

            // If the current GameObject doesn't have a VLWorkerBehaviour
            // attached, just use the first VLWorkerBehaviour from anywhere in
            // the scene
            if (this.workerBehaviour == null)
            {
                this.workerBehaviour = FindObjectOfType<VLWorkerBehaviour>();

                // Failed to get VLWorkerBehaviour?
                if (this.workerBehaviour == null)
                {
                    return;
                }
            }
        }

        // Try to get the VLDeviceInfo
        this.deviceInfo = this.workerBehaviour.GetDeviceInfo();
        if (this.deviceInfo == null)
        {
            return;
        }

        this.initialized = true;
    }

    private void Start ()
    {
        Init();
    }

    private void Update()
    {
        // Just in case the initialization failed during the Start, try to
        // initialize again
        if (!this.initialized)
        {
            Init();
        }
    }

    private void StartTrackingWithCamera(string deviceID)
    {
        this.windowOpen = false;

        if (this.trackingConfigURI == null)
        {
            return;
        }

        // Tell the tracker to use the selected device, by specifying the
        // "useDeviceID" parameter as query string
        string newURI = trackingConfigURI;
        if (deviceID != null)
        {
            bool hasParameters = trackingConfigURI.Contains("?");
            if (hasParameters)
            {
                newURI = this.trackingConfigURI + "&input.useDeviceID="
                    + deviceID;
            }
            else
            {
                newURI = this.trackingConfigURI + "?input.useDeviceID="
                    + deviceID;
            }
        }

        this.workerBehaviour.StartTracking(newURI);
    }

    private void DoWindow(int windowID)
    {
        if (!this.initialized)
        {
            return;
        }

        // Add a button for each camera
        foreach (VLDeviceInfo.Camera camera in  this.deviceInfo.availableCameras)
        {
            if (GUILayout.Button(camera.cameraName))
            {
                StartTrackingWithCamera(camera.deviceID);
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            this.windowOpen = false;
        }
    }

    private void OnGUI ()
    {
        // Camera selection window closed?
        if (!this.windowOpen)
        {
            return;
        }

        // Don't do anything if the initialization wasn't successful so far
        if (!this.initialized)
        {
            return;
        }

        this.guiScaler.Update();
        this.guiScaler.Set();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA_10_0
        // More than one camera connected?
        if (this.deviceInfo.availableCameras.Length >= 2)
        {
            // Display a window with all available cameras
            // (We call GUILayout.Window twice. In order to properly position
            // the window in the centre of the screen)
            this.windowRect = GUILayout.Window(
                this.windowID, this.windowRect, DoWindow, "Select your camera");
            this.windowRect.x =
                (this.guiScaler.GetScaledScreenRect().width - this.windowRect.width) / 2.0f;
            this.windowRect.y =
                (this.guiScaler.GetScaledScreenRect().height - this.windowRect.height) / 2.0f;
            this.windowRect = GUILayout.Window(
                this.windowID, this.windowRect, DoWindow, "Select your camera");
        }
        else
        {
            // There is at most one camera, just use it
            StartTrackingWithCamera(null);
        }
#else
        // Let the device automatically select the device to be used
        StartTrackingWithCamera(null);
#endif

        this.guiScaler.Unset();
    }
}