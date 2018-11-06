using System;
using UnityEngine;

public class CameraCalibrationExampleBehaviour : MonoBehaviour
{
    private enum State
    {
        CameraSelection,
        Calibration
    };

    /// <summary>
    ///  Reference to used VLWorkerBehaviour.
    /// </summary>
    /// <remarks>
    ///  If this is not defined, then the VLWorkerBehaviour will get found
    //   automatically.
    /// </remarks>
    public VLWorkerBehaviour workerBehaviour = null;

    /// <summary>
    ///  Location used to store the camera calibration.
    /// </summary>
    /// <remarks>
    ///  The <c>local_storage_dir</c> scheme can be, which points to a writable
    ///  location for each platform:
    ///  * Windows: Current users home directory
    ///  * MacOS: Current users document directory
    ///  * iOS / Android: The current applications document directory
    /// </remarks>
    public string destinationURI = "local_storage_dir:calibration.json";

    /// <summary>
    ///  Whether to offer a resolution selection.
    /// </summary>
    /// <remarks>
    ///  If this is <c>false</c>, then a default resolution will be used.
    ///  If this is <c>true</c>, then the user can select a specific
    //   resolution, which will be used for the calibration.
    /// </remarks>
    public bool resolutionSelection = false;

    private State state = State.CameraSelection;
    private VLDeviceInfo deviceInfo = null;
    private CameraResolutionSelector resolutionSelector = null;

    private string calibState = "";
    private int frameCount = 0;

    /// <summary>
    ///  Used to scale the UI inside the OnGUI function.
    /// </summary>
    private GUIMatrixScaler guiScaler = new GUIMatrixScaler(640, 480);

    private bool InitWorker()
    {
        // VLWorkerBeahaviour specified explicitly or previously found?
        if (this.workerBehaviour != null)
        {
            return true;
        }

        // Try to get the VLWorkerBehaviour from the current GameObject
        this.workerBehaviour = GetComponent<VLWorkerBehaviour>();
        if (this.workerBehaviour != null)
        {
            return true;
        }

        // If the current GameObject doesn't have a VLWorkerBehaviour
        // attached, just use the first VLWorkerBehaviour from anywhere in
        // the scene
        this.workerBehaviour = FindObjectOfType<VLWorkerBehaviour>();
        if (this.workerBehaviour != null)
        {
            return true;
        }

        return false;
    }

    private bool InitDeviceInfo()
    {
        if (this.deviceInfo != null)
        {
            return true;
        }

        if (this.workerBehaviour == null)
        {
            return false;
        }

        this.deviceInfo = this.workerBehaviour.GetDeviceInfo();
        if (this.deviceInfo != null)
        {
            return true;
        }

        return false;
    }

    private void Start()
    {
        InitWorker();
        InitDeviceInfo();
    }

    private void OnDestroy()
    {
        if (this.workerBehaviour != null)
        {
            this.workerBehaviour.StopTracking();
        }
    }

    private void Update ()
    {
        InitWorker();
        InitDeviceInfo();
    }

    private void OnGUI ()
    {
        this.guiScaler.Update();
        this.guiScaler.Set();

        if (this.workerBehaviour == null)
        {
            GUI.Box(new Rect(10, 10, 200, 30), "Waiting for VLWorkerBehaviour ...");
            this.guiScaler.Unset();
            return;
        }

        if (this.deviceInfo == null)
        {
            GUI.Box(new Rect(10, 10, 200, 30), "Waiting for VLDeviceInfo ...");
            this.guiScaler.Unset();
            return;
        }

        if (this.state == State.CameraSelection)
        {
            if (this.resolutionSelector == null)
            {
                this.resolutionSelector = new CameraResolutionSelector(
                    this.deviceInfo, this.resolutionSelection);
            }

            // Once the camera device + resolution is selected, 'queryString'
            // will contain the appropriate tracking parameters. Then load
            // the calibration configuration file and switch to the
            // 'calibration' state.
            string queryString = this.resolutionSelector.OnGUI();
            if (queryString != null)
            {
                Debug.Log("Starting calibration with DeviceID-parameters=" +
                    queryString);
                this.workerBehaviour.StartTracking("cameraCalibration.vl?" +
                    queryString);

                this.resolutionSelector = null;

                this.state = State.Calibration;
            }
        }
        // Calibration is running
        else if (this.state == State.Calibration)
        {
            int buttonHeight = 20;
            int buttonWidth = 280;
            int buttonMarginBottom = 10;
            int yPos = 40;

            GUI.Box(new Rect(10, yPos, buttonWidth + 20, buttonHeight + buttonMarginBottom),
                "State: " + this.calibState + " (" + this.frameCount + ")");
            yPos += buttonHeight + buttonMarginBottom + buttonMarginBottom;
            if (this.calibState.Equals("paused"))
            {
                if (GUI.Button(new Rect(
                    20, yPos, buttonWidth, buttonHeight), "Run"))
                {
                    this.workerBehaviour.SetCameraCalibrationState("run");
                }
            }
            else if (this.calibState.Equals("collecting") ||
                this.calibState.Equals("collectingActive"))
            {
                if (GUI.Button(new Rect(20, yPos, buttonWidth, buttonHeight),
                    "Pause"))
                {
                    this.workerBehaviour.SetCameraCalibrationState("pause");
                }
                yPos += buttonHeight + buttonMarginBottom;
                if (GUI.Button(new Rect(20, yPos, buttonWidth, buttonHeight),
                    "Calibrate"))
                {
                    this.workerBehaviour.SetCameraCalibrationState("optimize");
                }
            }
            else if (this.calibState.Equals("optimizing"))
            {
                if (GUI.Button(new Rect(20, yPos, buttonWidth, buttonHeight),
                    "Stop"))
                {
                    this.workerBehaviour.SetCameraCalibrationState("reset");
                }
                yPos += buttonHeight + buttonMarginBottom;
            }
            else if (this.calibState.Equals("stopping"))
            {
                GUI.TextArea(new Rect(20, yPos, buttonWidth, buttonHeight),
                    "Stopping");
                yPos += buttonHeight + buttonMarginBottom;
            }
            else if (this.calibState.Equals("done"))
            {
                if (GUI.Button(new Rect(20, yPos, buttonWidth, buttonHeight),
                    "Restart"))
                {
                    this.workerBehaviour.SetCameraCalibrationState("reset");
                }
                yPos+=30;
                this.destinationURI = GUI.TextField(
                    new Rect(20, yPos, buttonWidth, buttonHeight), this.destinationURI);

                yPos += 30;
                if (GUI.Button(new Rect(20, yPos, buttonWidth, buttonHeight),
                    "Write"))
                {
                    this.workerBehaviour.SetCameraCalibrationState("write");
                    this.workerBehaviour.WriteCameraCalibration(this.destinationURI);
                }
                yPos += 30;
                if (GUI.Button(new Rect(20, yPos, buttonWidth, buttonHeight),
                    "GetResults"))
                {
                    this.workerBehaviour.SetCameraCalibrationState("getResults");
                }
            }
        }

        this.guiScaler.Unset();
    }

    private void OnCalibDataResult(VLCameraCalibration calib)
    {
        if (calib.calibrated)
        {
            Debug.Log("Calibration was successful (error: " +
                calib.intrinsics.calibrationError + ")");
        }
        else
        {
            Debug.LogError("Calibration failed");
        }
    }

    private void StoreTrackingStates(VLTrackingState trackingState)
    {
        if (trackingState.objects == null ||
            trackingState.objects.Length == 0)
        {
            return;
        }
        VLTrackingState.TrackingObject obj = trackingState.objects[0];
        this.calibState = obj.state;
        this.frameCount = obj._NumberOfTemplates;
        // The name "_NumberOfTemplates" is a bit misleading. This actually
        // contains the number of successfully captures frames.
    }

    private void OnEnable()
    {
        VLWorkerBehaviour.OnTrackingStates += StoreTrackingStates;
        VLWorkerBehaviour.OnCameraCalibrationData += OnCalibDataResult;
    }

    private void OnDisable()
    {
        VLWorkerBehaviour.OnTrackingStates -= StoreTrackingStates;
        VLWorkerBehaviour.OnCameraCalibrationData -= OnCalibDataResult;
    }
}
