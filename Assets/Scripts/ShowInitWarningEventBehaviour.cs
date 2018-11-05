using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInitWarningEventBehaviour : MonoBehaviour
{
    /// <summary>
    ///  Text component for displaying any issues.
    /// </summary>
    ///  If this is not null, then it will be used to output any errors. If
    ///  it is null, then the OnGUI function will be used to output the errors
    ///  on the screen.
    /// </remarks>
    public Text textComponent;

    private List<string> warningTextList = new List<string>();
    private List<string> errorTextList = new List<string>();

    private string issuesText = "";

    private static string GetErrorMessage(
        VLTrackingIssues.VLTrackingIssue issue)
    {
        switch (issue.code)
        {
        case 1:
            return "Device name is empty";
        case 2:
            return "No calibration available for device";
        case 3:
        case 4:
            return "No camera found";
        case 5:
            return "Device with specified deviceID not connected";
        case 6:
            return "Device with specified position not connected";
        case 10:
            return "Unable to load camera calibration database from URI";
        case 11:
            return "Unable to parse camera calibration database from URI";
        case 12:
            return "Failed to add camera calibration";
        case 13:
            return "Overwriting camera calibration";
        case 14:
            return "Overwriting camera calibration by alternative ID";
        case 20:
            return "Use of deprecated 'lineSearchLengthInit' or 'lineSearchLengthTracking' parameter";
        case 98:
            return "Failed to load tracking configuration from file";
        case 99:
            return "No valid tracking configuration";
        case 100:
            return "No license to load tracking configuration";
        case 101:
            return "No valid license file found";
        case 102:
            return "License expired";
        case 103:
            return "License runs exceeded";
        case 104:
            return "No license for model";
        case 105:
            return "Unlicensed hostID found; Please register your hostID \"" +
                issue.info + "\" at visionlib.com";
        case 106:
            return "License invalid for UWP";
        case 107:
            return "License error: Invalid Platform";
        case 108:
            return "License error: Using unregistered Model";
        case 109:
            return "License file not found";
        case 110:
            return "Incompatible Program version";
        case 111:
            return "License error: Invalid Seat";
        case 112:
            return "License error: Invalid feature";
        case 113:
            return "License warning: License will expire in " + issue.info + " days";
        case 114:
            return "Unlicensed bundleID found; Please register your bundleID \"" +
                issue.info + "\" at visionlib.com";
        case 300:
            return "Failed to find model file";
        case 301:
            return "Failed to load model";
        }
        return null;
    }

    private void HandleTrackerInitializing()
    {
        this.warningTextList.Clear();
        this.errorTextList.Clear();
        this.issuesText = "";
        if (this.textComponent != null)
        {
            this.textComponent.text = "";
            this.textComponent.enabled = false;
        }
    }

    private void LogTrackerInitIssues(VLTrackingIssues errors,
        VLTrackingIssues warnings)
    {
        // Notify the user of problems during the initialization of the
        // tracking pipeline. Take a look at the VisionLib SDK documentation
        // for a complete list of error codes.

        // If an error occurred, then the tracking won't work
        if (errors != null)
        {
            if (errors.message != null && errors.message != "")
            {
                this.errorTextList.Add(errors.message);
                Debug.LogError("Error: " + errors.message);
            }
            if (errors.issues != null)
            {
                foreach (VLTrackingIssues.VLTrackingIssue issue in errors.issues)
                {
                    string errorMessage = GetErrorMessage(issue);
                    if (errorMessage != null)
                    {
                        this.errorTextList.Add(errorMessage);
                        Debug.LogError("Issue " + issue.code +
                            " (\"" + issue.info + "\"): " + errorMessage);
                    }
                    else
                    {
                        Debug.LogError("Issue " + issue.code +
                            " (\"" + issue.info + "\")");
                    }
                }
            }
        }

        // If a warning occurred, then the tracking will work, but the tracking
        // quality might be harmed (e.g. camera not calibrated)
        if (warnings != null)
        {
            if (warnings.message != null && warnings.message != "")
            {
                this.warningTextList.Add(warnings.message);
                Debug.LogWarning("Warning: " + warnings.message);
            }
            if (warnings.issues != null)
            {
                foreach (VLTrackingIssues.VLTrackingIssue issue in warnings.issues)
                {
                    string errorMessage = GetErrorMessage(issue);
                    if (errorMessage != null)
                    {
                        this.warningTextList.Add(errorMessage);
                        Debug.LogWarning("Issue " + issue.code +
                            " (\"" + issue.info + "\"): " + errorMessage);
                    }
                    else
                    {
                        Debug.LogWarning("Issue " + issue.code +
                            " (\"" + issue.info + "\")");
                    }
                }
            }
        }

        if (this.textComponent != null)
        {
            foreach (string errorText in this.errorTextList)
            {
                if (this.issuesText != "")
                {
                    this.issuesText += "\n";
                }
                this.issuesText += "[Error] " + errorText;
            }
            foreach (string warningText in this.warningTextList)
            {
                if (this.issuesText != "")
                {
                    this.issuesText += "\n";
                }
                this.issuesText += "[Warning] " + warningText;
            }
            // Updating the text of the textComponent here caused crashes on
            // the HoloLens. As a workaround, we update the text inside the
            // OnGUI function.
        }
    }

    private void OnEnable()
    {
        VLWorkerBehaviour.OnTrackerInitializing += HandleTrackerInitializing;
        VLWorkerBehaviour.OnTrackerInitializedWithIssues += LogTrackerInitIssues;
    }

    private void OnDisable()
    {
        VLWorkerBehaviour.OnTrackerInitializing -= HandleTrackerInitializing;
        VLWorkerBehaviour.OnTrackerInitializedWithIssues -= LogTrackerInitIssues;
    }

    private void OnGUI()
    {
        if (this.textComponent != null)
        {
            if (this.issuesText != "" && !this.textComponent.enabled)
            {
                this.textComponent.enabled = true;
                this.textComponent.text = this.issuesText;
            }
            return;
        }

        int margin = 8;
        int height = 30;
        int spacing = 4;

        GUI.color = Color.yellow;
        for (int i = 0; i < this.warningTextList.Count; ++i)
        {
            GUI.Label(
                new Rect(margin, Screen.height - (i * (height + spacing)) - height - margin,
                    Screen.width - (2 * margin), height),
                this.warningTextList[i], GUI.skin.textField);
        }

        GUI.color = Color.red;
        int warningsOffset = this.warningTextList.Count * (height + spacing);
        for (int i = 0; i < this.errorTextList.Count; ++i)
        {
            GUI.Label(
                new Rect(margin, Screen.height - warningsOffset - (i * (height + spacing)) - height - margin,
                    Screen.width - (2 * margin), height),
                this.errorTextList[i], GUI.skin.textField);
        }
    }
}