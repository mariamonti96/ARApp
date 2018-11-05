using System;
using System.Globalization;
using UnityEngine;

/// <summary>
///  This class allows to select a connected cameras and resolution.
/// </summary>
/// <remarks>
///  <para>
///   If only one camera is connected, can this camera will get selected
///   automatically.
///  </para>
///  <para>
///   The selection of the resolution is currently only supported for Windows.
///   On other platforms a default resolution will get selected.
///  </para>
///  <para>
///   This is just an example to show you how a camera selection could be
///   implemented. Please feel free to write you own camera selection.
///  </para>
///  <para>
///   NOTICE: The camera selection will only work correctly, if the vl-file
///   doesn't specify the "useImageSource" attribute.
///  </para>
/// </remarks>
public class CameraResolutionSelector
{
    /// <summary>
    ///  Reference to device information.
    /// </summary>
    private VLDeviceInfo deviceInfo = null;

    /// <summary>
    ///  Whether to offer a resolution selection.
    /// </summary>
    /// <remarks>
    ///  If this is <c>false</c>, then the resolution selection will be skipped
    ///  and a default resolution will be used. If this is <c>true</c>, then
    ///  the user can select a specific resolution.
    /// </remarks>
    private bool resolutionSelection = false;

    /// <summary>
    ///  Selected camera.
    /// </summary>
    /// <remarks>
    ///  While the selected camera is null a camera selection window will be
    ///  shown. If the selected camera is not null, then a camera resolution
    ///  window will be shown.
    /// </remarks>
    private VLDeviceInfo.Camera selectedCamera = null;

    /// <summary>
    ///  Selected format.
    /// </summary>
    /// <remarks>
    ///  While the selected format is null a format selection window will be
    ///  shown. If the selected format is not null, then OnGUI will return
    ///  a query string with the selected format.
    /// </remarks>
    private VLDeviceInfo.Camera.Format selectedFormat = null;

    /// <summary>
    ///  Generated query string.
    /// </summary>
    /// <remarks>
    ///  Value will generated after selecting a camera and format. The current
    ///  value will be returned by the OnGUI function.
    /// </remarks>
    private string queryString = null;

    /// <summary>
    ///  A unique ID used for the selection window.
    /// </summary>
    /// <remarks>
    ///  Set this to a unique value, if you also use the GUI.Window or
    ///  GUILayout.Window functions with the default ID.
    /// </remarks>
    public int windowID = 0;

    /// <summary>
    ///  Rectangle for the camera/resolution selection window. The actual
    ///  values will get determined automatically at runtime.
    /// </summary>
    private Rect windowRect;

    /// <summary>
    ///  Used to scale the UI inside the OnGUI function.
    /// </summary>
    private GUIMatrixScaler guiScaler = new GUIMatrixScaler(640, 480);

    /// <summary>
    ///  Just some dummy format to distinguish between "default" and a custom
    ///  format.
    /// </summary>
    private static readonly VLDeviceInfo.Camera.Format defaultFormat =
        new VLDeviceInfo.Camera.Format();

    public CameraResolutionSelector(VLDeviceInfo deviceInfo, bool resolutionSelection)
    {
        this.deviceInfo = deviceInfo;
        this.resolutionSelection = resolutionSelection;

        // Skip camera selection, if only one is available
        if (this.deviceInfo.availableCameras != null &&
            this.deviceInfo.availableCameras.Length == 1)
        {
            SelectCamera(this.deviceInfo.availableCameras[0]);
        }
    }

    private void SelectCamera(VLDeviceInfo.Camera camera)
    {
        this.selectedCamera = camera;

        // Skip resolution selection, if this wasn't requested
        if (!this.resolutionSelection)
        {
            SelectFormat(defaultFormat);
            return;
        }

        // Don't skip resolution selection, because 'resolutionSelection' is
        // activated and there should be some effect even if only the default
        // resolution can be selected
    }

    private void SelectFormat(VLDeviceInfo.Camera.Format format)
    {
        this.selectedFormat = format;
        BuildQueryString();
    }

    private void BuildQueryString()
    {
        if (this.selectedCamera == null)
        {
            Debug.LogWarning("Can't build query string without a selected camera");
            return;
        }

        if (this.selectedFormat == null)
        {
            Debug.LogWarning("Can't build query string without a selected format");
            return;
        }

        // Default format selected?
        if (System.Object.ReferenceEquals(this.selectedFormat, defaultFormat))
        {
            this.queryString = "input.useDeviceID=" +
                this.selectedCamera.deviceID;
            return;
        }

        // Otherwise, a custom format was selected

        // Calculate reasonable focal lengths, which can be used as initial
        // guess for a large range of cameras -> arithmetic mean of their
        // normalized values equals one
        double dw = (double)(this.selectedFormat.width);
        double dh = (double)(this.selectedFormat.height);
        double fnorm = 2.0 / (dw + dh);
        double fx = dh * fnorm;
        double fy = dw * fnorm;

        this.queryString =
            "input.imageSources[0].data.deviceID=" +
                this.selectedCamera.deviceID +
            "&input.imageSources[0].data.calibration.width=" +
                this.selectedFormat.width.ToString(CultureInfo.InvariantCulture) +
            "&input.imageSources[0].data.calibration.height=" +
                this.selectedFormat.height.ToString(CultureInfo.InvariantCulture) +
            "&input.imageSources[0].data.calibration.fx=" +
                fx.ToString("R", CultureInfo.InvariantCulture) +
            "&input.imageSources[0].data.calibration.fy=" +
                fy.ToString("R", CultureInfo.InvariantCulture) +
            "&input.imageSources[0].data.calibration.cx=0.5" +
            "&input.imageSources[0].data.calibration.cy=0.5" +
            "&input.useImageSource=camera0" +
            "&input.imageSources[0].name=camera0" +
            "&input.imageSources[0].type=camera" +
            "&input.SmartDownsamplingDisabled=true";
    }

    /// <summary>
    ///  Creates selection window for all available cameras.
    /// </summary>
    private void DoWindowCameraType(int windowID)
    {
        foreach (VLDeviceInfo.Camera camera in this.deviceInfo.availableCameras)
        {
            if (GUILayout.Button(camera.cameraName))
            {
                SelectCamera(camera);
                return;
            }
        }
    }

    /// <summary>
    ///  Creates selection window for all available resolutions of the selected
    ///  camera.
    /// </summary>
    private void DoWindowResolution(int windowID)
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();

        // Default button
        if (GUILayout.Button("Default"))
        {
            SelectFormat(defaultFormat);
            return;
        }

        // Add a button for each possible resolution
        int buttonRow = 1;
        foreach (VLDeviceInfo.Camera.Format format in
            this.selectedCamera.availableFormats)
        {
            string formatText = format.width + "x" +
                format.height + " " + format.compression;
            if (GUILayout.Button(formatText))
            {
                SelectFormat(format);
                return;
            }

            // Only show 10 buttons per column
            ++buttonRow;
            if (buttonRow >= 10)
            {
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                buttonRow = 0;
            }
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    public string OnGUI ()
    {
        this.guiScaler.Update();
        this.guiScaler.Set();

        if (this.deviceInfo.availableCameras == null ||
            this.deviceInfo.availableCameras.Length < 1)
        {
            GUI.Box(new Rect(10, 10, 200, 30), "No camera available");
            this.guiScaler.Unset();
            return null;
        }

        int windowMinWidth = 200;
        if (this.selectedCamera == null)
        {
            this.windowRect = GUILayout.Window(
                this.windowID, this.windowRect, DoWindowCameraType, "Select your camera");
            this.windowRect.width = Math.Max(this.windowRect.width, windowMinWidth);
            this.windowRect.x =
                (this.guiScaler.GetScaledScreenRect().width - this.windowRect.width) / 2.0f;
            this.windowRect.y =
                (this.guiScaler.GetScaledScreenRect().height - this.windowRect.height) / 2.0f;
            this.windowRect = GUILayout.Window(
                this.windowID, this.windowRect, DoWindowCameraType, "Select your camera");
        }
        else if (this.selectedFormat == null)
        {
            this.windowRect = GUILayout.Window(
                this.windowID, this.windowRect, DoWindowResolution, "Select desired resolution");
            this.windowRect.width = Math.Max(this.windowRect.width, windowMinWidth);
            this.windowRect.x =
                (this.guiScaler.GetScaledScreenRect().width - this.windowRect.width) / 2.0f;
            this.windowRect.y =
                (this.guiScaler.GetScaledScreenRect().height - this.windowRect.height) / 2.0f;
            this.windowRect = GUILayout.Window(
                this.windowID, this.windowRect, DoWindowResolution, "Select desired resolution");
        }

        guiScaler.Unset();

        return this.queryString;
    }
}