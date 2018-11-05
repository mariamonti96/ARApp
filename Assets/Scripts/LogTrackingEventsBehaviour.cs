using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
///  The LogTrackingEventsBehaviour is an example class, which shows how to
///  work with the tracking events.
/// </summary>
public class LogTrackingEventsBehaviour : MonoBehaviour
{
    private Text textComponent;
    private string trackingState = "";
    private string trackingStates = "";
    private string performanceInfo = "";

    void LogTrackerInitializing()
    {
        Debug.Log("Tracker initializing");
    }

    void LogTrackerInitialized(bool success)
    {
        Debug.Log("Tracker initialized: " + success);
    }

	void LogTrackerInitIssues(VLTrackingIssues errorIssues,VLTrackingIssues warningIssues)
	{
		// Please use this function in order notify the user of unforseen events during initialization of the tracking pipe
		// Review the documentation of the visionLib SDK in order to get information on possible errorCodes that
		// can help you interpret the error/warning messages

		// Error means, that the visionLib will not work at this moment-> It will NOT start running then
		if (errorIssues != null)
		{
			// Errors on init, which lead to better not advance right now...
			Debug.LogError("LogTrackerInitIssues Errors:" + errorIssues.message);
			for (int i = 0;i<errorIssues.issues.Length;i++)
			{
				Debug.LogError("LogTrackerInitIssue: " + i+"->"+errorIssues.issues[i].code+"->"+errorIssues.issues[i].info);
			}
		}
		// Warning means, that the visionLib will work but might have an issue which can harm the tracking quality
		// (e.g. a not well recognized/calibrated camera)
		if (warningIssues != null)
		{
			// We have some advises from the system that are fine to interpret and 
			// pass to the user...
			Debug.LogWarning("LogTrackerInitIssues Warnings:" + warningIssues.message);
			for (int i = 0;i<warningIssues.issues.Length;i++)
			{
				Debug.LogWarning("LogTrackerInitIssue: " + i+"->"+warningIssues.issues[i].code+"->"+warningIssues.issues[i].info);
			}

		}
	}

    void LogTrackerStopped(bool success)
    {
        Debug.Log("Tracker stopped: " + success);
    }

    void LogTrackerPaused(bool success)
    {
        Debug.Log("Tracker paused: " + success);
    }

    void LogTrackerRunning(bool success)
    {
        Debug.Log("Tracker running: " + success);
    }

    void LogTrackerRanOnce(bool success)
    {
        Debug.Log("Tracker ran once: " + success);
    }

    void LogTrackerResetSoft(bool success)
    {
        Debug.Log("Tracker reset soft: " + success);
    }

    void LogTrackerResetHard(bool success)
    {
        Debug.Log("Tracker reset hard: " + success);
    }

    void LogWriteInitData(bool success)
    {
        Debug.Log("Write init data: " + success);
    }

    void StoreTrackingState(int state, string objectID)
    {
        trackingState = "LogTrackingState: " + state;
    }

    void StoreTrackingStates(VLTrackingState state)
    {
        string str = "";
        for (int i=0; i < state.objects.Length; ++i)
        {
            VLTrackingState.TrackingObject obj = state.objects[i];
            str += obj.name + "\n";
            str += "* State: " + obj.state + "\n";
            str += "* Quality: " + obj.quality + "\n";
            str += "* _InitInlierRatio: " + obj._InitInlierRatio + "\n";
            str += "* _InitNumOfCorresp: " + obj._InitNumOfCorresp + "\n";
            str += "* _TrackingInlierRatio: " + obj._TrackingInlierRatio + "\n";
            str += "* _TrackingNumOfCorresp: " + obj._TrackingNumOfCorresp + "\n";
            str += "* _SFHFrameDist: " + obj._SFHFrameDist + "\n";
            str += "* _Total3DFeatureCount: " + obj._Total3DFeatureCount + "\n";
			str += "* _NumberOfTemplates: " + obj._NumberOfTemplates + "\n"; ;
			str += "* _NumberOfTemplatesDynamic: " + obj._NumberOfTemplatesDynamic + "\n"; ;
			str += "* _NumberOfTemplatesStatic: " + obj._NumberOfTemplatesStatic + "\n"; ;
            str += "* _TrackingImageSize: " + obj._TrackingImageWidth + "x"
                + obj._TrackingImageHeight;

            // Not the last tracked object?
            if (i < state.objects.Length - 1)
            {
                str += "\n";
            }
        }

        trackingStates = str;
    }

    void StorePerformanceInfo(VLPerformanceInfo info)
    {
        this.performanceInfo = "ProcessingTime: " + info.processingTime;
    }

    void Awake()
    {
        this.textComponent = GetComponent<Text>();
    }

    void OnEnable()
    {
        VLWorkerBehaviour.OnTrackerInitializing += LogTrackerInitializing;
        VLWorkerBehaviour.OnTrackerInitialized += LogTrackerInitialized;
		VLWorkerBehaviour.OnTrackerInitializedWithIssues += LogTrackerInitIssues;
        VLWorkerBehaviour.OnTrackerStopped += LogTrackerStopped;
        VLWorkerBehaviour.OnTrackerRunning += LogTrackerRunning;
        VLWorkerBehaviour.OnTrackerPaused += LogTrackerPaused;
        VLWorkerBehaviour.OnTrackerRanOnce += LogTrackerRanOnce;
        VLWorkerBehaviour.OnTrackerResetSoft += LogTrackerResetSoft;
        VLWorkerBehaviour.OnTrackerResetHard += LogTrackerResetHard;
        VLWorkerBehaviour.OnWriteInitData += LogWriteInitData;
        VLWorkerBehaviour.OnTrackingState += StoreTrackingState;
        VLWorkerBehaviour.OnTrackingStates += StoreTrackingStates;
        VLWorkerBehaviour.OnPerformanceInfo += StorePerformanceInfo;
    }

    void OnDisable()
    {
        VLWorkerBehaviour.OnTrackerInitializing -= LogTrackerInitializing;
        VLWorkerBehaviour.OnTrackerInitialized -= LogTrackerInitialized;
		VLWorkerBehaviour.OnTrackerInitializedWithIssues -= LogTrackerInitIssues;
        VLWorkerBehaviour.OnTrackerStopped -= LogTrackerStopped;
        VLWorkerBehaviour.OnTrackerRunning -= LogTrackerRunning;
        VLWorkerBehaviour.OnTrackerPaused -= LogTrackerPaused;
        VLWorkerBehaviour.OnTrackerRanOnce -= LogTrackerRanOnce;
        VLWorkerBehaviour.OnTrackerResetSoft -= LogTrackerResetSoft;
        VLWorkerBehaviour.OnTrackerResetHard -= LogTrackerResetHard;
        VLWorkerBehaviour.OnWriteInitData -= LogWriteInitData;
        VLWorkerBehaviour.OnTrackingState -= StoreTrackingState;
        VLWorkerBehaviour.OnTrackingStates -= StoreTrackingStates;
        VLWorkerBehaviour.OnPerformanceInfo -= StorePerformanceInfo;
    }

    void Update()
    {
        if (this.textComponent != null)
        {
            this.textComponent.text =
                performanceInfo + "\n" + trackingState + "\n" + trackingStates;
        }
    }

    void OnGUI()
    {
        if (this.textComponent == null)
        {
            int h = 175;
            GUI.Label(new Rect(0, Screen.height-h, Screen.width, h),
                performanceInfo + "\n" + trackingState + "\n" + trackingStates,
                GUI.skin.textField);
        }
    }
}
