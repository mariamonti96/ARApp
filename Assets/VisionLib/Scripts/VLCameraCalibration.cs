/** @addtogroup vlUnitySDK
 *  @{
 */

using System;



/// <summary>
///  VLTrackingIssues stores the issues arrising during startup
/// More information on possible codes: @ref tracking-init-issues
/// Such an object is usually passed, when subscribing to the OnTrackerInitializedWithIssues event.
/// </summary>
[Serializable]
public class VLCameraCalibration
{
	/// <summary>
	///  TrackingIssue stores an issue when tracking or when initialized
	/// </summary>
	[Serializable]
	public class VLCameraIntrinsics
	{
		public int width;
		public int height;

		public float fx;
		public float fy;
		public float cx;
		public float cy;

		public float k1;
		public float k2;
		public float k3;
		public float k4;
		public float k5;
		public float s;

		public float calibrationError;
		public string quality;
		public string deviceID;

	}

	public string type;
	public int version;

	public float timestamp;
	public string organization;

	public string deviceID;
	public string cameraName;

	public bool calibrated;

	public VLCameraIntrinsics intrinsics;
	public VLCameraIntrinsics intrinsicsDist;

	/// <summary>
	///  Array with the tracking state of all tracking objects.
	/// </summary>
	public string[] alternativeDeviceIDs;

}

[Serializable]
public class VLCameraCalibrationAnswer
{
	[Serializable]
	public class VLCameraCalibrationAnswerStateChange
	{
		public string from;
		public string to;
		public string command;
	}

	public string message;
	public VLCameraCalibrationAnswerStateChange stateChange;
	public VLCameraCalibration calibration = null;

}

/**@}*/
